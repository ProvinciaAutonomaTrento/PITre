using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regola per il calcolo del "Pignoramento presso terzi".
    /// 
    /// La regola è applicabile alle seguenti tipologie fascicolo:
    /// - Pignoramento presso terzi
    /// 
    /// Prevede sue sottoregole:
    /// - Scadenziare in agenda udienza dichiarazione terzo ex 543 c.p.c.
    ///     Non genera alcuna scadenza in calendario
    ///     
    /// - Scadenziare in agenda termine per dichiarazione del terzo a mezzo raccomandata
    ///     Richiede la scadenza di +10gg decorrente dalla data di notifica termine per dichiarazione a mezzo raccomandata  
    /// </summary>
    public class PignoramentoPressoTerziRule : AvvocaturaBaseRule
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
        /// 
        /// </summary>
        /// <returns></returns>
        public override string[] GetSubRules()
        {
            List<string> list = new List<string>();
            list.Add(SUBRULE_NAME_UDIENZA_DICHIARAZIONE_TERZO);
            list.Add(SUBRULE_NAME_TERMINE_DICHIARAZIONE_TERZO);
            return list.ToArray();
        }

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(PignoramentoPressoTerziRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "PIGNORAMENTO_PRESSO_TERZI";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Pignoramento";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_PIGNORATO = "Pignorato";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_PIGNORANTE = "Pignorante";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NUMERO_ORDINE = "N° d'ordine - AP";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_NAME_DICHIARAZIONE = "Dichiarazione";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DESCRIZIONE_FASCICOLO = "Descrizione del fasciolo";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DATA_UDIENZA = "Udienza dichiarazione del terzo ex 543 c.p.c.";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_UDIENZA_DICHIARAZIONE_TERZO = "UDIENZA_DICHIARAZIONE_TERZO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_TERMINE_DICHIARAZIONE_TERZO = "TERMINE_DICHIARAZIONE_TERZO";

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
            if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_TERMINE_DICHIARAZIONE_TERZO))
            {
                // La scadenza legata al termine dichiarazione del terzo 
                // deve essere calcolata se e solo se il campo dichiarazione
                // è impostato "A mezzo raccomandata"

                // Reperimento del valore del campo dichiarazione
                Property pDichiarazione = this.FindProperty(FIELD_NAME_DICHIARAZIONE);

                if (pDichiarazione != null)
                {
                    if (!pDichiarazione.IsEmpty)
                    {
                        // Possibili valori del campo "Dichiarazione"
                        const string MEZZO_RACCOMANDATA = "A mezzo raccomandata";
                        const string IN_UDIENZA = "In udienza";

                        if (this.AreEquals(pDichiarazione.Value.ToString(), MEZZO_RACCOMANDATA))
                        {
                            // Calcolo della scadenza se il valore del campo è a mezzo raccomandata
                            this.ExecuteDateRule(subRule, false);
                        }
                        else if (this.AreEquals(pDichiarazione.Value.ToString(), IN_UDIENZA))
                        {
                            RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(subRule);

                            if (lastExecutedRule != null && lastExecutedRule.Published)
                            {
                                if (this.AreEquals(lastExecutedRule.MailMessageSnapshot.Appointment.Method, Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentMethodTypes.REQUEST))
                                {
                                    // Invio appuntamento di cancellazione per la regola legata al termine dichiarazione del terzo
                                    this.SendCancelAppointment(subRule);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Campo non presente tra i dati del profilo
                    throw new SubscriberException(ErrorCodes.MISSING_FIELD_VALUE,
                                                    string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, FIELD_NAME_DICHIARAZIONE));
                }
            }
            else
                base.InternalExecuteSubRule(subRule);
        }

        /// <summary>
        /// La causa non prevede la sospensione feriale dei termini
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected override bool HasSospensioneFerialeTermini(BaseRuleInfo rule)
        {            
            return false;
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
                isDirty = this.GetFieldState(FIELD_PIGNORATO, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(FIELD_PIGNORANTE, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(FIELD_NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(CommonFields.AUTORITA_GIUDIZIARIA, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(CommonFields.COMPETENZA_TERRITORIALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_PIGNORATO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_PIGNORANTE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_DATA_UDIENZA, rule) != FieldStateTypesEnum.Unchanged;
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

            // Per la causa del Pignoramento, l'oggetto deve essere composto nella seguente maniera:
            // Pignorato: {0}, Pignorante: {1} - Evento: {2}

            const string FORMAT = "Pignorato: {0}, Pignorante: {1} - {2}, N° d'ordine: {3}";

            // Creazione dell'oggetto
            subject = string.Format(FORMAT,
                this.GetPropertyValueAsString(this.FindProperty(FIELD_PIGNORATO)),
                this.GetPropertyValueAsString(this.FindProperty(FIELD_PIGNORANTE)),
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
            // Per la causa del contenzioso civile, il body deve avere i seguenti campi:
            // - Descrizione del fascicolo
            // - Autorità giudiziaria
            // - Competenza territoriale
            // - Pignorato
            // - Pignorante
            // - Data udienza

            formatter.AddData(FIELD_DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description);
            formatter.AddData(CommonFields.AUTORITA_GIUDIZIARIA + ":", this.GetPropertyValueAsString(CommonFields.AUTORITA_GIUDIZIARIA));
            formatter.AddData(CommonFields.COMPETENZA_TERRITORIALE + ":", this.GetPropertyValueAsString(CommonFields.COMPETENZA_TERRITORIALE));
            formatter.AddData(FIELD_PIGNORATO + ":", this.GetPropertyValueAsString(FIELD_PIGNORATO));
            formatter.AddData(FIELD_PIGNORANTE + ":", this.GetPropertyValueAsString(FIELD_PIGNORANTE));
            formatter.AddData(FIELD_DATA_UDIENZA + ":", this.GetPropertyValueAsString(FIELD_DATA_UDIENZA));

            return formatter.ToString();
        }


        #endregion
    }
}
