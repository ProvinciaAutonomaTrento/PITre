using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regola per il calcolo delle scadenze in agenda relative alla causa "Ricorso straordinario presidente repubblica"
    /// </summary>
    public class RicorsoStraordinarioPresRepRule : AvvocaturaBaseRule
    {
        #region Public members

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

            list.Add(SUBRULE_NAME_SCADENZA_TRASMISSIONE_DEDUZIONI);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_TRASMISSIONE_DEDUZIONI);

            return list.ToArray();
        }

        #endregion

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RicorsoStraordinarioPresRepRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "RICORSO_STRAORDINARIO_PRESIDENTE_REPUBBLICA";

        /// <summary>
        /// Campo del profilo, riportato nell'oggetto mail
        /// </summary>
        private const string FIELD_RICORRENTE_ATTORE = "Ricorrente/Attore";

        /// <summary>
        /// Campo del profilo, riportato nell'oggetto mail
        /// </summary>
        private const string FIELD_RESISTENTE_CONVENUTO = "Resistente/Convenuto";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NUMERO_ORDINE = "N° d'ordine - PdR";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO = "Contro interessato/terzo chiamato";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DESCRIZIONE_FASCICOLO = "Descrizione del fascicolo";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_TRASMISSIONE_DEDUZIONI = "SCADENZA_TRASMISSIONE_DEDUZIONI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_TRASMISSIONE_DEDUZIONI = "REMINDER_SCADENZA_TRASMISSIONE_DEDUZIONI";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Ricorso straordinario Presidente della Repubblica";

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
                isDirty = this.GetFieldState(FIELD_RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(FIELD_RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(FIELD_NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;
                
                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(FIELD_RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO, rule) != FieldStateTypesEnum.Unchanged;
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

            // Per la causa del ricorso al pres repubblica, l'oggetto deve essere composto nella seguente maniera:
            // Ricorrente/Attore: {0} Resistente/Convenuto: {1} - {2}

            const string FORMAT = "Ricorrente/Attore: {0}, Resistente/Convenuto: {1} - {2}, N° d'ordine: {3}";

            // Creazione dell'oggetto
            subject = string.Format(FORMAT,
                this.GetPropertyValueAsString(this.FindProperty(FIELD_RICORRENTE_ATTORE)),
                this.GetPropertyValueAsString(this.FindProperty(FIELD_RESISTENTE_CONVENUTO)),
                rule.RuleDescription,
                this.GetPropertyValueAsString(this.FindProperty(FIELD_NUMERO_ORDINE)));

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
            formatter.AddData("Tipologia fascicolo:", this.GetTemplateName());
            formatter.AddData(FIELD_DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description);
            formatter.AddData(FIELD_RICORRENTE_ATTORE + ":", this.GetPropertyValueAsString(FIELD_RICORRENTE_ATTORE));
            formatter.AddData(FIELD_RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(FIELD_RESISTENTE_CONVENUTO));
            formatter.AddData(FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO + ":", this.GetPropertyValueAsString(FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO));
            
            return formatter.ToString();
        }

        #endregion
    }
}
