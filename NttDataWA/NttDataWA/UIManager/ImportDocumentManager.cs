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
using System.Threading.Tasks;

namespace NttDataWA.UIManager
{

    public class ImportDocumentManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(ImportDocumentManager));
        private static DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();


        public static ResultsContainer ImportDocuments(byte[] content,
           string fileName,
           string serverPath,
           InfoUtente userInfo,
           Ruolo userRole)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;
            else
                isProfilationRequired = false;


            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
                isClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportDocuments(
                    content,
                    fileName,
                    serverPath,
                    userInfo,
                    userRole,
                    isProfilationRequired,
                    isClassificationRequired,
                    false,
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static DocumentRowDataContainer ReadDocumentDataFromExcelFile(
            byte[] content,
            InfoUtente userInfo,
            Ruolo role,
            bool isStampaUnione,
            out string error)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                logger.Debug("Reperimento informazioni documenti");

                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ReadDocumentDataFromExcelFile(
                    content,
                    userInfo,
                    role,
                    importazionePregressiAbilitata(),
                    isStampaUnione,
                    out error);

                logger.Debug("Informazioni sui documenti reperite.");

            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static ImportResult[] ImportDocumentsFromArray(
            DocumentRowData[] documentToImport,
            ProtoType protoType,
            string serverPath,
            InfoUtente userInfo,
            Ruolo role,
            ref ResultsContainer resultsContainer)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
                isClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportDocumentsFromArray(
                    documentToImport,
                    protoType,
                    serverPath,
                    userInfo,
                    role,
                    isProfilationRequired,
                    isClassificationRequired,
                    false,
                    ref resultsContainer,
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static ImportResult ImportDocument(
            DocumentRowData documentToImport,
            ProtoType protoType,
            string serverPath,
            InfoUtente userInfo,
            Ruolo role,
            ref ResultsContainer resultsContainer)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
                isClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportDocument(
                    documentToImport,
                    protoType,
                    serverPath,
                    userInfo,
                    role,
                    isProfilationRequired,
                    isClassificationRequired,
                    false,
                    ref resultsContainer,
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static ImportResult ImportAndAcquireDocument(
            DocumentRowData documentToImport,
            ProtoType protoType,
            string serverPath,
            InfoUtente userInfo,
            Ruolo role,
            ref ResultsContainer resultsContainer)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString())))
                isClassificationRequired = bool.Parse(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_FASC_RAPIDA_REQUIRED.ToString()));

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportAndAcquireDocument(
                    documentToImport,
                    protoType,
                    serverPath,
                    userInfo,
                    role,
                    isProfilationRequired,
                    isClassificationRequired,
                    false,
                    ref resultsContainer,
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static byte[] CreateZipFromReport(ResultsContainer report, InfoUtente userInfo)
        {
            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.CreateZipFromReport(report, userInfo);
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        /// <summary>
        /// Funzione per il reperimento del massimo numero di documenti importabili
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente</param>
        /// <returns>Il numero massimo di documenti che è possibile importare</returns>
        public static int GetMaxDocumentsNumber(InfoUtente userInfo)
        {
            int toReturn = 500;

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MAX_DOC_CAN_IMPORT.ToString()]))
                toReturn = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MAX_DOC_CAN_IMPORT.ToString()]);

            return toReturn;

        }

        /// <summary>
        /// Funzione per il reperimento della microfunzione IMP_DOC_MASSIVA_PREG nel ruolo
        /// </summary>
        /// <returns>True se bisogna considerare l'importazione documenti pregressi</returns>
        public static bool importazionePregressiAbilitata()
        {
            return UIManager.UserManager.IsAuthorizedFunctions("IMP_DOC_MASSIVA_PREG");
        }


        public static bool UploadFileOnServer(System.IO.Stream fileStream, string filename, InfoUtente infoUtente)
        {
            bool result = true;
            byte[] input;
            try
            {
                input = _convertStreamToBytreArray(fileStream);
                result = ws.UploadFileOnSharedFolder(input, filename, infoUtente);
            } catch(Exception ex)
            {
                logger.Error(ex.Message, ex);
                result = false;
            }

            return result;
        }

        public static bool UploadAttachmentsOnServer(System.IO.Stream fileStream, string filename, InfoUtente infoUtente)
        {
            bool result = true;
            byte[] input;
            try
            {
                input = _convertStreamToBytreArray(fileStream);
                result = ws.UploadAttachmentOnSharedFolder(input, filename, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                result = false;
            }

            return result;
        }

        public static bool UploadFileFormazione(System.IO.Stream fileStream, string filename, string idUO, InfoUtente infoUtente)
        {
            bool result = true;
            byte[] input;

            try
            {
                input = _convertStreamToBytreArray(fileStream);
                result = ws.UploadFileToFormazionePath(input, filename, idUO, infoUtente, false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }


            return result;
        }

        public static bool UploadTemplateFormazione(System.IO.Stream fileStream, string idUO, InfoUtente infoUtente)
        {
            bool result = true;
            byte[] input;

            try
            {
                input = _convertStreamToBytreArray(fileStream);
                result = ws.UploadTemplateFormazionePath(input, idUO, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }


            return result;
        }

        public static bool UploadAllegatiFormazione(System.IO.Stream fileStream, string filename, string idUO, InfoUtente infoUtente)
        {
            bool result = true;
            byte[] input;

            try
            {
                input = _convertStreamToBytreArray(fileStream);
                result = ws.UploadFileToFormazionePath(input, filename, idUO, infoUtente, true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }


            return result;
        }

        public static bool DeleteFileFormazione(string idUO, InfoUtente infoUtente)
        {
            bool result = true;
            byte[] input;

            try
            {
                result = ws.DeleteFileFormazione(idUO, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }


            return result;
        }




        private static byte[] _convertStreamToBytreArray(System.IO.Stream fileStream)
        {
            byte[] input;
            byte[] buffer = new byte[16 * 1024];
            System.IO.MemoryStream ms = null;
            try
            {
                ms = new System.IO.MemoryStream();
                int read;
                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                input = ms.ToArray();
                ms.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                if(ms != null) { ms.Dispose(); }                
            }

            return input;
        }
    }


    /// <summary>
    /// Questa classe si occupa di effettuare l'importazione di documenti
    /// descritti in un array di oggetti DocumentRowData e di produrre
    /// il report di importazione.
    /// </summary>
    public class ImportDocumentExecutor {

        // Il numero di documenti totali da importare ed il numero di
        // documenti analizzati
        int totalNumberOfDocumentToImport, totalNumberOfAnalyzedDocuments;

        // L'oggetto con il report di esecuzione
        ResultsContainer report;
        private delegate ImportResult ImportDelegate(DocumentRowData documentToImport,ProtoType protoType,string serverPath,InfoUtente userInfo,Ruolo role,ref ResultsContainer resultsContainer);
        private ImportDelegate _delegate;
        private bool _stampaUnione;


        /// <summary>
        /// Costruttore di default.
        /// </summary>
        public ImportDocumentExecutor(bool stampaUnione)
        {
            // Inizializzazione del report
            this.report = new ResultsContainer();

            // Inizializzazione del numero di documenti importati
            this.totalNumberOfAnalyzedDocuments = 0;

            // Inizializzazione del numero di documenti da importare
            this.totalNumberOfDocumentToImport = 0;

            // Inizializzazione dell'oggetto di sincronizzazione manuale
            if (!stampaUnione)
            {
                this._delegate = ImportDocumentManager.ImportDocument;
            }
            else
            {
                this._delegate = ImportDocumentManager.ImportAndAcquireDocument;
            }
            this._stampaUnione = stampaUnione;
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
                drdc.AttachmentDocument.Length +
                drdc.GrayDocument.Length +
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
                    tempReport.Add(_delegate(
                                rowData,
                                ProtoType.A,
                                url,
                                userInfo,
                                role,
                                ref report));
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
                    tempReport.Add(_delegate(
                                rowData,
                                ProtoType.P,
                                url,
                                userInfo,
                                role,
                                ref report));
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

            // Importazione dei documenti interni
            foreach (DocumentRowData rowData in drdc.OwnDocument)
            {
                try
                {
                    tempReport.Add(_delegate(
                                rowData,
                                ProtoType.I,
                                url,
                                userInfo,
                                role,
                                ref report));
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

            #region Importazione dei documenti grigi

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sull'importazione dei
            // documenti grigi
            tempReport = new List<ImportResult>();

            // Importazione dei documenti grigi
            foreach (DocumentRowData rowData in drdc.GrayDocument)
            {
                try
                {
                    tempReport.Add(_delegate(
                                rowData,
                                ProtoType.G,
                                url,
                                userInfo,
                                role,
                                ref report));
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

            // Salvataggio del report per i documenti grigi
            report.GrayDocument = tempReport.ToArray();

            #endregion

            #region Importazione allegati

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sull'importazione
            // degli allegati
            if (!_stampaUnione)
            {
                tempReport = new List<ImportResult>();

                // Importazione degli allegati
                foreach (DocumentRowData rowData in drdc.AttachmentDocument)
                {
                    try
                    {
                        tempReport.Add(_delegate(
                                    rowData,
                                    ProtoType.ATT,
                                    url,
                                    userInfo,
                                    role,
                                    ref report));
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

                // Salvataggio del report per gli allegati
                report.Attachment = tempReport.ToArray();
            }
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

            //Calcolo dei risultati non acquisiti
            int notAcquiredNumber = tempReport.Where(e => e.Outcome == OutcomeEnumeration.FileNotAcquired).Count();

            // Inizializzazione dell'elemento
            toReturn = new ImportResult()
            {
                Outcome = OutcomeEnumeration.NONE,
                Message = String.Format(
                    "Documenti importati correttamente: {0}. Documenti non importati {1}. Documenti importati ma non acquisiti {2}",
                    tempReport.Count - unsuccessNumber - notAcquiredNumber, unsuccessNumber, notAcquiredNumber)
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

            // Viene restituito il report
            return this.report;
        }

    }

}