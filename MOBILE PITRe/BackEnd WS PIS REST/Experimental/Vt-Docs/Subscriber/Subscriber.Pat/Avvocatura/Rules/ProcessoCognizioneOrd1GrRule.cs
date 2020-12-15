using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Regole di pubblicazione per i fascicoli di tipo 'Contenzioso civile'
    /// </summary>
    public class ProcessoCognizioneOrd1GrRule : AvvocaturaBaseRule
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

            // SubRules per evento "Fissata data e ora udienza di comparizione" (I udienza)
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_COMPARIZIONE);
            list.Add(SUBRULE_NAME_SCADENZA_COSTITUZIONE_IN_GIUDIZIO);
            list.Add(SUBRULE_NAME_SCADENZA_PRESENTAZIONE_AVVISO_DI_COSTITUZIONE);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA);

            // SubRules per evento "Fissata data e ora udienza di trattazione" (II udienza)
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_TRATTAZIONE);
            list.Add(SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE);
            list.Add(SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA);
            list.Add(SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_TRATTAZIONE);

            // SubRules per evento "Fissata data e ora udienza di assunzione dei mezzi istruttori" (III udienza)
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_MEZZI_ISTRUTTORI);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_MEZZI_ISTRUTTORI);

            // SubRules per evento "Fissata data e ora udienza di assunzione testi"  (IV udienza)
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI);
            list.Add(SUBRULE_NAME_REMINDER_UDIENZA_ASSUNZIONE_TESTI);
            list.Add(SUBRULE_NAME_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI);
            list.Add(SUBRULE_NAME_REMINDER_NOTIFICA_ATTO_CITAZIONE_TESTI);

            // SubRules per evento "Fissata data e ora udienza giuramento CTU"  (V udienza)
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU);
            list.Add(SUBRULE_NAME_SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_GIURAMENTO_CTU);

            // SubRules per evento "Data deposito elaborato CTU"
            list.Add(SUBRULE_NAME_REMINDER_RITIRO_ELABORATO_CTU);

            // SubRules per evento "Fissata data e ora udienza precisazione conclusioni"  (VI udienza)
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_PRECISAZIONI_CONCLUSIONI);
            list.Add(SUBRULE_NAME_SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_PRECISAZIONE_CONCLUSIONI);
            list.Add(SUBRULE_NAME_SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE);
            list.Add(SUBRULE_NAME_MEMO_RITIRO_COMPARSA_CONCLUSIONALE_ALTRE_PARTI);
            list.Add(SUBRULE_NAME_SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI);
            list.Add(SUBRULE_NAME_MEMO_RITIRO_REPLICA_ALTRE_PARTI);

            // SubRules per evento "Data desposito sentenza"
            list.Add(SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO);

            // SubRules per evento "Data desposito sentenza di primo grado"
            list.Add(SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO);
            list.Add(SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO);

            // SubRules per evento "Udienza altro tipo"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ALTRO);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ALTRO);

            // SubRules per evento "Udienza richiamo CTU"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RICHIAMO_CTU);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU);

            // SubRules per evento "Udienza camera di consiglio"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO);

            // SubRules per evento "Udienza di discussione"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_DISCUSSIONE);

            // SubRules per evento "Tentativo di conciliazione"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_TENTATIVO_DI_CONCILIAZIONE);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_TENTATIVO_DI_CONCILIAZIONE);

            // SubRules per evento "Udienza rinvio pendenti trattative"
            list.Add(SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE);
            list.Add(SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE);

            return list.ToArray();
        }

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(ProcessoCognizioneOrd1GrRule));

        /// <summary>
        /// Nome della regola
        /// </summary>
        private const string RULE_NAME = "PROCESSO_COGNIZIONE_1GR";

        /// <summary>
        /// Nome del tipo fascicolo in PITRE
        /// </summary>
        private const string TEMPLATE_NAME = "Contenzioso civile";

        /// <summary>
        /// Campo del profilo, necessario per il calcolo dei termini
        /// </summary>
        private const string FIELD_SOSPENSIONE_FERIALE = "Sospensione feriale dei termini";

        /// <summary>
        /// Campo del profilo, necessario per il calcolo dei termini
        /// </summary>
        private const string FIELD_TERMINE_APPELLO = "Termine per l'appello";

        /// <summary>
        /// Campo del profilo, necessario per il calcolo dei termini
        /// </summary>
        private const string FIELD_TERMINE_IMPUGNAZIONE = "Termine per l'impugnazione";

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
        private const string FIELD_NUMERO_ORDINE = "N° d'ordine - CIV.";

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


        #region SubRules per evento "Fissata data e ora udienza di comparizione"

        /// <summary>
        /// Sottoregola per il calcolo dell'appuntamento per l'udienza di comparizione
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_COMPARIZIONE = "APPUNTAMENTO_UDIENZA_COMPARIZIONE";

        /// <summary>
        /// Sottoregola per il calcolo dell'appuntamento per la scadenza di costituzione in giudizio
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_COSTITUZIONE_IN_GIUDIZIO = "SCADENZA_COSTITUZIONE_IN_GIUDIZIO";

        /// <summary>
        /// Sottoregola per il calcolo dell'appuntamento per la scadenza presentazione avviso di costituzione
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_PRESENTAZIONE_AVVISO_DI_COSTITUZIONE = "SCADENZA_PRESENTAZIONE_AVVISO_DI_COSTITUZIONE";

        /// <summary>
        /// Sottoregola per il calcolo dell'appuntamento relativo al rinvio dell'udienza
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA = "REMINDER_RINVIO_UDIENZA";

        #endregion

        #region SubRules per evento "Fissata udienza di trattazione"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_TRATTAZIONE = "APPUNTAMENTO_UDIENZA_TRATTAZIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE = "SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE = "REMINDER_SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA = "SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA = "REMINDER_SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA = "SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA = "REMINDER_SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_TRATTAZIONE = "REMINDER_RINVIO_UDIENZA_TRATTAZIONE";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza di assunzione dei mezzi istruttori"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_MEZZI_ISTRUTTORI = "APPUNTAMENTO_UDIENZA_MEZZI_ISTRUTTORI";

        /// <summary>
        /// Sottoregola per il calcolo dell'appuntamento relativo al rinvio dell'udienza
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_MEZZI_ISTRUTTORI = "REMINDER_RINVIO_UDIENZA_MEZZI_ISTRUTTORI";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza assunzione testi"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI = "APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_UDIENZA_ASSUNZIONE_TESTI = "REMINDER_RINVIO_UDIENZA_ASSUNZIONE_TESTI";
        
        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI = "SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_NOTIFICA_ATTO_CITAZIONE_TESTI = "REMINDER_NOTIFICA_ATTO_CITAZIONE_TESTI";
        
        #endregion

        #region SubRules per evento "Fissata data e ora udienza giuramento CTU"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU = "APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU = "REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_GIURAMENTO_CTU  = "SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_GIURAMENTO_CTU";

        #endregion

        #region SubRules per evento "Data deposito elaborato CTU"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RITIRO_ELABORATO_CTU = "REMINDER_RITIRO_ELABORATO_CTU";

        #endregion

        #region SubRules per evento "Fissata data e ora udienza precisazione conclusioni"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_PRECISAZIONI_CONCLUSIONI = "APPUNTAMENTO_UDIENZA_PRECISAZIONI_CONCLUSIONI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_PRECISAZIONE_CONCLUSIONI = "SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_PRECISAZIONE_CONCLUSIONI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE = "SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE = "REMINDER_SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_MEMO_RITIRO_COMPARSA_CONCLUSIONALE_ALTRE_PARTI = "MEMO_RITIRO_COMPARSA_CONCLUSIONALE_ALTRE_PARTI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI = "SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI = "REMINDER_SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI";
        
        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_MEMO_RITIRO_REPLICA_ALTRE_PARTI = "MEMO_RITIRO_REPLICA_ALTRE_PARTI";
        
        #endregion

        #region SubRules per evento "Data deposito sentenza"
        
        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO = "SCADENZA_PRESENTAZIONE_APPELLO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO = "REMINDER_SCADENZA_PRESENTAZIONE_APPELLO";

        #endregion

        #region SubRules per evento "Data deposito sentenza di primo grado"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO = "SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO = "REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO";
       
        #endregion

        #region SubRules per evento "Udienza richiamo CTU"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RICHIAMO_CTU = "APPUNTAMENTO_UDIENZA_RICHIAMO_CTU";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU = "REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU";

        #endregion

        #region SubRules per evento "Udienza camera di consiglio"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO = "APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO = "REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO";

        #endregion

        #region SubRules per evento "Udienza di discussione"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE = "APPUNTAMENTO_UDIENZA_DISCUSSIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_DISCUSSIONE = "REMINDER_RINVIO_UDIENZA_DISCUSSIONE";

        #endregion

        #region SubRules per evento "Tentativo di conciliazione"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_TENTATIVO_DI_CONCILIAZIONE = "APPUNTAMENTO_TENTATIVO_DI_CONCILIAZIONE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_TENTATIVO_DI_CONCILIAZIONE = "REMINDER_RINVIO_TENTATIVO_DI_CONCILIAZIONE";

        #endregion

        #region SubRules per evento "Udienza rinvio pendenti trattative"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE = "APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE = "REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE";

        #endregion

        #region SubRules per evento "Altra udienza"

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ALTRO = "APPUNTAMENTO_UDIENZA_ALTRO";

        /// <summary>
        /// 
        /// </summary>
        private const string SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ALTRO = "REMINDER_RINVIO_UDIENZA_ALTRO";

        #endregion

        /// <summary>
        /// Reperimento del nome del template
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplateName()
        {
            return TEMPLATE_NAME;
        }

        /// <summary>
        /// Esecuzione di una SubRule
        /// </summary>
        /// <param name="subRule"></param>
        protected override void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            // NB. Le istruzioni IF sono state definite in modo così prolisso solo per chiarezza espositiva


            if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_COMPARIZIONE) ||
                this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_COSTITUZIONE_IN_GIUDIZIO ) ||
                this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_PRESENTAZIONE_AVVISO_DI_COSTITUZIONE ) ||
                this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA))
            {
                // Analisi delle SubRules relative all'evento "Fissata data e ora udienza di comparizione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_TRATTAZIONE ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_PRECISAZIONE ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_ISTRUTTORIA ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_MEMORIA_PROVA_CONTRARIA ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_TRATTAZIONE))
            {
                // Analisi delle SubRules relative all'evento "Fissata data e ora udienza di trattazione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_MEZZI_ISTRUTTORI ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_MEZZI_ISTRUTTORI))
            {
                // Analisi delle SubRules relative all'evento "Fissata data e ora udienza di assunzione dei mezzi istruttori"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ASSUNZIONE_TESTI ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_UDIENZA_ASSUNZIONE_TESTI ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_NOTIFICA_ATTO_CITAZIONE_TESTI ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_NOTIFICA_ATTO_CITAZIONE_TESTI))
            {
                // SubRules per evento "Fissata data e ora udienza assunzione testi"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_GIURAMENTO_CTU ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_GIURAMENTO_CTU ) ||
                    this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_GIURAMENTO_CTU))
            {
                // SubRules per evento "Fissata data e ora udienza giuramento CTU"  (V udienza)
                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_PRECISAZIONI_CONCLUSIONI ) ||
                        this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_REDAZIONE_RELAZIONE_UDIENZA_PRECISAZIONE_CONCLUSIONI ) ||
                        this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE ) ||
                        this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_SCADENZA_DEPOSITO_SCAMBIO_COMPARSA_CONCLUSIONALE ) ||
                        this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_MEMO_RITIRO_COMPARSA_CONCLUSIONALE_ALTRE_PARTI ) ||
                        this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI ) ||
                        this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_SCADENZA_REPLICA_PRECISAZIONE_CONCLUSIONI ) ||
                        this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_MEMO_RITIRO_REPLICA_ALTRE_PARTI))
            {
                // SubRules per evento "Fissata data e ora udienza precisazione conclusioni"  (VI udienza)

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
            else if (this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_APPUNTAMENTO_UDIENZA_ALTRO ) ||
                     this.AreEquals(subRule.SubRuleName, SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_ALTRO))
            {
                // SubRules per evento "Udienza altro tipo"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RICHIAMO_CTU ) ||
                     this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RICHIAMO_CTU))
            {
                // SubRules per evento "Udienza richiamo CTU"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_CAMERA_CONSIGLIO ) ||
                     this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_CAMERA_CONSIGLIO))
            {
                // SubRules per evento "Udienza camera di consiglio"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_DISCUSSIONE ) ||
                     this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_DISCUSSIONE))
            {
                // SubRules per evento "Udienza di discussione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_TENTATIVO_DI_CONCILIAZIONE ) ||
                     this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_TENTATIVO_DI_CONCILIAZIONE))
            {
                // SubRules per evento "Tentativo di conciliazione"

                this.ExecuteUdienzaRule(subRule);
            }
            else if (this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_APPUNTAMENTO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE ) ||
                     this.AreEquals(subRule.SubRuleName , SUBRULE_NAME_REMINDER_RINVIO_UDIENZA_RINVIO_PENDENTI_TRATTATIVE))
            {
                // SubRules per evento "Udienza rinvio pendenti trattative"

                this.ExecuteUdienzaRule(subRule);
            }
            else
            {
                // Esecuzione altre regole associate alle date

                base.InternalExecuteSubRule(subRule);
            }
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
            else if (name == SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO)
            {
                // Regola "Scadenza presentazione appello (con decadimento termine lungo precedente)":
                // La decorrenza dei termini è stabilita, a partire dal valore del campo 
                // "Data notifica sentenza", in base alla scelta del campo radio button "Termine per l'impugnazione" 

                Property pTerminePerImpugnazione = this.FindProperty(FIELD_TERMINE_IMPUGNAZIONE);

                if (pTerminePerImpugnazione != null && pTerminePerImpugnazione.Value != null)
                {
                    const string VALUE_APPELLO = "Appello";
                    const string VALUE_CASSAZIONE = "Cassazione";

                    if (this.AreEquals(pTerminePerImpugnazione.Value.ToString(), VALUE_APPELLO.ToString()))
                    {
                        opts.Decorrenza = 30;
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Giorni;
                    }
                    else if (this.AreEquals(pTerminePerImpugnazione.Value.ToString(), VALUE_CASSAZIONE.ToString()))
                    {
                        opts.Decorrenza = 60;
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Giorni;
                    }
                    else
                    {
                        // Valore non valido per il campo
                        throw new Subscriber.SubscriberException(ErrorCodes.INVALID_FIELD_VALUE, string.Format(ErrorDescriptions.INVALID_FIELD_VALUE, pTerminePerImpugnazione.Value.ToString(), FIELD_TERMINE_IMPUGNAZIONE));
                    }
                }
                else
                {
                    // Campo non presente nel profilo
                    throw new Subscriber.SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, FIELD_TERMINE_IMPUGNAZIONE));
                }
            }
            else if (name == SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO)
            {
                // Regola "Reminder scadenza presentazione appello (con decadimento termine lungo precedente)"

                Property pTerminePerImpugnazione = this.FindProperty(FIELD_TERMINE_IMPUGNAZIONE);

                if (pTerminePerImpugnazione != null && pTerminePerImpugnazione.Value != null)
                {
                    const string VALUE_APPELLO = "Appello";
                    const string VALUE_CASSAZIONE = "Cassazione";

                    if (this.AreEquals(pTerminePerImpugnazione.Value.ToString(), VALUE_APPELLO.ToString()))
                    {
                        opts.Decorrenza = 20;   // Appuntamento 10gg prima
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Giorni;
                    }
                    else if (this.AreEquals(pTerminePerImpugnazione.Value.ToString(), VALUE_CASSAZIONE.ToString()))
                    {
                        opts.Decorrenza = 50;   // Appuntamento 10gg prima
                        opts.TipoDecorrenza = AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Giorni;
                    }
                    else
                    {
                        // Valore non valido per il campo
                        throw new Subscriber.SubscriberException(ErrorCodes.INVALID_FIELD_VALUE, string.Format(ErrorDescriptions.INVALID_FIELD_VALUE, pTerminePerImpugnazione.Value.ToString(), FIELD_TERMINE_IMPUGNAZIONE));
                    }
                }
                else
                {
                    // Campo non presente nel profilo
                    throw new Subscriber.SubscriberException(ErrorCodes.MISSING_FIELD_VALUE, string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, FIELD_TERMINE_IMPUGNAZIONE));
                }
            }

            return opts;
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

                    if (!isDirty)
                    {
                        if (this.AreEquals(((SubRuleInfo)rule).SubRuleName, SUBRULE_NAME_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO) ||
                            this.AreEquals(((SubRuleInfo)rule).SubRuleName, SUBRULE_NAME_REMINDER_SCADENZA_PRESENTAZIONE_APPELLO_CON_DECADIMENTO))
                        {
                            // Determina lo stato del campo Termine per l'impugnazione
                            FieldStateTypesEnum stateTermineImpugnazione = this.GetFieldState(FIELD_TERMINE_IMPUGNAZIONE, rule);

                            isDirty = (stateTermineImpugnazione == FieldStateTypesEnum.Changed);
                        }
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
        /// <param name="computedAppointmentDate"></param>
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