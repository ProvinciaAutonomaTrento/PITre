using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// 
    /// </summary>
    public class RitoLavoro1gr2grRule : AvvocaturaBaseRule
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

            // SubRules per evento "Fissata data e ora udienza discussione 420 / 436 c.p.c."
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE_420_436);
            list.Add(SUBRULE_NAME_SCADENZA_COSTITUZIONE_IN_GIUDIZIO);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_AVVISO_COSTITUZIONE);
            list.Add(SUBRULE_NAME_REMINDER_UDIENZA_DISCUSSIONE_420_436);

            // SubRules per evento "Fissata data e ora udienza di discussione"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI);
            list.Add(SUBRULE_NAME_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI);

            // SubRules per evento "Fissata data e ora udienza giuramento CTU"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU);

            // SubRules per evento "Fissata data e ora udienza richiamo CTU"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RICHIAMO_CTU);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU);

            // SubRules per evento "Fissata data e ora tentativo di conciliazione"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_TENTATIVO_CONCILIAZIONE);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_TENTATIVO_CONCILIAZIONE);

            // SubRules per evento "Fissata data e ora udienza rinvio pendenti trattative"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE);

            // SubRules per evento "Fissata data e ora udienza altro"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ALTRO);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ALTRO);

            // SubRules per evento "Fissata data e ora udienza di assunzione dei mezzi di prova"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_MEZZI_PROVA);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ASSUNZIONE_MEZZI_PROVA);

            // SubRules per evento "Data deposito elaborato CTU"
            list.Add(SUBRULE_NAME_REMINDER_RITIRO_ELABORATO_CTU);

            // SubRules per evento "Fissata data e ora udienza di discussione"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE);
            list.Add(SUBRULE_NAME_SCADENZA_DEPOSITO_NOTE_DIFENSIVE);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_NOTE_DIFENSIVE);
            list.Add(SUBRULE_NAME_REMINDER_RITIRO_NOTE_DIFENSIVE_ALTRE_PARTI);

            // SubRules per evento "Fissata data e ora udienza prima comparizione"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_PRIMA_COMPARIZIONE);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_PRIMA_COMPARIZIONE);

            // SubRules per evento "Data pubblicazione sentenza"
            list.Add(SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO);

            // SubRules per evento "Data notifica sentenza"
            list.Add(SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO);

            return list.ToArray();
        }
        
        #region Private members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(RitoLavoro1gr2grRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "RITO_LAVORO_1GR_2GR";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Contenzioso del lavoro";

        /// <summary>
        /// Campo del profilo, necessario per il calcolo dei termini
        /// </summary>
        private const string FIELD_SOSPENSIONE_FERIALE = "Sospensione feriale dei termini";

        /// <summary>
        /// Campo del profilo, riportato nell'oggetto mail
        /// </summary>
        private const string FIELD_RICORRENTE_ATTORE = "Ricorrente/Attore";

        /// <summary>
        /// Campo del profilo, riportato nell'oggetto mail
        /// </summary>
        private const string FIELD_RESISTENTE_CONVENUTO = "Resistente/Convenuto";

        /// <summary>
        /// Campo del profilo, riportato nell'oggetto mail
        /// </summary>
        private const string FIELD_NUMERO_ORDINE = "N° d'ordine - LAV.";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DESCRIZIONE_FASCICOLO = "Descrizione del fascicolo";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_RUOLO_GENERALE = "Ruolo generale";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_GIUDICE = "Giudice";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_AVVOCATO_RICORRENTE_ATTORE = "Avvocato ricorrente/attore";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_AVVOCATO_RESISTENTE_CONVENUTO = "Avvocato resistente/Convenuto";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO = "Contro interessato/terzo chiamato";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO = "Avvocato contro int/terzo chiamato";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DATA_NOTIFICA_SENTENZA = "Data notifica sentenza";

        /// <summary>
        /// 
        /// </summary>
        private const string FIELD_DATA_PUBBLICAZIONE_SENTENZA = "Data pubblicazione sentenza";

        /// <summary>
        /// Campo del profilo, necessario per il calcolo dei termini
        /// </summary>
        private const string FIELD_TERMINE_APPELLO = "Termine per l'appello";


        #region SubRules per evento "Fissata data e ora udienza discussione 420 / 436 c.p.c."

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE_420_436 = "APPUNTAMENTO_UDIENZA_DISCUSSIONE_420_436";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_COSTITUZIONE_IN_GIUDIZIO = "SCADENZA_COSTITUZIONE_IN_GIUDIZIO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_AVVISO_COSTITUZIONE = "REMINDER_SCADENZA_PRESENTAZIONE_AVVISO_COSTITUZIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_UDIENZA_DISCUSSIONE_420_436 = "REMINDER_UDIENZA_DISCUSSIONE_420_436";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza assunzione testi"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI = "APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI = "REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI = "SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI = "REMINDER_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza assunzione testi"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU = "APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU = "REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU";

        #endregion

        #region SubRules per evento "Fissata data e ora tentativo di conciliazione"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_TENTATIVO_CONCILIAZIONE = "APPUNTAMENTO_TENTATIVO_CONCILIAZIONE";
        
        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_TENTATIVO_CONCILIAZIONE = "REMINDER_RINVIO_TENTATIVO_CONCILIAZIONE";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza rinvio pendenti trattative"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE = "APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE = "REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza altro"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ALTRO = "APPUNTAMENTO_UDIENZA_ALTRO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ALTRO = "REMINDER_RINVIO_UDIENZA_ALTRO";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza assunzione dei mezzi di prova"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_MEZZI_PROVA = "APPUNTAMENTO_UDIENZA_ASSUNZIONE_MEZZI_PROVA";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ASSUNZIONE_MEZZI_PROVA = "REMINDER_RINVIO_UDIENZA_ASSUNZIONE_MEZZI_PROVA";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza prima comparizione"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_PRIMA_COMPARIZIONE = "APPUNTAMENTO_UDIENZA_PRIMA_COMPARIZIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_PRIMA_COMPARIZIONE = "REMINDER_RINVIO_UDIENZA_PRIMA_COMPARIZIONE";

        #endregion

        #region SubRules per evento "Data deposito elaborato CTU"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RITIRO_ELABORATO_CTU = "REMINDER_RITIRO_ELABORATO_CTU";

        #endregion

        #region SubRules per evento "Data ora udienza di discussione"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE = "APPUNTAMENTO_UDIENZA_DISCUSSIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_DEPOSITO_NOTE_DIFENSIVE = "SCADENZA_DEPOSITO_NOTE_DIFENSIVE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_NOTE_DIFENSIVE = "REMINDER_SCADENZA_DEPOSITO_NOTE_DIFENSIVE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RITIRO_NOTE_DIFENSIVE_ALTRE_PARTI = "REMINDER_RITIRO_NOTE_DIFENSIVE_ALTRE_PARTI";

        #endregion

        #region SubRules per evento "Data e ora udienza richiamo CTU"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RICHIAMO_CTU = "APPUNTAMENTO_UDIENZA_RICHIAMO_CTU";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU = "REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU";

        #endregion

        #region SubRules per evento "Data pubblicazione sentenza"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO = "SCADENZA_PRESENTAZIONE_APPELLO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO = "REMINDER_SCADENZA_PRESENTAZIONE_APPELLO";

        #endregion

        #region SubRules per evento "Data notifica sentenza"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO = "SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO = "REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO";

        #endregion


        /// <summary>
        /// Nome del template
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplateName()
        {
            return TEMPLATE_NAME;
        }

        /// <summary>
        /// Reperimento delle opzioni tipizzate per una regola dell'avvocatura
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        /// <remarks>
        /// La sovrascrittura è un'estensione del reperimento delle opzioni per la singola
        /// regola. Necessaria per alcuni eventi in cui, staticamente, non è possibile
        /// determinare la decorrenza dei giorni / mesi per il calcolo della scadenza, 
        /// ma tale decorrenza può essere stabilita solamente tramite la 
        /// scelta dell'utente su altri campi dipendenti
        /// </remarks>
        protected override AvvocaturaBaseRuleOptions GetAvvocaturaOptions(BaseRuleInfo rule)
        {
            AvvocaturaBaseRuleOptions opts = base.GetAvvocaturaOptions(rule);

            string name = string.Empty;

            // Reperimento del nome della regola / sottoregola
            if (rule.GetType() == typeof(SubRuleInfo))
                name = ((SubRuleInfo)rule).SubRuleName;
            else
                name = rule.RuleName;

            if (this.AreEquals(name, SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO))
            {
                // Regola "Scadenza presentazione appello":
                // La decorrenza dei termini è stabilita, a partire dal valore del campo 
                // "Data pubblicazione sentenza", in base alla scelta del campo radio button "Termine per l'appello" 

                Property pTerminePerAppello = this.FindProperty(FIELD_TERMINE_APPELLO);

                if (pTerminePerAppello != null && pTerminePerAppello.Value != null)
                {
                    const string VALUE_6_MESI = "6 mesi";
                    const string VALUE_12_MESI = "12 mesi";

                    if (this.AreEquals(pTerminePerAppello.Value.ToString(), VALUE_6_MESI.ToString()))
                    {
                        opts.Decorrenza = 6;
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                    }
                    else if (this.AreEquals(pTerminePerAppello.Value.ToString(), VALUE_12_MESI.ToString()))
                    {
                        opts.Decorrenza = 12;
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                    }
                    else
                    {
                        // Valore non valido per il campo
                        throw new Subscriber.SubscriberException(ErrorCodes.INVALID_FIELD_VALUE, string.Format(ErrorDescriptions.INVALID_FIELD_VALUE, FIELD_TERMINE_APPELLO));
                    }
                }
                else
                {
                    // Campo non presente nel profilo
                    throw new Subscriber.SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, FIELD_TERMINE_APPELLO));
                }
            }
            else if (name == SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO)
            {
                // Regola "Reminder scadenza presentazione appello"

                Property pTerminePerAppello = this.FindProperty(FIELD_TERMINE_APPELLO);

                if (pTerminePerAppello != null && pTerminePerAppello.Value != null)
                {
                    const string VALUE_6_MESI = "6 mesi";
                    const string VALUE_12_MESI = "12 mesi";

                    if (this.AreEquals(pTerminePerAppello.Value.ToString(), VALUE_6_MESI.ToString()))
                    {
                        opts.Decorrenza = 5;    // Appuntamento 30 gg prima
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                    }
                    else if (this.AreEquals(pTerminePerAppello.Value.ToString(), VALUE_12_MESI.ToString()))
                    {
                        opts.Decorrenza = 11;   // Appuntamento 30 gg prima
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi;
                    }
                    else
                    {
                        // Valore non valido per il campo
                        throw new Subscriber.SubscriberException(ErrorCodes.INVALID_FIELD_VALUE, string.Format(ErrorDescriptions.INVALID_FIELD_VALUE, FIELD_TERMINE_APPELLO));
                    }
                }
                else
                {
                    // Campo non presente nel profilo
                    throw new Subscriber.SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, FIELD_TERMINE_APPELLO));
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
            if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE_420_436) ||
                this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_SCADENZA_COSTITUZIONE_IN_GIUDIZIO) ||
                this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_AVVISO_COSTITUZIONE) ||
                this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_UDIENZA_DISCUSSIONE_420_436))
            {
                // Analisi delle SubRules per evento "Fissata data e ora udienza discussione 420 / 436 c.p.c."

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI) ||
                    this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI) ||
                    this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI) ||
                    this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI))
            {
                // SubRules per evento "Fissata data e ora udienza di discussione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU) ||
                    this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU))
            {
                // SubRules per evento "Fissata data e ora udienza giuramento CTU"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE) ||
                    this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_SCADENZA_DEPOSITO_NOTE_DIFENSIVE) ||
                this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_NOTE_DIFENSIVE) ||
                this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RITIRO_NOTE_DIFENSIVE_ALTRE_PARTI))
            {
                // SubRules per evento "Fissata data e ora udienza di discussione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RICHIAMO_CTU) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU))
            {
                // SubRules per evento "Fissata data e ora udienza richiamo CTU"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_TENTATIVO_CONCILIAZIONE) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_TENTATIVO_CONCILIAZIONE))
            {
                // SubRules per evento "Fissata data e ora tentativo di conciliazione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE))
            {
                // SubRules per evento "Fissata data e ora udienza rinvio pendenti trattative"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ALTRO) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ALTRO))
            {
                // SubRules per evento "Fissata data e ora udienza altro"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_MEZZI_PROVA) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ASSUNZIONE_MEZZI_PROVA))
            {
                // SubRules per evento "Fissata data e ora udienza assunzione dei mezzi di prova"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_PRIMA_COMPARIZIONE) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_PRIMA_COMPARIZIONE))
            {
                // SubRules per evento "Fissata data e ora udienza prima comparizione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO))
            {
                // Verifica la presenza della "Data notifica sentenza" (regola SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO).
                // Nel caso sia presente, la regola legata alla "Data pubblicazione sentenza" non dovrà essere eseguita,
                // quindi NON dovranno essere generati appuntamenti per quest'ultima. Piuttosto sarà necessario
                // inviare un appuntamento di cancellazione qualora non sia stato già inviato.

                this.ExecuteDateRuleAlternativeFields(subRule, FIELD_DATA_NOTIFICA_SENTENZA, FIELD_DATA_PUBBLICAZIONE_SENTENZA);
            }
            else
            {
                // Per tutte le altre regole

                base.InternalExecuteSubRule(subRule);
            }
        }

        /// <summary>
        /// Il profilo prevede il campo "Sospensione feriale dei termini",
        /// pertanto ne viene riportato il valore
        /// </summary>
        /// <param name="rule"></param>
        protected override bool HasSospensioneFerialeTermini(BaseRuleInfo rule)
        {
            Property pSospensioneFerialeTermini = this.FindProperty(FIELD_SOSPENSIONE_FERIALE);

            if (pSospensioneFerialeTermini != null && pSospensioneFerialeTermini.Value != null)
                return this.AreEquals(pSospensioneFerialeTermini.Value.ToString(), "si");
            else
                return base.HasSospensioneFerialeTermini(rule);
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
                FieldStateTypesEnum stateSospensioneFeriale = this.GetFieldState(FIELD_SOSPENSIONE_FERIALE, rule);

                isDirty = (stateSospensioneFeriale == FieldStateTypesEnum.Changed);
            }

            if (!isDirty)
            {
                // Verifica se, per la causa, è stato modificato il valore del campo "Termine per l'appello".
                // Tale campo può avere valore di 6 mesi o 12 mesi e determina la decorrenza
                // temporale per la presentazione dell'appello per la regola "SCADENZA_PRESENTAZIONE_APPELLO"
                // legata al campo "Data pubblicazione sentenza"

                if (rule.GetType() == typeof(SubRuleInfo))
                {
                    if (this.AreEquals(((SubRuleInfo)rule).SubRuleName, SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO) ||
                        this.AreEquals(((SubRuleInfo)rule).SubRuleName, SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO))
                    {
                        // Determina lo stato del campo Termine per l'appello
                        FieldStateTypesEnum stateTermineAppello = this.GetFieldState(FIELD_TERMINE_APPELLO, rule);

                        isDirty = (stateTermineAppello == FieldStateTypesEnum.Changed);
                    }
                }
            }

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
                        this.GetFieldState(CommonFields.AUTORITA_GIUDIZIARIA, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(CommonFields.COMPETENZA_TERRITORIALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_RUOLO_GENERALE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_GIUDICE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_AVVOCATO_RICORRENTE_ATTORE, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_AVVOCATO_RESISTENTE_CONVENUTO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO, rule) != FieldStateTypesEnum.Unchanged ||
                        this.GetFieldState(FIELD_AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO, rule) != FieldStateTypesEnum.Unchanged;
                }
            }

            return isDirty;
        }

        /// <summary>
        /// Creazione dell'oggetto del messaggio di posta
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected override string CreateMailSubject(BaseRuleInfo rule, DateTime computedAppointmentDate)
        {
            string subject = string.Empty;

            // Per la causa del contenzioso civile, l'oggetto deve essere composto nella seguente maniera:
            // Ricorrente/Attore: {0}; Resistente/Convenuto: {1} - Evento: {2}; N° d'ordine: {3}

            const string FORMAT = "Ricorrente/Attore: {0}, Resistente/Convenuto: {1} - {2} - N° d'ordine: {3}";

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
            // Per la causa del contenzioso civile, il body deve avere i seguenti campi:
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

            formatter.AddData(FIELD_DESCRIZIONE_FASCICOLO + ":", this.ListenerRequest.EventInfo.PublishedObject.Description);
            formatter.AddData(CommonFields.AUTORITA_GIUDIZIARIA + ":", this.GetPropertyValueAsString(CommonFields.AUTORITA_GIUDIZIARIA));
            formatter.AddData(CommonFields.COMPETENZA_TERRITORIALE + ":", this.GetPropertyValueAsString(CommonFields.COMPETENZA_TERRITORIALE));
            formatter.AddData(FIELD_RUOLO_GENERALE + ":", this.GetPropertyValueAsString(FIELD_RUOLO_GENERALE));
            formatter.AddData(FIELD_GIUDICE + ":", this.GetPropertyValueAsString(FIELD_GIUDICE));
            formatter.AddData(FIELD_RICORRENTE_ATTORE + ":", this.GetPropertyValueAsString(FIELD_RICORRENTE_ATTORE));
            formatter.AddData(FIELD_AVVOCATO_RICORRENTE_ATTORE + ":", this.GetPropertyValueAsString(FIELD_AVVOCATO_RICORRENTE_ATTORE));
            formatter.AddData(FIELD_RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(FIELD_RESISTENTE_CONVENUTO));
            formatter.AddData(FIELD_AVVOCATO_RESISTENTE_CONVENUTO + ":", this.GetPropertyValueAsString(FIELD_AVVOCATO_RESISTENTE_CONVENUTO));
            formatter.AddData(FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO + ":", this.GetPropertyValueAsString(FIELD_CONTRO_INTERESSATO_TERZO_CHIAMATO));
            formatter.AddData(FIELD_AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO + ":", this.GetPropertyValueAsString(FIELD_AVVOCATO_CONTRO_INTERESSATO_TERZO_CHIAMATO));

            return formatter.ToString();
        }

        #endregion
    }
}
