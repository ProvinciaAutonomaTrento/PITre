using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.MassiveOperation
{
    public partial class TimestampMassivo : MassivePage
    {

        #region Event handler

        protected override string PageName
        {
            get
            {
                return "Timestamp Massivo";
            }
        }

        protected override bool IsFasc
        {
            get { return false; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Al click sul pulsante bisogna applicare il timestamp a tutti i documenti selezionati
        /// </summary>
        protected override bool btnConferma_Click(object sender, EventArgs e)
        {
            #region Dichiarazione variabili

            // Lista di system id degli elementi selezionati
            List<MassiveOperationTarget> selectedItem;

            // Il report da mostrare all'utente
            MassiveOperationReport report;

            #endregion

            // Inizializzazione del report
            report = new MassiveOperationReport();

            // Recupero della lista dei system id dei documenti selezionati
            selectedItem = MassiveOperationUtils.GetSelectedItems();

            // Esecuzione dell'applicazione del timestamp
            this.ApplyTimeStampToDocuments(selectedItem, report);

            // Introduzione della riga di summary
            string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
            report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

            // Visualizzazione del report e termine della fase di attesa
            this.generateReport(report,"Timestamp massivo");
            return true;
        }

        #endregion

        #region Funzioni di utilità

        /// <summary>
        /// Funzione per l'applicazione del timestamp a tutti i documenti selezionati
        /// </summary>
        /// <param name="selectedItem">Lista dei system id dei documenti selezionati</param>
        /// <param name="report">Report di esecuzione</param>
        private void ApplyTimeStampToDocuments(List<MassiveOperationTarget> selectedItem, MassiveOperationReport report)
        {
            #region Dichiarazione variabili

            DocsPaWebService ws = new DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;
            // Lista delle informazioni di base sul documento
            // contenete il file a cui aggiungere il timestamp
            BaseInfoDoc[] baseInfoDocs= null;

            // Le informazioni sul documento di interesse
            BaseInfoDoc tmpDoc;

            // Il file a cui applicare il timestamp
            string convertedFile;

            // Messaggio da aggiungere al report
            string message;

            // Risultato dell'applicazione del timestamp ad un documento
            MassiveOperationReport.MassiveOperationResultEnum result;

            #endregion

            // Per ogni documento a cui bisogna applicare il timestamp...
            foreach(MassiveOperationTarget temp in selectedItem)
            {
                // ...inizializzazione del messaggio e dell'esito
                message = String.Empty;
                result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                string accessRight = ws.getAccessRightDocBySystemID(temp.Id, UserManager.getInfoUtente(this));
                if (accessRight.Equals("20"))
                {
                    message = "Il documento è in attesa di accettazione, quindi non può essere il effettuato Timestamp";
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                }
                
                // ...recupero del contenuto delle informazioni di base sul documento
                if (result == MassiveOperationReport.MassiveOperationResultEnum.OK)
                {
                    try
                    {
                        if (accessRight.Equals("20"))
                        {
                            message = "Il documento è in attesa di accettazione, quindi non può essere il Timestamp";
                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        }
                        else
                        {
                            baseInfoDocs = DocumentManager.GetBaseInfoForDocument(
                                temp.Id,
                                String.Empty,
                                String.Empty);
                        }
                    }
                    catch (Exception e)
                    {
                        message = "Errore durante il recupero dei dati sul documento.";
                        result = MassiveOperationReport.MassiveOperationResultEnum.KO;

                        return;
                    }

                    // ...se baseInfoDocs non contiene elementi o non contiene un documento
                    // con idProfile ugual a idProfile -> errore
                    if (baseInfoDocs != null &&
                        baseInfoDocs.Where(e => e.IdProfile.Equals(temp.Id)).Count() == 1)
                    {
                        try
                        {
                            // Recupero delle informazioni sul documento
                            tmpDoc = baseInfoDocs.Where(e => e.IdProfile.Equals(temp.Id)).FirstOrDefault();

                            // Recupero del contenuto del file a cui applicare il timestamp
                            convertedFile = this.GetFileForDocument(tmpDoc);

                            // Recupero del file e applicazione del time stamp
                            message = this.ApplyTimeStampToDocument(
                                convertedFile,
                                tmpDoc.DocNumber,
                                tmpDoc.VersionId);
                        }
                        catch (Exception e)
                        {
                            message = e.Message;
                            result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                        }
                    }
                    else
                    {
                        message = "Errore durante il recupero dei dati sul documento.";
                        result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    }
                }
                // Aggiunta della riga al report
                report.AddReportRow(
                    temp.Codice,
                    result,
                    message);

            }
        }

        /// <summary>
        /// Applicazione del time stamp ad un documento
        /// </summary>
        /// <param name="convertedFile">Contenuto del file a cui applicare il timestamp</param>
        /// <param name="docNumber">Doc number del documento a cui applicare il timestamp</param>
        /// <param name="versionId">Id della versione del documento a cui applicare il timestamp</param>
        /// <returns>Esito dell'applicazione del timestamp</returns>
        private string ApplyTimeStampToDocument(string convertedFile, string docNumber, string versionId)
        {
            // L'oggetto di input marca
            InputMarca inputMarca;

            // Informazioni sull'utente
            InfoUtente userInfo;

            // Il file request
            FileRequest fileRequest;

            // La stringa descrittiva del risultato
            string toReturn = String.Empty;

            // Recupero delle informazioni sull'utente
            userInfo = UserManager.getInfoUtente(this);

            // Impostazione di doc number e version id nel file request
            fileRequest = new FileRequest();
            fileRequest.docNumber = docNumber;
            fileRequest.versionId = versionId;
            
                
            // Creazione e inizializzazione dell'oggetto per l'input marca
            inputMarca = new DocsPaWR.InputMarca();
            inputMarca.applicazione = userInfo.urlWA;
            inputMarca.file_p7m = convertedFile;
            inputMarca.riferimento = userInfo.userId;

            // Applicazione del time stamp
            try
            {
                DocumentManager.ApplyTimeStampAM(
                    userInfo,
                    inputMarca,
                    fileRequest,
                    out toReturn);
            }
            catch (Exception e)
            {
                throw new Exception("Errore durante l'applicazione del timestamp.");
            }

            // Restituzione del risultato
            return toReturn;
        }

        /// <summary>
        /// Funzione per il recupero e la conversione del file a cui applicare il timestamp
        /// </summary>
        /// <param name="baseInfoDocs">Le informazioni di base sul documento</param>
        /// <returns>Il contenuto del file convertito</returns>
        private string GetFileForDocument(BaseInfoDoc baseInfoDocs)
        {
            // Il contenuto del file
            byte[] fileContent;

            // Il risultato dell'elaborazione
            string toReturn;

            // Se il documento non ha un file associato, viene lanciata
            // un'eccezione
            if (!baseInfoDocs.HaveFile)
                throw new Exception("Il documento non ha un file associato");

            // Se il documento non è reperibile, viene sollevata un'eccezione
            //if (!File.Exists(baseInfoDocs.FileName))
            //    throw new Exception("Errore durante il reperimento del file");

            // Recupero del contenuto del file
            try
            {
                
                      /*
                        fileContent = FileManager.getInstance(Session.SessionID).GetFile(
                            this,
                            baseInfoDocs.VersionId,
                            baseInfoDocs.VersionNumber.ToString(),
                            baseInfoDocs.DocNumber,
                            baseInfoDocs.Path,
                            baseInfoDocs.FileName,
                            false).content;
                      */
                    FileRequest fileRequest = new FileRequest{
                        versionId =baseInfoDocs.VersionId,
                        version = baseInfoDocs.VersionNumber.ToString(),
                        docNumber = baseInfoDocs.DocNumber,
                        path = baseInfoDocs.Path,
                        fileName =  baseInfoDocs.FileName
                    };

                    fileContent = FileManager.getInstance(Session.SessionID).getFile(this, fileRequest, false, true).content;
            }
            catch (Exception e)
            {
                throw new Exception("Errore durante il reperimento del file associato al documento.");
            }

            // Conversione del file
            toReturn = BitConverter.ToString(fileContent);
            toReturn = toReturn.Replace("-", "");

            // Restituzione del risultato dell'operazione
            return toReturn;

        }

        #endregion

    }
}