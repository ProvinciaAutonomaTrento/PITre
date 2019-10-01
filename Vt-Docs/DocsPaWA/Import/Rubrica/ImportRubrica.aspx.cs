using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using System.Threading;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;
using System.Data;
using System.Text;

namespace DocsPAWA.Import.Rubrica
{
    public partial class ImportRubrica : System.Web.UI.Page
    {
        // Oggetto preposto alla memorizzazione del report
        AddressBookImportResultContainer report;

        #region Event Handler

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void btnImportaCorrispondenti_Click(object sender, EventArgs e)
        {
            #region Definizione variabili

            // Un booleano utilizzato per determinare la validità del
            // file excel
            bool canImport;

            // Il file postato
            HttpPostedFile postedFile;

            // Il content del documento
            byte[] content;

            // Il nome da attribuire al file temporaneo
            string temporaryFileName;

            // Il server path
            string serverPath;

            // L'eventuale errore avvenuto durante la lettura dei dati dal foglio
            string error;

            // L'oggetto con le informazioni sui corrispondenti da importare
            AddressBookRowDataContainer abrdc;

            // L'oggetto cui delegare l'esecuzione dell'importazione
            AsyncImportAddressBookExecutor importExecutor;

            // Oggetto per specificare un thread con parametri
            ParameterizedThreadStart entry;

            // Thread
            Thread thread;

            // Oggetto in cui memorizzare i parametri
            object[] parameters;

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
                content = this.fileUploader.FileBytes;

                // Creazione del nome per il file temporaneo
                temporaryFileName = String.Format("{0}.xls",
                    Guid.NewGuid().ToString());

                // Prelevamento del serverPath
                serverPath = Utils.getHttpFullPath();

                try
                {
                    // Pulizia del call context
                    CallContextStack.CurrentContext.ContextState["addressBookExporter"] = null;
                    CallContextStack.CurrentContext.ContextState["reportImport"] = null;

                    // Reset del campo nascosto con il valore da raggiungere.
                    this.hfTargetPerc.Value = "0";

                    // Disassociazione delle sorgenti dati
                    this.grdGenerale.DataSource = null;
                    this.grdInseriti.DataSource = null;
                    this.grdModificati.DataSource = null;
                    this.grdCancellati.DataSource = null;
                    this.grdGenerale.DataBind();
                    this.grdInseriti.DataBind();
                    this.grdModificati.DataBind();
                    this.grdCancellati.DataBind();

                    // Reperimento delle informazioni sui corrispondenti da importare
                    abrdc = ImportAddressBookUtils.ReadDataFromExcel(
                        content,
                        Guid.NewGuid().ToString() + ".xls",
                        UserManager.getInfoUtente());

                    // Creazione di un nuovo oggetto cui delegare l'importazione dei corrispondenti
                    importExecutor = new AsyncImportAddressBookExecutor();

                    // Salvataggio dell'oggetto esecutore nel CallContext
                    CallContextStack.CurrentContext.ContextState["addressBookExporter"] = importExecutor;

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

                    importExecutor.ExecuteImport(
                        new object[] { 
                        abrdc, 
                        Utils.getHttpFullPath(), 
                        UserManager.getInfoUtente(), 
                        UserManager.getRuolo() 
                    });

                    int analyzedCorr, totalCorr;
                    // Si richiedono le statistiche
                    importExecutor.GetStatistics(out analyzedCorr, out totalCorr);

                    // Viene prelevato il report
                    report = importExecutor.GetReport();

                    // Se il report Generale non contiene elementi, viene aggiunto un
                    // result positivo
                    if (report.General == null || report.General.Length == 0)
                    {
                        report.General = new AddressBookImportResult[1];

                        report.General[0] = new AddressBookImportResult()
                        {
                            Result = ResultEnum.OK,
                            Message = "Nessun messaggio generale da mostrare"
                        };
                    }

                }
                catch (Exception ex)
                {
                    // Creazione di un array di result con un solo elemento
                    // che conterrà il dettaglio dell'eccezione
                    report = new AddressBookImportResultContainer();

                    report.General = new AddressBookImportResult[1];

                    report.General[0] = new AddressBookImportResult()
                    {
                        Result = ResultEnum.KO,
                        Message = ex.Message,
                    };

                }

            }

