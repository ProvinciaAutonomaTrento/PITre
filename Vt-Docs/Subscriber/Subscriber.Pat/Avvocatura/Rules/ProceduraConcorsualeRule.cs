using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regola per il calcolo delle scadenze in agenda relative alla causa "Procedura concorsuale"
    /// </summary>
    public class ProceduraConcorsualeRule : AvvocaturaBaseRule
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

            // Sottoregole legate al fallimento
            list.Add(SubRulesUdienzaVerificaStatoPassivo.SCADENZA_DOMANDA_TEMPESTIVA);
            list.Add(SubRulesUdienzaVerificaStatoPassivo.REMINDER_SCADENZA_DOMANDA_TEMPESTIVA);
            // Sottoregole legate al concordato
            list.Add(SubRulesUdienzaVerificaStatoPassivo.SCADENZA_DOMANDA);
            list.Add(SubRulesUdienzaVerificaStatoPassivo.REMINDER_SCADENZA_DOMANDA);
            list.Add(SubRulesUdienzaVerificaStatoPassivo.ADUNANZA_CREDITORI);
            list.Add(SubRulesUdienzaVerificaStatoPassivo.REMINDER_ADESIONE_CONCORDATO);


            list.Add(SubRulesDecretoEsecStatoPassivo.SCADENZA_DOMANDA_TARDIVA);
            list.Add(SubRulesDecretoEsecStatoPassivo.REMINDER_SCADENZA_DOMANDA_TARDIVA);


            list.Add(SubRulesComunicazioneEsitoAccertamento.SCADENZA_PRESENTAZIONE_OPPOSIZIONE);
            list.Add(SubRulesComunicazioneEsitoAccertamento.REMINDER_SCADENZA_PRESENTAZIONE_OPPOSIZIONE);

            return list.ToArray();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(ProceduraConcorsualeRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "PROCEDURA_CONCORSUALE";
        
        /// <summary>
        /// 
        /// </summary>
        protected class Fields
        {
            /// <summary>
            /// 
            /// </summary>
            public const string CURATORE = "Curatore / Commissario giudiziale";

            /// <summary>
            /// 
            /// </summary>
            public const string GIUDICE_DELEGATO = "Giudice delegato";

            /// <summary>
            /// 
            /// </summary>
            public const string SOSPENSIONE_FERIALE = "Sospensione feriale";

            /// <summary>
            /// 
            /// </summary>
            public const string FALLITO = "Fallito / Debitore";

            /// <summary>
            /// 
            /// </summary>
            public const string NUMERO_ORDINE = "N° d'ordine - PC";

            /// <summary>
            /// 
            /// </summary>
            public const string TIPOLOGIA = "Tipologia";

            /// <summary>
            /// 
            /// </summary>
            public const string DESCRIZIONE_FASCICOLO = "Descrizione del fascicolo";

            /// <summary>
            /// 
            /// </summary>
            public const string NUMERO_FALLIMENTO = "Numero Fallimento";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_UDIENZA_VERIFICA_STATO_PASSIVO = "Data udienza verifica stato passivo/ adunanza dei creditori";
        }

        /// <summary>
        /// 
        /// </summary>
        protected class SubRulesUdienzaVerificaStatoPassivo
        {
            // Regole da eseguire solo se la tipologia è "Fallimento"
            public const string SCADENZA_DOMANDA_TEMPESTIVA = "SCADENZA_DOMANDA_TEMPESTIVA";
            public const string REMINDER_SCADENZA_DOMANDA_TEMPESTIVA = "REMINDER_SCADENZA_DOMANDA_TEMPESTIVA";

            // Regole da eseguire solo se la tipologia è "Concordato"
            public const string SCADENZA_DOMANDA = "SCADENZA_DOMANDA";
            public const string REMINDER_SCADENZA_DOMANDA = "REMINDER_SCADENZA_DOMANDA";
            public const string ADUNANZA_CREDITORI = "ADUNANZA_CREDITORI";
            public const string REMINDER_ADESIONE_CONCORDATO = "REMINDER_ADESIONE_CONCORDATO";
        }

        /// <summary>
        /// 
        /// </summary>
        protected class SubRulesDecretoEsecStatoPassivo
        {
            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_DOMANDA_TARDIVA = "SCADENZA_DOMANDA_TARDIVA";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_SCADENZA_DOMANDA_TARDIVA = "REMINDER_SCADENZA_DOMANDA_TARDIVA";
        }

        /// <summary>
        /// 
        /// </summary>
        protected class SubRulesComunicazioneEsitoAccertamento
        {
            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_PRESENTAZIONE_OPPOSIZIONE = "SCADENZA_PRESENTAZIONE_OPPOSIZIONE";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_SCADENZA_PRESENTAZIONE_OPPOSIZIONE = "REMINDER_SCADENZA_PRESENTAZIONE_OPPOSIZIONE";
        }

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Procedura concorsuale";

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
            if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaVerificaStatoPassivo.SCADENZA_DOMANDA_TEMPESTIVA) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaVerificaStatoPassivo.REMINDER_SCADENZA_DOMANDA_TEMPESTIVA))
            {
                // Valore ammesso nel campo "Tipologia"
                const string FALLIMENTO = "Fallimento";

                // Ricerca del campo "Termine" nel profilo
                Property pTermine = this.FindProperty(Fields.TIPOLOGIA);

                if (pTermine != null && !pTermine.IsEmpty)
                {
                    if (this.AreEquals(pTermine.Value.ToString(), FALLIMENTO))
                    {
                        // Esecuzione della regola se e solo se il valore del campo "Termine" è impostato a "Fallimento"
                        this.ExecuteDateRule(subRule, false);
                    }
                    else
                    {
                        // Verifica se inviare un appuntamento di cancellazione
                        RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(subRule);

                        if (lastExecutedRule != null &&
                            lastExecutedRule.MailMessageSnapshot != null &&
                            lastExecutedRule.MailMessageSnapshot.Appointment != null &&
                            lastExecutedRule.MailMessageSnapshot.Appointment.Status == Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CONFIRMED)
                        {
                            this.SendCancelAppointment(subRule);
                        }
                    }
                }
                else
                {
                    // Campo obbligatorio
                    throw new SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, Fields.TIPOLOGIA));
                }
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaVerificaStatoPassivo.SCADENZA_DOMANDA) ||
                     this.AreEquals(subRule.SubRuleName, SubRulesUdienzaVerificaStatoPassivo.REMINDER_SCADENZA_DOMANDA) ||
                     this.AreEquals(subRule.SubRuleName, SubRulesUdienzaVerificaStatoPassivo.ADUNANZA_CREDITORI) ||
                     this.AreEquals(subRule.SubRuleName, SubRulesUdienzaVerificaStatoPassivo.REMINDER_ADESIONE_CONCORDATO))
            {
                // Valore ammesso nel campo "Tipologia"
                const string CONCORDATO_PREVENTIVO = "Concordato preventivo";

                // Ricerca del campo "Termine" nel profilo
                Property pTermine = this.FindProperty(Fields.TIPOLOGIA);

                if (pTermine != null && !pTermine.IsEmpty)
                {
                    if (this.AreEquals(pTermine.Value.ToString(), CONCORDATO_PREVENTIVO))
                    {
                        // Esecuzione della regola se e solo se il valore del campo "Termine" è impostato a "Concordato preventivo"
                        this.ExecuteDateRule(subRule, false);
                    }
                    else
                    {
                        // Verifica se inviare un appuntamento di cancellazione
                        RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(subRule);

                        if (lastExecutedRule != null &&
                            lastExecutedRule.MailMessageSnapshot != null &&
                            lastExecutedRule.MailMessageSnapshot.Appointment != null &&
                            lastExecutedRule.MailMessageSnapshot.Appointment.Status == Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CONFIRMED)
                        {
                            this.SendCancelAppointment(subRule);
                        }
                    }
                }
                else
                {
                    // Campo obbligatorio
                    throw new SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, Fields.TIPOLOGIA));
                }
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
                // Verifica se il campo tipologia è stato modificato
                FieldStateTypesEnum tipologiaFieldState = this.GetFieldState(Fields.TIPOLOGIA, rule);

                isDirty = (tipologiaFieldState != FieldStateTypesEnum.Unchanged);
            }

            if (!isDirty)
            {
                // Verifica se sono stati modificati i campi i cui valori sono 
                // visualizzati come oggetto e body del messaggio di posta / iCal

                // Campi dell'oggetto
                isDirty = this.GetFieldState(Fields.FALLITO, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(CommonFields.AUTORITA_GIUDIZIARIA, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(CommonFields.COMPETENZA_TERRITORIALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.FALLITO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.NUMERO_FALLIMENTO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.CURATORE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.GIUDICE_DELEGATO, rule) != FieldStateTypesEnum.Unchanged;
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

            // Per la causa Procedura concorsuale, l'oggetto deve essere composto nella seguente maniera:
            // Fallito: {0} - {1}, N° d'ordine: {2}

            const string FORMAT = "Fallito / Debitore: {0} - {1}, N° d'ordine: {2}";

            // Creazione dell'oggetto
            subject = string.Format(FORMAT,
                    this.GetPropertyValueAsString(this.FindProperty(Fields.FALLITO)), 
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
            // Per la causa del contenzioso civile, il body deve avere i seguenti campi:
            // - Descrizione del fascicolo
            // - Autorità giudiziaria
            // - Competenza territoriale
            // - Fallito
            // - Numero fallimento
            // - Curatore
            // - Giudice delegato

            formatter.AddData(Fields.DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description);
            formatter.AddData(CommonFields.AUTORITA_GIUDIZIARIA + ":", this.GetPropertyValueAsString(CommonFields.AUTORITA_GIUDIZIARIA));
            formatter.AddData(CommonFields.COMPETENZA_TERRITORIALE + ":", this.GetPropertyValueAsString(CommonFields.COMPETENZA_TERRITORIALE));
            formatter.AddData(Fields.FALLITO + ":", this.GetPropertyValueAsString(Fields.FALLITO));
            formatter.AddData(Fields.NUMERO_FALLIMENTO + ":", this.GetPropertyValueAsString(Fields.NUMERO_FALLIMENTO));
            formatter.AddData(Fields.CURATORE + ":", this.GetPropertyValueAsString(Fields.CURATORE));
            formatter.AddData(Fields.GIUDICE_DELEGATO + ":", this.GetPropertyValueAsString(Fields.GIUDICE_DELEGATO));

            return formatter.ToString();
        }

        #endregion
    }
}
