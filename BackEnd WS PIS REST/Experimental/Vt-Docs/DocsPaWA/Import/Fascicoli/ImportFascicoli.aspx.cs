using System;
using System.Text;
using System.Web;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using DocsPAWA.SiteNavigation;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace DocsPAWA.Import.Fascicoli
{
    public partial class ImportFascicoli : CssPage
    {
        #region Event handler

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreaFascicoli_Click(object sender, EventArgs e)
        {
            #region Definizione variabili
            // Un booleano utilizzato per determinare la validità del
            // file excel
            bool canImport;

            // Il risultato della chiamata al webservice
            List<ImportResult> report = null;

            // Il file postato
            HttpPostedFile postedFile;

            // Il content del documento
            byte[] content;

            // Il nome da attribuire al file temporaneo
            string temporaryFileName;

            // Il server path
            string serverPath;

            // L'array con le informazioni sui fascicoli da importare
            ProjectRowData[] prds;

            // L'oggetto con la definizione della funzione da far eseguire al thread
            AsyncImportProjectExecutor importExecutor;

            // Il numero massimo di fascicoli che è possibile importare
            int maxNumber;

            #endregion

            // Prelevamento del file postato
            postedFile = this.fileUploader.PostedFile;

            // Verifica del content type e dell'estensione del file al fine
            // di verificarne la validità
            canImport = (postedFile.ContentType == "application/vnd.ms-excel")
                && postedFile.FileName.ToLower().EndsWith("xls");

            // Se la verifica ha avuto esito positivo...
            if (canImport)
            {
                // Prelevamento del contenuto del file
                content = this.GetContent();

                // Creazione del nome per il file temporaneo
                temporaryFileName = String.Format("{0}.{1}",
                    Guid.NewGuid().ToString(), Path.GetExtension(this.fileUploader.FileName));

                // Prelevamento del serverPath
                serverPath = Utils.getHttpFullPath();

                // Pulizia del call context
                CallContextStack.CurrentContext.ContextState["projectExporter"] = null;
                CallContextStack.CurrentContext.ContextState["reportImport"] = null;

                // Reset del campo nascosto con il valore da raggiungere.
                //this.hfTargetPerc.Value = "0";

                // Disassociazione della sorgente dati
                this.grdReport.DataSource = null;
                this.grdReport.DataBind();

                try
                {
                    // Reperimento delle informazioni sui fascicoli da importare
                    prds = ImportProjectsUtils.ReadDataFromExcel(
                        content,
                        Guid.NewGuid().ToString() + Path.GetExtension(this.fileUploader.FileName),
                        UserManager.getInfoUtente(this),
                        UserManager.getRuolo(this));

                }
                catch (Exception ex)
                {
                    report = new List<ImportResult>()
                        {
                            new ImportResult()
                                {
                                    Message = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex).Message,
                                    Outcome = OutcomeEnumeration.KO
                                }
                        };

                    this.grdReport.DataSource = report;
                    this.grdReport.DataBind();

                    // Hiding del popup
                    this.mdlPopupWait.Hide();

                    return;
                }

                try
                {
                    // Reperimento del numero massimo di documenti importabili
                    maxNumber = ImportProjectsUtils.GetMaxProjectsNumber(UserManager.getInfoUtente());

                    // Se maxNumber è più minore del numero di documenti estratti dal foglio excel
                    if (maxNumber < prds.Length)
                        throw new Exception(String.Format(
                            "E' possibile importare al massimo {0} fascicoli per volta.", maxNumber));
                }
                catch (Exception ex)
                {

                    report = new List<ImportResult>()
                        {
                            new ImportResult()
                                {
                                    Message = ex.Message,
                                    Outcome = OutcomeEnumeration.KO
                                }
                        };

                    this.grdReport.DataSource = report;
                    this.grdReport.DataBind();

                    // Hiding del popup
                    this.mdlPopupWait.Hide();

                    return;
                }

                // Creazione di un nuovo oggetto cui delegare l'importazione dei fascicoli
                importExecutor = new AsyncImportProjectExecutor();

                // Salvataggio dell'oggetto esecutore nel CallContext
                CallContextStack.CurrentContext.ContextState["projectExporter"] = importExecutor;

                // Creazione del thread con parametri
                //entry = new ParameterizedThreadStart(importExecutor.ExecuteImport);

                // Creazione del thread
                //thread = new Thread(entry);

                // Creazione dell'array dei parametri
                //parameters = new object[] { 
                //    drdc, 
                //    Utils.getHttpFullPath(), 
                //    UserManager.getInfoUtente(), 
                //    UserManager.getRuolo() 
                //};

                // Partenza del thread
                //thread.Start(parameters);

                // Avvio del timer
                //this.tmrUpdateInfo.Enabled = true;

                try
                {
                    importExecutor.ExecuteImport(
                           new object[] { 
                        prds, 
                        Utils.getHttpFullPath(), 
                        UserManager.getInfoUtente(), 
                        UserManager.getRuolo() 
                    });

                    int analyzedDocument, totalDocument;
                    // Si richiedono le statistiche
                    importExecutor.GetStatistics(out analyzedDocument, out totalDocument);

                    // Viene prelevato il report
                    report = importExecutor.GetReport();

                    // Associazione degli array dei risultati alla griglia
                    this.grdReport.DataSource = report;

                    // Binding della sorgente dati
                    this.grdReport.DataBind();

                    // Creazione del data set per l'esportazione del report di importazione
                    DataSet dataSet = this.GenerateDataSet(report);

                    // Path e nome file del template
                    string templateFilePath = Server.MapPath("formatPdfExport.xml");

                    // Aggiunta nell call context del file documento  con le informazioni
                    // da scivere nel report
                    CallContextStack.CurrentContext.ContextState["reportImport"] =
                        global::ProspettiRiepilogativi.Frontend.PdfReport.do_MakePdfReport(
                        global::ProspettiRiepilogativi.Frontend.ReportDisponibili.ReportLogMassiveImport,
                        templateFilePath,
                        dataSet,
                        null);

                    // Abilitazione pulsante esportazione 
                    this.btnEsportaReport.Enabled = true;

                    // Aggiornamento pannello bottoniera
                    this.upButtons.Update();
                }
                catch (Exception ex)
                {
                    // Creazione di un array di result con un solo elemento
                    // che conterrà il dettaglio dell'eccezione
                    report = new List<ImportResult>()
                    {
                        new ImportResult()
                        {
                            Outcome = OutcomeEnumeration.KO,
                            Message = ex.Message,
                        }
                    };

                    this.grdReport.DataSource = report;
                    this.grdReport.DataBind();
                }

            }

            // Si nasconde il popup
            this.mdlPopupWait.Hide();
        }

        /// <summary>
        /// Allo scatenarsi di questo evento viene memorizzato nel call context il contenuto del file uploadato
        /// </summary>
        protected void fileUploader_UploadedComplete(object sender, EventArgs e)
        {
            CallContextStack.CurrentContext.ContextState["file"] = ((AjaxControlToolkit.AsyncFileUpload)sender).FileBytes;

        }

        //        protected void tmrUpdateInfo_Tick(object sender, EventArgs e)
        //        {
        //            // Prelevamento dell'esecutore
        //            AsyncImportDocumentExecutor executor = 
        //                CallContextStack.CurrentContext.ContextState["documentExporter"] as AsyncImportDocumentExecutor;

        //            // Numero di documenti analizzati e non importati
        //            int analyzedDocument, totalDocument;

        //            // Dataset del report
        //            DataSet dataSet;

        //            // Percentuale da raggingere
        //            double percTarget = 0;

        //            // Path e nome file del template
        //            string templateFilePath = Server.MapPath("formatPdfExport.xml");

        //            // Se è stato recuperato con successo l'esecutore...
        //            if (executor != null)
        //            {
        //                // Si richiedono le statistiche
        //                executor.GetStatistics(out analyzedDocument, out totalDocument);

        //                // Calcolo della percentuale target
        //                try
        //                {
        //                    percTarget = (analyzedDocument * 100 / totalDocument);
        //                }
        //                catch (Exception ex) 
        //                {
        //                    Debugger.Write(ex);
        //                }

        //                // Impostazione della percentuale target
        //                this.hfTargetPerc.Value = percTarget.ToString();

        //                this.lblInfo.Text = String.Format("Elaborati {0} documenti su {1}",
        //                    analyzedDocument, totalDocument);

        //                // Se sono stati analizzati tutti i documenti...
        //                if (analyzedDocument == totalDocument)
        //                {
        //                    // Viene disabilitato il timer
        ////                    this.tmrUpdateInfo.Enabled = false;

        //                    // Viene cancellato il contenuto della casella di testo con le informazioni
        //                    this.lblInfo.Text = String.Empty;

        //                    // Viene prelevato il report
        //                    report = executor.GetReport();

        //                    // Se il report Generale non contiene elementi, viene aggiunto un
        //                    // result positivo
        //                    if (report.General == null || report.General.Length == 0)
        //                    {
        //                        report.General = new ImportResult[1];

        //                        report.General[0] = new ImportResult()
        //                        {
        //                            Outcome = OutcomeEnumeration.OK,
        //                            Message = "Nessun messaggio generale da mostrare"
        //                        };
        //                    }

        //                    // Associazione degli array dei risultati alle varie griglia
        //                    this.grdGenerale.DataSource = report.General;
        //                    this.grdArrivo.DataSource = report.InDocument;
        //                    this.grdPartenza.DataSource = report.OutDocument;
        //                    this.grdInterni.DataSource = report.OwnDocument;
        //                    this.grdGrigi.DataSource = report.GrayDocument;
        //                    this.grdAllegati.DataSource = report.Attachment;

        //                    // Binding delle sorgenti dati
        //                    this.grdGenerale.DataBind();
        //                    this.grdArrivo.DataBind();
        //                    this.grdPartenza.DataBind();
        //                    this.grdInterni.DataBind();
        //                    this.grdGrigi.DataBind();
        //                    this.grdAllegati.DataBind();

        //                    // Si nasconde il popup
        //                    this.mdlPopupWait.Hide();

        //                    // Creazione del data set per l'esportazione del report di importazione
        //                    dataSet = this.GenerateDataSet(report);

        //                    // Aggiunta nell call context del file documento  con le informazioni
        //                    // da scivere nel report
        //                    CallContextStack.CurrentContext.ContextState["reportImport"] =
        //                        global::ProspettiRiepilogativi.Frontend.PdfReport.do_MakePdfReport(
        //                        global::ProspettiRiepilogativi.Frontend.ReportDisponibili.ReportLogMassiveImport,
        //                        templateFilePath,
        //                        dataSet, 
        //                        null);

        //                    // Abilitazione pulsante esportazione 
        //                    this.btnEsportaReport.Enabled = true;

        //                    // Aggiornamento pannello bottoniera
        //                    this.upButtons.Update();

        //                }

        //            }

        //        }

        /// <summary>
        /// Funzione per la creazione del dataset del report
        /// </summary>
        /// <param name="report">Gli oggetti con i risultati dell'importazione</param>
        /// <returns>Il dataset con i dati da inserire nel PDF</returns>
        private DataSet GenerateDataSet(List<ImportResult> report)
        {
            #region Dichiarazione variabili

            // L'oggetto da restituire
            DataSet toReturn;

            // La tabella in cui inserire i dati
            DataTable dataTable;

            #endregion

            #region Creazione struttura del dataset

            // Creazione del dataset
            toReturn = new DataSet();

            // Creazione di un nuovo data table
            dataTable = new DataTable();

            // Aggiunta delle quattro colonne con le informazioni sui risultati di importazione
            dataTable.Columns.Add("Ordinale", typeof(string));  // Ordinale
            dataTable.Columns.Add("Messaggio", typeof(string)); // Messaggio
            dataTable.Columns.Add("Risultato", typeof(string)); // Risultato
            dataTable.Columns.Add("Dettagli", typeof(string)); // Dettagli

            // Aggiunta della tabella al data set
            toReturn.Tables.Add(dataTable);

            #endregion

            #region Aggiunta dei fascicoli

            if (report != null)
                foreach (ImportResult importResult in report)
                    this.AddRow(dataTable, importResult);

            #endregion

            // Restituzione del dataset generato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'aggiunta di una nuova riga alla lista delle righe del report
        /// </summary>
        /// <param name="dataTable">Il data table cui aggiungere la riga</param>
        /// <param name="importResult">L'oggetto da cui estrarre i dati per la costruzione del report</param>
        private void AddRow(DataTable dataTable, ImportResult importResult)
        {
            // Aggiunta di una nuova riga alla tabella
            DataRow dataRow = dataTable.NewRow();

            // Impostazione dei valori
            dataRow["Ordinale"] = importResult.Ordinal;                 // Ordinale
            dataRow["Messaggio"] = importResult.Message;                // Messaggio
            dataRow["Risultato"] = importResult.Outcome.ToString();     // Risultato

            // Dettagli
            StringBuilder details = new StringBuilder(String.Empty);

            if (importResult.OtherInformation != null)
                foreach (string detail in importResult.OtherInformation)
                    details.AppendLine(" - " + detail);

            dataRow["Dettagli"] = details.ToString();

            // Aggiunta della riga
            dataTable.Rows.Add(dataRow);

        }

        /// <summary>
        /// Funzione per il recupero del content del file da utilizzare per l'import. Questa funzione si occupa
        /// anche di cancellare tale contenuto dal call context
        /// </summary>
        /// <returns>Array di byte corrispondente la content del file uploadato</returns>
        public Byte[] GetContent()
        {
            Byte[] content = CallContextStack.CurrentContext.ContextState["file"] as Byte[];
            CallContextStack.CurrentContext.ContextState["file"] = null;
            return content;

        }

        #endregion

        #region Funzioni griglia

        /// <summary>
        /// Funzione per la restituzione del numero ordinale
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>L'ordinale</returns>
        protected string GetOrdinalNumber(ImportResult result)
        {
            string toReturn;

            // Se l'ordinale è presente, viene restituito, altrimenti viene restituito "N.A."
            if (!String.IsNullOrEmpty(result.Ordinal))
                toReturn = result.Ordinal;
            else
                toReturn = String.Empty;

            // Restituzione del testo
            return toReturn;

        }

        /// <summary>
        /// Funzione per la restituzione dell'esito dell'operazione
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>L'esito</returns>
        protected string GetResult(ImportResult result)
        {
            string toReturn;

            // A seconda dell'esito bisogna visualizzarlo in rosso, in verde o in giallo
            switch (result.Outcome)
            {
                case OutcomeEnumeration.KO:
                    toReturn = String.Format("<span style=\"color:Red;\">{0}</span>",
                        result.Outcome);
                    break;

                case OutcomeEnumeration.OK:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Outcome);
                    break;

                case OutcomeEnumeration.Warnings:
                    toReturn = String.Format("<span style=\"color:Yellow;\">{0}</span>",
                        result.Outcome);
                    break;

                default:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Outcome);
                    break;
            }

            // Restituzione del testo
            return toReturn;
        }

        /// <summary>
        /// Funzione per la restituzione dei dettagli sull'esito
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>Gli eventuali dettagli sull'esito</returns>
        protected string GetDetails(ImportResult result)
        {
            string toReturn;

            StringBuilder message = new StringBuilder();

            // Se ci sono dettagli da mostrare
            if (result.OtherInformation != null)
            {
                // ...aggiunta del tag di inizio numerazione
                message.AppendLine("<ul>");

                // ...per ogni dettaglio...
                foreach (string str in result.OtherInformation)
                    // ...aggiunta dell'item 
                    message.AppendFormat("<li>{0}</li>",
                        str);

                // ...aggiunta del tag di chiusura della lista
                message.AppendLine("</ul>");

            }

            // Restituzione dei dettagli
            toReturn = message.ToString();

            // Restituzione del testo
            return toReturn;

        }

        #endregion

    }
}
