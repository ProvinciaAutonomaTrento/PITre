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
    public partial class ImportPrevious : System.Web.UI.Page
    {

        #region Fields

        public static string componentType = Constans.TYPE_SMARTCLIENT;

        #endregion

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

        protected DocsPaWR.EsitoImportPregressi EsitoImport
        {
            get
            {
                return HttpContext.Current.Session["esitoImport"] as DocsPaWR.EsitoImportPregressi;
            }

            set
            {
                HttpContext.Current.Session["esitoImport"] = value;
            }
        }

        protected List<DocsPaWR.ItemReportPregressi> ReportInError
        {
            get
            {
                return HttpContext.Current.Session["ReportInError"] as List<DocsPaWR.ItemReportPregressi>;
            }

            set
            {
                HttpContext.Current.Session["ReportInError"] = value;
            }
        }

        public DocsPaWR.FileDocumento FileDocumento
        {
            get
            {
                return HttpContext.Current.Session["fileDocumento"] as DocsPaWR.FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDocumento"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            componentType = UserManager.getComponentType(Request.UserAgent);
            this.InitApplet();

            if (!IsPostBack)
            {
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
            }

            this.RefreshScript();
        }

        protected void gridReport_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            ImageButton imgDelete = (ImageButton)e.Row.FindControl("btn_Rimuovi");
            ImageButton imgDettaglio = (ImageButton)e.Row.FindControl("btn_dettagli");
            Image imgErrore = (Image)e.Row.FindControl("img_errore");

            if (imgDettaglio != null)
            {
                TableCell cell = (TableCell)imgDettaglio.Parent;
                GridViewRow dgItem = (GridViewRow)cell.Parent;
                Label a = (Label)dgItem.FindControl("PERC");
                if (a != null && !string.IsNullOrEmpty(a.Text) && a.Text.Equals("100%"))
                {
                    imgDettaglio.Visible = true;

                    if (imgDelete != null)
                    {
                        imgDelete.Visible = true;
                    }

                    if (imgErrore != null)
                    {
                        imgErrore.Visible = true;
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.Pager)
            {
                if (e.Row.Cells.Count > 0)
                {
                    e.Row.Cells[0].Attributes.Add("colspan", e.Row.Cells[0].ColumnSpan.ToString());
                }
            }
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

                DocsPaWR.EsitoImportPregressi result = this.SendRequest(content);
                //Se mi torna null il foglio excel non è valido
                if (result == null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningImportPreviousNoServer', 'warning', '');", true);
                }
                else
                {
                    if (result.itemPregressi != null && result.itemPregressi.Length > 0)
                    {
                        List<DocsPaWR.ItemReportPregressi> itemInErrorOrWarning = result.itemPregressi.Where(x => x.esito != "S").ToList();

                        this.FetchData(itemInErrorOrWarning);

                        if (result.esito)
                        {
                            this.BtnContinue.Enabled = true;
                            this.EsitoImport = result;

                            this.lbl_alert.Text = this.GetLabel("ImportPreviousNoProblem");
                            this.BtnImport.Enabled = false;
                            this.fileUpload.Disabled = true;
                            this.BtnContinue.Enabled = true;

                            //Bottone esporta disabilitato
                            //this.BtnExport.Visible = true;
                            this.BtnExport.Enabled = false;
                        }
                        else
                        {
                            this.BtnContinue.Enabled = false;
                            this.EsitoImport = new DocsPaWR.EsitoImportPregressi();

                            this.BtnImport.Enabled = false;
                            this.fileUpload.Disabled = true;
                            if (itemInErrorOrWarning.Count > 0)
                            {
                                //Andrea - lista itemInErrorOrWarning messa nel CallContext per poterla passare come parametro dell'export excel
                                this.ReportInError = itemInErrorOrWarning;

                                if (itemInErrorOrWarning.Count == 1)
                                {
                                    this.lbl_alert.Text = this.GetLabel("ImportPreviousOneProblem");
                                }
                                else
                                {
                                    this.lbl_alert.Text = this.GetLabel("ImportPreviousMoreProblems").Replace("@@", itemInErrorOrWarning.Count.ToString());
                                }

                                //this.BtnExport.Visible = true;
                                this.BtnExport.Enabled = true;
                            }
                            else
                            {
                                this.lbl_alert.Text = this.GetLabel("ImportPreviousXlsCorrupted");
                                this.BtnExport.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        this.BtnContinue.Enabled = false;
                        this.EsitoImport = new DocsPaWR.EsitoImportPregressi();
                        this.BtnImport.Enabled = false;
                        this.fileUpload.Disabled = true;
                        this.lbl_alert.Text = this.GetLabel("ImportPreviousXlsEmpty");
                    }
                    this.pnlReport.Visible = true;
                    this.BtnNew.Enabled = true;
                    this.pnlAvviso.Visible = true;
                    this.pnlReport.Visible = true;
                    this.box_upload.Update();
                    this.upPnlReport.Update();
                    this.UpPnlButtons.Update();
                }
            }
        }

        protected void BtnNew_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            this.pnlReport.Visible = false;
            this.BtnNew.Enabled = false;
            this.BtnImport.Enabled = true;
            this.BtnExport.Enabled = false;
            this.BtnContinue.Enabled = false;
            this.pnlAvviso.Visible = false;
            this.pnlReport.Visible = false;
            this.fileUpload.Disabled = false;
            this.fileUpload = null;
            this.lbl_alert.Text = string.Empty;
            this.grvItems.DataSource = null;
            this.grvItems.DataBind();
            this.box_upload.Update();
            this.upPnlReport.Update();
            this.UpdatePanel1.Update();
            this.UpPnlButtons.Update();
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.Export(this.ReportInError);
        }

        protected void BtnContinue_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            string descrizione = this.txtDescrizione.Text;
            mDelegate = new asyncDelegate(importa);
            AsyncCallback callback = new AsyncCallback(asyncImport);
            mDelegate.BeginInvoke(UserManager.GetInfoUser(), EsitoImport, descrizione, callback, null);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('InfoImportPreviousSendSuccesfully', 'info', '');", true);
            this.pnlReport.Visible = false;
            this.BtnNew.Enabled = false;
            this.BtnImport.Enabled = true;
            this.pnlAvviso.Visible = false;
            this.pnlReport.Visible = false;
            this.fileUpload.Disabled = false;
            this.fileUpload = null;
            this.BtnContinue.Enabled = false;
            this.box_upload.Update();
            this.upPnlReport.Update();

            this.txtDescrizione.Text = string.Empty;
        }

        protected void BtnRefresh_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            this.FetchData();
        }

        #endregion

        #region Methods

        private void InitApplet()
        {
            if (componentType == Constans.TYPE_APPLET)
                this.plcApplet.Visible = true;
            else
            {
                Control ShellWrapper = Page.LoadControl("../ActivexWrappers/ShellWrapper.ascx");
                this.plcActiveX.Controls.Add(ShellWrapper);

                Control AdoStreamWrapper = Page.LoadControl("../ActivexWrappers/AdoStreamWrapper.ascx");
                this.plcActiveX.Controls.Add(AdoStreamWrapper);

                Control FsoWrapper = Page.LoadControl("../ActivexWrappers/FsoWrapper.ascx");
                this.plcActiveX.Controls.Add(FsoWrapper);

                this.plcActiveX.Visible = true;
            }
        }

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.ImportPreviousReport.ReturnValue))
            {
                Session["ImportPreviousReport_ID"] = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('ImportPreviousReport','');", true);
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.FetchData();

            this.BtnContinue.Enabled = false;
            this.BtnExport.Enabled = false;
            this.BtnNew.Enabled = false;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.pageTitle.Text = Utils.Languages.GetLabelFromCode("ImportPreviousTitle", language);
            this.BtnImport.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsBtnImport", language);
            this.BtnContinue.Text = Utils.Languages.GetLabelFromCode("ImportPreviousBtnContinue", language);
            this.BtnNew.Text = Utils.Languages.GetLabelFromCode("ImportPreviousBtnNew", language);
            this.BtnExport.Text = Utils.Languages.GetLabelFromCode("ImportPreviousBtnExport", language);
            this.BtnRefresh.Text = Utils.Languages.GetLabelFromCode("ImportPreviousBtnRefresh", language);
            this.lnkTemplate.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsDownloadTemplate", language);
            this.lblTabNew.Text = Utils.Languages.GetLabelFromCode("ImportPreviousTabNew", language);
            this.lblTabStatus.Text = Utils.Languages.GetLabelFromCode("ImportPreviousTabStatus", language);
            this.titleReport.Text = Utils.Languages.GetLabelFromCode("ImportPreviousReportTitle", language);
            this.ImportPreviousReport.Title = Utils.Languages.GetLabelFromCode("ImportPreviousPopupReportTitle", language);
            this.lblFilename.Text = Utils.Languages.GetLabelFromCode("ImportDocumentsFilename", language);
            this.lblDescription.Text = Utils.Languages.GetLabelFromCode("ImportPreviousDescription", language);
            this.lblAvviso.Text = Utils.Languages.GetLabelFromCode("ImportPreviousTitleAnalisys", language);
            this.grvItems.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGrdError", language);
            this.grvItems.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGrdOrdinal", language);
            this.gridReport.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportDescription", language);
            this.gridReport.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportDateStart", language);
            this.gridReport.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportDateEnd", language);
            this.gridReport.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportNumDoc", language);
            this.gridReport.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportPercentage", language);
            this.gridReport.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportStato", language);
            this.gridReport.Columns[7].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportReport", language);
            this.gridReport.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousGridReportDelete", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tabs", "$(function() {$('#tabs').tabs();});", true);
        }

        protected void FetchData()
        {
            this.gridReport.DataSource = ImportPreviousManager.GetReports();
            this.gridReport.PageIndex = 0;
            this.gridReport.DataBind();
        }

        protected string GetTipoAvviso(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.esito))
            {
                if (item.esito.Equals("W"))
                {
                    result = "Attenzione";
                }
                else
                {
                    if (item.esito.Equals("E"))
                    {
                        result = "Errore";
                    }
                }
            }
            return result;
        }

        protected string GetErrore(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.errore))
            {
                string[] errori = item.errore.Split('|');
                result = "<ul>";
                foreach (string err in errori)
                {
                    result += "<li>" + err + "</li>";
                }
                result += "</ul>";
            }
            return result;
        }

        protected string GetLineaExcel(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.ordinale))
            {
                result = item.ordinale;
            }
            return result;
        }

        protected String GetImageAvviso(DocsPaWR.ItemReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.esito))
            {
                if (item.esito.Equals("W"))
                {
                    result = "../Images/Icons/ico_warning.png";
                }
                else
                {
                    if (item.esito.Equals("E"))
                    {
                        result = "../Images/Icons/ico_error.png";
                    }
                }
            }
            return result;
        }

        protected void ViewDetails(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            GridViewRow dgItem = (GridViewRow)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");

            //Apri export excel dei dati dell'imports
            Session["ImportPreviousReport_ID"] = a.Text;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "ajaxModalPopupImportPreviousReport();", true);
        }

        protected void DeleteReport(object sender, EventArgs e)
        {
            ImageButton image = (ImageButton)sender;
            TableCell cell = (TableCell)image.Parent;
            GridViewRow dgItem = (GridViewRow)cell.Parent;
            Label a = (Label)dgItem.FindControl("SYSTEM_ID");

            ImportPreviousManager.DeleteReportById(a.Text);
            this.FetchData();
            this.upPanelReport.Update();
        }

        protected string GetReportID(DocsPaWR.ReportPregressi item)
        {
            string result = item.systemId;
            return result;
        }

        protected string GetDescrizione(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.descrizione))
            {
                result = item.descrizione;
            }
            return result;
        }

        protected string GetDataInizio(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.dataEsecuzione))
            {
                result = item.dataEsecuzione;
            }
            return result;
        }

        protected string GetDataFine(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.dataFine))
            {
                result = item.dataFine;
            }
            return result;
        }

        protected string GetNumeroDocumenti(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.numDoc))
            {
                result = item.numDoc;
            }
            return result;
        }

        protected string GetPercentuale(DocsPaWR.ReportPregressi item)
        {
            string result = "0";
            int numDoc = Int32.Parse(item.numDoc);
            int elaborati = 0;
            double percentuale = 0;
            if (item.itemPregressi != null && item.itemPregressi.Length > 0)
            {
                elaborati = Int32.Parse(item.numeroElaborati);
                percentuale = (100 * elaborati) / numDoc;
                result = percentuale.ToString();
            }
            return result + "%";
        }

        protected string GetImmaginePercentuale(DocsPaWR.ReportPregressi item)
        {
            string result = "../images/progressBar/00_perc.jpg";
            int numDoc = Int32.Parse(item.numDoc);
            int elaborati = 0;
            double percentuale = 0;
            if (item.itemPregressi != null && item.itemPregressi.Length > 0)
            {
                elaborati = item.itemPregressi.Length;
                percentuale = (100 * elaborati) / numDoc;
                result = percentuale.ToString();
            }
            if (percentuale >= 0 && percentuale < 10)
            {
                result = "../images/progressBar/00_perc.jpg";
            }
            if (percentuale >= 10 && percentuale < 20)
            {
                result = "../images/progressBar/10_perc.jpg";
            }
            if (percentuale >= 20 && percentuale < 30)
            {
                result = "../images/progressBar/20_perc.jpg";
            }
            if (percentuale >= 30 && percentuale < 40)
            {
                result = "../images/progressBar/30_perc.jpg";
            }
            if (percentuale >= 40 && percentuale < 50)
            {
                result = "../images/progressBar/40_perc.jpg";
            }
            if (percentuale >= 50 && percentuale < 60)
            {
                result = "../images/progressBar/50_perc.jpg";
            }
            if (percentuale >= 60 && percentuale < 70)
            {
                result = "../images/progressBar/60_perc.jpg";
            }
            if (percentuale >= 70 && percentuale < 80)
            {
                result = "../images/progressBar/70_perc.jpg";
            }
            if (percentuale >= 80 && percentuale < 90)
            {
                result = "../images/progressBar/80_perc.jpg";
            }
            if (percentuale >= 90 && percentuale < 100)
            {
                result = "../images/progressBar/90_perc.jpg";
            }
            if (percentuale == 100)
            {
                result = "../images/progressBar/100_perc.jpg";
            }

            return result;
        }

        protected string GetNumeroDiErrori(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(item.inError))
            {
                if (item.inError.Equals("0"))
                {
                    result = this.GetLabel("ImportPreviousNoError");
                }
                else
                {
                    if (item.inError.Equals("1"))
                    {
                        result = this.GetLabel("ImportPreviousOneError");
                    }
                    else
                    {
                        result = this.GetLabel("ImportPreviousMoreErrors").Replace("@@", item.inError);
                    }
                }
            }
            return result;
        }

        protected String GetImageErrore(DocsPaWR.ReportPregressi item)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(item.inError))
            {
                if (!item.inError.Equals("0"))
                {
                    result = "../Images/Icons/ico_warning.png";
                }
                else
                    result = "../Images/Icons/flag_ok.png";
            }
            return result;
        }

        protected DocsPaWR.EsitoImportPregressi SendRequest(byte[] fileToImport)
        {
            return ImportPreviousManager.ImportPregresso(fileToImport);
        }

        protected void FetchData(List<DocsPaWR.ItemReportPregressi> result)
        {
            this.grvItems.DataSource = result;
            this.grvItems.PageIndex = 0;
            this.grvItems.DataBind();
        }

        protected void Export(List<DocsPaWR.ItemReportPregressi> reportInErr)
        {
            DocsPaWR.FileDocumento file = new DocsPaWR.FileDocumento();
            try
            {
                file = this.GetFile(reportInErr);
                //Andrea De Marco - Aggiunto importFileName al Nome del File: EsitoControllo_nomefile.
                if (file != null)
                    file.name = file.name + importFileName;

                if (file == null || file.content == null || file.content.Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ImportPreviousReportNoDocuments', 'error', '');", true);
                }
                else
                {
                    string js = string.Empty;
                    switch (componentType)
                    {
                        case(Constans.TYPE_SOCKET):
                            js = "OpenFileSocket('XLS');\n";
                            break;
                        case(Constans.TYPE_APPLET):
                            js = "OpenFileApplet('XLS');\n";
                            break;
                        default:
                            js = "OpenFileActiveX('XLS');\n";
                            break;
                            
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", js, true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + utils.FormatJs(ex.Message) + "');", true);
            }
        }

        public DocsPaWR.FileDocumento GetFile(List<DocsPaWR.ItemReportPregressi> repInErr)
        {
            this.FileDocumento = ImportPreviousManager.ExportReportExcel(repInErr);
            return this.FileDocumento;
        }

        private delegate bool asyncDelegate(DocsPaWR.InfoUtente infoUtente, DocsPaWR.EsitoImportPregressi esitoImpor, string descrizione);

        asyncDelegate mDelegate;

        private bool importa(DocsPaWR.InfoUtente infoUtente, DocsPaWR.EsitoImportPregressi esitoImport, string descrizione)
        {
            ImportPreviousManager.AsyncImportPregresso(infoUtente, esitoImport, descrizione);
            return true;
        }

        private void asyncImport(IAsyncResult ar)
        {
            bool bResult = mDelegate.EndInvoke(ar);

            string mess = string.Empty;

            if (ar.IsCompleted)
                mess = "Importazione eseguita correttamente.";
            else
                mess = "Errori nell'importazione.";

            this.lbl_alert.Text = mess;
        }

        #endregion

    }
}