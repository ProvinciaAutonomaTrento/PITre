using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CuteWebUI;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA;
using System.IO;
using System.Threading;

namespace NttDataWA.Popup
{
    public partial class UploadFile : System.Web.UI.Page
    {
        private static string UPLOADFOLDER = "Uploads";

        private const string DEFAULT_ACROBAT_INTEGRATION_CLASS_ID = "AcrobatInterop.AcrobatServices";
        private const string ERROR_ACQUIRED_DOCUMENT = "ErrorAcquiredDocument";

        public static string componentType = Constans.TYPE_ACTIVEX;
        public static string fullpath = "";
        public static bool cartaceo = false;
        public static string randomVersion = "";
        //public static bool isSaved = false;

        #region Properties

        protected int FileAcquisitionSizeMax
        {
            get
            {
                int result = 20480 * 1024;
                if (HttpContext.Current.Session["FileAcquisitionSizeMax"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["FileAcquisitionSizeMax"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["FileAcquisitionSizeMax"] = value;
            }
        }

        protected bool IsAdobeIntegrationActive
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsAdobeIntegrationActive"] != null)
                {
                    result = (bool)HttpContext.Current.Session["IsAdobeIntegrationActive"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["IsAdobeIntegrationActive"] = value;
            }
        }

        protected string AcrobatIntegrationClassId
        {
            get
            {
                string result = DEFAULT_ACROBAT_INTEGRATION_CLASS_ID;
                if (HttpContext.Current.Session["AcrobatIntegrationClassId"] != null)
                {
                    result = HttpContext.Current.Session["AcrobatIntegrationClassId"] as string;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["AcrobatIntegrationClassId"] = value;
            }
        }

        protected bool ScanWithAcrobatIntegration
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["ScanWithAcrobatIntegration"] != null)
                {
                    result = (bool)HttpContext.Current.Session["ScanWithAcrobatIntegration"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ScanWithAcrobatIntegration"] = value;
            }
        }

        protected bool IsAcrobat6Interop
        {
            get
            {
                return (this.AcrobatIntegrationClassId.ToLower().Equals("acrobat6interop.acrobatservices"));
            }
        }

        protected bool DocumentPdfConvertEnabled 
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["DocumentPdfConvertEnabled"] != null)
                {
                    result = (bool)HttpContext.Current.Session["DocumentPdfConvertEnabled"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["DocumentPdfConvertEnabled"] = value;
            }
        }

        protected bool EnableConvertPdfOnSign
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["EnableConvertPdfOnSign"] != null)
                {
                    result = (bool)HttpContext.Current.Session["EnableConvertPdfOnSign"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["EnableConvertPdfOnSign"] = value;
            }
        }

        protected bool PdfConversionSynchronousLC
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["PdfConversionSynchronousLC"] != null)
                {
                    result = (bool)HttpContext.Current.Session["PdfConversionSynchronousLC"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["PdfConversionSynchronousLC"] = value;
            }
        }

        protected bool PdfConvertServerSide
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["PdfConvertServerSide"] != null)
                {
                    result = (bool)HttpContext.Current.Session["PdfConvertServerSide"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["PdfConvertServerSide"] = value;
            }
        }

        private bool AlreadyOpened
        {
            get
            {
                if (HttpContext.Current.Session["UploadFileAlreadyOpened" + Session.SessionID] != null)
                    return (bool)HttpContext.Current.Session["UploadFileAlreadyOpened" + Session.SessionID];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["UploadFileAlreadyOpened" + Session.SessionID] = value;
            }
        }

        #endregion



        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                //isSaved = (string.IsNullOrEmpty(Request.QueryString["idDoc"]) ? false : (Request.QueryString["idDoc"] == "0" ? false : true));
                

                randomVersion = "?v=" + Guid.NewGuid().ToString("N");
 
                componentType = UserManager.getComponentType(Request.UserAgent);
                string componentCall = string.Empty;

