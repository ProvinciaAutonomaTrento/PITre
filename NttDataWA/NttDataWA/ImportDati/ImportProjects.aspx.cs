using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.ImportDati
{
    public partial class ImportProjects : System.Web.UI.Page
    {

        #region Properties

        /// <summary>
        /// importFileName
        /// </summary>
        private string importFileName
        {
            get
            {
                return HttpContext.Current.Session["importFileName"] as string;
            }
            set
            {
                HttpContext.Current.Session["importFileName"] = value;
            }
        }

        private bool StampaUnione
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["stampaUnione"] != null)
                    result = bool.Parse(HttpContext.Current.Session["stampaUnione"].ToString());
                return result;
            }
            set
            {
                HttpContext.Current.Session["stampaUnione"] = value;
            }
        }

        private ImportProjectExecutor importExecutor
        {
            get
            {
                return HttpContext.Current.Session["importExecutor"] as ImportProjectExecutor;
            }
            set
            {
                HttpContext.Current.Session["importExecutor"] = value;
            }
        }

        private FileDocumento reportImport
        {
            get
            {
                return HttpContext.Current.Session["reportImport"] as FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["reportImport"] = value;
            }
        }

        private List<ImportResult> report
        {
            get
            {
                return HttpContext.Current.Session["report"] as List<ImportResult>;
            }
            set
            {
                HttpContext.Current.Session["report"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }

            this.RefreshScript();
        }

        protected void BtnUploadHidden_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            if (this.fileUpload != null && !string.IsNullOrEmpty(this.fileUpload.Value) && this.fileUpload.PostedFile.FileName.ToLower().EndsWith("xls"))
            {
                // Nel CallContext inserisco nome del file(con estensione) a partire dal path del file di import
                this.importFileName = Path.GetFileName(this.fileUpload.Value);

                // Prelevamento del contenuto del file
                HttpPostedFile p = this.fileUpload.PostedFile;
                Stream fs = p.InputStream;
                byte[] content = new byte[fs.Length];
                fs.Read(content, 0, (int)fs.Length);
                fs.Close();

                if (content.Length > 0)
                {
                    // Creazione del nome per il file temporaneo
                    string temporaryFileName = String.Format("{0}.xls", Guid.NewGuid().ToString());

                    // Prelevamento del serverPath
                    string serverPath = utils.getHttpFullPath();

                    try
                    {
                        // Disassociazione delle sorgenti dati
                        this.grdGenerale.DataSource = null;
                        this.grdGenerale.DataBind();

                        // Reperimento delle informazioni sui documenti da importare
                        string error;
                        ProjectRowData[] prds = prds = ImportProjectManager.ReadDataFromExcel(
                        content,
                        temporaryFileName,
                        UserManager.GetInfoUser(),
                        RoleManager.GetRoleInSession());

                        // Reperimento del numero massimo di documenti importabili
                        int maxNumber = ImportProjectManager.GetMaxProjectsNumber(UserManager.GetInfoUser());

                        // Se maxNumber è più minore del numero di documenti estratti dal foglio excel
                        if (maxNumber < prds.Length)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorImportProjectsMaxNumber', 'warning', '', '" + utils.FormatJs(maxNumber.ToString()) + "');", true);
                            return;
                        }

                        // Creazione di un nuovo oggetto cui delegare l'importazione dei documenti
                        this.importExecutor = new ImportProjectExecutor();

                        importExecutor.ExecuteImport(
                            new object[] { 
                            prds, 
                            utils.getHttpFullPath(), 
                            UserManager.GetInfoUser(), 
                            RoleManager.GetRoleInSession() 
                        });

                        int analyzedDocument, totalDocument;
                        // Si richiedono le statistiche
                        importExecutor.GetStatistics(out analyzedDocument, out totalDocument);

                        // Viene prelevato il report
                        report = importExecutor.GetReport();

                        // Associazione degli array dei risultati alla griglia
                        this.grdGenerale.DataSource = report;

                        // Binding della sorgente dati
                        this.grdGenerale.DataBind();

                        this.plcReport.Visible = true;
                        this.upReport.Update();

                        // Creazione del data set per l'esportazione del report di importazione
                        DataSet dataSet = this.GenerateDataSet(report);

                        // Path e nome file del template
                        string templateFilePath = Server.MapPath("formatPdfExport_proj.xml");

                        // Aggiunta nell call context del file documento  con le informazioni
                        // da scivere nel report
                        this.reportImport =
                            global::ProspettiRiepilogativi.Frontend.PdfReport.do_MakePdfReport(
                            global::ProspettiRiepilogativi.Frontend.ReportDisponibili.ReportLogMassiveImport,
                            templateFilePath,
                            dataSet,
                            null);

                        //// Abilitazione pulsante esportazione 
                        //this.BtnReportExport.Enabled = true;

                        ////link di scarica zip
                        //if (StampaUnione)
                        //    this.BtnDownloadReport.Visible = true;

                        // Aggiornamento pannello bottoniera
                        this.BtnImport.Enabled = false;
                        this.UpPnlButtons.Update();
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + utils.FormatJs(ex.Message) + "');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorImportDocumentsFileInvalid', 'error', '');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorImportDocumentsFileInvalid', 'error', '');", true);
            }
        }

        #endregion

        #region Methods

        protected void InitializePage()
        {
            this.InitializeLanguage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnImport.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsBtnImport", language);
            this.lnkTemplate.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsDownloadTemplate", language);
            this.pageTitle.Text = Utils.Languages.GetLabelFromCode("ImportProjectsTitle", language);
            this.lblFilename.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsFilename", language);
            this.lblTabGeneral.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsTabReport", language);
            this.lblTabReportPdf.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsTabReportPdf", language);
            this.grdGenerale.Columns[0].HeaderText = Utils.Languages.GetLabelFromCode("ImportDocumentsGridOrdinal", language);
            this.grdGenerale.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ImportDocumentsGridMessage", language);
            this.grdGenerale.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ImportDocumentsGridResult", language);
            this.grdGenerale.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("ImportDocumentsGridDetails", language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tabs", "$(function() {$('#tabs').tabs();});", true);
        }

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

        protected string GetOutput(ImportResult result)
        {
            string toReturn;

            switch (result.Outcome)
            {
                case OutcomeEnumeration.KO:
                    toReturn = "KO";
                    break;

                case OutcomeEnumeration.OK:
                    toReturn = "OK";
                    break;

                case OutcomeEnumeration.Warnings:
                    toReturn = "Warnings";
                    break;
                case OutcomeEnumeration.NONE:
                    toReturn = "";
                    break;
                case OutcomeEnumeration.FileNotAcquired:
                    toReturn = "File non acquisito";
                    break;
                default:
                    toReturn = result.Outcome.ToString();
                    break;
            }
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
                    toReturn = String.Format("<span>{0}</span>",
                        result.Outcome);
                    break;

                case OutcomeEnumeration.OK:
                    toReturn = String.Format("<span>{0}</span>",
                        result.Outcome);
                    break;

                case OutcomeEnumeration.Warnings:
                    toReturn = String.Format("<span>{0}</span>",
                        result.Outcome);
                    break;
                case OutcomeEnumeration.NONE:
                    toReturn = "<span></span>";
                    break;
                case OutcomeEnumeration.FileNotAcquired:
                    toReturn = "<span>File non acquisito</span>";
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
            dataTable.TableName = "TABLE_DATA";

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

        #endregion

    }
}