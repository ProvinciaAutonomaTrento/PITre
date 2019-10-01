using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using NttDataWA.DocsPaWR;
using NttDataWA.UserControls;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDataWA.CheckInOut;



namespace NttDataWA.UserControls
{
    public partial class DocumentButtons : System.Web.UI.UserControl
    {


        public static string componentType = Constans.TYPE_SMARTCLIENT;

        #region Properties

        protected string IsAlreadyDownloaded
        {
            get
            {
                return Session["IsAlreadyDownloaded"] as string;
            }
            set
            {
                Session["IsAlreadyDownloaded"] = value;
            }
        }

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
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

        protected string ValueRblFilterAttachments
        {
            set
            {
                HttpContext.Current.Session["valueRblFilterAttachments"] = value;
            }
        }

        /// <summary>
        /// Indentifica se siamo nella situazione di apertura della popup(inserita per evitare che alla chiusura della popup riesegua il tutto il page_load)
        /// </summary>
        private bool OpenSignaturePopup
        {
            get
            {
                if (HttpContext.Current.Session["OpenSignaturePopup"] != null)
                    return (bool)HttpContext.Current.Session["OpenSignaturePopup"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["OpenSignaturePopup"] = value;
            }
        }

        #endregion

        #region const
        private const string CALLER_ATTACHMENT = "ATTACHMENTS";
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            componentType = UserManager.getComponentType(Request.UserAgent);
            if (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET)
            {
                this.checkInOutPanel.Visible = false;
                this.DocumentImgOpenModelActiveX.Visible = false;
                this.DocumentImgOpenModelApplet.Visible = true;
                //this.checkInOutAppletPanel.Visible = false; //true;
            }
            else
            {
                //this.checkInOutAppletPanel.Visible = false;
                this.checkInOutPanel.Visible = true;
                this.DocumentImgOpenModelActiveX.Visible = true;
                this.DocumentImgOpenModelApplet.Visible = false;
            }

            try
            {
                if (!IsPostBack)
                {
                    this.InitializePage();
                    this.InitializeLanguage();
                    this.VisibilityRoleFunction();
                }
                if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Equals("UpDocumentButtons"))
                {
                    if ((ViewDocument)Parent.FindControl("ViewDocument") != null)
                    {
                        ViewDocument view = (ViewDocument)Parent.FindControl("ViewDocument");
                        this.DocumentImgViewFile.Click += view.LinkViewFileDocument;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resetEventTarget", "$('#__EVENTTARGET').val('');", true);
                    }
                }

                //HERMES MOMENTANEO
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
                {
                    this.DocumentImgTimestamp.Visible = false;
                    this.DocumentImgSaveLocalFile.Visible = false;
                    this.DocumentImgLock.Visible = false;
                    this.DocumentImgUnLock.Visible = false;
                    this.DocumentImgUnlockWithoutSave.Visible = false;
                    this.DocumentImgOpenFile.Visible = false;
                    this.CheckOutShowStatus.Visible = false;
                    this.DocumentImgOpenModelApplet.Visible = false;
                    this.DocumentImgOpenModelActiveX.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgVerifyCrlSignature.Visible = false;
                    this.DocumentImgSignatureDetails.Visible = false;
                    this.DocumentImgViewFile.Visible = false;
                    this.Image1.Visible = false;
                    this.Image2.Visible = false;
                }
                //if (!IsPostBack) 
                this.InitializeCheckInOutPanel();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void InitializeCheckInOutPanel()
        {
            SchedaDocumento selDoc = UIManager.DocumentManager.getSelectedRecord();
            this.ResetModelSession();
            // APPLET_G
            if (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET)
            {
                CheckOutStatus ckStatus = null;

                if (DocumentManager.getSelectedAttachId() != null)
                {
                    if (DocumentManager.GetSelectedAttachment() != null)
                        ckStatus = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber);
                }
                else
                {
                    if (selDoc != null && selDoc.checkOutStatus != null)
                        ckStatus = selDoc.checkOutStatus;
                }

                if (ckStatus != null)
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    this.CheckOutShowStatus.AlternateText = Utils.Languages.GetLabelFromCode("CheckOutShowStatus", language) + " " + ckStatus.UserName;
                    this.CheckOutShowStatus.ToolTip = Utils.Languages.GetLabelFromCode("CheckOutShowStatus", language) + " " + ckStatus.UserName;
                }
                //CheckInOutApplet.CheckInOutPanel ctl1 = this.FindControl("checkInOutAppletPanel") as CheckInOutApplet.CheckInOutPanel;
                //if (ctl1 != null && !this.IsPostBack)
                //{
                //ctl1.Initialize(selDoc.systemId, (DocumentManager.getSelectedAttachId() != null ? DocumentManager.getSelectedAttachId() : selDoc.docNumber));
                //}
            }
            else
            {
                CheckInOut.CheckInOutPanel ctl = this.FindControl("checkInOutPanel") as CheckInOut.CheckInOutPanel;
                if (selDoc != null)
                {
                    ctl.Initialize(selDoc.systemId, (DocumentManager.getSelectedAttachId() != null ? DocumentManager.getSelectedAttachId() : selDoc.docNumber));
                }
            }
            //ctl.OnCheckIn += new EventHandler(this.OnCheckInDocument);



        }

