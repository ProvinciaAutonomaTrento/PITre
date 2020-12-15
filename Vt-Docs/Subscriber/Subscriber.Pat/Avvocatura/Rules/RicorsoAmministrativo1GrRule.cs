using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regola per il calcolo del "Ricorso amministrativo 1° grado".
    /// 
    /// La regola è applicabile alle seguenti tipologie fascicolo:
    /// - Contensioso amministrativo
    /// 
    /// </summary>
    public class RicorsoAmministrativo1GrRule : AvvocaturaBaseRule
    {
        #region Private members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RicorsoAmministrativo1GrRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "RICORSO_AMMINISTRATIVO_1GR";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Contenzioso amministrativo";

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
            public const string NUMERO_ORDINE = "N° d'ordine - AMM.";

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
            public const string DESCRIZIONE_FASCICOLO = "Descrizione del fascicolo";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_NOTIFICA_SENTENZA = "Data notifica sentenza";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_PUBBLICAZIONE_SENTENZA = "Data pubblicazione sentenza";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_COMUNICAZIONE_DEPOSITO_ORDINANZA = "Data comunicazione deposito ordinanza";

            /// <summary>
            /// 
            /// </summary>
            public const string DATA_NOTIFICA_ORDINANZA = "Data notifica ordinanza";

            /// <summary>
            /// 
            /// </summary>
            public const string TERMINI_DIMEZZATI = "Termini dimezzati";
        }

        #region SubRules associate al campo "Data notifica atto introduttivo"

        /// <summary>
        /// Sottoregole legate alla "Notifica atto introduttivo" SENZA dimezzamento dei termini
        /// </summary>
        protected class SubRulesNotificaAttoIntrodutto
        {
            /// <summary>
            /// Termini dimezzati NON attivi
            /// </summary>
            public const string SCADENZA_COSTITUZIONE = "SCADENZA_COSTITUZIONE";

            /// <summary>
            /// Termini dimezzati NON attivi
            /// </summary>
            public const string REMINDER_SCADENZA_COSTITUZIONE = "REMINDER_SCADENZA_COSTITUZIONE";

            /// <summary>
            /// Termini dimezzati NON attivi
            /// </summary>
            public const string SCADENZA_COSTITUZIONE_RICORRENTE = "SCADENZA_COSTITUZIONE_RICORRENTE";

            /// <summary>
            /// Termini dimezzati NON attivi
            /// </summary>
            public const string REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE = "REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE";
        }

        /// <summary>
        /// Sottoregole legate alla "Notifica atto introduttivo" CON dimezzamento dei termini
        /// </summary>
        protected class SubRulesNotificaAttoIntroduttivoConTerminiDimezzati
        {
            /// <summary>
            /// REGOLA ESEGUITA SOLO SE TERMINI DIMEZZATI
            /// </summary>
            public const string SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI = "SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI";

            /// <summary>
            /// REGOLA ESEGUITA SOLO SE TERMINI DIMEZZATI
            /// </summary>
            public const string REMINDER_SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI = "REMINDER_SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI";

            /// <summary>
            /// REGOLA ESEGUITA SOLO SE TERMINI DIMEZZATI
            /// </summary>
            public const string SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI = "SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI";

            /// <summary>
            /// REGOLA ESEGUITA SOLO SE TERMINI DIMEZZATI
            /// </summary>
            public const string REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI = "REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI";

            /// <summary>
            /// REGOLA ESEGUITA SOLO SE TERMINI DIMEZZATI
            /// </summary>
            public const string SCADENZA_NOTIFICA_RICORSO_INCIDENTALE = "SCADENZA_NOTIFICA_RICORSO_INCIDENTALE";

            /// <summary>
            /// REGOLA ESEGUITA SOLO SE TERMINI DIMEZZATI
            /// </summary>
            public const string REMINDER_SCADENZA_NOTIFICA_RICORSO_INCIDENTALE = "REMINDER_SCADENZA_NOTIFICA_RICORSO_INCIDENTALE";
        }

        #endregion

        #region SubRules associate all'evento "Fissata data e ora udienza camera di consiglio"

        /// <summary>
        /// SubRules associate all'evento "Fissata data e ora udienza camera di consiglio"
        /// </summary>
        protected class SubRulesUdienzaCameraConsiglio
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO = "APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO = "REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO";
        }

        #endregion

        #region SubRules associate all'evento "Fissata data e ora udienza cautelare"

        /// <summary>
        /// SubRules associate all'evento "Fissata data e ora udienza cautelare"
        /// </summary>
        protected class SubRulesUdienzaCautelare
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_CAUTELARE = "APPUNTAMENTO_UDIENZA_CAUTELARE";

            /// <summary>
            /// 
            /// <remarks>
            /// Unica scadenza che non deve tener conto della sospensione feriale
            /// </remarks>
            /// </summary>
            public const string SCADENZA_COSTITUZIONE_UDIENZA_CAUTELARE = "SCADENZA_COSTITUZIONE_UDIENZA_CAUTELARE";
        }

        #endregion

        #region SubRules associate all'evento "Data comunicazione deposito ordinanza"

        /// <summary>
        /// SubRules associate all'evento "Data comunicazione deposito ordinanza"
        /// </summary>
        protected class SubRulesComunicazioneDepositoOrdinanza
        {
            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_PRESENTAZIONE_APPELLO_DEPOSITO_ORDINANZA = "SCADENZA_PRESENTAZIONE_APPELLO_DEPOSITO_ORDINANZA";
        }

        #endregion

        #region SubRules associate all'evento "Data notifica ordinanza"

        /// <summary>
        /// SubRules associate all'evento "Data notifica ordinanza"
        /// </summary>
        protected class SubRulesComunicazioneNotificaOrdinanza
        {
            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_DEPOSITO_MEMORIA_NOTIFICA_ORDINANZA = "SCADENZA_DEPOSITO_MEMORIA_NOTIFICA_ORDINANZA";
        }
        
        #endregion

        #region SubRules associate all'evento "Data e ora udienza pubblica"

        /// <summary>
        /// SubRules associate all'evento "Data e ora udienza pubblica"
        /// </summary>
        protected class SubRulesUdienzaPubblica
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_PUBBLICA = "APPUNTAMENTO_UDIENZA_PUBBLICA";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RINVIO_UDIENZA_PUBBLICA = "REMINDER_RINVIO_UDIENZA_PUBBLICA";

            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_DEPOSITO_DOCUMENTI = "SCADENZA_DEPOSITO_DOCUMENTI";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_SCADENZA_DEPOSITO_DOCUMENTI = "REMINDER_SCADENZA_DEPOSITO_DOCUMENTI";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RITIRO_DOCUMENTI_CONTROPARTI = "REMINDER_RITIRO_DOCUMENTI_CONTROPARTI";

            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_DEPOSITO_MEMORIA = "SCADENZA_DEPOSITO_MEMORIA";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_DEPOSITO_MEMORIA_CONTROPARTI = "REMINDER_DEPOSITO_MEMORIA_CONTROPARTI";

            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_DEPOSITO_REPLICA = "SCADENZA_DEPOSITO_REPLICA";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RITIRO_REPLICHE = "REMINDER_RITIRO_REPLICHE";
        }

        #endregion

        #region SubRules associate all'evento "Data e ora udienza assunzione testi"

        /// <summary>
        /// SubRules associate all'evento "Data e ora udienza assunzione testi"
        /// </summary>
        protected class SubRulesUdienzaAssunzioneTesti
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI = "APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI = "REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI";
        }

        #endregion

        #region SubRules associate all'evento "Data e ora udienza giuramento CTU"

        /// <summary>
        /// SubRules associate all'evento "Data e ora udienza giuramento CTU"
        /// </summary>
        protected class SubRulesUdienzaGiuramentoCTU
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU = "APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU = "REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU";
        }

        #endregion

        #region SubRules associate all'evento "Data e ora udienza richiamo CTU"

        /// <summary>
        /// SubRules associate all'evento "Data e ora udienza richiamo CTU"
        /// </summary>
        protected class SubRulesUdienzaRichiamoCTU
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_RICHIAMO_CTU = "APPUNTAMENTO_UDIENZA_RICHIAMO_CTU";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU = "REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU";
        }

        #endregion

        #region SubRules associate all'evento "Data e ora udienza rinvio pendenti trattative"

        /// <summary>
        /// SubRules associate all'evento "Data e ora udienza rinvio pendenti trattative"
        /// </summary>
        protected class SubRulesUdienzaRinvioPendentiTrattative
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE = "APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE = "REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE";
        }

        #endregion

        #region SubRules associate all'evento "Data e ora udienza altro"

        /// <summary>
        /// SubRules associate all'evento "Data e ora udienza altro"
        /// </summary>
        protected class SubRulesUdienzaAltro
        {
            /// <summary>
            /// 
            /// </summary>
            public const string APPUNTAMENTO_UDIENZA_ALTRO = "APPUNTAMENTO_UDIENZA_ALTRO";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_RINVIO_UDIENZA_ALTRO = "REMINDER_RINVIO_UDIENZA_ALTRO";
        }

        #endregion

        #region SubRules associate all'evento "Data pubblicazione sentenza"

        /// <summary>
        /// SubRules associate all'evento "Data pubblicazione sentenza"
        /// </summary>
        protected class SubRulesPubblicazioneSentenza
        {
            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_PRESENTAZIONE_APPELLO = "SCADENZA_PRESENTAZIONE_APPELLO";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_SCADENZA_PRESENTAZIONE_APPELLO = "REMINDER_SCADENZA_PRESENTAZIONE_APPELLO";
        }

        #endregion

        #region SubRules associate all'evento "Data notifica sentenza"

        /// <summary>
        /// SubRules associate all'evento "Data notifica sentenza"
        /// </summary>
        protected class SubRulesNotificaSentenza
        {
            /// <summary>
            /// 
            /// </summary>
            public const string SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA = "SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA";

            /// <summary>
            /// 
            /// </summary>
            public const string REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA = "REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA";
        }
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplateName()
        {
            return TEMPLATE_NAME;
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

            if (this.AreEquals(ruleName, SubRulesUdienzaCautelare.SCADENZA_COSTITUZIONE_UDIENZA_CAUTELARE))
            {
                // Unica scadenza che non deve tener conto della sospensione feriale
                return false;
            }
            else
            {
                Property pSospensioneFerialeTermini = this.FindProperty(Fields.SOSPENSIONE_FERIALE);

                if (pSospensioneFerialeTermini != null && pSospensioneFerialeTermini.Value != null)
                    return this.AreEquals(pSospensioneFerialeTermini.Value.ToString(), "si");
                else
                    return base.HasSospensioneFerialeTermini(rule);
            }
        }

        /// <summary>
        /// Indica se la regola ammette il dimezzamento dei termini delle scadenze sulla base del valore inserito nel profilo
        /// </summary>
        /// <param name="rule"></param>
        /// <remarks>
        /// Se la regola è un promemoria antecedente ad una scadenza importante,
        /// il dimezzamento in quel caso non dovrà essere calcolato.
        /// Piuttosto dovrà mantenersi il calcolo (es. 10gg prima).
        /// </remarks>
        /// <returns></returns>
        protected virtual bool RuleHasTerminiDimezzati(BaseRuleInfo rule)
        {
            string ruleName = string.Empty;

            if (rule.GetType() == typeof(SubRuleInfo))
                ruleName = ((SubRuleInfo)rule).SubRuleName;
            else
                ruleName = rule.RuleName;

            if (this.AreEquals(ruleName, SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_DOCUMENTI) ||
                this.AreEquals(ruleName, SubRulesUdienzaPubblica.REMINDER_SCADENZA_DEPOSITO_DOCUMENTI) ||
                this.AreEquals(ruleName, SubRulesUdienzaPubblica.REMINDER_RITIRO_DOCUMENTI_CONTROPARTI) ||
                this.AreEquals(ruleName, SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_MEMORIA) ||
                this.AreEquals(ruleName, SubRulesUdienzaPubblica.REMINDER_DEPOSITO_MEMORIA_CONTROPARTI) ||
                this.AreEquals(ruleName, SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_REPLICA) ||
                this.AreEquals(ruleName, SubRulesUdienzaPubblica.REMINDER_RITIRO_REPLICHE) ||
                this.AreEquals(ruleName, SubRulesPubblicazioneSentenza.SCADENZA_PRESENTAZIONE_APPELLO) ||
                this.AreEquals(ruleName, SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_PRESENTAZIONE_APPELLO) ||
                this.AreEquals(ruleName, SubRulesNotificaSentenza.SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA) ||
                this.AreEquals(ruleName, SubRulesNotificaSentenza.REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA))
            {
                // Casi un cui la regola prevede il dimezzamento dei termini

                return this.TerminiDimezzati;
            }
            else
            {
                // Casi in cui le regole non prevedono il dimezzamento dei termini
                return false;
            }
        }

        /// <summary>
        /// Reperimento del valore del campo "Termini dimezzati" del profilo
        /// </summary>
        protected virtual bool TerminiDimezzati
        {
            get
            {
                Property pTerminiDimezzati = this.FindProperty(Fields.TERMINI_DIMEZZATI);

                if (pTerminiDimezzati != null && pTerminiDimezzati.Value != null)
                    return this.AreEquals(pTerminiDimezzati.Value.ToString(), "si");
                else
                    return false;
            }
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

            if (this.RuleHasTerminiDimezzati(rule))
            {
                // La regola prevede il dimezzamento dei termini

                if (opts.Decorrenza != 0)
                {
                    // Eccezioni
                    if (this.AreEquals(name, SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_PRESENTAZIONE_APPELLO))
                    {
                        // La regola "REMINDER_SCADENZA_PRESENTAZIONE_APPELLO" normalmente prevede il 
                        // termine di 5 mesi (1 mese dopo il termine presentazione appello, ovvero 6 mesi).
                        // In caso di rito abbreviato, il termine presentazione appello sarà 3 mesi, 
                        // il reminder va calcolato a 2 mesi piuttosto che 3 (con l'arrotondamento per eccesso)
                        opts.Decorrenza = 2;
                    }
                    else
                    {
                        if ((opts.Decorrenza % 2) != 0)
                        {
                            if (opts.Decorrenza < 0)
                            {
                                // Arrotondamento per eccesso
                                opts.Decorrenza = (opts.Decorrenza / 2) - 1;
                            }
                            else if (opts.Decorrenza > 0)
                            {
                                // Arrotondamento per eccesso
                                opts.Decorrenza = (opts.Decorrenza / 2) + 1;
                            }
                        }
                        else
                            opts.Decorrenza = (opts.Decorrenza / 2);
                    }
                }
            }

            return opts;
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
                // Verifica se, per la causa, è stato modificato il valore del campo "Termini dimezzati"
                // rispetto alla versione precedente del profilo
                FieldStateTypesEnum terminiDimezzati = this.GetFieldState(Fields.TERMINI_DIMEZZATI, rule);

                isDirty = (terminiDimezzati != FieldStateTypesEnum.Unchanged);
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
            // Per la causa del contenzioso amministrativo, il body deve avere i seguenti campi:
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
        /// Esecuzione di una SubRule
        /// </summary>
        /// <param name="subRule"></param>
        protected override void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            if (this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntrodutto.SCADENZA_COSTITUZIONE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntrodutto.SCADENZA_COSTITUZIONE_RICORRENTE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntrodutto.REMINDER_SCADENZA_COSTITUZIONE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntrodutto.REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE))
            {
                if (this.TerminiDimezzati)
                {
                    // Se la causa prevede i termini dimezzati, è necessario inviare gli appuntamenti di cancellazione
                    // per le 4 regole di notifica atto introduttivo non previsti per i termini dimezzati

                    RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(subRule);

                    if (lastExecutedRule != null &&
                        lastExecutedRule.MailMessageSnapshot != null &&
                        lastExecutedRule.MailMessageSnapshot.Appointment != null &&
                        lastExecutedRule.MailMessageSnapshot.Appointment.Status == Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CONFIRMED)
                    {
                        // Invio appuntamento di cancellazione, solo se l'ultimo appuntamento inviato per la regola è di conferma
                        this.SendCancelAppointment(subRule);
                    }
                }
                else
                {
                    // Se i termini non sono dimezzati, gestisce normalmente la regola
                    this.ExecuteDateRule(subRule, false);
                }
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.SCADENZA_NOTIFICA_RICORSO_INCIDENTALE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.REMINDER_SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.REMINDER_SCADENZA_NOTIFICA_RICORSO_INCIDENTALE) ||
                this.AreEquals(subRule.SubRuleName, SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI))
            {
                if (!this.TerminiDimezzati)
                {
                    // Se la causa NON prevede i termini dimezzati, è necessario inviare gli appuntamenti di cancellazione
                    // per le 6 regole di notifica atto introduttivo previste per i termini dimezzati

                    RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(subRule);

                    if (lastExecutedRule != null &&
                        lastExecutedRule.MailMessageSnapshot != null &&
                        lastExecutedRule.MailMessageSnapshot.Appointment != null &&
                        lastExecutedRule.MailMessageSnapshot.Appointment.Status == Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CONFIRMED)
                    {
                        // Invio appuntamento di cancellazione, solo se l'ultimo appuntamento inviato per la regola è di conferma
                        this.SendCancelAppointment(subRule);
                    }
                }
                else
                {
                    // Se i termini sono dimezzati, gestisce normalmente la regola
                    this.ExecuteDateRule(subRule, false);
                }
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesComunicazioneDepositoOrdinanza.SCADENZA_PRESENTAZIONE_APPELLO_DEPOSITO_ORDINANZA))
            {
                // Appuntamenti derivanti dalla "Data comunicazione deposito ordinanza"

                // L'appuntamento è alternativo a quello calcolato in base al campo "Data notifica ordinanza".
                // Se presente quest'ultimo, è necessario rimuovere l'eventuale appuntamento già inserito per la "Data comunicazione deposito ordinanza"

                this.ExecuteDateRuleAlternativeFields(subRule,
                                Fields.DATA_NOTIFICA_ORDINANZA,
                                Fields.DATA_COMUNICAZIONE_DEPOSITO_ORDINANZA);
            }

            else if (this.AreEquals(subRule.SubRuleName, SubRulesPubblicazioneSentenza.SCADENZA_PRESENTAZIONE_APPELLO) ||
                     this.AreEquals(subRule.SubRuleName, SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_PRESENTAZIONE_APPELLO))
            {
                // Appuntamenti derivanti dalla "Data pubblicazione sentenza".

                // NB: (in maniera analoga al contenzioso civile)
                // Se risulta valorizzata la "Data notifica sentenza", è necessario cancellare gli appuntamenti derivanti dalla "Data pubblicazione sentenza" (termine lungo per l'appello)

                this.ExecuteDateRuleAlternativeFields(subRule,
                                Fields.DATA_NOTIFICA_SENTENZA,
                                Fields.DATA_PUBBLICAZIONE_SENTENZA);
            }
            else if (this.AreEquals(subRule.SubRuleName, SubRulesUdienzaCautelare.APPUNTAMENTO_UDIENZA_CAUTELARE) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaCautelare.SCADENZA_COSTITUZIONE_UDIENZA_CAUTELARE) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.APPUNTAMENTO_UDIENZA_PUBBLICA) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.REMINDER_RINVIO_UDIENZA_PUBBLICA) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_DOCUMENTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.REMINDER_SCADENZA_DEPOSITO_DOCUMENTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.REMINDER_RITIRO_DOCUMENTI_CONTROPARTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_MEMORIA) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.REMINDER_DEPOSITO_MEMORIA_CONTROPARTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_REPLICA) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaPubblica.REMINDER_RITIRO_REPLICHE) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAssunzioneTesti.APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAssunzioneTesti.REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaGiuramentoCTU.APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaGiuramentoCTU.REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaRichiamoCTU.APPUNTAMENTO_UDIENZA_RICHIAMO_CTU) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaRichiamoCTU.REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaRinvioPendentiTrattative.APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaRinvioPendentiTrattative.REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAltro.APPUNTAMENTO_UDIENZA_ALTRO) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaAltro.REMINDER_RINVIO_UDIENZA_ALTRO) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaCameraConsiglio.APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO) ||
                    this.AreEquals(subRule.SubRuleName, SubRulesUdienzaCameraConsiglio.REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO)
                )
            {
                // Regole associate ad uno dei 5 campi udienza del profilo

                this.ExecuteUdienzaRule(subRule);
            }
            else
            {
                // Esecuzione altre regole associate alle date

                base.InternalExecuteSubRule(subRule);
            }
        }

        #endregion

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

            // SubRules associate al campo "Data notifica atto introduttivo"
            list.Add(SubRulesNotificaAttoIntrodutto.SCADENZA_COSTITUZIONE);
            list.Add(SubRulesNotificaAttoIntrodutto.REMINDER_SCADENZA_COSTITUZIONE);
            list.Add(SubRulesNotificaAttoIntrodutto.SCADENZA_COSTITUZIONE_RICORRENTE);
            list.Add(SubRulesNotificaAttoIntrodutto.REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE);
            list.Add(SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI);
            list.Add(SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.REMINDER_SCADENZA_COSTITUZIONE_TERMINI_DIMEZZATI);
            list.Add(SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI);
            list.Add(SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.REMINDER_SCADENZA_COSTITUZIONE_RICORRENTE_TERMINI_DIMEZZATI);
            list.Add(SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.SCADENZA_NOTIFICA_RICORSO_INCIDENTALE);
            list.Add(SubRulesNotificaAttoIntroduttivoConTerminiDimezzati.REMINDER_SCADENZA_NOTIFICA_RICORSO_INCIDENTALE);

            // SubRules associate all'evento "Data comunicazione deposito ordinanza"
            list.Add(SubRulesComunicazioneDepositoOrdinanza.SCADENZA_PRESENTAZIONE_APPELLO_DEPOSITO_ORDINANZA);

            // SubRules associate all'evento "Data notifica ordinanza"
            list.Add(SubRulesComunicazioneNotificaOrdinanza.SCADENZA_DEPOSITO_MEMORIA_NOTIFICA_ORDINANZA);

            // SubRules associate all'evento "Fissata data e ora udienza camera di consiglio"
            list.Add(SubRulesUdienzaCameraConsiglio.APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO);
            list.Add(SubRulesUdienzaCameraConsiglio.REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO);

            // SubRules associate all'evento "Fissata data e ora udienza cautelare"
            list.Add(SubRulesUdienzaCautelare.APPUNTAMENTO_UDIENZA_CAUTELARE);
            list.Add(SubRulesUdienzaCautelare.SCADENZA_COSTITUZIONE_UDIENZA_CAUTELARE);

            // SubRules associate all'evento "Data e ora udienza pubblica"
            list.Add(SubRulesUdienzaPubblica.APPUNTAMENTO_UDIENZA_PUBBLICA);
            list.Add(SubRulesUdienzaPubblica.REMINDER_RINVIO_UDIENZA_PUBBLICA);
            list.Add(SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_DOCUMENTI);
            list.Add(SubRulesUdienzaPubblica.REMINDER_SCADENZA_DEPOSITO_DOCUMENTI);
            list.Add(SubRulesUdienzaPubblica.REMINDER_RITIRO_DOCUMENTI_CONTROPARTI);
            list.Add(SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_MEMORIA);
            list.Add(SubRulesUdienzaPubblica.REMINDER_DEPOSITO_MEMORIA_CONTROPARTI);
            list.Add(SubRulesUdienzaPubblica.SCADENZA_DEPOSITO_REPLICA);
            list.Add(SubRulesUdienzaPubblica.REMINDER_RITIRO_REPLICHE);
            
            // SubRules associate all'evento "Data e ora udienza assunzione testi"
            list.Add(SubRulesUdienzaAssunzioneTesti.APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI);
            list.Add(SubRulesUdienzaAssunzioneTesti.REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI);

            // SubRules associate all'evento "Data e ora udienza giuramento CTU"
            list.Add(SubRulesUdienzaGiuramentoCTU.APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU);
            list.Add(SubRulesUdienzaGiuramentoCTU.REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU);

            // SubRules associate all'evento "Data e ora udienza richiamo CTU"
            list.Add(SubRulesUdienzaRichiamoCTU.APPUNTAMENTO_UDIENZA_RICHIAMO_CTU);
            list.Add(SubRulesUdienzaRichiamoCTU.REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU);

            // SubRules associate all'evento "Data e ora udienza rinvio pendenti trattative"
            list.Add(SubRulesUdienzaRinvioPendentiTrattative.APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE);
            list.Add(SubRulesUdienzaRinvioPendentiTrattative.REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE);

            // SubRules associate all'evento "Data e ora udienza altro"
            list.Add(SubRulesUdienzaAltro.APPUNTAMENTO_UDIENZA_ALTRO);
            list.Add(SubRulesUdienzaAltro.REMINDER_RINVIO_UDIENZA_ALTRO);

            // SubRules associate all'evento "Data pubblicazione sentenza"
            list.Add(SubRulesPubblicazioneSentenza.SCADENZA_PRESENTAZIONE_APPELLO);
            list.Add(SubRulesPubblicazioneSentenza.REMINDER_SCADENZA_PRESENTAZIONE_APPELLO);

            // SubRules associate all'evento "Data notifica sentenza"
            list.Add(SubRulesNotificaSentenza.SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA);
            list.Add(SubRulesNotificaSentenza.REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_NOTIFICA_SENTENZA);

            return list.ToArray();
        }

        #endregion
    }
}
