using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regola per il calcolo del "Procedimento penale"
    /// </summary>
    public class ProcedimentoPenaleRule : AvvocaturaBaseRule
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

            // SubRules Udienza preliminare
            list.Add(SubRulesUdienzaPreliminare.APPUNTAMENTO_UDIENZA_PRELIMINARE);
            list.Add(SubRulesUdienzaPreliminare.REMINDER_ASSUNTA_DELIBERA_COSTITUZIONE);
            list.Add(SubRulesUdienzaPreliminare.REMINDER_RINVIO_UDIENZA_PRELIMINARE);
            list.Add(SubRulesUdienzaPreliminare.SCADENZA_VERIFICA_ELEMENTI_DI_RISPOSTA);

            // SubRules udienza di rinvio
            list.Add(SubRulesUdienzaRinvio.APPUNTAMENTO_UDIENZA_RINVIO);
            list.Add(SubRulesUdienzaRinvio.REMINDER_RINVIO_UDIENZA_RINVIO);
            list.Add(SubRulesUdienzaRinvio.REMINDER_APPUNTAMENTO_UDIENZA_RINVIO);

            // SubRules udienza di apertura del dibattimanto
            list.Add(SubRulesUdienzaAperturaDibattimento.APPUNTAMENTO_UDIENZA_APERTURA_DIBATTIMENTO);
            list.Add(SubRulesUdienzaAperturaDibattimento.REMINDER_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO);
            list.Add(SubRulesUdienzaAperturaDibattimento.SCADENZA_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO);
            list.Add(SubRulesUdienzaAperturaDibattimento.REMINDER_RINVIO_UDIENZA_APERTURA_DIBATTIMENTO);

            // SubRules udienza dibattimentale
            list.Add(SubRulesUdienzaDibattimentale.APPUNTAMENTO_UDIENZA_DIBATTIMENTALE);
            list.Add(SubRulesUdienzaDibattimentale.REMINDER_RINVIO_UDIENZA_DIBATTIMENTALE);
            list.Add(SubRulesUdienzaDibattimentale.REMINDER_APPUNTAMENTO_UDIENZA_DIBATTIMENTALE);

            // SubRules udienza giuramento CTU
            list.Add(SubRulesUdienzaGiuramentoCTU.APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU);
            list.Add(SubRulesUdienzaGiuramentoCTU.REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU);

            // SubRules udienza testi
            list.Add(SubRulesUdienzaTesti.APPUNTAMENTO_UDIENZA_TESTI);
            list.Add(SubRulesUdienzaTesti.REMINDER_RINVIO_UDIENZA_TESTI);
            list.Add(SubRulesUdienzaTesti.REMINDER_SCADENZA_TERMINE_CITAZIONE_TESTI);
            list.Add(SubRulesUdienzaTesti.SCADENZA_TERMINE_CITAZIONE_TESTI);

            // SubRules termine per l'appello
            list.Add(SubRulesTermineAppello.SCADENZA_PRESENTAZIONE_APPELLO);

            return list.ToArray();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(ProcedimentoPenaleRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "PROCEDIMENTO_PENALE";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Procedimento penale";

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
            /// Campo del profilo, necessario per il calcolo dei termini
            /// </summary>
            public const string SOSPENSIONE_FERIALE = "Sospensione feriale dei termini";

            /// <summary>
            /// 
            /// </summary>
            public const string IMPUTATO_I = "Imputato I";

            /// <summary>
            /// 
            /// </summary>
            public const string AVVOCATO_IMPUTATO_I = "Avvocato imputato I";

            /// <summary>
            /// 
            /// </summary>
            public const string IMPUTATO_II = "Imputato II";

            /// <summary>
            /// 
            /// </summary>
            public const string AVVOCATO_IMPUTATO_II = "Avvocato imputato II";

            /// <summary>
            /// 
            /// </summary>
            public const string NUMERO_ORDINE = "N° d'ordine - PP";

            /// <summary>
            /// 
            /// </summary>
            public const string RUOLO_GENERALE_NOTIZIE_REATO = "Ruolo Generale Notizie di Reato";

            /// <summary>
            /// 
            /// </summary>
            public const string PUBBLICO_MINISTERO = "Pubblico Ministero";

            /// <summary>
            /// 
            /// </summary>
            public const string RUOLO_GENERALE_GIP_GUP = "Ruolo generale G.I.P./G.U.P";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_LETTURA_DISPOSITIVO_DEPOSITO_SCADENZA = "Data lettura dispositivo / avviso di deposito sentenza / termine deposito sentenza";

            /// <summary>
            /// 
            /// </summary>
            public const string TERMINE_APPELLO = "Termine per l'appello";
        }

        /// <summary>
        /// Sottoregole per l'udienza preliminare
        /// </summary>
        protected class SubRulesUdienzaPreliminare
        {
            // SubRules per l'udienza preliminare
            public const string APPUNTAMENTO_UDIENZA_PRELIMINARE = "APPUNTAMENTO_UDIENZA_PRELIMINARE";
            public const string REMINDER_RINVIO_UDIENZA_PRELIMINARE = "REMINDER_RINVIO_UDIENZA_PRELIMINARE";
            public const string SCADENZA_VERIFICA_ELEMENTI_DI_RISPOSTA = "SCADENZA_VERIFICA_ELEMENTI_DI_RISPOSTA";
            public const string REMINDER_ASSUNTA_DELIBERA_COSTITUZIONE = "REMINDER_ASSUNTA_DELIBERA_COSTITUZIONE";
        }

        /// <summary>
        /// Sottoregole per l'udienza di rinvio
        /// </summary>
        protected class SubRulesUdienzaRinvio
        {
            public const string APPUNTAMENTO_UDIENZA_RINVIO = "APPUNTAMENTO_UDIENZA_RINVIO";
            public const string REMINDER_APPUNTAMENTO_UDIENZA_RINVIO = "REMINDER_APPUNTAMENTO_UDIENZA_RINVIO";
            public const string REMINDER_RINVIO_UDIENZA_RINVIO = "REMINDER_RINVIO_UDIENZA_RINVIO";
        }

        /// <summary>
        /// Sottoregole per l'udienza di apertura del dibattimento
        /// </summary>
        protected class SubRulesUdienzaAperturaDibattimento
        {
            public const string APPUNTAMENTO_UDIENZA_APERTURA_DIBATTIMENTO = "APPUNTAMENTO_UDIENZA_APERTURA_DIBATTIMENTO";
            public const string REMINDER_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO = "REMINDER_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO";
            public const string SCADENZA_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO = "SCADENZA_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO";
            public const string REMINDER_RINVIO_UDIENZA_APERTURA_DIBATTIMENTO = "REMINDER_RINVIO_UDIENZA_APERTURA_DIBATTIMENTO";
        }

        /// <summary>
        /// Sottoregole per l'udienza dibattimentale
        /// </summary>
        protected class SubRulesUdienzaDibattimentale
        {
            public const string APPUNTAMENTO_UDIENZA_DIBATTIMENTALE = "APPUNTAMENTO_UDIENZA_DIBATTIMENTALE";
            public const string REMINDER_APPUNTAMENTO_UDIENZA_DIBATTIMENTALE = "REMINDER_APPUNTAMENTO_UDIENZA_DIBATTIMENTALE";
            public const string REMINDER_RINVIO_UDIENZA_DIBATTIMENTALE = "REMINDER_RINVIO_UDIENZA_DIBATTIMENTALE";
        }

        /// <summary>
        /// Sottoregole per l'udienza giuramento CTU
        /// </summary>
        protected class SubRulesUdienzaGiuramentoCTU
        {
            public const string APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU = "APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU";
            public const string REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU = "REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU";
        }

        /// <summary>
        /// Sottoregole per udienza testi
        /// </summary>
        protected class SubRulesUdienzaTesti
        {
            public const string APPUNTAMENTO_UDIENZA_TESTI = "APPUNTAMENTO_UDIENZA_TESTI";
            public const string REMINDER_SCADENZA_TERMINE_CITAZIONE_TESTI = "REMINDER_SCADENZA_TERMINE_CITAZIONE_TESTI";
            public const string SCADENZA_TERMINE_CITAZIONE_TESTI = "SCADENZA_TERMINE_CITAZIONE_TESTI";
            public const string REMINDER_RINVIO_UDIENZA_TESTI = "REMINDER_RINVIO_UDIENZA_TESTI";
        }

        /// <summary>
        /// Sottoregole per l'appello
        /// </summary>
        protected class SubRulesTermineAppello
        {
            public const string SCADENZA_PRESENTAZIONE_APPELLO = "SCADENZA_PRESENTAZIONE_APPELLO";
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
        protected override AvvocaturaBaseRuleOptions GetAvvocaturaOptions(BaseRuleInfo rule)
        {
            string ruleName = string.Empty;

            if (rule.GetType() == typeof(BaseRuleInfo))
            {
                ruleName = rule.RuleName;
            }
            else if (rule.GetType() == typeof(SubRuleInfo))
            {
                ruleName = ((SubRuleInfo)rule).SubRuleName;
            }

            if (this.AreEquals(ruleName, SubRulesTermineAppello.SCADENZA_PRESENTAZIONE_APPELLO))
            {
                // Per l'appuntamento "Termine presentazione appello", la decorrenza dei termini
                // deve essere calcolata dinamicamente. Ovvero al campo data del profilo 
                // vanno sommati il numero di giorni scelto nel campo "Termine per l'appello"

                Property pTermineAppello = this.FindProperty(Fields.TERMINE_APPELLO);

                if (pTermineAppello != null && !pTermineAppello.IsEmpty)
                {
                    const string VALUE_15GG = "15 gg";
                    const string VALUE_30GG = "30 gg";
                    const string VALUE_45GG = "45 gg";

                    AvvocaturaBaseRuleOptions opts = new AvvocaturaBaseRuleOptions(rule);
                    opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Giorni;

                    if (this.AreEquals(pTermineAppello.Value.ToString(), VALUE_15GG))
                    {
                        // Imposta il numero di giorni di decorrenza
                        opts.Decorrenza = 15;    
                    }
                    else if (this.AreEquals(pTermineAppello.Value.ToString(), VALUE_30GG))
                    {
                        // Imposta il numero di giorni di decorrenza
                        opts.Decorrenza = 30;
                    }
                    else if (this.AreEquals(pTermineAppello.Value.ToString(), VALUE_45GG))
                    {
                        // Imposta il numero di giorni di decorrenza
                        opts.Decorrenza = 45;
                    }
                    else
                    {
                        // Valore del campo non ammesso
                        throw new SubscriberException(ErrorCodes.INVALID_FIELD_VALUE, string.Format(ErrorDescriptions.INVALID_FIELD_VALUE, Fields.TERMINE_APPELLO));
                    }

                    return opts;
                }
                else
                {
                    // Campo obbligatorio
                    throw new SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, Fields.TERMINE_APPELLO));
                }
            }
            else
            {
                return base.GetAvvocaturaOptions(rule);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subRule"></param>
        protected override void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPreliminare.APPUNTAMENTO_UDIENZA_PRELIMINARE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPreliminare.REMINDER_RINVIO_UDIENZA_PRELIMINARE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPreliminare.REMINDER_ASSUNTA_DELIBERA_COSTITUZIONE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPreliminare.SCADENZA_VERIFICA_ELEMENTI_DI_RISPOSTA))
            {
                // Udienza preliminare
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaRinvio.APPUNTAMENTO_UDIENZA_RINVIO) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaRinvio.REMINDER_RINVIO_UDIENZA_RINVIO) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaRinvio.REMINDER_APPUNTAMENTO_UDIENZA_RINVIO))
            {
                // Udienza di rinvio
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAperturaDibattimento.APPUNTAMENTO_UDIENZA_APERTURA_DIBATTIMENTO) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAperturaDibattimento.REMINDER_RINVIO_UDIENZA_APERTURA_DIBATTIMENTO) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAperturaDibattimento.REMINDER_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAperturaDibattimento.SCADENZA_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO))
            {
                // Udienza di apertura del dibattimento
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaDibattimentale.APPUNTAMENTO_UDIENZA_DIBATTIMENTALE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaDibattimentale.REMINDER_RINVIO_UDIENZA_DIBATTIMENTALE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaDibattimentale.REMINDER_APPUNTAMENTO_UDIENZA_DIBATTIMENTALE))
            {
                // Udienza dibattimentale
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaGiuramentoCTU.APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaGiuramentoCTU.REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU))
            {
                // Udienza giuramento CTU
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTesti.APPUNTAMENTO_UDIENZA_TESTI) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTesti.REMINDER_RINVIO_UDIENZA_TESTI) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTesti.REMINDER_SCADENZA_TERMINE_CITAZIONE_TESTI) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTesti.SCADENZA_TERMINE_CITAZIONE_TESTI))
            {
                // Udienza testi
                this.ExecuteUdienzaRule(subRule);
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
                // Verifica se, per la causa, è stato modificato il valore del campo sopensione feriale dei termini
                // rispetto alla versione precedente del profilo
                FieldStateTypesEnum stateSospensioneFeriale = this.GetFieldState(Fields.SOSPENSIONE_FERIALE, rule);

                isDirty = (stateSospensioneFeriale != FieldStateTypesEnum.Unchanged);
            }

            if (!isDirty)
            {
                // Verifica se, per la causa, è stato modificato il valore del campo "Termine per l'appello"
                // rispetto alla versione precedente del profilo
                FieldStateTypesEnum stateTermineAppello = this.GetFieldState(Fields.TERMINE_APPELLO, rule);

                isDirty = (stateTermineAppello != FieldStateTypesEnum.Unchanged);
            }

            if (!isDirty)
            {
                // Verifica se sono stati modificati i campi i cui valori sono 
                // visualizzati come oggetto e body del messaggio di posta / iCal

                // Campi dell'oggetto
                isDirty = this.GetFieldState(Fields.IMPUTATO_I, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.IMPUTATO_II, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(CommonFields.AUTORITA_GIUDIZIARIA, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(CommonFields.COMPETENZA_TERRITORIALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.RUOLO_GENERALE_NOTIZIE_REATO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.PUBBLICO_MINISTERO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.RUOLO_GENERALE_GIP_GUP, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.IMPUTATO_I, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.AVVOCATO_IMPUTATO_I, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.IMPUTATO_II, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.AVVOCATO_IMPUTATO_II, rule) != FieldStateTypesEnum.Unchanged;
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

            // Per la causa del procedimento penale, l'oggetto deve essere composto nella seguente maniera:
            // Imputato I: {0}, Imputato II: {1} - Evento: {2}, N° d'ordine: {3}

            const string FORMAT = "Imputato I: {0}, Imputato II: {1} - {2}, N° d'ordine: {3}";

            // Creazione dell'oggetto
            subject = string.Format(FORMAT,
                this.GetPropertyValueAsString(this.FindProperty(Fields.IMPUTATO_I)),
                this.GetPropertyValueAsString(this.FindProperty(Fields.IMPUTATO_II)),
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
            // Per la causa del procedimento penale, il body deve avere i seguenti campi:
            // - Descrizione del fascicolo
            // - Autorità giudiziaria
            // - Competenza territoriale
            // - Ruolo generale notizie di reato
            // - Pubblico ministero
            // - Ruolo generale GIP / GUP
            // - Imputato I
            // - Avvocato imputato I
            // - Imputato II
            // - Avvocato imputato II

            formatter.AddData(CommonFields.DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description);
            formatter.AddData(CommonFields.AUTORITA_GIUDIZIARIA + ":", this.GetPropertyValueAsString(CommonFields.AUTORITA_GIUDIZIARIA));
            formatter.AddData(CommonFields.COMPETENZA_TERRITORIALE + ":", this.GetPropertyValueAsString(CommonFields.COMPETENZA_TERRITORIALE));
            formatter.AddData(Fields.RUOLO_GENERALE_NOTIZIE_REATO + ":", this.GetPropertyValueAsString(Fields.RUOLO_GENERALE_NOTIZIE_REATO));
            formatter.AddData(Fields.PUBBLICO_MINISTERO + ":", this.GetPropertyValueAsString(Fields.PUBBLICO_MINISTERO));
            formatter.AddData(Fields.RUOLO_GENERALE_GIP_GUP + ":", this.GetPropertyValueAsString(Fields.RUOLO_GENERALE_GIP_GUP));
            formatter.AddData(Fields.IMPUTATO_I + ":", this.GetPropertyValueAsString(Fields.IMPUTATO_I));
            formatter.AddData(Fields.AVVOCATO_IMPUTATO_I + ":", this.GetPropertyValueAsString(Fields.AVVOCATO_IMPUTATO_I));
            formatter.AddData(Fields.IMPUTATO_II + ":", this.GetPropertyValueAsString(Fields.IMPUTATO_II));
            formatter.AddData(Fields.AVVOCATO_IMPUTATO_II + ":", this.GetPropertyValueAsString(Fields.AVVOCATO_IMPUTATO_II));

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

            if (this.AreEquals(ruleName, SubRulesTermineAppello.SCADENZA_PRESENTAZIONE_APPELLO) ||
                this.AreEquals(ruleName, SubRulesUdienzaAperturaDibattimento.SCADENZA_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO) ||
                this.AreEquals(ruleName, SubRulesUdienzaAperturaDibattimento.REMINDER_DEPOSITO_UDIENZA_APERTURA_DIBATTIMENTO))
            {
                // Uniche regole di calcolo delle scadenze che devono tenere conto della sospensione feriale dei termini

                Property pSospensioneFerialeTermini = this.FindProperty(Fields.SOSPENSIONE_FERIALE);

                if (pSospensioneFerialeTermini != null && pSospensioneFerialeTermini.Value != null)
                    return this.AreEquals(pSospensioneFerialeTermini.Value.ToString(), "si");
                else
                    return base.HasSospensioneFerialeTermini(rule);
            }
            else
            {
                // Le altre regole non prevedono la sospensione feriale dei termini
                return false;
            }
        }

        #endregion
    }
}