using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regola per il calcolo della "Ricorso alla Corte Costituzionale"
    /// </summary>
    public class RicorsoCorteCostituzionaleRule : AvvocaturaBaseRule
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

            // Sottoregole notifica atto introduttivo
            list.Add(SubRulesNotificaAttoIntroduttivo.SCADENZA_COSTITUZIONE);
            list.Add(SubRulesNotificaAttoIntroduttivo.REMINDER_SCADENZA_COSTITUZIONE);
            list.Add(SubRulesNotificaAttoIntroduttivo.SCADENZA_ASSUNZIONE_DELIBERAZIONE_CONFERIMENTO_INCARICO);

            list.Add(SubRulesDataPubblicazioneGU.SCADENZA_NOTIFICA_RICORSO);
            list.Add(SubRulesDataPubblicazioneGU.REMINDER_SCADENZA_NOTIFICA_RICORSO);

            list.Add(SubRulesUdienza.SCADENZA_DEPOSITO_MEMORIA);
            list.Add(SubRulesUdienza.REMINDER_SCADENZA_DEPOSITO_MEMORIA);

            return list.ToArray();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RicorsoCorteCostituzionaleRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "RICORSO_CORTE_COSTITUZIONALE";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Ricorso Corte Costituzionale";

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
            public const string NUMERO_ORDINE = "N° d'ordine - CC";

            /// <summary>
            /// 
            /// </summary>
            public const string GIUDIZIO_DI = "Giudizio di";

            /// <summary>
            /// 
            /// </summary>
            public const string RICORRENTE_ATTORE = "Ricorrente/Attore";

            /// <summary>
            /// 
            /// </summary>
            public const string RESISTENTE_CONVENUTO = "Resistente/Convenuto";

            /// <summary>
            /// 
            /// </summary>
            public const string CONTRO_INTERESSATO_TERZO_CHIAMATO = "Contro interessato/terzo chiamato";

            /// <summary>
            /// 
            /// </summary>
            public const string NOTIFICA_ATTO_INTRODUTTIVO = "Notifica atto introduttivo / comunicazione ordinanza rinvio alla Corte";
            
            /// <summary>
            /// 
            /// </summary>
            public const string DATA_PUBBLICAZIONE_GU = "Data di pubblicazione in GU/BUR o conoscenza provvedimento da impugnare";

            /// <summary>
            /// 
            /// </summary>
            public const string SOSPENSIONE_FERIALE = "Sospensione feriale";
        }

        /// <summary>
        /// Sottoregole per le pubblicazioni legate alla notifica atto introduttivo
        /// </summary>
        protected class SubRulesNotificaAttoIntroduttivo
        {
            public const string SCADENZA_COSTITUZIONE = "SCADENZA_COSTITUZIONE";
            public const string REMINDER_SCADENZA_COSTITUZIONE = "REMINDER_SCADENZA_COSTITUZIONE";
            public const string SCADENZA_ASSUNZIONE_DELIBERAZIONE_CONFERIMENTO_INCARICO = "SCADENZA_ASSUNZIONE_DELIBERAZIONE_CONFERIMENTO_INCARICO";
        }

        /// <summary>
        /// Sottoregole per le pubblicazioni legate al campo "Data di pubblicazione in GU/BUR o conoscenza provvedimento da impugnare"
        /// </summary>
        protected class SubRulesDataPubblicazioneGU
        {
            public const string SCADENZA_NOTIFICA_RICORSO = "SCADENZA_NOTIFICA_RICORSO";
            public const string REMINDER_SCADENZA_NOTIFICA_RICORSO = "REMINDER_SCADENZA_NOTIFICA_RICORSO";
        }

        /// <summary>
        /// Sottoregole per le pubblicazioni legate alle udienze
        /// </summary>
        protected class SubRulesUdienza
        {
            public const string SCADENZA_DEPOSITO_MEMORIA = "SCADENZA_DEPOSITO_MEMORIA";
            public const string REMINDER_SCADENZA_DEPOSITO_MEMORIA = "REMINDER_SCADENZA_DEPOSITO_MEMORIA";
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
        /// <param name="subRule"></param>
        protected override void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            if (this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivo.SCADENZA_COSTITUZIONE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivo.REMINDER_SCADENZA_COSTITUZIONE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivo.SCADENZA_ASSUNZIONE_DELIBERAZIONE_CONFERIMENTO_INCARICO))
            {
                // La data di pubblicazione in gazzetta ufficiale ha la precedenza quando
                // è valorizzata anche la data notifica atto introduttivo
                this.ExecuteDateRuleAlternativeFields(subRule, 
                                Fields.DATA_PUBBLICAZIONE_GU, 
                                Fields.NOTIFICA_ATTO_INTRODUTTIVO);
            }
            else
                base.InternalExecuteSubRule(subRule);
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
                // Verifica se, per la causa, è stato modificato il valore del campo sopensione feriale dei termini
                // rispetto alla versione precedente del profilo
                FieldStateTypesEnum stateSospensioneFeriale = this.GetFieldState(Fields.SOSPENSIONE_FERIALE, rule);

                isDirty = (stateSospensioneFeriale != FieldStateTypesEnum.Unchanged);
            }

            if (!isDirty)
            {
                // Verifica se sono stati modificati i campi i cui valori sono 
                // visualizzati come oggetto e body del messaggio di posta / iCal

                // Campi dell'oggetto
                isDirty = this.GetFieldState(Fields.RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(Fields.GIUDIZIO_DI, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.CONTRO_INTERESSATO_TERZO_CHIAMATO, rule) != FieldStateTypesEnum.Unchanged;
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

            const string FORMAT = "Ricorrente/Attore: {0}; Resistente/Convenuto: {1} - {2} - N° d'ordine: {3}";

            // Creazione dell'oggetto
            subject = string.Format(FORMAT,
                this.GetPropertyValueAsString(this.FindProperty(Fields.RICORRENTE_ATTORE)),
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
            formatter.AddData("Tipologia fascicolo:", this.ListenerRequest.EventInfo.PublishedObject.TemplateName);
            formatter.AddData(CommonFields.DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description.Trim());
            formatter.AddData(Fields.GIUDIZIO_DI + ":", this.GetPropertyValueAsString(Fields.GIUDIZIO_DI));
            formatter.AddData(Fields.RICORRENTE_ATTORE + ":", this.GetPropertyValueAsString(Fields.RICORRENTE_ATTORE));
            formatter.AddData(Fields.RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(Fields.RESISTENTE_CONVENUTO));
            formatter.AddData(Fields.CONTRO_INTERESSATO_TERZO_CHIAMATO + ":", this.GetPropertyValueAsString(Fields.CONTRO_INTERESSATO_TERZO_CHIAMATO));

            return formatter.ToString();
        }

        /// <summary>
        /// Il profilo prevede il campo "Sospensione feriale dei termini",
        /// pertanto ne viene riportato il valore
        /// </summary>
        /// <param name="rule"></param>
        protected override bool HasSospensioneFerialeTermini(BaseRuleInfo rule)
        {
            string ruleName = string.Empty;

            if (rule.GetType() == typeof(SubRuleInfo))
                ruleName = ((SubRuleInfo)rule).SubRuleName;
            else
                ruleName = rule.RuleName;

            Property pSospensioneFerialeTermini = this.FindProperty(Fields.SOSPENSIONE_FERIALE);

            if (pSospensioneFerialeTermini != null && pSospensioneFerialeTermini.Value != null)
                return this.AreEquals(pSospensioneFerialeTermini.Value.ToString(), "si");
            else
                return base.HasSospensioneFerialeTermini(rule);
        }

        #endregion
    }
}