        public void RefreshCheckInOutPanel()
        {
            SchedaDocumento selDoc = UIManager.DocumentManager.getSelectedRecord();

            // APPLET_G
            if (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET)
            {
                CheckOutStatus ckStatus = null;

                if (DocumentManager.getSelectedAttachId() != null)
                {
                    if (DocumentManager.GetSelectedAttachment() != null)
                        ckStatus = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber);
                }
                else
                {
                    if (selDoc != null && selDoc.checkOutStatus != null)
                        ckStatus = selDoc.checkOutStatus;
                }

                if (ckStatus != null)
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    this.CheckOutShowStatus.AlternateText = Utils.Languages.GetLabelFromCode("CheckOutShowStatus", language) + " " + ckStatus.UserName;
                    this.CheckOutShowStatus.ToolTip = Utils.Languages.GetLabelFromCode("CheckOutShowStatus", language) + " " + ckStatus.UserName;
                }
            }
            else
            {
                CheckInOut.CheckInOutPanel ctl = this.FindControl("checkInOutPanel") as CheckInOut.CheckInOutPanel;
                if (ctl != null)
                {
                    if (selDoc != null)
                    {
                        ctl.Refresh(selDoc.systemId, selDoc.docNumber);
                    }
                    else
                    {
                        ctl.RefreshButtons();
                    }
                }
            }
            //ctl.OnCheckIn += new EventHandler(this.OnCheckInDocument)
        }

        private void VisibilityRoleFunction()
        {
            if (!UserManager.IsAuthorizedFunctions("DO_DOC_VISUALIZZAZOOM"))
            {
                this.DocumentImgZoomFile.Visible = false;
            }

            if (!UserManager.IsAuthorizedFunctions("DO_DOC_VISUALIZZA"))
            {
                this.DocumentImgSignaturePosition.Visible = false;
            }

            if (!UserManager.IsAuthorizedFunctions("DO_DOC_VISUALIZZA"))
            {
                this.DocumentImgSignaturePosition.Visible = false;
            }

            if (!UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA"))
            {
                this.DocumentImgSignature.Visible = false;
                this.DocumentImgCoSignature.Visible = false;
            }

            if (!UserManager.IsAuthorizedFunctions("FIRMA_HSM"))
            {
                this.DocumentImgSignatureHSM.Visible = false;
            }

            if (!UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA_ELETTRONICA") || UserManager.IsAuthorizedFunctions("DO_DISABLE_FIRMA_ELETTRONICA"))
            {
                this.DocumentImgVisureSign.Visible = false;
            }
            if (!UserManager.IsAuthorizedFunctions("DO_START_SIGNATURE_PROCESS"))
            {
                this.DocumentImgStartProcessSignature.Visible = false;
            }
            if (!UserManager.IsAuthorizedFunctions("DO_STATE_SIGNATURE_PROCESS"))
            {
                this.DocumentImgProcessState.Visible = false;
            }
            if(!UserManager.IsAuthorizedFunctions("IMPORTA_FATTURE"))
            {
                this.DocumentImgInvoicePreviewPdf.Visible = false;
            }
        }

        protected void InitializePage()
        {
            this.LoadKeys();
            this.SettingPdfServerSide();

        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DOCUMENT_PDF_CONVERT_ENABLED.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DOCUMENT_PDF_CONVERT_ENABLED.ToString()] == "true")
            {
                this.DocumentPdfConvertEnabled = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_CONVERT_PDF_ON_SIGN.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_CONVERT_PDF_ON_SIGN.ToString()] == "true")
            {
                this.EnableConvertPdfOnSign = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_SINCRONA_LC.ToString())) && Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_SINCRONA_LC.ToString()).Equals("1"))
            {
                this.PdfConversionSynchronousLC = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_LATO_SERVER.ToString())) && Utils.InitConfigurationKeys.GetValue(UserManager.GetInfoUser().idAmministrazione, DBKeys.CONVERSIONE_PDF_LATO_SERVER.ToString()).Equals("1"))
            {
                this.PdfConvertServerSide = true;
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()))
                && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            {
                //if ((!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString()).Equals("1")))
                
                //Valore della Chiave FE_SET_TIPO_FIRMA
                //  0: Annidata
                //  1: Parallela
                //  2: Annidata non modificabile
                //  3: Parallela non modificabile
                string setTipoFirma = string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString())) ? "0" : Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString());
                if (setTipoFirma.Equals("1") || setTipoFirma.Equals("3"))    
                    this.DocumentImgCoSignature.Visible = false;
            }
            else
            {
                this.DocumentImgStartProcessSignature.Visible = false;
                this.DocumentImgProcessState.Visible = false;
                if (UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA"))
                {
                    this.DocumentImgCoSignature.Visible = true;
                }

            }

            //Modifica condizioni Libro Firma per APSS: se il libro firma è attivo ma è abilitata UNLOCK_COFIRMA ripristino la selezione del tipo firma(firma/cofirma)
            //if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) 
            //    && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("1"))
            //{
            //    if ((string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.UNLOCK_COFIRMA.ToString())) || Utils.InitConfigurationKeys.GetValue("0", DBKeys.UNLOCK_COFIRMA.ToString()).Equals("0")))
            //        this.DocumentImgCoSignature.Visible = false;
            //}
            //else
            //{
            //    this.DocumentImgStartProcessSignature.Visible = false;
            //    this.DocumentImgProcessState.Visible = false;
            //    if (UserManager.IsAuthorizedFunctions("DO_DOC_FIRMA"))
            //    {
            //        this.DocumentImgCoSignature.Visible = true;
            //    }
              
            //}
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.Timestamp.Title = Utils.Languages.GetLabelFromCode("TimestampTitle", language);
            this.DocumentImgUploadFile.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgUploadFile", language);
            this.DocumentImgUploadFile.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgUploadFile", language);
            this.DocumentImgViewFile.AlternateText = Utils.Languages.GetLabelFromCode("DocumentViewFile", language);
            this.DocumentImgViewFile.ToolTip = Utils.Languages.GetLabelFromCode("DocumentViewFile", language);
            this.DocumentImgZoomFile.AlternateText = Utils.Languages.GetLabelFromCode("DocumentZoomFile", language);
            this.DocumentImgZoomFile.ToolTip = Utils.Languages.GetLabelFromCode("DocumentZoomFile", language);
            this.DocumentImgSignaturePosition.AlternateText = Utils.Languages.GetLabelFromCode("DocumentSignaturePosition", language);
            this.DocumentImgSignaturePosition.ToolTip = Utils.Languages.GetLabelFromCode("DocumentSignaturePosition", language);
            this.DocumentImgSignature.AlternateText = Utils.Languages.GetLabelFromCode("DocumentSignature", language);
            this.DocumentImgSignature.ToolTip = Utils.Languages.GetLabelFromCode("DocumentSignature", language);
            this.DocumentImgCoSignature.AlternateText = Utils.Languages.GetLabelFromCode("DocumentCoSignature", language);
            this.DocumentImgVisureSign.AlternateText = Utils.Languages.GetLabelFromCode("DocumentElectronicSignature", language);
            this.DocumentImgCoSignature.ToolTip = Utils.Languages.GetLabelFromCode("DocumentCoSignature", language);
            this.DocumentImgVisureSign.ToolTip = Utils.Languages.GetLabelFromCode("DocumentElectronicSignature", language);
            this.DocumentImgSignatureDetails.AlternateText = Utils.Languages.GetLabelFromCode("DocumentSignatureDetails", language);
            this.DocumentImgSignatureDetails.ToolTip = Utils.Languages.GetLabelFromCode("DocumentSignatureDetails", language);
            this.DocumentImgVerifyCrlSignature.AlternateText = Utils.Languages.GetLabelFromCode("DocumentVerifyCrlSignature", language);
            this.DocumentImgVerifyCrlSignature.ToolTip = Utils.Languages.GetLabelFromCode("DocumentVerifyCrlSignature", language);
            this.DocumentImgTimestamp.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgTimestamp", language);
            this.DocumentImgTimestamp.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgTimestamp", language);
            this.DocumentImgSaveLocalFile.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgSaveLocalFile", language);
            this.DocumentImgSaveLocalFile.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgSaveLocalFile", language);
            this.DocumentImgLock.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgLock", language);
            this.DocumentImgLock.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgLock", language);
            this.DocumentImgUnLock.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgUnLock", language);
            this.DocumentImgUnLock.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgUnLock", language);
            this.DocumentImgUnlockWithoutSave.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgUnlockWithoutSave", language);
            this.DocumentImgUnlockWithoutSave.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgUnlockWithoutSave", language);
            this.DocumentImgOpenFile.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgOpenFile", language);
            this.DocumentImgOpenFile.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgOpenFile", language);
            this.DocumentImgOpenModelApplet.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgOpenModel", language);
            this.DocumentImgOpenModelApplet.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgOpenModel", language);
            this.DocumentImgOpenModelActiveX.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgOpenModel", language);
            this.DocumentImgOpenModelActiveX.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgOpenModel", language);
            this.DocumentImgConsolidateStep1.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgConsolidateStep1", language);
            this.DocumentImgConsolidateStep1.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgConsolidateStep1", language);
            this.DocumentImgConvertPdf.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgConvertPDF", language);
            this.DocumentImgConvertPdf.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgConvertPDF", language);
            this.InformationFile.Title = Utils.Languages.GetLabelFromCode("InformationFileTitle", language);
            this.DocumentImgInvoicePreviewPdf.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgInvoicePreviewPDF", language);
            this.DocumentImgInvoicePreviewPdf.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgInvoicePreviewPDF", language);
            this.DocumentImgSignatureHSM.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgSignatureHSM", language);
            this.DocumentImgSignatureHSM.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgSignatureHSM", language);
            this.DocumentImgStartProcessSignature.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgStartProcessSignatureTooltip", language);
            this.DocumentImgStartProcessSignature.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgStartProcessSignatureTooltip", language);

            this.DocumentImgProcessState.ToolTip = Utils.Languages.GetLabelFromCode("DocumentImgProcessStateTooltip", language);
            this.DocumentImgProcessState.AlternateText = Utils.Languages.GetLabelFromCode("DocumentImgProcessStateTooltip", language);
        }

        public virtual void ResetCheckInOutState()
        {
            if (checkInOutPanel.Visible) this.checkInOutPanel.ResetViewState("IsUserEnabled");
            //if (checkInOutAppletPanel.Visible) this.checkInOutAppletPanel.ResetViewState("IsUserEnabled");
        }

        private void ResetModelSession()
        {
            HttpContext.Current.Session["ModelProcessed"] = null;
        }

        public virtual void RefreshButtons(TypeRefresh typeRefresh)
        {
            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();

            //ResetModelSession();

            if (doc != null)
            {
                bool hasTemplate = false;
                bool hasTemplatePPT = false;
                if (DocumentManager.getSelectedAttachId() == null)
                {
                    if (doc.tipologiaAtto != null && !DocumentManager.IsDocumentCheckedOut()
                        && !DocumentManager.IsDocumentAnnul() && !DocumentManager.IsDocumentInBasket() &&
                        ((doc.documenti != null &&
                                doc.documenti.Length == 1 &&
                                doc.template != null &&
                                !string.IsNullOrEmpty(doc.template.PATH_MODELLO_1)
                                ) ||
                                (
                                doc.documenti != null &&
                                doc.documenti.Length > 1 &&
                                !string.IsNullOrEmpty(doc.template.PATH_MODELLO_2)
                                ))
                        )
                    {
                        hasTemplate = true;
                        if (doc.template.PATH_MODELLO_1_EXT.ToUpper() == "PPT" || doc.template.PATH_MODELLO_1_EXT.ToUpper() == "PPTX")
                            hasTemplatePPT = true;
                    }
                }
                else
                {
                    Boolean Attachment_Locked = (DocumentManager.GetSelectedAttachment() != null && CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.GetSelectedAttachment().docNumber, DocumentManager.GetSelectedAttachment().docNumber, UserManager.GetInfoUser(), false, DocumentManager.getSelectedRecord()));
                    if (doc.tipologiaAtto != null && !Attachment_Locked
                        && !DocumentManager.IsDocumentAnnul() && !DocumentManager.IsDocumentInBasket() &&
                        (doc.allegati != null &&
                                doc.allegati.Length > 0 &&
                                doc.template != null &&
                                !string.IsNullOrEmpty(doc.template.PATH_ALLEGATO_1)
                                )
                        )
                    {
                        hasTemplate = true;
                    }
                }

                if (hasTemplatePPT)
                {
                    this.modelType = doc.template.PATH_MODELLO_1_EXT.ToUpper();
                    this.DocumentImgOpenModelActiveX.OnClientClick = "";
                    this.DocumentImgOpenModelActiveX.Attributes.Add("onClick", "return CheckOut('',true);");
                    this.DocumentImgOpenModelApplet.OnClientClick = "";
                    this.DocumentImgOpenModelApplet.Attributes.Add("onClick", "return ajaxModalPopupCheckOutDocument();");
                }
                else
                {
                    this.modelType = string.Empty;
                }

                switch (typeRefresh)
                {
                    case TypeRefresh.E_ALL:
                        this.DocumentImgUploadFile.Enabled = true;
                        this.DocumentImgViewFile.Enabled = true;
                        this.DocumentImgZoomFile.Enabled = true;
                        this.DocumentImgSignaturePosition.Enabled = true;
                        this.DocumentImgSaveLocalFile.Enabled = true;
                        this.DocumentImgVisureSign.Enabled = true;
                        this.DocumentImgSignature.Enabled = true;
                        this.DocumentImgSignatureHSM.Enabled = true;
                        this.DocumentImgLock.Enabled = true;
                        this.DocumentImgOpenModelApplet.Enabled = hasTemplate;
                        this.DocumentImgOpenModelActiveX.Enabled = hasTemplate;
                        this.DocumentImgStartProcessSignature.Enabled = true;
                        if (DocumentManager.IsDocumentCheckedOut() || CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                        {
                            this.DocumentImgUploadFile.Enabled = false;
                            this.DocumentImgSignature.Enabled = false;
                            this.DocumentImgSignatureHSM.Enabled = false;
                            this.DocumentImgCoSignature.Enabled = false;
                            if (!CheckInOut.CheckInOutServices.IsOwnerCheckedOutDocument())
                            {
                                this.DocumentImgUnLock.Enabled = false;
                                this.DocumentImgUnlockWithoutSave.Enabled = false;
                                this.DocumentImgOpenFile.Enabled = false;
                            }
                        }
                        if (DocumentManager.getSelectedRecord().checkOutStatus != null && DocumentManager.getSelectedRecord().checkOutStatus.InConversionePdf)
                        {
                            this.DocumentImgUnLock.Enabled = false;
                            this.DocumentImgUnlockWithoutSave.Enabled = false;
                        }
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_TIMESTAMP_DOC.ToString()]) &&
                            System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ENABLE_TIMESTAMP_DOC.ToString()] == "true")
                        {
                            this.DocumentImgTimestamp.Enabled = true;
                        }
                        this.DocumentImgProcessState.Enabled = true;
                        break;
                    case TypeRefresh.D_ALL:
                        this.DocumentImgViewFile.Enabled = false;
                        this.DocumentImgZoomFile.Enabled = false;
                        this.DocumentImgSignaturePosition.Enabled = false;
                        this.DocumentImgSaveLocalFile.Enabled = false;
                        this.DocumentImgSignature.Enabled = false;
                        this.DocumentImgCoSignature.Enabled = false;
                        this.DocumentImgVisureSign.Enabled = false;
                        this.DocumentImgSignatureHSM.Enabled = false;
                        this.DocumentImgLock.Enabled = false;
                        this.DocumentImgUploadFile.Enabled = false;
                        this.DocumentImgConvertPdf.Enabled = false;
                        this.DocumentImgConsolidateStep1.Visible = false;
                        this.DocumentImgUnLock.Visible = true;
                        this.DocumentImgUnlockWithoutSave.Visible = true;
                        this.DocumentImgOpenFile.Visible = true;
                        this.CheckOutShowStatus.Visible = true;
                        this.DocumentImgOpenModelApplet.Visible = (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET);
                        this.DocumentImgOpenModelActiveX.Visible = (componentType != Constans.TYPE_APPLET && componentType != Constans.TYPE_SOCKET);
                        this.DocumentImgOpenModelApplet.Enabled = false;
                        this.DocumentImgOpenModelActiveX.Enabled = false;
                        this.DocumentImgTimestamp.Enabled = false;
                        //this.DocumentImgTimestamp.ImageUrl = "../Images/Icons/timestamp.png";
                        //this.DocumentImgTimestamp.ImageUrlDisabled = "../Images/Icons/timestamp_disabled.png";
                        //this.DocumentImgTimestamp.OnMouseOverImage = "../Images/Icons/timestamp_hover.png";
                        //this.DocumentImgTimestamp.OnMouseOutImage = "../Images/Icons/timestamp.png";

                        this.DocumentImgLock.Enabled = false;
                        this.DocumentImgUnLock.Enabled = false;
                        this.DocumentImgUnlockWithoutSave.Enabled = false;
                        this.DocumentImgOpenModelApplet.Enabled = false;
                        this.DocumentImgOpenModelActiveX.Enabled = false;
                        this.DocumentImgOpenFile.Enabled = false;
                        this.CheckOutShowStatus.Enabled = false;
                        this.DocumentImgStartProcessSignature.Enabled = false;
                        this.DocumentImgProcessState.Enabled = false;
                        //HERMES MOMENTANEO
                        if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.LITEDOCUMENT.ToString()]))
                        {
                            this.DocumentImgTimestamp.Visible = false;
                            this.DocumentImgSaveLocalFile.Visible = false;
                            this.DocumentImgLock.Visible = false;
                            this.DocumentImgUnLock.Visible = false;
                            this.DocumentImgUnlockWithoutSave.Visible = false;
                            this.DocumentImgOpenFile.Visible = false;
                            this.CheckOutShowStatus.Visible = false;
                            this.DocumentImgOpenModelApplet.Visible = false;
                            this.DocumentImgOpenModelActiveX.Visible = false;
                            this.DocumentImgConvertPdf.Visible = false;
                            this.imgSeparator6.Visible = false;
                            this.DocumentImgVerifyCrlSignature.Visible = false;
                            this.DocumentImgSignatureDetails.Visible = false;
                            this.Image1.Visible = false;
                            this.Image2.Visible = false;
                            this.DocumentImgViewFile.Visible = false;
                        }
                        break;
                    case TypeRefresh.E_UPLOADFILE:
                        Boolean IsDocumentCheckedOut = DocumentManager.IsDocumentCheckedOut() || CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord());
                        if (IsDocumentCheckedOut)
                        {
                            if (DocumentManager.getSelectedRecord().checkOutStatus == null && DocumentManager.getSelectedAttachId() == null)
                            {
                                this.DocumentImgUploadFile.Enabled = false;
                                this.DocumentImgSignature.Enabled = false;
                                this.DocumentImgSignatureHSM.Enabled = false;
                                this.DocumentImgCoSignature.Enabled = false;
                                this.DocumentImgVisureSign.Enabled = false;
                                this.DocumentImgLock.Enabled = false;
                                this.DocumentImgUnLock.Enabled = false;
                                this.DocumentImgUnlockWithoutSave.Enabled = false;
                                this.DocumentImgOpenModelApplet.Enabled = false;
                                this.DocumentImgOpenModelActiveX.Enabled = false;
                                this.DocumentImgOpenFile.Enabled = false;
                                this.CheckOutShowStatus.Enabled = false;
                                this.DocumentImgConvertPdf.Enabled = false;
                                this.DocumentImgOpenModelApplet.Enabled = false;
                                this.DocumentImgOpenModelActiveX.Enabled = false;
                            }
                            else
                            {
                                this.DocumentImgUploadFile.Enabled = false;
                                this.DocumentImgSignature.Enabled = false;
                                this.DocumentImgSignatureHSM.Enabled = false;
                                this.DocumentImgCoSignature.Enabled = false;
                                this.DocumentImgVisureSign.Enabled = false;
                                this.DocumentImgLock.Enabled = false;
                                //DISABILITO : se il documento è bloccato ma non per una conversione 
                                if (
                                        (DocumentManager.getSelectedRecord().checkOutStatus != null && !DocumentManager.getSelectedRecord().checkOutStatus.InConversionePdf) ||
                                        (DocumentManager.getSelectedAttachId() != null && IsDocumentCheckedOut)
                                    )
                                {
                                    this.DocumentImgUnLock.Enabled = true;
                                    this.DocumentImgUnlockWithoutSave.Enabled = true;
                                }
                                
                                this.DocumentImgOpenModelApplet.Enabled = false;
                                this.DocumentImgOpenModelActiveX.Enabled = false;
                                this.DocumentImgOpenFile.Enabled = true;
                                this.CheckOutShowStatus.Enabled = true;
                                this.DocumentImgConvertPdf.Enabled = false;
                                this.DocumentImgOpenModelApplet.Enabled = false;
                                this.DocumentImgOpenModelActiveX.Enabled = false;
                            }
                        }
                        else
                        {
                            this.DocumentImgUploadFile.Enabled = true;
                            this.DocumentImgLock.Enabled = true;
                            this.DocumentImgOpenModelApplet.Enabled = hasTemplate;
                            this.DocumentImgOpenModelActiveX.Enabled = hasTemplate;
                        }
                        this.DocumentImgProcessState.Enabled = true;
                        this.RefreshCheckInOutPanel();

                        break;
                    case TypeRefresh.D_UPLOADFILE:
                        this.DocumentImgUploadFile.Enabled = false;
                        this.SettingPdfServerSide();
                        break;
                    case TypeRefresh.E_SAVELOCALFILE:
                        this.DocumentImgSaveLocalFile.Enabled = true;
                        break;
                    case TypeRefresh.D_SAVELOCALFILE:
                        this.DocumentImgSaveLocalFile.Enabled = false;
                        break;
                    case TypeRefresh.E_POSITIONSIGNATURE:
                        this.DocumentImgSignaturePosition.Enabled = true;
                        break;
                    case TypeRefresh.D_POSITIONSIGNATURE:
                        this.DocumentImgSignaturePosition.Enabled = false;
                        break;
                    case TypeRefresh.D_ZOOM:
                        this.DocumentImgZoomFile.Enabled = false;
                        break;
                    case TypeRefresh.E_ZOOM:
                        this.DocumentImgZoomFile.Enabled = true;
                        break;
                    case TypeRefresh.D_VIEWFILE:
                        this.DocumentImgViewFile.Enabled = false;
                        break;
                    case TypeRefresh.E_VIEWFILE:
                        this.DocumentImgViewFile.Enabled = true;
                        break;
                    case TypeRefresh.D_TIMESTAMP:
                        this.DocumentImgTimestamp.Enabled = false;
                        break;
                    case TypeRefresh.E_TIMESTAMP:
                        this.DocumentImgTimestamp.Enabled = true;
                        break;
                    case TypeRefresh.D_ABORT:
                        this.DocumentImgSaveLocalFile.Enabled = false;
                        this.DocumentImgSignature.Enabled = false;
                        this.DocumentImgSignatureHSM.Enabled = false;
                        this.DocumentImgCoSignature.Enabled = false;
                        this.DocumentImgVisureSign.Enabled = false;
                        this.DocumentImgLock.Enabled = false;
                        this.DocumentImgUploadFile.Enabled = false;
                        this.DocumentImgUnLock.Enabled = false;
                        this.DocumentImgUnlockWithoutSave.Enabled = false;
                        this.DocumentImgOpenFile.Enabled = false;
                        this.CheckOutShowStatus.Enabled = false;
                        this.DocumentImgOpenModelApplet.Enabled = false;
                        this.DocumentImgOpenModelActiveX.Enabled = false;
                        break;
                    case TypeRefresh.D_CONSOLIDATION:
                        //this.DocumentImgSaveLocalFile.Enabled = false;
                        this.DocumentImgSignature.Enabled = false;
                        this.DocumentImgSignatureHSM.Enabled = false;
                        this.DocumentImgCoSignature.Enabled = false;
                        this.DocumentImgVisureSign.Enabled = false;
                        this.DocumentImgLock.Visible = false;
                        this.DocumentImgUploadFile.Enabled = false;
                        this.DocumentImgUnLock.Visible = false;
                        this.DocumentImgUnlockWithoutSave.Visible = false;
                        this.DocumentImgOpenFile.Visible = false;
                        this.CheckOutShowStatus.Visible = false;
                        this.DocumentImgConsolidateStep1.Visible = true;
                        this.DocumentImgOpenModelApplet.Visible = false;
                        this.DocumentImgOpenModelActiveX.Visible = false;
                        this.DocumentImgStartProcessSignature.Visible = false;
                        break;

                    case TypeRefresh.D_CHECKOUT:
                        this.DocumentImgSignature.Enabled = false;
                        this.DocumentImgSignatureHSM.Enabled = false;
                        this.DocumentImgCoSignature.Enabled = false;
                        this.DocumentImgVisureSign.Enabled = false;
                        this.DocumentImgLock.Enabled = false;
                        //this.DocumentImgSaveLocalFile = (CheckInOut.CheckInOutServices.IsOwnerCheckedOutDocument());
                        if (DocumentManager.getSelectedRecord().checkOutStatus != null && DocumentManager.getSelectedRecord().checkOutStatus.InConversionePdf)
                        {
                            this.DocumentImgUnLock.Enabled = false;
                            this.DocumentImgUnlockWithoutSave.Enabled = false;
                        }
                        else
                        {
                            this.DocumentImgUnLock.Enabled = (CheckInOut.CheckInOutServices.IsOwnerCheckedOutDocument());
                            this.DocumentImgUnlockWithoutSave.Enabled = (CheckInOut.CheckInOutServices.IsOwnerCheckedOutDocument());
                        }
                        this.DocumentImgOpenFile.Enabled = (CheckInOut.CheckInOutServices.IsOwnerCheckedOutDocument());
                        this.CheckOutShowStatus.Enabled = true;
                        this.DocumentImgConvertPdf.Enabled = false;
                        this.DocumentImgOpenModelApplet.Enabled = false;
                        this.DocumentImgOpenModelActiveX.Enabled = false;
                        break;
                    case TypeRefresh.D_CONVERTPDF:
                        this.DocumentImgConvertPdf.Enabled = false;
                        break;

                    case TypeRefresh.D_UNLOCKED:
                        this.DocumentImgLock.Enabled = true;
                        this.DocumentImgUnLock.Enabled = false;
                        this.DocumentImgUnlockWithoutSave.Enabled = false;
                        this.DocumentImgOpenModelApplet.Enabled = false;
                        this.DocumentImgOpenModelActiveX.Enabled = false;
                        this.DocumentImgOpenFile.Enabled = false;
                        this.CheckOutShowStatus.Enabled = false;
                        this.DocumentImgOpenModelApplet.Enabled = hasTemplate;
                        this.DocumentImgOpenModelActiveX.Enabled = hasTemplate;
                        break;

                    case TypeRefresh.SIGNED:
                        this.DocumentImgCoSignature.Enabled = true;
                        this.DocumentImgSignatureDetails.Enabled = true;
                        this.DocumentImgVisureSign.Enabled = true;
                        break;
                    case TypeRefresh.D_SIGNED:
                        this.DocumentImgCoSignature.Enabled = false;
                        //this.DocumentImgVisureSign.Enabled = false;
                        this.DocumentImgSignatureDetails.Enabled = false;
                        break;
                    case TypeRefresh.D_COSIGNED:
                        this.DocumentImgCoSignature.Enabled = false;
                        break;
                    case TypeRefresh.D_LIBROFIRMA_LOCK:
                        this.DocumentImgUploadFile.Enabled = false;
                        this.DocumentImgSignature.Enabled = false;
                        this.DocumentImgSignatureHSM.Enabled = false;
                        this.DocumentImgCoSignature.Enabled = false;
                        this.DocumentImgVisureSign.Enabled = false;
                        this.DocumentImgLock.Enabled = false;
                        this.DocumentImgUnLock.Enabled = false;
                        this.DocumentImgUnlockWithoutSave.Enabled = false;
                        this.DocumentImgOpenFile.Enabled = false;
                        this.CheckOutShowStatus.Enabled = false;
                        this.DocumentImgConvertPdf.Enabled = false;
                        this.DocumentImgOpenModelApplet.Enabled = false;
                        this.DocumentImgOpenModelActiveX.Enabled = false;
                        this.DocumentImgStartProcessSignature.Enabled = false;
                        this.DocumentImgSaveLocalFile.Enabled = false;
                        break;
                    case TypeRefresh.D_LIBROFIRMA_UNLOCK:
                        this.DocumentImgUploadFile.Enabled = false;
                        this.DocumentImgSignature.Enabled = true;
                        this.DocumentImgSignatureHSM.Enabled = false;
                        this.DocumentImgCoSignature.Enabled = false;
                        this.DocumentImgVisureSign.Enabled = true;
                        this.DocumentImgLock.Enabled = false;
                        this.DocumentImgUnLock.Enabled = false;
                        this.DocumentImgUnlockWithoutSave.Enabled = false;
                        this.DocumentImgOpenFile.Enabled = false;
                        this.CheckOutShowStatus.Enabled = false;
                        this.DocumentImgConvertPdf.Enabled = false;
                        this.DocumentImgOpenModelApplet.Enabled = false;
                        this.DocumentImgOpenModelActiveX.Enabled = false;
                        this.DocumentImgStartProcessSignature.Enabled = false;
                        this.DocumentImgSaveLocalFile.Enabled = false;
                        break;
                    case  TypeRefresh.E_LIBROFIRMA_UNLOCK:
                         this.DocumentImgSignature.Enabled = true;
                        this.DocumentImgSignatureHSM.Enabled = true;
                        this.DocumentImgCoSignature.Enabled = true;
                        this.DocumentImgVisureSign.Enabled = true;
                        break;
                    case TypeRefresh.D_START_SIGNATURE_PROCESS:
                        this.DocumentImgStartProcessSignature.Enabled = false;
                        break;
                    case TypeRefresh.E_START_SIGNATURE_PROCESS:
                        this.DocumentImgStartProcessSignature.Enabled = true;
                        break;
                    case TypeRefresh.D_PROCESS_STATE:
                        this.DocumentImgProcessState.Enabled = false;
                        break;
                    case TypeRefresh.E_PROCESS_STATE:
                        this.DocumentImgProcessState.Enabled = true;
                        break;
                    case TypeRefresh.D_DIGITAL_SIGN:
                        this.DocumentImgSignature.Enabled = false;
                        this.DocumentImgSignatureHSM.Enabled = false;
                        this.DocumentImgCoSignature.Enabled = false;
                        break;
                    case TypeRefresh.D_ELECTRONIC_SIGN:
                        this.DocumentImgVisureSign.Enabled = false;
                        break;
                }
            }

            this.UpDocumentButtons.Update();
        }


        private bool IsCheckedOutDocument()
        {
            return true;
        }

        protected void DocumentImgZoomFile_Click(object sender, EventArgs e)
        {
            try
            {
                IsZoom = true;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupDocumentViewer", "ajaxModalPopupDocumentViewer();", true);
                NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgViewFile_Click(object sender, EventArgs e)
        {
            if ((ViewDocument)Parent.FindControl("ViewDocument") != null)
            {
                ViewDocument view = (ViewDocument)Parent.FindControl("ViewDocument");
                EventHandler eventLinkViewFileDocument = new EventHandler(view.LinkViewFileDocument);
                eventLinkViewFileDocument.Invoke(sender, e);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "resetEventTarget", "$('#__EVENTTARGET').val('');", true);
            }
            this.UpDocumentButtons.Update();
        }

        protected void DocumentImgStartProcessSignature_Click(object sender, EventArgs e)
        {
            if (!CheckMaxFileSize())
            {
                return;
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupStartProcessSignature", "ajaxModalPopupStartProcessSignature();", true);

        }

        protected void DocumentImgVisureSign_Click(object sender, EventArgs e)
        {
            if (BloccaModificaAllegatoPerDocumentoPrincipaleInLibroFirma())
                return;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupDigitalVisureSelector", "ajaxModalPopupDigitalVisureSelector();", true);
        }

        protected void DocumentImgSignatureHSM_Click(object sender, EventArgs e)
        {
            if (!CheckMaxFileSize())
            {
                return;
            }
            if (BloccaModificaAllegatoPerDocumentoPrincipaleInLibroFirma())
                return;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupHSMSignature", "ajaxModalPopupHSMSignature();", true);
        }

        protected void DocumentImgUploadFile_Click(object sender, EventArgs e)
        {
            if (BloccaModificaAllegatoPerDocumentoPrincipaleInLibroFirma())
                return;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupUplodadFile", "ajaxModalPopupUplodadFile();", true);
        }

        protected void DocumentImgSignature_Click(object sender, EventArgs e)
        {
            if (!CheckMaxFileSize())
            {
                return;
            }
            if (BloccaModificaAllegatoPerDocumentoPrincipaleInLibroFirma())
                return;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupHSMSignature", "ajaxModalPopupDigitalSignSelector();", true);
        }

        private bool CheckMaxFileSize()
        {
            DocsPaWR.FileRequest fileReq = null;
            if (FileManager.GetSelectedAttachment() == null)
            {
                fileReq = UIManager.FileManager.getSelectedFile();
            }
            else
            {
                fileReq = FileManager.GetSelectedAttachment();
            }
            int maxDimFileSign = 0;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString())) &&
               !Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()).Equals("0"))
                maxDimFileSign = Convert.ToInt32(Utils.InitConfigurationKeys.GetValue(UserManager.GetUserInSession().idAmministrazione, Utils.DBKeys.FE_DO_BIG_FILE_MIN.ToString()));
            if (maxDimFileSign > 0 && Convert.ToInt32(fileReq.fileSize) > maxDimFileSign)
            {
                string maxSize = Convert.ToString(Math.Round((double)maxDimFileSign / 1048576, 3));
                string msgDesc = "WarningStartProcessSignatureMaxDimFile";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + maxSize + "');} else {parent.ajaxDialogModal('" + utils.FormatJs(msgDesc) + "', 'warning', '', '" + maxSize + "');}", true);
                return false;
            }
            else
                return true;
        }

        private bool BloccaModificaAllegatoPerDocumentoPrincipaleInLibroFirma()
        {
            bool result = false;
            //Nel caso di allegati, per cui non è attivo un processo di firma, non posso compiere azioni se il documento principale è in libro firma. 
            if(FileManager.GetSelectedAttachment() != null)
            {
                FileRequest fileReq = FileManager.GetSelectedAttachment();
                if(!fileReq.inLibroFirma && DocumentManager.IsDocumentoInLibroFirma(DocumentManager.getSelectedRecord()) && LibroFirmaManager.IsAttivoBloccoModificheDocumentoInLibroFirma())
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');} else {parent.parent.ajaxDialogModal('WarningBloccoModificheDocumentoInLf', 'warning');}", true);
                    result = true;
                }
            }
            return result;
        }

        protected void DocumentImgSignaturePosition_Click(object sender, EventArgs e)
        {
            //Imposto la seguente variabile di sessione a true per risolvere un'anomalia di alcune versioni del browser, ovvero
            //alla chiusura della popup IsPostBack è false e quindi riesegue tutto il codice.
            
            if (!(UIManager.UserManager.IsAuthorizedFunctions("IGNORE_BIGFILE_LIMITATION")) && (!CheckMaxFileSize()))
            {
                return;
            }
            else
            {
                OpenSignaturePopup = true;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupSignature", "ajaxModalPopupSignature();", true);

            }
        }

        protected void DocumentImgConvertPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.DocumentInWorking != null)
                {
                    if (BloccaModificaAllegatoPerDocumentoPrincipaleInLibroFirma())
                        return;

                    DocsPaWR.FileRequest fr = new FileRequest();

                    if (UIManager.DocumentManager.getSelectedAttachId() != null)
                    {
                        fr = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                    }
                    else
                    {
                        fr = FileManager.GetFileRequest();
                    }

                    DocsPaWR.FileDocumento fileDocumento = FileManager.getInstance(DocumentManager.getSelectedRecord().systemId).GetFile(this.Page, fr, false);

                    if (fileDocumento != null &&
                            fileDocumento.content != null &&
                            !string.IsNullOrEmpty(fileDocumento.name))
                    {
                        // APPLET_G
                        if (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET)
                        {
                            FileManager.EnqueueServerPdfConversion(
                                            UserManager.GetInfoUser(),
                                            fileDocumento.content,
                                            fileDocumento.name,
                                            CheckInOutApplet.CheckInOutServices.CurrentSchedaDocumento);

                            //Aggiornamento contesto checkInOutApplet
                            CheckInOutApplet.CheckInOutServices.RefreshCheckOutStatus();
                        }
                        else
                        {

                            FileManager.EnqueueServerPdfConversion(
                                            UserManager.GetInfoUser(),
                                            fileDocumento.content,
                                            fileDocumento.name,
                                            CheckInOut.CheckInOutServices.CurrentSchedaDocumento);

                            //Aggiornamento contesto checkInOut
                            CheckInOut.CheckInOutServices.RefreshCheckOutStatus();
                        }
                    }

                    this.RefreshButtons(TypeRefresh.D_CHECKOUT);

                    ((Literal)((ViewDocument)Parent.FindControl("ViewDocument")).FindControl("LitDocumentConversionPDF")).Text = Utils.Languages.GetLabelFromCode("DocumentButtonsConversionPDF", UIManager.UserManager.GetUserLanguage());
                    ((UpdatePanel)((ViewDocument)Parent.FindControl("ViewDocument")).FindControl("UpBottomButtons")).Update();

                    if (!string.IsNullOrEmpty(this.PageCaller) && this.PageCaller.Equals("DOCUMENT"))
                        ((NttDataWA.Document.Document)this.Page).DisableConvertPdf();

                    // INC000000598064
                    // Disabilito il tasto modifica allegati quando il doc è in conversione
                    Control ctl = Parent.Parent.Parent.Parent.Parent;
                    if (ctl != null)
                    {
                        Control ctl2 = ctl.FindControl("ContentPlaceOldersButtons");
                        if (ctl2 != null)
                        {
                            UpdatePanel panel = (UpdatePanel)ctl2.FindControl("panelButtons");
                            if (panel != null)
                            {
                                Button btn = (Button)panel.FindControl("AttachmentsBtnModify");
                                if (btn != null)
                                {
                                    btn.Enabled = false;
                                }

                                btn = (Button)panel.FindControl("AttachmentsBtnSwap");
                                if (btn != null)
                                {
                                    btn.Enabled = false;
                                }

                                btn = (Button)panel.FindControl("AttachmentsBtnRemove");
                                if (btn != null)
                                {
                                    btn.Enabled = false;
                                }
                                panel.Update();
                            }
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentImgLock_Click(object sender, EventArgs e)
        {
            if (!CheckMaxFileSize())
            {
                return;
            }
            if (BloccaModificaAllegatoPerDocumentoPrincipaleInLibroFirma())
                return;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupCheckOutDocument", "ajaxModalPopupCheckOutDocument();", true);

        }

        protected void DocumentImgUnLock_Click(object sender, EventArgs e)
        {
            if (!CheckMaxFileSize())
            {
                return;
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupCheckInDocument", "ajaxModalPopupCheckInDocument();", true);

        }


        protected void DocumentImgUnlockWithoutSave_Click(object sender, EventArgs e)
        {
            if (!CheckMaxFileSize())
            {
                return;
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupUndoCheckOut", "ajaxModalPopupUndoCheckOut();", true);

        }


        protected void DocumentImgOpenFile_Click(object sender, EventArgs e)
        {
            if (UserManager.IsAuthorizedFunctions("DOWNLOAD_BIG_FILE"))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupOpenLocalCheckOutFile", "ajaxModalPopupOpenLocalCheckOutFile();", true);
            }
            else if (!CheckMaxFileSize())
            {
                return;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupOpenLocalCheckOutFile", "ajaxModalPopupOpenLocalCheckOutFile();", true);
            }
        }

        protected void DocumentImgSaveLocalFile_Click(object sender, EventArgs e)
        {
            if (UserManager.IsAuthorizedFunctions("DOWNLOAD_BIG_FILE"))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupSaveDialog", "ajaxModalPopupSaveDialog();", true);
            }
            else if (!CheckMaxFileSize())
            {
                return;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupSaveDialog", "ajaxModalPopupSaveDialog();", true);
            }
        }

        protected void DocumentImgInvoicePreviewPdf_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupInvoicePreview", "ajaxModalPopupInvoicePreview();", true);
                NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void DisableButtonImageSign()
        {
            this.DocumentImgSignature.Enabled = false;
            this.DocumentImgSignatureHSM.Enabled = false;
            this.DocumentImgCoSignature.Enabled = false;
            this.DocumentImgVisureSign.Enabled = false;
            //this.DocumentImgSignatureDetails.Enabled = false;
        }

        public void EnableButtonImageSign()
        {
            this.DocumentImgSignature.Enabled = true;
            this.DocumentImgSignatureHSM.Enabled = true;
            this.DocumentImgCoSignature.Enabled = true;
            this.DocumentImgVisureSign.Enabled = true;
            //this.DocumentImgSignatureDetails.Enabled = false;
        }

        public void SetButtonImageTimestamp()
        {
            FileRequest FileReq = null;
            //if (DocumentManager.getSelectedNumberVersion() != null)
            //{
            //    FileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            //}
            //if (FileReq == null)
            //{
            //    if (FileManager.GetSelectedAttachment() == null)
            //    {
            //        FileReq = UIManager.FileManager.getSelectedFile();
            //    }
            //    else
            //    {
            //        FileReq = FileManager.GetSelectedAttachment();
            //    }
            //}
            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                FileReq = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
            }
            else
            {
                FileReq = FileManager.GetFileRequest();
            }
            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
            {
                FileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            }
            List<TimestampDoc> timestamps = DocumentManager.getTimestampsDoc(UserManager.GetInfoUser(), FileReq);
            if (timestamps != null && timestamps.Count > 0 && this.DocumentImgTimestamp.Enabled)
            {
                DocsPaWR.TimestampDoc timestampDoc = (DocsPaWR.TimestampDoc)timestamps[0];
                //Timestamp valido
                if (Convert.ToDateTime(timestampDoc.DTA_SCADENZA) > System.DateTime.Now)
                {
                    this.DocumentImgTimestamp.ImageUrl = "../Images/Icons/timestamp_v.png";
                    this.DocumentImgTimestamp.ImageUrlDisabled = "../Images/Icons/timestamp_v_disabled.png";
                    this.DocumentImgTimestamp.OnMouseOverImage = "../Images/Icons/timestamp_v_hover.png";
                    this.DocumentImgTimestamp.OnMouseOutImage = "../Images/Icons/timestamp_v.png";
                }
                //Timestamp scaduto
                if (Convert.ToDateTime(timestampDoc.DTA_SCADENZA) < System.DateTime.Now)
                {
                    this.DocumentImgTimestamp.ImageUrl = "../Images/Icons/timestamp_x.png";
                    this.DocumentImgTimestamp.ImageUrlDisabled = "../Images/Icons/timestamp_x_disabled.png";
                    this.DocumentImgTimestamp.OnMouseOverImage = "../Images/Icons/timestamp_x_hover.png";
                    this.DocumentImgTimestamp.OnMouseOutImage = "../Images/Icons/timestamp_x.png";
                }
            }
            else
            {
                this.DocumentImgTimestamp.ImageUrl = "../Images/Icons/timestamp.png";
                this.DocumentImgTimestamp.ImageUrlDisabled = "../Images/Icons/timestamp_disabled.png";
                this.DocumentImgTimestamp.OnMouseOverImage = "../Images/Icons/timestamp_hover.png";
                this.DocumentImgTimestamp.OnMouseOutImage = "../Images/Icons/timestamp.png";
            }
        }

        private void SettingPdfServerSide()
        {
            //Reset
            this.imgSeparator6.Visible = true;
            this.DocumentImgConvertPdf.Visible = true;

            DocsPaWR.SchedaDocumento documentoSelezionato = null;
            // Reperimento della scheda documento correntemente selezionata nel contesto del checkout
            // APPLET_G
            if (componentType == Constans.TYPE_APPLET || componentType == Constans.TYPE_SOCKET)
            {
                documentoSelezionato = CheckInOutApplet.CheckInOutServices.CurrentSchedaDocumento;
            }
            else
            {

                documentoSelezionato = CheckInOut.CheckInOutServices.CurrentSchedaDocumento;
            }

            if (this.PdfConvertServerSide)
            {
                this.DocumentImgConvertPdf.Enabled = true;

                if (DocumentManager.IsDocumentConsolidate() || DocumentManager.IsDocumentAnnul())
                {
                    // Il documento consolidato non può essere convertito
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    return;
                }

                DocsPaWR.FileRequest fileRequest = new FileRequest();
                if (UIManager.DocumentManager.getSelectedAttachId() != null)
                {
                    fileRequest = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                }
                else
                {
                    fileRequest = FileManager.GetFileRequest();
                }

                //DISABILITO : se non esiste un file selezionato
                if (fileRequest == null)
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    this.DocumentImgInvoicePreviewPdf.Enabled = false;
                    return;
                }

                //DISABILITO : se il documento è nuovo
                if (documentoSelezionato == null || documentoSelezionato.docNumber == "")
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    this.DocumentImgInvoicePreviewPdf.Visible = false;
                    return;
                }

                //DISABILITO : se non esiste un file acquisito
                if (string.IsNullOrEmpty(fileRequest.fileName))
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    this.DocumentImgInvoicePreviewPdf.Visible = false;
                    return;
                }

                int fileSize;
                Int32.TryParse(fileRequest.fileSize, out fileSize);

                if (fileSize == 0)
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    return;
                }
                 

                bool isAllegato = (fileRequest.GetType() == typeof(DocsPaWR.Allegato));
                if (!isAllegato)
                {
                    //DISABILITO : se non è selezionata l'ultima versione del doc
                    // Non se si è in selezione di un allegato: da tab allegati viene sempre visualizzata l'ultima versione dell'allegato
                    if (documentoSelezionato.documenti != null &&
                        documentoSelezionato.documenti.Length > 1 &&
                        !fileRequest.version.Equals(documentoSelezionato.documenti[0].version))
                    {
                        this.imgSeparator6.Visible = false;
                        this.DocumentImgConvertPdf.Visible = false;
                        this.DocumentImgInvoicePreviewPdf.Enabled = false;
                        return;
                    }
                }

                //DISABILITO : se esiste un file acquisito firmato
                /*
                if (fileRequest.fileName.ToUpper().EndsWith("P7M"))
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    return;
                }
                 * */
                if (fileRequest.firmato.Equals("1"))
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    return;
                }

                //DISABILITO : se esiste un file acquisito ed è già un PDF
                if (fileRequest.fileName.ToUpper().EndsWith("PDF"))
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    this.DocumentImgInvoicePreviewPdf.Enabled = false;
                    return;
                }

                if (!(fileRequest.fileName.ToUpper().EndsWith("XML") || (fileRequest.fileName.ToUpper().EndsWith("P7M"))))
                {
                    this.DocumentImgInvoicePreviewPdf.Enabled = false;
                }

                //DISABILITO : se il documento è bloccato ma non per una conversione 
                if (documentoSelezionato.checkOutStatus != null && !documentoSelezionato.checkOutStatus.InConversionePdf)
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    return;
                }

                //Controllo Pacchetto Zanotti
                if (!UserManager.IsRightsWritingInherits(documentoSelezionato))
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    return;
                }

                //Se fattura elettronica disabilito la conversione in PDF
                /*
                if (DocumentManager.IsFatturaElettronica())
                {
                    this.imgSeparator6.Visible = false;
                    this.DocumentImgConvertPdf.Visible = false;
                    return;
                }
                */
                if (UserManager.isFiltroAooEnabled())
                {
                    //DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriWithRF(RoleManager.GetRoleInSession().systemId, "01", string.Empty);
                    DocsPaWR.Registro[] userRegistri = UIManager.RegistryManager.GetRFListInSession();
                    bool visibility = UserManager.verifyRegNoAOO(this.DocumentInWorking, userRegistri);
                    this.DocumentImgConvertPdf.Enabled = true;
                    this.imgSeparator6.Visible = visibility;
                    this.DocumentImgConvertPdf.Visible = visibility;
                    return;
                }

                if (documentoSelezionato != null && !string.IsNullOrEmpty(documentoSelezionato.systemId))
                {
                    //Verifico se il documento è già in conversione

                    if (documentoSelezionato.checkOutStatus != null && documentoSelezionato.checkOutStatus.InConversionePdf)
                    {
                        this.imgSeparator6.Enabled = false;
                        this.DocumentImgConvertPdf.Enabled = false;
                        return;
                    }
                }
            }
            else
            {
                this.imgSeparator6.Visible = false;
                this.DocumentImgConvertPdf.Visible = false;
            }
        }

        public enum TypeRefresh
        {
            E_ALL,
            D_ALL,
            E_UPLOADFILE,
            D_UPLOADFILE,
            E_SAVELOCALFILE,
            D_SAVELOCALFILE,
            E_POSITIONSIGNATURE,
            D_POSITIONSIGNATURE,
            D_ABORT,
            D_CONSOLIDATION,
            D_CHECKOUT,
            D_ZOOM,
            E_ZOOM,
            E_VIEWFILE,
            D_VIEWFILE,
            LITEDOCUMENT,
            D_CONVERTPDF,
            SIGNED,
            D_LOCKED,
            D_UNLOCKED,
            E_TIMESTAMP,
            D_TIMESTAMP,
            D_SIGNED,
            V_IDENTITYCARD,
            NOTV_IDENTITYCARD,
            D_IDENTITYCARD,
            D_LIBROFIRMA_LOCK,
            D_LIBROFIRMA_UNLOCK,
            D_START_SIGNATURE_PROCESS,
            E_START_SIGNATURE_PROCESS,
            D_PROCESS_STATE,
            E_PROCESS_STATE,
            D_DIGITAL_SIGN,
            D_ELECTRONIC_SIGN,
            E_LIBROFIRMA_UNLOCK,
            D_COSIGNED
        }



        /// <summary>
        /// Visualizza/nasconde  i pulsanti del visualizzatore
        /// </summary>
        public virtual void VisibilityButton(TypeVisibilityButton typeVisibilityFinction)
        {
            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();

            if (doc != null)
            {
                switch (typeVisibilityFinction)
                {
                    case TypeVisibilityButton.V_VIEWFILE:
                        this.DocumentImgViewFile.Visible = true;
                        break;
                    case TypeVisibilityButton.U_VIEWFILE:
                        this.DocumentImgViewFile.Visible = false;
                        break;
                    case TypeVisibilityButton.V_ZOOMFILE:
                        this.DocumentImgZoomFile.Visible = true;
                        break;
                    case TypeVisibilityButton.U_ZOOMFILE:
                        this.DocumentImgZoomFile.Visible = false;
                        break;
                    case TypeVisibilityButton.V_UPLOADFILE:
                        this.DocumentImgUploadFile.Visible = true;
                        break;
                    case TypeVisibilityButton.U_UPLOADFILE:
                        this.DocumentImgUploadFile.Visible = false;
                        break;
                }
            }
        }

        public enum TypeVisibilityButton
        {
            V_VIEWFILE,
            U_VIEWFILE,
            V_ZOOMFILE,
            U_ZOOMFILE,
            V_UPLOADFILE,
            U_UPLOADFILE
        }

        #region Session Utility
        [Browsable(true)]
        public string PageCaller
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["pageCaller"] != null)
                {
                    result = HttpContext.Current.Session["pageCaller"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["pageCaller"] = value;
            }
        }

        private bool IsZoom
        {
            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }

        public string modelType
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["templateModelType"] != null)
                {
                    result = HttpContext.Current.Session["templateModelType"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["templateModelType"] = value;
            }
        }

        #endregion

    }
}