            // Associazione degli array dei risultati alle varie griglia
            this.grdGenerale.DataSource = report.General;
            this.grdInseriti.DataSource = report.Inserted;
            this.grdModificati.DataSource = report.Modified;
            this.grdCancellati.DataSource = report.Deleted;

            // Binding delle sorgenti dati
            this.grdGenerale.DataBind();
            this.grdInseriti.DataBind();
            this.grdModificati.DataBind();
            this.grdCancellati.DataBind();

            // Si nasconde il popup
            this.mdlPopupWait.Hide();

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

        #endregion

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
        /// <param name="report">Il file con i risultati dell'importazione</param>
        /// <returns>Il dataset con i dati da inserire nel PDF</returns>
        private DataSet GenerateDataSet(AddressBookImportResultContainer report)
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
            dataTable.Columns.Add("Operazione", typeof(string));    // Operazione
            dataTable.Columns.Add("Messaggio", typeof(string));     // Messaggio
            dataTable.Columns.Add("Risultato", typeof(string));     // Risultato
            dataTable.Columns.Add("Dettagli", typeof(string));      // Dettagli

            // Aggiunta della tabella al data set
            toReturn.Tables.Add(dataTable);

            #endregion

            #region Aggiunta dei dati sui corrispondenti inseriti

            if (report.Inserted != null)
                foreach (AddressBookImportResult importResult in report.Inserted)
                    this.AddRow(dataTable, importResult, "Inserimento");

            #endregion

            #region Aggiunta dei dati sui corrispondenti modificati

            if (report.Modified != null)
                foreach (AddressBookImportResult importResult in report.Modified)
                    this.AddRow(dataTable, importResult, "Modifica");

            #endregion

            #region Aggiunta dei dati sui corrispondenti cancellati

            if (report.Deleted != null)
                foreach (AddressBookImportResult importResult in report.Deleted)
                    this.AddRow(dataTable, importResult, "Cancellazione");

            #endregion

            // Restituzione del dataset generato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'aggiunta di una nuova riga alla lista delle righe del report
        /// </summary>
        /// <param name="dataTable">Il data table cui aggiungere la riga</param>
        /// <param name="importResult">L'oggetto da cui estrarre i dati per la costruzione del report</param>
        /// <param name="operation">L'operazione compiuta</param>
        private void AddRow(DataTable dataTable, AddressBookImportResult importResult, string operation)
        {
            // Aggiunta di una nuova riga alla tabella
            DataRow dataRow = dataTable.NewRow();

            // Impostazione dei valori
            dataRow["Operazione"] = operation;                          // Operazione
            dataRow["Messaggio"] = importResult.Message;                // Messaggio
            dataRow["Risultato"] = importResult.Result.ToString();      // Risultato

            // Dettagli
            StringBuilder details = new StringBuilder(String.Empty);

            if (importResult.Problems != null)
                foreach (string detail in importResult.Problems)
                    details.AppendLine(" - " + detail);

            dataRow["Dettagli"] = details.ToString();

            // Aggiunta della riga
            dataTable.Rows.Add(dataRow);

        }

        #region Funzioni griglia

        /// <summary>
        /// Funzione per la restituzione dell'esito dell'operazione
        /// </summary>
        /// <param name="result">L'oggetto result associato alla riga corrente</param>
        /// <returns>L'esito</returns>
        protected string GetResult(AddressBookImportResult result)
        {
            string toReturn;

            switch (result.Result)
            {
                case ResultEnum.KO:
                    toReturn = String.Format("<span style=\"color:Red;\">{0}</span>",
                        result.Result);
                    break;

                case ResultEnum.OK:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Result);
                    break;

                case ResultEnum.Warning:
                    toReturn = String.Format("<span style=\"color:Orange;\">{0}</span>",
                        result.Result);
                    break;

                default:
                    toReturn = String.Format("<span style=\"color:Green;\">{0}</span>",
                        result.Result);
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
        protected string GetDetails(AddressBookImportResult result)
        {
            string toReturn;

            StringBuilder message = new StringBuilder();

            // Se ci sono dettagli da mostrare
            if (result.Problems != null)
            {
                // ...aggiunta del tag di inizio numerazione
                message.AppendLine("<ul>");

                // ...per ogni dettaglio...
                foreach (string str in result.Problems)
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