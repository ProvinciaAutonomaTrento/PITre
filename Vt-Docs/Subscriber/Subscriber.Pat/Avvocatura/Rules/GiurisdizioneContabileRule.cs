using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regola per il calcolo della "Giurisdizione contabile"
    /// </summary>
    public class GiurisdizioneContabileRule : AvvocaturaBaseRule
    {
        #region Public Members

        /// <summary>
        /// Reperimento del nome della regola
        /// </summary>
        public override string RuleName
        {
            get
            {
                return RULE_NAME;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string[] GetSubRules()
        {
            List<string> list = new List<string>();

            // SubRules notifica atto introduttivo
            list.Add(SubRulesDataPubblicazioneSentenza.REMINDER_RECUPERO_IMPORTI_LIQUIDATI);

            return list.ToArray();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(GiurisdizioneContabileRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "GIURISDIZIONE_CONTABILE";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Giurisdizione contabile";

        /// <summary>
        /// Campi significativi del profilo
        /// </summary>
        protected class Fields
        {
            /// <summary>
            /// 
            /// </summary>
            public const string DESCRIZIONE_FASCICOLO = "Descrizione del fascicolo";

            /// <summary>
            /// 
            /// </summary>
            public const string NUMERO_ORDINE = "N° d'ordine - C.CONTI";

            /// <summary>
            /// 
            /// </summary>
            public const string RUOLO_GENERALE = "Ruolo generale";

            /// <summary>
            /// 
            /// </summary>
            public const string RESISTENTE_CONVENUTO = "Resistente/Convenuto";

            /// <summary>
            /// 
            /// </summary>
            public const string AVVOCATO_RESISTENTE_CONVENUTO = "Avvocato resistente/Convenuto";
        }

        /// <summary>
        /// SubRules eventi per data pubblicazione sentenza
        /// </summary>
        protected class SubRulesDataPubblicazioneSentenza
        {
            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RECUPERO_IMPORTI_LIQUIDATI = "REMINDER_RECUPERO_IMPORTI_LIQUIDATI";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplateName()
        {
            return TEMPLATE_NAME;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected override bool IsDirtyDependentFields(BaseRuleInfo rule)
        {
            bool isDirty = base.IsDirtyDependentFields(rule);

            
            if (!isDirty)
            {
                // Verifica se sono stati modificati i campi i cui valori sono 
                // visualizzati come oggetto e body del messaggio di posta / iCal

                // Campi dell'oggetto
                isDirty = this.GetFieldState(Fields.RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetDescriptionFieldState(rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(CommonFields.AUTORITA_GIUDIZIARIA, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(CommonFields.COMPETENZA_TERRITORIALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.RUOLO_GENERALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.AVVOCATO_RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged;
                }
            }

            return isDirty;
        }

        /// <summary>
        /// Creazione dell'oggetto del messaggio di posta
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="computedAppointmentDate"></param>
        /// <returns></returns>
        protected override string CreateMailSubject(BaseRuleInfo rule, DateTime computedAppointmentDate)
        {
            string subject = string.Empty;

            const string FORMAT = "Resistente/Convenuto: {0} - {1} - N° d'ordine: {2}";

            // Creazione dell'oggetto
            subject = string.Format(FORMAT,
                this.GetPropertyValueAsString(this.FindProperty(Fields.RESISTENTE_CONVENUTO)),
                rule.RuleDescription,
                this.GetPropertyValueAsString(this.FindProperty(Fields.NUMERO_ORDINE)));

            return subject;
        }

        /// <summary>
        /// Creazione del body del messaggio di posta
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="rule"></param>
        /// <param name="computedAppointmentDate"></param>
        /// <returns></returns>
        protected override string CreateMailBody(Dispatcher.CalendarMail.IMailBodyFormatter formatter, BaseRuleInfo rule, DateTime computedAppointmentDate)
        {
            formatter.AddData(CommonFields.DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description.Trim());
            formatter.AddData(CommonFields.AUTORITA_GIUDIZIARIA + ":", this.GetPropertyValueAsString(CommonFields.AUTORITA_GIUDIZIARIA));
            formatter.AddData(CommonFields.COMPETENZA_TERRITORIALE + ":", this.GetPropertyValueAsString(CommonFields.COMPETENZA_TERRITORIALE));
            formatter.AddData(Fields.RUOLO_GENERALE + ":", this.GetPropertyValueAsString(Fields.RUOLO_GENERALE));
            formatter.AddData(Fields.RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(Fields.RESISTENTE_CONVENUTO));
            formatter.AddData(Fields.AVVOCATO_RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(Fields.AVVOCATO_RESISTENTE_CONVENUTO));

            return formatter.ToString();
        }

        #endregion
    }
}