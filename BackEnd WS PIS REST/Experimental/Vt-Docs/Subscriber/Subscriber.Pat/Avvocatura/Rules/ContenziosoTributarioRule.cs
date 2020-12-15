using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
/// <summary>
    /// Regola per il calcolo del "Contenzioso tributario"
    /// </summary>
    public class ContenziosoTributarioRule : AvvocaturaBaseRule
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
            list.Add(SubRulesNotificaAttoIntroduttivo.SCADENZA_COSTITUZIONE_RESISTENTE);
            list.Add(SubRulesNotificaAttoIntroduttivo.REMINDER_SCADENZA_COSTITUZIONE_RESISTENTE);
            list.Add(SubRulesNotificaAttoIntroduttivo.SCADENZA_COSTITUZIONE_RICORRENTE);
            list.Add(SubRulesNotificaAttoIntroduttivo.REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE);

            // SubRules udienza cautelare
            list.Add(SubRulesUdienzaCautelare.APPUNTAMENTO_UDIENZA_CAUTELARE);
            list.Add(SubRulesUdienzaCautelare.SCADENZA_COSTITUZIONE);

            // SubRules udienza altro
            list.Add(SubRulesUdienzaAltro.APPUNTAMENTO_UDIENZA_ALTRO);
            list.Add(SubRulesUdienzaAltro.REMINDER_RINVIO_UDIENZA_ALTRO);

            // SubRules udienza trattazione
            list.Add(SubRulesUdienzaTrattazione.APPUNTAMENTO_UDIENZA_TRATTAZIONE);
            list.Add(SubRulesUdienzaTrattazione.REMINDER_RITIRO_DOCUMENTI_CONTROPARTI);
            list.Add(SubRulesUdienzaTrattazione.REMINDER_RITIRO_MEMORIE_CONTROPARTI);
            list.Add(SubRulesUdienzaTrattazione.SCADENZA_DEPOSITO_DOCUMENTI);
            list.Add(SubRulesUdienzaTrattazione.REMINDER_SCADENZA_DEPOSITO_DOCUMENTI);
            list.Add(SubRulesUdienzaTrattazione.SCADENZA_DEPOSITO_MEMORIA);
            list.Add(SubRulesUdienzaTrattazione.SCADENZA_REDAZIONE_RELAZIONE_UDIENZA);

            // SubRules pubblicazione sentenza
            list.Add(SubRulesPubblicazioneSentenza.SCADENZA_NOTIFICA_APPELLO);
            list.Add(SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_NOTIFICA_APPELLO);

            // SubRules notifica sentenza
            list.Add(SubRulesNotificaSentenza.SCADENZA_NOTIFICA_APPELLO_RICORSO);
            list.Add(SubRulesNotificaSentenza.REMINDER_SCADENZA_NOTIFICA_APPELLO_RICORSO);

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
        private const string RULE_NAME = "CONTENZIOSO_TRIBUTARIO";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Contenzioso tributario";

        /// <summary>
        /// Campi significativi del profilo
        /// </summary>
        protected class Fields
        {
            /// <summary>
            /// Campo del profilo, necessario per il calcolo dei termini
            /// </summary>
            public const string SOSPENSIONE_FERIALE = "Sospensione feriale dei termini";

            /// <summary>
            /// Campo del profilo, riportato nell'oggetto mail
            /// </summary>
            public const string RICORRENTE_ATTORE = "Ricorrente/Attore";

            /// <summary>
            /// Campo del profilo, riportato nell'oggetto mail
            /// </summary>
            public const string RESISTENTE_CONVENUTO = "Resistente/Convenuto";

            /// <summary>
            /// Campo del profilo, riportato nell'oggetto mail
            /// </summary>
            public const string NUMERO_ORDINE = "N° d'ordine - TRIB.";

            /// <summary>
            /// 
            /// </summary>
            public const string RUOLO_GENERALE = "Ruolo generale";

            /// <summary>
            /// 
            /// </summary>
            public const string AVVOCATO_RICORRENTE_ATTORE = "Avvocato ricorrente/attore";

            /// <summary>
            /// 
            /// </summary>
            public const string AVVOCATO_RESISTENTE_CONVENUTO = "Avvocato resistente/Convenuto";

            /// <summary>
            /// 
            /// </summary>
            public const string CONTRO_INTERESSATO_TERZO_CHIAMATO = "Contro interessato/terzo chiamato";

            /// <summary>
            /// 
            /// </summary>
            public const string AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO = "Avvocato contro int./terzo chiamato";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_PUBBLICAZIONE_SENTENZA = "Data pubblicazione sentenza";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_NOTIFICA_SENTENZA = "Data notifica sentenza";

            /// <summary>
            /// 
            /// </summary>
            public const string TERMINE_PER_APPELLO = "Termine per appello";
        }

        /// <summary>
        /// SubRules eventi per notifica atto introduttivo
        /// </summary>
        protected class SubRulesNotificaAttoIntroduttivo
        {
            public const string SCADENZA_COSTITUZIONE_RESISTENTE = "SCADENZA_COSTITUZIONE_RESISTENTE";
            public const string REMINDER_SCADENZA_COSTITUZIONE_RESISTENTE = "REMINDER_SCADENZA_COSTITUZIONE_RESISTENTE";
            public const string SCADENZA_COSTITUZIONE_RICORRENTE = "SCADENZA_COSTITUZIONE_RICORRENTE";
            public const string REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE = "REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE";
        }

        /// <summary>
        /// SubRules eventi per udienza cautelare
        /// </summary>
        protected class SubRulesUdienzaCautelare
        {
            public const string APPUNTAMENTO_UDIENZA_CAUTELARE = "APPUNTAMENTO_UDIENZA_CAUTELARE";
            public const string SCADENZA_COSTITUZIONE = "SCADENZA_COSTITUZIONE";
        }

        /// <summary>
        /// SubRules eventi udienza altro
        /// </summary>
        protected class SubRulesUdienzaAltro
        {
            public const string APPUNTAMENTO_UDIENZA_ALTRO = "APPUNTAMENTO_UDIENZA_ALTRO";
            public const string REMINDER_RINVIO_UDIENZA_ALTRO = "REMINDER_RINVIO_UDIENZA_ALTRO";
        }

        /// <summary>
        /// SubRules eventi udienza trattazione
        /// </summary>
        protected class SubRulesUdienzaTrattazione
        {
            public const string APPUNTAMENTO_UDIENZA_TRATTAZIONE = "APPUNTAMENTO_UDIENZA_TRATTAZIONE";
            public const string SCADENZA_DEPOSITO_DOCUMENTI = "SCADENZA_DEPOSITO_DOCUMENTI";
            public const string REMINDER_SCADENZA_DEPOSITO_DOCUMENTI = "REMINDER_SCADENZA_DEPOSITO_DOCUMENTI";
            public const string SCADENZA_DEPOSITO_MEMORIA = "SCADENZA_DEPOSITO_MEMORIA";
            public const string REMINDER_RITIRO_DOCUMENTI_CONTROPARTI = "REMINDER_RITIRO_DOCUMENTI_CONTROPARTI";
            public const string REMINDER_RITIRO_MEMORIE_CONTROPARTI = "REMINDER_RITIRO_MEMORIE_CONTROPARTI";
            public const string SCADENZA_REDAZIONE_RELAZIONE_UDIENZA = "SCADENZA_REDAZIONE_RELAZIONE_UDIENZA";
        }

        /// <summary>
        /// SubRules eventi per pubblicazione sentenza
        /// </summary>
        protected class SubRulesPubblicazioneSentenza
        {
            public const string SCADENZA_NOTIFICA_APPELLO = "SCADENZA_NOTIFICA_APPELLO";
            public const string REMINDER_SCADENZA_NOTIFICA_APPELLO = "REMINDER_SCADENZA_NOTIFICA_APPELLO";
        }

        /// <summary>
        /// SubRules eventi notifica sentenza
        /// </summary>
        protected class SubRulesNotificaSentenza
        {
            public const string SCADENZA_NOTIFICA_APPELLO_RICORSO = "SCADENZA_NOTIFICA_APPELLO_RICORSO";
            public const string REMINDER_SCADENZA_NOTIFICA_APPELLO_RICORSO = "REMINDER_SCADENZA_NOTIFICA_APPELLO_RICORSO";
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
            AvvocaturaBaseRuleOptions opts = base.GetAvvocaturaOptions(rule);

            string name = string.Empty;

            // Reperimento del nome della regola / sottoregola
            if (rule.GetType() == typeof(SubRuleInfo))
                name = ((SubRuleInfo)rule).SubRuleName;
            else
                name = rule.RuleName;

            if (this.AreEquals(name, SubRulesPubblicazioneSentenza.SCADENZA_NOTIFICA_APPELLO) ||
                this.AreEquals(name, SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_NOTIFICA_APPELLO))
            {
                bool reminder = this.AreEquals(name, SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_NOTIFICA_APPELLO);

                // Regola "Scadenza notifica appello":
                // La decorrenza dei termini è stabilita in base alla scelta del campo radio button "Termine per l'appello" 

                Property pTerminePerAppello = this.FindProperty(Fields.TERMINE_PER_APPELLO);

                if (pTerminePerAppello != null && pTerminePerAppello.Value != null)
                {
                    const string VALUE_6_MESI = "6 mesi";
                    const string VALUE_12_MESI = "12 mesi";

                    if (this.AreEquals(pTerminePerAppello.Value.ToString(), VALUE_6_MESI.ToString()))
                    {
                        if (reminder)
                        {
                            // Il promemoria scatta 30gg prima
                            opts.Decorrenza = 5;
                            opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                        }
                        else
                        {
                            opts.Decorrenza = 6;
                            opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                        }
                    }
                    else if (this.AreEquals(pTerminePerAppello.Value.ToString(), VALUE_12_MESI.ToString()))
                    {
                        if (reminder)
                        {
                            // Il promemoria scatta 30gg prima
                            opts.Decorrenza = 11;
                            opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                        }
                        else
                        {
                            opts.Decorrenza = 12;
                            opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                        }
                    }
                    else
                    {
                        // Valore non valido per il campo
                        throw new Subscriber.SubscriberException(ErrorCodes.INVALID_FIELD_VALUE, string.Format(ErrorDescriptions.INVALID_FIELD_VALUE, Fields.TERMINE_PER_APPELLO));
                    }
                }
                else
                {
                    // Campo non presente nel profilo
                    throw new Subscriber.SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, Fields.TERMINE_PER_APPELLO));
                }
            }

            return opts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subRule"></param>
        protected override void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaCautelare.APPUNTAMENTO_UDIENZA_CAUTELARE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaCautelare.SCADENZA_COSTITUZIONE))
            {
                // SubRules udienza cautelare
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAltro.APPUNTAMENTO_UDIENZA_ALTRO) ||
                this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAltro.REMINDER_RINVIO_UDIENZA_ALTRO))
            {
                // SubRules udienza altro
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTrattazione.APPUNTAMENTO_UDIENZA_TRATTAZIONE) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTrattazione.REMINDER_RITIRO_DOCUMENTI_CONTROPARTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTrattazione.REMINDER_RITIRO_MEMORIE_CONTROPARTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTrattazione.SCADENZA_DEPOSITO_DOCUMENTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTrattazione.REMINDER_SCADENZA_DEPOSITO_DOCUMENTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTrattazione.SCADENZA_DEPOSITO_MEMORIA) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaTrattazione.SCADENZA_REDAZIONE_RELAZIONE_UDIENZA))
            {
                // SubRules udienza trattazione
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesPubblicazioneSentenza.SCADENZA_NOTIFICA_APPELLO) ||
                     this.AreEquals(subRule.SubRuleName, SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_NOTIFICA_APPELLO))
            {
                // Verifica la presenza della "Data notifica sentenza" 
                // Nel caso sia presente, le regole legate alla "Data pubblicazione sentenza" non dovranno essere eseguita,
                // quindi NON dovranno essere generati appuntamenti per quest'ultima. Piuttosto sarà necessario
                // inviare un appuntamento di cancellazione qualora non sia stato già inviato.

                this.ExecuteDateRuleAlternativeFields(subRule, 
                                Fields.DATA_NOTIFICA_SENTENZA, 
                                Fields.DATA_PUBBLICAZIONE_SENTENZA);
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
                FieldStateTypesEnum stateTerminePerAppello = this.GetFieldState(Fields.TERMINE_PER_APPELLO, rule);

                isDirty = (stateTerminePerAppello != FieldStateTypesEnum.Unchanged);
            }

            if (!isDirty)
            {
                // Verifica se sono stati modificati i campi i cui valori sono 
                // visualizzati come oggetto e body del messaggio di posta / iCal

                // Campi dell'oggetto
                isDirty = this.GetFieldState(Fields.RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                          this.GetFieldState(Fields.NUMERO_ORDINE, rule) != FieldStateTypesEnum.Unchanged;

                // Campi del body
                if (!isDirty)
                {
                    isDirty =
                        this.GetFieldState(CommonFields.AUTORITA_GIUDIZIARIA, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(CommonFields.COMPETENZA_TERRITORIALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.RUOLO_GENERALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.AVVOCATO_RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.AVVOCATO_RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.CONTRO_INTERESSATO_TERZO_CHIAMATO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(Fields.AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO, rule) != FieldStateTypesEnum.Unchanged;
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

            // Per la causa del contenzioso amministrativo, l'oggetto deve essere composto nella seguente maniera:
            // Ricorrente/Attore: {0}; Resistente/Convenuto: {1} - Evento: {2}; N° d'ordine: {3}

            const string FORMAT = "Ricorrente/Attore: {0}, Resistente/Convenuto: {1} - {2} - N° d'ordine: {3}";

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
            // Per la causa del contenzioso tributario, il body deve avere i seguenti campi:
            // - Descrizione del fascicolo
            // - Autorità giudiziaria
            // - Competenza territoriale
            // - Ruolo generale
            // - Giudice
            // - Ricorrente/Attore
            // - Avvocato ricorrente/attore
            // - Resistente/Convenuto
            // - Avvocato resistente/Convenuto
            // - Contro interessato/terzo chiamato
            // - Avvocato contro int/terzo chiamato

            formatter.AddData(CommonFields.DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description.Trim());
            formatter.AddData(CommonFields.AUTORITA_GIUDIZIARIA + ":", this.GetPropertyValueAsString(CommonFields.AUTORITA_GIUDIZIARIA));
            formatter.AddData(CommonFields.COMPETENZA_TERRITORIALE + ":", this.GetPropertyValueAsString(CommonFields.COMPETENZA_TERRITORIALE));
            formatter.AddData(Fields.RUOLO_GENERALE + ":", this.GetPropertyValueAsString(Fields.RUOLO_GENERALE));
            formatter.AddData(Fields.RICORRENTE_ATTORE + ":", this.GetPropertyValueAsString(Fields.RICORRENTE_ATTORE));
            formatter.AddData(Fields.RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(Fields.RESISTENTE_CONVENUTO));
            formatter.AddData(Fields.AVVOCATO_RICORRENTE_ATTORE + ":", this.GetPropertyValueAsString(Fields.AVVOCATO_RICORRENTE_ATTORE));
            formatter.AddData(Fields.AVVOCATO_RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(Fields.AVVOCATO_RESISTENTE_CONVENUTO));
            formatter.AddData(Fields.CONTRO_INTERESSATO_TERZO_CHIAMATO + ":" + this.GetPropertyValueAsString(Fields.CONTRO_INTERESSATO_TERZO_CHIAMATO));
            formatter.AddData(Fields.AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO + ":" + this.GetPropertyValueAsString(Fields.AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO));

            return formatter.ToString();
        }

        /// <summary>
        /// Il profilo prevede il campo "Sospensione feriale dei termini",
        /// pertanto ne viene riportato il valore
        /// </summary>
        /// <param name="rule"></param>
        protected override bool HasSospensioneFerialeTermini(BaseRuleInfo rule)
        {
            Property pSospensioneFerialeTermini = this.FindProperty(Fields.SOSPENSIONE_FERIALE);

            if (pSospensioneFerialeTermini != null && pSospensioneFerialeTermini.Value != null)
                return this.AreEquals(pSospensioneFerialeTermini.Value.ToString(), "si");
            else
                return base.HasSospensioneFerialeTermini(rule);
        }

        #endregion
    }
}
