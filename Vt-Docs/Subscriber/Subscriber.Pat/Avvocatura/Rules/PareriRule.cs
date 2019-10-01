using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public class PareriRule : AvvocaturaBaseRule
    {
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
        /// Pubblicazione delle sottoregole di calcolo
        /// </summary>
        /// <returns></returns>
        public override string[] GetSubRules()
        {
            List<string> list = new List<string>();

            list.Add(SUBRULE_NAME_SCADENZA_EVASIONE_PARERE);
            list.Add(SUBRULE_NAME_SCADENZA_EVASIONE_PARERE_INTEGRAZIONE);

            return list.ToArray();
        }

        /// <summary>
        /// Nome del template
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplateName()
        {
            return TEMPLATE_NAME;
        }

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(PareriRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "PARERE";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Parere";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_EVASIONE_PARERE = "SCADENZA_EVASIONE_PARERE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_EVASIONE_PARERE_INTEGRAZIONE = "SCADENZA_EVASIONE_PARERE_INTEGRAZIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_RICHIEDENTE = "Richiedente";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NUMERO_ORDINE = "N° d'ordine - PAR";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DESCRIZIONE_FASCICOLO = "Descrizione del fascicolo";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DATA_ARRIVO_RICHIESTA = "Data arrivo richiesta";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DATA_ARRIVO_ELEMENTI_INTEGRATIVI = "Data arrivo elementi integrativi";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subRule"></param>
        protected override void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_SCADENZA_EVASIONE_PARERE))
            {
                // Il campo "Data richiesta elementi integrativi" ha la precedenza
                // sul campo "Data arrivo richiesta"
                this.ExecuteDateRuleAlternativeFields(subRule,
                                    FIELD_DATA_ARRIVO_ELEMENTI_INTEGRATIVI, 
                                    FIELD_DATA_ARRIVO_RICHIESTA);
            }
            else
            {
                base.InternalExecuteSubRule(subRule);
            }
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

            // Per la causa del parere, l'oggetto deve essere composto nella seguente maniera:
            // Richiedente: {0}

            const string FORMAT = "Richiedente: {0}, - {1}, N° d'ordine: {2}";

            // Creazione dell'oggetto
            subject = string.Format(FORMAT, 
                    this.GetPropertyValueAsString(this.FindProperty(FIELD_RICHIEDENTE)), 
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
            // Per la causa del parere, il body deve avere i seguenti campi:
            // - Descrizione del fascicolo
            // - Data arrivo richiesta

            formatter.AddData(FIELD_DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description);

            if (!FindProperty(FIELD_DATA_ARRIVO_ELEMENTI_INTEGRATIVI).IsEmpty)
            {
                // Se risulta valorizzato il campo "Data richiesta elementi integrativi",
                // deve essere visualizzato nel body del messaggio in quanto è il campo che ha generato la scadenza
                formatter.AddData(FIELD_DATA_ARRIVO_ELEMENTI_INTEGRATIVI + ":", this.GetPropertyValueAsString(this.FindProperty(FIELD_DATA_ARRIVO_ELEMENTI_INTEGRATIVI)));
            }
            else
            {
                formatter.AddData(FIELD_DATA_ARRIVO_RICHIESTA + ":", this.GetPropertyValueAsString(this.FindProperty(FIELD_DATA_ARRIVO_RICHIESTA)));
            }

            return formatter.ToString();
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
                isDirty = this.GetFieldState(FIELD_RICHIEDENTE, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(FIELD_NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(FIELD_RICHIEDENTE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_DATA_ARRIVO_RICHIESTA, rule) != FieldStateTypesEnum.Unchanged;
                }
            }

            return isDirty;
        }

        #endregion
    }
}
