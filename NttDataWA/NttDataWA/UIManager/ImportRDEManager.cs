using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using log4net;

namespace NttDataWA.UIManager
{

    public class ImportRDEManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(ImportDocumentManager));
        private static DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();


        public static ResultsContainer ImportRDE(byte[] content,
            string fileName,
            string serverPath,
            InfoUtente userInfo,
            Ruolo userRole)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
                isClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));

            try
            {
                // Chiamata del webservice per l'importazione RDE e restituzione 
                // della lista dei risultati
                return ws.ImportRDE(
                    content,
                    fileName,
                    serverPath,
                    userInfo,
                    userRole,
                    isClassificationRequired,
                    false,
                    IsEnabledOwnerProto() ? 2 : 1);
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati dal foglio Excel
        /// </summary>
        /// <param name="content">Il contenuto del foglio Excel</param>
        /// <param name="fileName">Il nome da attribuire al file</param>
        /// <returns>L'oggetto con i dati sui documenti da importare</returns>
        public static DocumentRowDataContainer ReadRDEDataFromExcel(
            byte[] content,
            string fileName)
        {
            // Impostazione del timeout
            ws.Timeout = System.Threading.Timeout.Infinite;

            // L'oggetto da restiture
            DocumentRowDataContainer toReturn = null;

            // L'eventuale errore avvenuto in fase di estrazione dati
            string errorMessage = String.Empty;

            try
            {
                // Chiamata del metodo web per l'estrazione dei dati dal foglio Excel
                toReturn = ws.ReadRDEDataFromExcel(
                    content,
                    fileName,
                    IsEnabledOwnerProto() ? 2 : 1,
                    out errorMessage);

                // Se si è verificato un errore un esecuzione, viene lanciata un'eccezione con il
                // dettaglio
                if (toReturn == null ||
                    !String.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);

            }
            catch (Exception e)
            {
                throw e;
            }

            return toReturn;
        }


        /// <summary>
        /// Funzione per l'importazione di un documento RDE
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati sul documento da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isSmistamentoEnabled">True se è abilito lo smistamento</param>
        /// <param name="protoType">Il tipo di documento da creare</param>
        /// <returns>Il riusltato dell'importazione</returns>
        public static ImportResult ImportRDEDocument(
            DocumentRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            ProtoType protoType)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
                isClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));

            try
            {
                // Chiamata del webservice per l'importazione RDE e restituzione 
                // della lista dei risultati
                return ws.ImportRDEDocument(
                    rowData,
                    userInfo,
                    role,
                    serverPath,
                    isClassificationRequired,
                    false,
                    false,
                    protoType);
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }

        }

        /// <summary>
        /// Funzione per il reperimento del valore associato al flag di presenza dei
        /// protocolli interni
        /// </summary>
        /// <returns>True se devono essere presi in considerazione i protocolli interni</returns>
        public static bool IsEnabledOwnerProto()
        {
            // Il valore da restituire
            bool toReturn = false;

            // Lettura del valore di configurazione
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_OWNER_PROTO_FOR_IMPORT_PROCEDURES.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_OWNER_PROTO_FOR_IMPORT_PROCEDURES.ToString()]))
                toReturn = true;

            // Restituzione del risultato
            return toReturn;

        }

    }

    /// <summary>
    /// Questa classe si occupa di effettuare l'importazione di documenti
    ///  di emergenza descritti in un array di oggetti DocumentRowData e di produrre
    /// il report di importazione.
    /// </summary>
    public class ImportRDEExecutor
    {
        #region Dichiarazione Variabili

        // L'oggetto con il report di esecuzione
        ResultsContainer report;

        // Il numero di documenti totali da importare ed il numero di
        // documenti analizzati
        int totalNumberOfDocumentToImport, totalNumberOfAnalyzedDocuments;

        #endregion

        /// <summary>
        /// Costruttore di default.
        /// </summary>
        public ImportRDEExecutor()
        {
            // Inizializzazione del report
            this.report = new ResultsContainer();

            // Inizializzazione del numero di documenti importati
            this.totalNumberOfAnalyzedDocuments = 0;

            // Inizializzazione del numero di documenti da importare
            this.totalNumberOfDocumentToImport = 0;
        }

        /// <summary>
        /// Funzione richiamata dal Thread per l'esecuzione dell'importazione
        /// </summary>
        /// <param name="prameters">Un array di oggetti. Sono attesi 4 elementi:
        ///     - Il primo deve essere un oggetto di tipo DocumentRowDataContainer
        ///     - Il secondo deve essere un oggetto di tipo string
        ///     - Il terzo deve essere un oggetto di tipo InfoUtente
        ///     - Il quarto deve essere un oggetto di tipo Ruolo
        /// </param>
        public void ExecuteImport(object parameters)
        {
            #region Dichiarazione variabili

            // L'array dei parametri
            object[] param;

            // L'oggetto con le informazioni sui documenti da importare
            DocumentRowDataContainer drdc;

            // L'url del frontend
            string url;

            // Le informazioni sull'utente che ha lanciato la procedura
            InfoUtente userInfo;

            // Il ruolo con cui è stata lanciata la procedura
            Ruolo role;

            // Lista in cui depositare temporaneamente il report
            // di importazione relativo ad una tipologia di documento
            List<ImportResult> tempReport;

            #endregion

            // Casting dell'oggetto a array di oggetti
            param = parameters as object[];

            // Se l'array non contiene quattro elementi, fine esecuzione
            if (param.Length != 4)
                return;

            // Prelevamento delle informazioni sui documenti da creare
            drdc = param[0] as DocumentRowDataContainer;

            // Prelevamento dell'url del Fontend
            url = param[1] as string;

            // Prelevamento delle informazioni sull'utente
            userInfo = param[2] as InfoUtente;

            // Prelevamento del ruolo
            role = param[3] as Ruolo;

            // Calcolo del numero totale di documenti da importare
            this.totalNumberOfDocumentToImport =
                drdc.InDocument.Length +
                drdc.OutDocument.Length +
                drdc.OwnDocument.Length;

            #region Importazione documenti in arrivo

            // Inizializzazione della lista da utulizzare per
            // memorizzare temporaneamente il report sull'importazione dei
            // documenti in ingresso
            tempReport = new List<ImportResult>();

            // Importazione dei documenti in ingresso
            foreach (DocumentRowData rowData in drdc.InDocument)
            {
                try
                {
                    tempReport.Add(ImportRDEManager.ImportRDEDocument(
                                rowData,
                                userInfo,
                                role,
                                userInfo.urlWA,
                                ProtoType.A));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new ImportResult()
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Ordinal = rowData.OrdinalNumber,
                        Message = e.Message
                    });

                }

                // Un documenti è stato analizzato
                totalNumberOfAnalyzedDocuments += 1;

            }

            // Aggiunta di una voce di report con il totale dei documenti
            // importati e non importati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i documenti in ingresso
            report.InDocument = tempReport.ToArray();

            #endregion

            #region Importazione documenti in partenza

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sull'importazione dei
            // documenti in partenza
            tempReport = new List<ImportResult>();

            // Importazione dei documenti in partenza
            foreach (DocumentRowData rowData in drdc.OutDocument)
            {
                try
                {
                    tempReport.Add(ImportRDEManager.ImportRDEDocument(
                                rowData,
                                userInfo,
                                role,
                                userInfo.urlWA,
                                ProtoType.P));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new ImportResult()
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Ordinal = rowData.OrdinalNumber,
                        Message = e.Message
                    });
                }

                // Un documenti è stato analizzato
                totalNumberOfAnalyzedDocuments += 1;

            }

            // Aggiunta di una voce di report con il totale dei documenti
            // importati e non importati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i documenti in partenza
            report.OutDocument = tempReport.ToArray();

            #endregion

            #region Importazione documenti interni

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sull'importazione dei
            // documenti interni
            tempReport = new List<ImportResult>();

            // Se non è abilitata l'importazione dei documenti interni, viene 
            // cancellata la lista dei documenti interni
            if (!ImportRDEManager.IsEnabledOwnerProto())
                drdc.OwnDocument = new DocumentRowData[0];

            // Importazione dei documenti interni
            foreach (DocumentRowData rowData in drdc.OwnDocument)
            {
                try
                {
                    tempReport.Add(ImportRDEManager.ImportRDEDocument(
                        rowData,
                        userInfo,
                        role,
                        userInfo.urlWA,
                        ProtoType.I));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new ImportResult()
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Ordinal = rowData.OrdinalNumber,
                        Message = e.Message
                    });
                }

                // Un documenti è stato analizzato
                totalNumberOfAnalyzedDocuments += 1;

            }

            // Aggiunta di una voce di report con il totale dei documenti
            // importati e non importati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i documenti interni
            report.OwnDocument = tempReport.ToArray();

            #endregion

            // Sospensione del thread fino a quando non viene letto
            // il report o non passano 1 minuto
            //waitReading.WaitOne(new TimeSpan(0,1,0));

        }

        /// <summary>
        /// Funzione per la creazione di un elemento di summary
        /// </summary>
        /// <param name="tempReport">L'array con gli elementi di cui creare un summary</param>
        /// <returns>L'elemento con il summary</returns>
        private ImportResult GetSummaryElement(List<ImportResult> tempReport)
        {
            // L'oggetto da restituire
            ImportResult toReturn;

            // Il numero di risultati negativi
            int unsuccessNumber = 0;

            // Calcolo dei risultati negativi
            unsuccessNumber = tempReport.Where(e => e.Outcome == OutcomeEnumeration.KO).Count();

            // Inizializzazione dell'elemento
            if (unsuccessNumber > 0)
            {
                toReturn = new ImportResult()
                {
                    Outcome = OutcomeEnumeration.NONE,
                    Message = String.Format(
                        "Documenti importati correttamente: {0}. Documenti non importati {1}",
                        tempReport.Count - unsuccessNumber, unsuccessNumber)
                };
            }
            else
                toReturn = new ImportResult()
                {
                    Outcome = OutcomeEnumeration.NONE,
                    Message = String.Format(
                        "Documenti importati correttamente: {0}. Documenti non importati {1}",
                        tempReport.Count - unsuccessNumber, unsuccessNumber)
                };

            // Restituzione dell'elemento creato
            return toReturn;

        }

        /// <summary>
        /// Funzione richiamabile per conoscere il numero di documenti analizzati
        /// ed il numero di documenti totali
        /// </summary>
        /// <param name="analyzed">Numero di documenti analizzati</param>
        /// <param name="total">Numero totale di documenti</param>
        public void GetStatistics(out int analyzed, out int total)
        {
            // Impostazione dei valori richiesti
            analyzed = this.totalNumberOfAnalyzedDocuments;
            total = this.totalNumberOfDocumentToImport;

        }

        /// <summary>
        /// Funzione per la restituzione del report
        /// </summary>
        /// <returns></returns>
        public ResultsContainer GetReport()
        {
            // Viene risvegliato il thread
            //waitReading.Set();

            // Numero di documenti non importati ed importati con
            // warning
            int importedWithWarningOrNotImported = 0;

            // Calcolo del numero di documenti importati con warning
            importedWithWarningOrNotImported += this.report.Attachment != null ?
                this.report.Attachment.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.GrayDocument != null ?
                this.report.GrayDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.InDocument != null ?
                this.report.InDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.OutDocument != null ?
                this.report.OutDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.OwnDocument != null ?
                this.report.OwnDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;

            ImportResult importResult = new ImportResult();

            // Aggiunta di un messaggio nel report generale
            // Se non ci sono documenti importati con warning o non importati,
            // viene aggiunto un messaggio positivo altrimenti viene aggiunto un
            // messaggio di warning
            if (this.totalNumberOfDocumentToImport == 0)
            {
                importResult.Outcome = OutcomeEnumeration.Warnings;
                importResult.Message = "Non è stato rilevato nessun documento da importare.";
            }
            else
                if (importedWithWarningOrNotImported == 0)
                    importResult.Message = "Nessun messaggio generale da mostrare.";
                else
                {
                    importResult.Message = String.Format("Attenzione! {0} documenti hanno presentato problemi durante l'importazione. Controllare gli altri tab per maggiori informazioni.",
                        importedWithWarningOrNotImported);
                    importResult.Outcome = OutcomeEnumeration.KO;

                }


            this.report.General = new ImportResult[] {
                    importResult
                };

            // Viene restituito il report
            return this.report;
        }

    }

}