                fullpath = utils.getHttpFullPath();
                if (!IsPostBack)
                {
                    this.InitPage();

                    Session["UploadMassivoConversionePDF"] = "0";
                    Session["UploadMassivoCartaceo"] = "0";

                    if (UserManager.IsAuthorizedFunctions("DO_ENABLE_BIGFILE") && IsSaved())
                    {
                        this.headerAccordionBigFile.Visible = true;
                        this.contentAccordionBigFile.Visible = true;
                        ///this.optRepository.Visible = true;
                    }
                    else
                    {
                        ///this.optRepository.Visible = false;
                        this.headerAccordionBigFile.Visible = false;
                        this.contentAccordionBigFile.Visible = false;
                    }



                    //this.optPapery.Selected = true;
                    //this.optPapery.Enabled = false;
                    // this.UpdatePanelFsoFile.Visible = false;
                    //this.UpdatePanelRepository.Visible = false;
                    ///this.UpdatePanelScanner.Visible = false;
                    // this.FsoFileUploadSection.Visible = false;
                    // this.UpdatePanelUploadMassivo.Visible = false;

                    this.scanAcquire.Attributes.Remove("onClick");
                    switch (componentType)
                    {
                        case Constans.TYPE_APPLET:
                            componentCall = "return openApplet();";
                            break;
                        case Constans.TYPE_ACTIVEX:
                            componentCall = "return openActiveX();";
                            break;
                        case Constans.TYPE_SMARTCLIENT:
                            componentCall = "return openSmartClient();";
                            break;
                        case Constans.TYPE_SOCKET:
                            componentCall = "return openSocket();";
                            break;
                        default:
                            componentCall = "return openActiveX();";
                            break; //ABBATANGELI - Modificare in openActiveX() subito dopo la demo del 12-12-2012
                    }
                    this.scanAcquire.Attributes.Add("onClick", componentCall); 

                    this.UpdatePanelFsoFile.Update();
                    //this.UpdatePanelScanner.Update();

                    this.AlreadyOpened = true;

                    SchedaDocumento doc = DocumentManager.getSelectedRecord();
                    if (String.IsNullOrWhiteSpace(doc.docNumber))
                    {
                        this.cellLabelAttachment.Visible = false;
                        this.cellUploadAttachment.Visible = false;
                    }
                    this.labelButtonUploadDocument.Text = "Acquisisci Documento";
                    this.labelCellHeaderUploadDocument.Text = "Documento Principale";


                    var attachId = DocumentManager.getSelectedAttachId();
                     var fileReq = (DocumentManager.getSelectedAttachId() != null) ?
                        FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                            FileManager.GetFileRequest();

                  

                    if (fileReq != null)
                    {
                        if (typeof(Allegato) == fileReq.GetType())
                        {
                            this.labelButtonUploadDocument.Text = "Acquisisci Allegato";
                            this.labelCellHeaderUploadDocument.Text = "Allegato";
                            this.cellLabelAttachment.Visible = false;
                            this.cellUploadAttachment.Visible = false;
                        }
                        //    var docNUmber = fileReq.docNumber;
                        //if(!String.IsNullOrWhiteSpace(doc.docNumber) && doc.docNumber != docNUmber)
                        //{
                        //    this.labelButtonUploadDocument.Text = "Carica Allegato";
                        //    this.labelCellHeaderUploadDocument.Text = "Allegato";
                        //    this.cellLabelAttachment.Visible = false;
                        //    this.cellUploadAttachment.Visible = false;
                        //}
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitPage()
        {
            this.LoadKeys();
            this.InitLanguage();
            this.InitSessionObject();

            if (!this.PdfConvertServerSide)
                this.optPDF.Enabled = false;

            //this.optScanner.Checked = true;

            //if (this.AlreadyOpened)
            //    this.plcApplet.Visible = false;


        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litCancel.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadCancel", language));
            this.litCancelNA.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadCancel", language));
            this.litCancel2.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadCancel", language));
            this.litAppletError.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadAppletError", language));
            this.litNoAppletError.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadAppletError", language));
            this.litSendOk.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadSendOk", language));
            this.litSendError.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadSendError", language));
            this.litSendErrorNA.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadSendError", language));
            this.litTitle.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadTitle", language));
            this.litTitle2.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadTitle", language));
            this.litUploadError.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadUploadError", language));
            this.litUploadError2.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadUploadError", language));
            this.litUploadError3.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadUploadError", language));
            this.litActiveXError.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadActiveXError", language));
            this.litPDFError.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadPDFError", language));
            this.litScanError.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadScanError", language));
            this.litSending.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadSending", language));
            this.litStartDownloadBytes.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadStartDownloadBytes", language));
            this.litStartDownloadBytes2.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadStartDownloadBytes", language));
            this.litStartDownloadBytes3.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadStartDownloadBytes", language));
            this.litStartDownloadBytes4.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadStartDownloadBytes", language));
            this.litExists.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadExists", language));
            this.litNone.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadNone", language));
            this.litCompleted.Text = utils.FormatJs(Utils.Languages.GetLabelFromCode("FileUploadCompleted", language));
            // this.optScanner.Text = Utils.Languages.GetLabelFromCode("FileUploadScanOption", language);
            
            // this.optUpload.Text = Utils.Languages.GetLabelFromCode("FileUploadUploadOption", language);
            // this.litOtherwise.Text = Utils.Languages.GetLabelFromCode("FileUploadOtherwise", language);
            this.litUploadMax.Text = Utils.Languages.GetLabelFromCode("FileUploadUploadMax", language) + " " + ((double)this.FileAcquisitionSizeMax/1024.0).ToString("n0") + "KB";
            this.scanAcquire.Text = Utils.Languages.GetLabelFromCode("FileUploadScanStart", language);
            if (this.PdfConversionSynchronousLC)
                this.optPDF.Text = Utils.Languages.GetLabelFromCode("FileUploadPDFOptionSynchronous", language);
            else
                this.optPDF.Text = Utils.Languages.GetLabelFromCode("FileUploadPDFOptionAsynchronous", language);
            // this.optPapery.Text = Utils.Languages.GetLabelFromCode("FileUploadPaperyOption", language);
            this.litStatus.Text = Utils.Languages.GetLabelFromCode("FileUploadStatus", language);
            this.litDownloadBytes.Text = Utils.Languages.GetLabelFromCode("FileUploadDownloadBytes", language);
            this.UploadBtnUploadFile.Text = Utils.Languages.GetLabelFromCode("FileUploadConfirm", language);
            //this.SenderBtnClose.Text = Utils.Languages.GetLabelFromCode("FileUploadClose", language);

            //this.optRepository.Text = Utils.Languages.GetLabelFromCode("FileUploadRepositoryOption", language);
            this.repositoryOpen.Text = Utils.Languages.GetLabelFromCode("FileUploadRepositoryOpen", language);

            // this.optUploadMassivo.Text = Utils.Languages.GetLabelFromCode("FileUploadMassivoOption", language);

            this.labelCaptionScanner.Text = Utils.Languages.GetLabelFromCode("FileUploadScanOption", language);
            //this.labelCaptionUploadFile.Text = Utils.Languages.GetLabelFromCode("FileUploadUploadOption", language);
            this.labelCaptionBigFile.Text = Utils.Languages.GetLabelFromCode("FileUploadRepositoryOption", language);
            this.labelCaptionUploadMassivo.Text = Utils.Languages.GetLabelFromCode("FileUploadMassivoOption", language);
        }

        private void InitSessionObject(){
            Session["UploadDetail"] = new UploadDetail { IsReady = false };
            Session["fileDoc"] = new NttDataWA.DocsPaWR.FileDocumento();
        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILE_ACQ_SIZE_MAX.ToString()]))
            {
                this.FileAcquisitionSizeMax = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.FILE_ACQ_SIZE_MAX.ToString()])*1024;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ADOBE_ACROBAT_INTEGRATION.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ADOBE_ACROBAT_INTEGRATION.ToString()]=="1")
            {
                this.IsAdobeIntegrationActive = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ADOBE_ACROBAT_INTEGRATION_CLASS_ID.ToString()]))
            {
                this.AcrobatIntegrationClassId = System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ADOBE_ACROBAT_INTEGRATION_CLASS_ID.ToString()].ToString();
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.SCAN_WITH_ADOBE_ACROBAT_INTEGRATION.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.SCAN_WITH_ADOBE_ACROBAT_INTEGRATION.ToString()] == "1")
            {
                this.ScanWithAcrobatIntegration = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DOCUMENT_PDF_CONVERT_ENABLED.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DOCUMENT_PDF_CONVERT_ENABLED.ToString()] == "true")
            {
                this.DocumentPdfConvertEnabled = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_CONVERT_PDF_ON_SIGN.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_CONVERT_PDF_ON_SIGN.ToString()] == "true")
            {
                this.EnableConvertPdfOnSign = true;
            }

            //ABBA - PDF CONVERSION
            DocsPaWR.DocsPaWebService ws = NttDataWA.Utils.ProxyManager.GetWS();
            DocsPaWR.SmartClientConfigurations smcConf = ws.GetSmartClientConfigurationsPerUser(UserManager.GetInfoUser());
            if (smcConf.ApplyPdfConvertionOnScan)
            {
                this.PdfConversionSynchronousLC = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_SINCRONA_LC.ToString())) && Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_SINCRONA_LC.ToString()).Equals("1"))
                {
                    this.PdfConversionSynchronousLC = true;
                }
            }
            //FINE

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_LATO_SERVER.ToString())) && Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_LATO_SERVER.ToString()).Equals("1"))
            {
                this.PdfConvertServerSide = true;
            }
        }      

        protected void SenderBtnClose_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('UplodadFile','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void UploadBtnUploadFile_Click(object sender, EventArgs e)
        {
            try
            {
                bool conversionePdfServer = this.fileOptions.Items[0].Selected;
                bool cartaceo = false; // this.fileOptions.Items[1].Selected;

                //if (this.optUpload.Checked)
                //{
                FileDocumento fileDoc = Session["fileDoc"] as FileDocumento;
                if (fileDoc != null && fileDoc.content != null)
                {
                    fileDoc.cartaceo = cartaceo;

                    try
                    {
                        string msgError = FileManager.uploadFile(this, fileDoc, cartaceo, conversionePdfServer, this.PdfConversionSynchronousLC);

                        if (string.IsNullOrEmpty(msgError))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.upPnlGeneral, this.upPnlGeneral.GetType(), "closeAJM", "parent.closeAjaxModal('UplodadFile','up');", true);
                        }
                        else
                        {
                            if (msgError.Equals("ErrorConversionePdf"))
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msgError.Replace("'", @"\'") + "', 'warning', '');", true);
                                ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('UplodadFile','up');", true);
                            }
                            else
                            {
                                string msg = "ErrorFileUpload_custom";
                                msgError = msgError.Equals(ERROR_ACQUIRED_DOCUMENT) ? Utils.Languages.GetMessageFromCode(msgError, UserManager.GetUserLanguage()) : msgError;
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'error', '', '" + utils.FormatJs(msgError) + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'error', '', '" + utils.FormatJs(msgError) + "');}; reallowOp();", true);
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        string msg = "ErrorFileUpload";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'error', '');} else {parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'error', '');}; reallowOp();", true);
                    }
                    finally
                    {
                        Session["fileDoc"] = null;
                        Session["UploadDetail"] = null;
                    }
                }
                // }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        //protected void rdbAquire_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try {
        //        return;
        //        this.UpdatePanelScanner.Visible = false;
        //        // this.UpdatePanelUploadMassivo.Visible = false;
        //        this.UpdatePanelFsoFile.Visible = false;
        //        this.UploadBtnUploadFile.Enabled = false;
        //        this.fileOptions.Items[1].Selected = cartaceo;
        //        this.fileOptions.Items[1].Enabled = true;
        //        this.FsoFileUploadSection.Visible = false;
        //        //this.UpdatePanelRepository.Visible = false;

        //        if (this.optUpload.Checked)
        //        {
        //            this.UpdatePanelFsoFile.Visible = true;
        //            this.FsoFileUploadSection.Visible = true;
        //        }
        //        else if (this.optScanner.Checked)
        //        {
        //            this.UpdatePanelScanner.Visible = true;
        //            cartaceo = this.fileOptions.Items[1].Selected;
        //            this.fileOptions.Items[1].Enabled = false;
        //        }
        //        else if (this.optRepository.Checked)
        //        {
        //            this.UpdatePanelRepository.Visible = true;
        //        }
        //        else if (this.optUploadMassivo.Checked)
        //        {
        //            //this.UpdatePanelUploadMassivo.Visible = true;
        //        }

        //        this.UpdatePanelFsoFile.Update();
        //        this.UpdatePanelScanner.Update();
        //        this.UpUpdateButtons.Update();
        //        this.FsoFileUploadSection.Update();
        //        // this.UpdatePanelUploadMassivo.Update();

        //        ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "reallow", "reallowOp();", true);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return;
        //    }
        //}



        #region FsoFileUploader

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static void InitUpload()
        {
            HttpContext.Current.Session["UploadDetail"] = new UploadDetail { IsReady = false };
            //HttpContext.Current.Session["fileDoc"] = new NttDataWA.DocsPaWR.FileDocumento();
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static object GetUploadStatus()
        {
            object retval = null;
            UploadDetail info = HttpContext.Current.Session["UploadDetail"] as UploadDetail;

            if (info != null && info.IsReady)
            {
                int soFar = info.UploadedLength;
                int total = info.ContentLength;
                int percentComplete = (int)Math.Ceiling((double)soFar / (double)total * 100);
                string message = Utils.Languages.GetLabelFromCode("FileUploadSending", UIManager.UserManager.GetUserLanguage());
                string fileName = string.Format("{0}", info.FileName);
                string downloadBytes = string.Format(Utils.Languages.GetLabelFromCode("FileUploadStartDownloadBytes2", UIManager.UserManager.GetUserLanguage()), soFar / 1024, total / 1024);         

                return new
                {
                    percentComplete = percentComplete,
                    message = message,
                    fileName = fileName,
                    downloadBytes = downloadBytes
                };
            }

            return retval;
        }

        public void DownloadFile(string filePath)
        {
            if (File.Exists(Server.MapPath(filePath)))
            {
                string strFileName = Path.GetFileName(filePath).Replace(" ", "%20");
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName);
                Response.Clear();
                Response.WriteFile(Server.MapPath(filePath));
                Response.End();
            }
        }

        public string DeleteFile(string FileName)
        {
            string strMessage = "";
            try
            {
                string strPath = Path.Combine(UPLOADFOLDER, FileName);
                if (File.Exists(Server.MapPath(strPath)) == true)
                {
                    File.Delete(Server.MapPath(strPath));
                    strMessage = "File Deleted";
                }
                else
                    strMessage = "File Not Found";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return strMessage;
        }

        public string CalculateFileSize(double FileInBytes)
        {
            string strSize = "00";
            if (FileInBytes < 1024)
                strSize = FileInBytes + " B";//Byte
            else if (FileInBytes > 1024 & FileInBytes < 1048576)
                strSize = Math.Round((FileInBytes / 1024), 2) + " KB";//Kilobyte
            else if (FileInBytes > 1048576 & FileInBytes < 107341824)
                strSize = Math.Round((FileInBytes / 1024) / 1024, 2) + " MB";//Megabyte
            else if (FileInBytes > 107341824 & FileInBytes < 1099511627776)
                strSize = Math.Round(((FileInBytes / 1024) / 1024) / 1024, 2) + " GB";//Gigabyte
            else
                strSize = Math.Round((((FileInBytes / 1024) / 1024) / 1024) / 1024, 2) + " TB";//Terabyte
            return strSize;
        }

        #endregion

        private bool IsSaved()
        {
            bool retVal = false;
            SchedaDocumento doc = DocumentManager.getSelectedRecord();

            if (doc != null && !string.IsNullOrEmpty(doc.docNumber))
                retVal = true;

            return retVal;
        }

        //protected void fileOptions_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Session["UploadMassivoConversionePDF"] = optPDF.Selected;
        //        Session["UploadMassivoCartaceo"] = optPapery.Selected;
        //    }
        //    catch (Exception) { }
        //}

    }
}