using System;
using System.Text;
using System.Web;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Threading;
using DocsPAWA.SiteNavigation;
using System.Data;

namespace DocsPAWA.Import.RDE
{
    public partial class ImportRDE : CssPage
    {
        #region Event Handler

        /// <summary>
        /// Allo scatenarsi di questo evento viene memorizzato nel call context il contenuto del file uploadato
        /// </summary>
        protected void fileUploader_UploadedComplete(object sender, EventArgs e)
        {
            CallContextStack.CurrentContext.ContextState["file"] = ((AjaxControlToolkit.AsyncFileUpload)sender).FileBytes;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Visualizzazione del tab dei protocolli interni a seconda 
            // del valore impostato
            this.tpInterni.Visible = ImportRDEUtils.IsEnabledOwnerProto();
            this.upReport.Update();

            // Impostazione delle informazioni sull'amministrazione
            this.ltlAdministrationInfo.Text =
                String.Format("Amministrazione: {0}, Codice: {1}",
                    UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Descrizione,
                    UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
 

            // Impostazione del link per il download del modellO in base alla versione RDE
            if (ImportRDEUtils.IsEnabledOwnerProto())
                // Versione 2
                this.hlLink.NavigateUrl = "ImportRDE.xls";
            else
                // Versione 1
                this.hlLink.NavigateUrl = "ModelloRDE_v01.xls";

        }

        protected void btnCreaRDE_Click(object sender, EventArgs e)
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

            // L'oggetto con le informazioni sui documenti da importare
            DocumentRowDataContainer drdc;

            // L'oggetto cui delegare l'esecuzione dell'importazione
            AsyncImportRDEExecutor importExecutor;

            // Oggetto per specificare un thread con parametri
            ParameterizedThreadStart entry;

            // Thread
            Thread thread;

            // Oggetto in cui memorizzare i parametri
            object[] parameters;

            // Il report dell'esecuzione
            ResultsContainer report;

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
                temporaryFileName = String.Format("{0}.xls",
                    Guid.NewGuid().ToString());

                // Prelevamento del serverPath
                serverPath = Utils.getHttpFullPath();

                try
                {
                    // Pulizia del call context
                    CallContextStack.CurrentContext.ContextState["rdeImporter"] = null;
                    CallContextStack.CurrentContext.ContextState["reportImport"] = null;

                    // Reset del campo nascosto con il valore da raggiungere.
                    //this.hfTargetPerc.Value = "0";

                    // Disassociazione delle sorgenti dati
                    this.grdArrivo.DataSource = null;
                    this.grdGenerale.DataSource = null;
                    this.grdInterni.DataSource = null;
                    this.grdPartenza.DataSource = null;
                    
                    this.grdArrivo.DataBind();
                    this.grdGenerale.DataBind();
                    this.grdInterni.DataBind();
                    this.grdPartenza.DataBind();

                    // Reperimento delle informazioni sui documenti da importare
                    drdc = ImportRDEUtils.ReadRDEDataFromExcel(
                        content,
                        temporaryFileName);

                    // Creazione di un nuovo oggetto cui delegare l'importazione dei documenti
                    importExecutor = new AsyncImportRDEExecutor();

                    // Salvataggio dell'oggetto esecutore nel CallContext
                    CallContextStack.CurrentContext.ContextState["rdeImporter"] = importExecutor;

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
                        drdc, 
                        Utils.getHttpFullPath(), 
                        UserManager.getInfoUtente(), 
                        UserManager.getRuolo() 
                    });

                    int analyzedDocument, totalDocument;
                    // Si richiedono le statistiche
                    importExecutor.GetStatistics(out analyzedDocument, out totalDocument);

                    // Viene prelevato il report
                    report = importExecutor.GetReport();

                    // Se il report Generale non contiene elementi, viene
                    // controllato se gli altri tab non contengono risultagti
                    // 
                    //if (report.General == null || report.General.Length == 0)
                    //{
                    //    report.General = new ImportResult[1];

                    //    report.General[0] = new ImportResult()
                    //    {
                    //        Outcome = OutcomeEnumeration.OK,
                    //        Message = "Nessun messaggio generale da mostrare"
                    //    };
                    //}

                    // Associazione degli array dei risultati alle varie griglia
                    this.grdGenerale.DataSource = report.General;
                    this.grdArrivo.DataSource = report.InDocument;
                    this.grdPartenza.DataSource = report.OutDocument;
                    this.grdInterni.DataSource = report.OwnDocument;

                    // Binding delle sorgenti dati
                    this.grdGenerale.DataBind();
                    this.grdArrivo.DataBind();
                    this.grdPartenza.DataBind();
                    this.grdInterni.DataBind();

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
                    report = new ResultsContainer();

                    report.General = new ImportResult[1];

                    report.General[0] = new ImportResult
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Message = ex.Message,
                    };

                    this.grdGenerale.DataSource = report.General;
                    this.grdGenerale.DataBind();

                }

            }

            // Si nasconde il popup
            this.mdlPopupWait.Hide();

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

        /// <summary>
        /// Funzione per la creazione del dataset del report
        /// </summary>
        /// <param name="report">Il file con i risultati dell'importazione</param>
        /// <returns>Il dataset con i dati da inserire nel PDF</returns>
        private DataSet GenerateDataSet(ResultsContainer report)
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
            dataTable.Columns.Add("Tipo", typeof(string));      // Tipo documento
            dataTable.Columns.Add("Messaggio", typeof(string)); // Messaggio
            dataTable.Columns.Add("Risultato", typeof(string)); // Risultato
            dataTable.Columns.Add("Dettagli", typeof(string));  // Dettagli

            // Aggiunta della tabella al data set
            toReturn.Tables.Add(dataTable);

            #endregion

            #region Aggiunta dei dati sui documenti in Arrivo

            if (report.InDocument != null)
                foreach (ImportResult importResult in report.InDocument)
                    this.AddRow(dataTable, importResult, "Arrivo");

            #endregion

            #region Aggiunta dei dati sui documenti in partenza

            if (report.OutDocument != null)
                foreach (ImportResult importResult in report.OutDocument)
                    this.AddRow(dataTable, importResult, "Partenza");

            #endregion

            #region Aggiunta dei dati sui documenti Interni

            if (report.OwnDocument != null)
                foreach (ImportResult importResult in report.OwnDocument)
                    this.AddRow(dataTable, importResult, "Interni");

            #endregion

            // Restituzione del dataset generato
            return toReturn;

        }

        /// <summary>
        /// Funzione per l'aggiunta di una nuova riga alla lista delle righe del report
        /// </summary>
        /// <param name="dataTable">Il data table cui aggiungere la riga</param>
        /// <param name="importResult">L'oggetto da cui estrarre i dati per la costruzione del report</param>
        /// <param name="documentTypeDescription">Descrizione del tipo di documento</param>
        private void AddRow(DataTable dataTable, ImportResult importResult, string documentTypeDescription)
        {
            // Aggiunta di una nuova riga alla tabella
            DataRow dataRow = dataTable.NewRow();

            // Impostazione dei valori
            dataRow["Ordinale"] = importResult.Ordinal;                 // Ordinale
            dataRow["Tipo"] = documentTypeDescription;                  // Tipo documento
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

                case OutcomeEnumeration.NONE:
                    toReturn = "";
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
