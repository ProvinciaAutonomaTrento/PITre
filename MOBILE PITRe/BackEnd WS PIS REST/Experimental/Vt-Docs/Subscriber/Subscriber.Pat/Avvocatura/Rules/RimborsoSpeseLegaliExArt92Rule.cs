using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public class RimborsoSpeseLegaliExArt92Rule : AvvocaturaBaseRule
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
            list.Add(SUBRULE_NAME_RICHIESTA_PARCELLA_O_COMUNICAZIONE_NOTIFICA);

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
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RimborsoSpeseLegaliExArt92Rule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "RIMBORSO_SPESE_LEGALI";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Rimborso spese legali";

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
        private const string SUBRULE_NAME_RICHIESTA_PARCELLA_O_COMUNICAZIONE_NOTIFICA = "RICHIESTA_PARCELLA_O_COMUNICAZIONE_NOTIFICA";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NAME_DATA_ARRIVO_RICHIESTA_RIMBORSO = "Data arrivo richiesta rimborso spese legali";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NAME_DATA_ARRIVO_ELEMENTI_INTEGRATIVI = "Data arrivo elementi integrativi";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NAME_DATA_DELIBERAZIONE_LIQUIDAZIONE = "Data deliberazione liquidazione";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NUMERO_ORDINE = "N° d'ordine - RSL";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NAME_RICHIEDENTE = "Richiedente";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subRule"></param>
        protected override void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_SCADENZA_EVASIONE_PARERE))
            {
                this.ExecuteDateRuleAlternativeFields(subRule, 
                                FIELD_NAME_DATA_ARRIVO_ELEMENTI_INTEGRATIVI,
                                FIELD_NAME_DATA_ARRIVO_RICHIESTA_RIMBORSO);
            }
            else
            {
                base.InternalExecuteSubRule(subRule);
            }
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
                isDirty = this.GetFieldState(FIELD_NAME_RICHIEDENTE, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(FIELD_NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(FIELD_NAME_RICHIEDENTE, rule) != FieldStateTypesEnum.Unchanged;
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
            const string FORMAT = "Richiedente: {0}, Data arrivo richiesta: {1} - {2}, N° d'ordine: {3}";

            // Creazione dell'oggetto
            return string.Format(FORMAT,
                this.GetPropertyValueAsString(this.FindProperty(FIELD_NAME_RICHIEDENTE)),
                this.GetPropertyValueAsString(this.FindProperty(FIELD_NAME_DATA_ARRIVO_RICHIESTA_RIMBORSO)),
                rule.RuleDescription,
                this.GetPropertyValueAsString(this.FindProperty(FIELD_NUMERO_ORDINE)));
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
            formatter.AddData(CommonFields.DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description);
            formatter.AddData("Data arrivo richiesta" + ":", this.GetPropertyValueAsString(FIELD_NAME_DATA_ARRIVO_RICHIESTA_RIMBORSO));

            return formatter.ToString();
        }

        #endregion
    }
}
