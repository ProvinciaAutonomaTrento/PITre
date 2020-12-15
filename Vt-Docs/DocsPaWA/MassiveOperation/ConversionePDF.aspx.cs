using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;

namespace DocsPAWA.MassiveOperation
{
    public partial class ConversionePDF : MassivePage
    {
        #region Proprietà di pagina

        protected override string PageName
        {
            get {
                return "Conversione PDF Massiva";
            }
        }

        protected override bool IsFasc
        {
            get { return false; }
        }
        /// <summary>
        /// Lista delle informazioni di base sui documenti da convertire in PDF
        /// </summary>
        private List<BaseInfoDoc> DocumentsInfo
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["InfoDocumentList"] as List<BaseInfoDoc>;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["InfoDocumentList"] = value;
            }
        }

        #endregion

        #region Funzioni di utilità

        /// <summary>
        /// Funzione per l'inizializzazione della pagina
        /// </summary>
        private void Initialize()
        {
            // La lista dei system id dei documenti selezionati
            List<MassiveOperationTarget> documentsIdProfile;

            // Lista delle informazioni di base sui documenti da convertire
            List<BaseInfoDoc> documents;

            // Recupero dei system id dei documenti da convertire
            documentsIdProfile = MassiveOperationUtils.GetSelectedItems();

            // Inizializzazione della lista delle informazioni sui documenti da convertire
            documents = new List<BaseInfoDoc>();

            // Per ogni id profile, vengono recuperate le informazioni di base sul
            // documento
            foreach (MassiveOperationTarget temp in documentsIdProfile)
            {
                // ...recupero del contenuto delle informazioni di base sul documento
                try
                {
                    documents.Add(DocumentManager.GetBaseInfoForDocument(
                       temp.Id,
                       String.Empty,
                       String.Empty).Where(e => e.IdProfile.Equals(temp.Id)).FirstOrDefault());
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format(
                        "Errore durante il reperimento delle informazioni sul documento {0}", temp.Codice));

                }
            }

            // Salvataggio delle informazioni sui documenti
            this.DocumentsInfo = documents;

        }

        /// <summary>
        /// Funzione per la conversione dei documenti in PDF
        /// </summary>
        /// <param name="documentsInfo">La lista con le informazioni sui documenti da convertire</param>
        /// <returns>Il report dell'elaborazione</returns>
        public MassiveOperationReport ConvertInPdf(List<BaseInfoDoc> documentsInfo)
        {
            DocsPaWebService ws = new DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;
            // Il report da restituire
            MassiveOperationReport report;

            // Il risultato della messa in conversione di un documento
            MassiveOperationReport.MassiveOperationResultEnum tempResult;

            // Il messaggio da inserire nel report
            string message;

            // Informazioni sull'utente che ha lanciato la procedura
            InfoUtente userInfo;

            // Il file associato al documento
            byte[] content = null;

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Recupero delle informazioni sull'utente
            userInfo = UserManager.getInfoUtente(this);
            foreach (BaseInfoDoc doc in documentsInfo)
            {
                // Inizializzazione del messaggio
                string codice = MassiveOperationUtils.getItem(doc.IdProfile).Codice;
                message = "Documento inserito correttamente nella coda di conversione PDF.";

                //Recupero i diritti sul documento
                string ar = ws.getAccessRightDocBySystemID(doc.IdProfile, UserManager.getInfoUtente(this));

                // Verifica delle informazioni sul documento
                tempResult = this.ValidateDocumentInformation(doc, out message);

                if (ar.Equals("20")){
                    message = "Il documento è in attesa di accettazione, quindi non può essere convertito";
                    tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                }
                    
                //Verifico che il documento non sia consolidato
                FirmaDigitale.FirmaDigitaleMng mng = new FirmaDigitale.FirmaDigitaleMng();
                DocsPaWR.SchedaDocumento schedaDocumento = mng.GetSchedaDocumento(doc.DocNumber);

                if (schedaDocumento != null)
                {
                    if (schedaDocumento.ConsolidationState != null &&
                       schedaDocumento.ConsolidationState.State > DocsPaWR.DocumentConsolidationStateEnum.None)
                    {
                        // Il documento risulta consolidato, non può essere firmato digitalmente
                        message = "Il documento risulta consolidato";
                        tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    }
                }

                // Se il risultato di validazione delle informazioni è OK, si pruò procedere
                if (tempResult == MassiveOperationReport.MassiveOperationResultEnum.OK)
                {
                    try
                    {
                        // Recupero delle informazioni sul file da convertire
                        content = FileManager.getInstance(Session.SessionID).GetFile(
                            this,
                            doc.VersionId,
                            doc.VersionNumber.ToString(),
                            doc.DocNumber,
                            doc.Path,
                            doc.FileName,
                            false).content;
                    }
                    catch (Exception e)
                    {
                        message = "Errore durante il reperimento del file associato al documento.";
                        tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    }
                }

                // Se si può procedere si mette in coda il file da convertire
                if (tempResult == MassiveOperationReport.MassiveOperationResultEnum.OK)
                {
                    try
                    {
                        // Avvio della conversione
                        FileManager.EnqueueServerPdfConversionAM(
                            this,
                            userInfo,
                            content,
                            doc.FileName,
                            new SchedaDocumento()
                                {
                                    systemId = doc.IdProfile,
                                    docNumber = doc.DocNumber
                                });
                    }
                    catch (Exception e)
                    {
                        tempResult = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        message = "Errore durante l'inserimento del documento nella coda di conversione.";
                    }

                }
                // Inserimento di una nuova riga nel report
                report.AddReportRow(
                    codice,
                    tempResult,
                    message);

            }

            // Restituzione del report
            return report;

        }

        /// <summary>
        /// Funzione per la validazione delle informazioni sul documento
        /// </summary>
        /// <param name="documentInfo">Le informazioni sul documento</param>
        /// <param name="message">L'eventuale messaggio di errore</param>
        /// <returns>Risultato della validazione</returns>
        private MassiveOperationReport.MassiveOperationResultEnum ValidateDocumentInformation(
            BaseInfoDoc documentInfo,
            out string message)
        {
            // Il risultato da restituire
            MassiveOperationReport.MassiveOperationResultEnum toReturn;

            // Inizializzazione del risultato e del messaggio
            toReturn = MassiveOperationReport.MassiveOperationResultEnum.OK;
            message = String.Empty;

            // Se non è stato acquisito un file per il documento, il risultato è negativo
            if (!documentInfo.HaveFile)
            {
                toReturn = MassiveOperationReport.MassiveOperationResultEnum.KO;
                message = "Nessun file acquisito per il documento.";
            }
            else
                // Altrimenti se il file è già un PDF il risultato è un AlreadyWorked
                if (Path.GetExtension(documentInfo.FileName).ToUpper().Equals(".PDF"))
                {
                    toReturn = MassiveOperationReport.MassiveOperationResultEnum.AlreadyWorked;
                    message = "Documento già convertito in PDF.";
                }

            // PALUMBO: Commentato in quanto se dualFileWriting = fals, non recupera i file.
            // Se il file risulta acquisito ma non è reperibile, il risultato è negativo
            //if (toReturn == MassiveOperationReport.MassiveOperationResultEnum.OK &&
            //    !File.Exists(documentInfo.FileName))
            //{
            //    toReturn = MassiveOperationReport.MassiveOperationResultEnum.KO;
            //    message = "Impossibile recuperare il file associato al documento.";
            //}

            // Restituzione del risultato
            return toReturn;

        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Al page load viene inizializzata la pagina
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                // Inizializzazione della pagina
                try
                {
                    this.Initialize();

                }
                catch (Exception ex)
                {
                    // In caso di errore, viene mostrato un alert
                    this.showMessage(ex.Message);
                }


        }

        /// <summary>
        /// Al click sul pulsante Conferma viene avviata la conversione PDF dei documenti 
        /// selezionati
        /// </summary>
        protected override bool btnConferma_Click(object sender, EventArgs e)
        {

           //Inserito perchè altrimenti la seconda volta che si converte in pdf perde i dati
            Initialize();

            // Il report di conversione
            MassiveOperationReport report;

            // Inserimento dei documenti in coda
            report = this.ConvertInPdf(this.DocumentsInfo);

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

            // Generazione del report da esportare
            this.generateReport(report,"Conversione PDF massiva");
            return true;
        }

        #endregion
    }
}