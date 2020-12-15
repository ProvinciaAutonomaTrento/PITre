using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;
using System.IO;

namespace NttDataWA.Popup
{
    public partial class DigitalSignDetails : System.Web.UI.Page
    {

        #region Fields

        private bool isAlternateRow = false;

        private const string IMAGE_CHECK = "../Images/Icons/check_grid.gif";
        private const string IMAGE_NO_CHECK = "../Images/Icons/no_check_grid.gif";
        private const int CODE_CERTIFICATO_SCADUTO = 5127;

        #endregion

        #region Properties

        private FileInformation FileInformation
        {
            get
            {
                return HttpContext.Current.Session["FileInformation"] as FileInformation;
            }
            set
            {
                HttpContext.Current.Session["FileInformation"] = value;
            }
        }

        private VerifySignatureResult _signatureResult {
            get
            {
                FileDocumento signedDocument = this.GetSignedDocument();
                return signedDocument.signatureResult;
            }
        }

        private int _requestDocumentIndex
        {
            get
            {
                int i = -1;
                if (this.trvDettagliFirma.SelectedNode.Value.Contains("&")) i = int.Parse(this.trvDettagliFirma.SelectedNode.Value.Split('&')[1].Split('=')[1]);
                return i;
            }
        }

        private string _requestSignerIndex
        {
            get
            {
                string i = "-1";
                if (this.trvDettagliFirma.SelectedNode.Value.Contains("&"))
                    i = this.trvDettagliFirma.SelectedNode.Value.Split('&')[2].Split('=')[1];
                return i;
            }
        }

        private bool VerifyCRL
        {
            set
            {
                HttpContext.Current.Session["VerifyCRL"] = value;
            }
        }

        private bool? VerifyStatusResult
        {
            get
            {
                return HttpContext.Current.Session["VerifyStatusResult"] as bool?;
            }
            set
            {
                HttpContext.Current.Session["VerifyStatusResult"] = value;
            }
        }

        private CertificateInfo CertificateInfo
        {
            get
            {
                return HttpContext.Current.Session["CertificateInfoDigitalSign"] as CertificateInfo;
            }
            set
            {
                HttpContext.Current.Session["CertificateInfoDigitalSign"] = value;
            }
        }



        private bool IsForwarded
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["IsForwarded"] != null) result = (bool)HttpContext.Current.Session["IsForwarded"];
                return result;

            }
            set
            {
                HttpContext.Current.Session["IsForwarded"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
                this.FillTable();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp1", "reallowOp();", true);
                this.ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        protected void BtnView_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            Session["isPopupOfPopup"] = true;
            Session["CallFromSignDetails"] = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopup", "ajaxModalPopupDocumentViewer();", true);
            NttDataWA.Popup.DocumentViewer.OpenDocumentViewer = true;
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnVerifyCRL_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            Session["VerifyCLR_returnValue"] = null;
            this.VerifyCRL = true;
            string requestType = this.trvDettagliFirma.SelectedNode != null ? this.trvDettagliFirma.SelectedNode.Value : string.Empty;
            if (requestType.Contains("&")) requestType = requestType.Split('&')[0];

            switch (requestType)
            {
                case "originalDocument":
                    //if (CheckIsPresentCadesAndPades())
                    //{
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningDigitalSignDetailsCadesAndPades', 'warning', '', '');} else {parent.ajaxDialogModal('WarningDigitalSignDetailsCadesAndPades', 'warning', '', '');}", true);
                    //    return;
                    //}
                    Session["VerifyCLR_OriginalDocument"] = requestType;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupVerifyCLR", "ajaxModalPopupVerifyCLR();", true);
                    break;

                case "sign":
                    //MEV 2020: Nel caso di verifica della CRL della firma non si apre più il popup ma viene effettuata in questa pagina e si aggiorna la parte destra
                    Session["VerifyCLR_date"] = DateTime.Parse(this.hdnDate.Value);
                    string status = CertificateGetInfo();
                    this.UpdateRowStatusCertificate(status);
                    break;
            }
        }

        private void UpdateRowStatusCertificate(string statusCertificate)
        {
            string extraInfo = string.Empty;
            if (!statusCertificate.Equals("VerifyCRLValid"))
            {
                if (!String.IsNullOrEmpty(this.CertificateInfo.RevocationStatusDescription))
                    extraInfo = ": " + this.CertificateInfo.RevocationStatusDescription;
            }
            switch (statusCertificate)
            {
                case "OK":
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateValid")) + "');", true);
                    break;
                case "VerifyCRLValid":
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLValid")) + "');", true);
                    break;
                case "VerifyCRLExpired":
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLExpired")) + extraInfo + "');", true);
                    break;
                case "VerifyCRLRevoked":
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLRevoked")) + extraInfo + "');", true);
                    break;
                case "VerifyCRLNotValid":
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLNotValid")) + extraInfo + "');", true);
                    break;
                case "VerifyCLRko":
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCLRko")) + extraInfo + "');", true);
                    break;
                case "VerifyCLRNoConnection":
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCLRNoConnection")) + extraInfo + "');", true);
                    break;
                default:
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateUnvalid")) + extraInfo + "');", true);
                    break;
            }
        }

        protected void trvDettagliFirma_SelectedNodeChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp3", "reallowOp();", true);
     
            //this.debug.Text = "VALUE: " + this.trvDettagliFirma.SelectedNode.Value;
            this.FillTable();
            if (this.trvDettagliFirmaElettronica != null && this.trvDettagliFirmaElettronica.SelectedNode != null)
            {
                this.trvDettagliFirmaElettronica.SelectedNode.Selected = false;
                this.UpPnlTreeViewFirmaElettronica.Update();
            }
        }

        protected void TrvDettagliFirmaElettronica_SelectedNodeChanged(object sender, EventArgs e)
        {
            this.plcOriginalDocument.Visible = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp2", "reallowOp();", true);
            string requestType = string.Empty;
            if (this.trvDettagliFirma != null && this.trvDettagliFirma.SelectedNode != null)
            {
                requestType = this.trvDettagliFirma.SelectedNode != null ? this.trvDettagliFirma.SelectedNode.Value : string.Empty;
                if (requestType.Contains("&")) requestType = requestType.Split('&')[0];
                this.trvDettagliFirma.SelectedNode.Selected = false;
                this.UpPnlTreeview.Update();
            }
            //if (string.IsNullOrEmpty(requestType) || !requestType.Equals("originalDocument"))
            //{
            //    this.FillOriginalDocument();
            //}
            this.BtnVerifyCRL.Enabled = false;
            this.UpPnlDetails.Update();
            this.UpPnlButtons.Update();
        }

        #endregion

        #region Methods

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.DocumentViewer.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('DocumentViewer','');", true);
            }
            string requestType = this.trvDettagliFirma.SelectedNode != null ? this.trvDettagliFirma.SelectedNode.Value : string.Empty;
            if (requestType.Contains("&")) requestType = requestType.Split('&')[0];
            if (requestType.Equals("sign") && Session["VerifyCLR_returnValue"] != null &&
                !string.IsNullOrEmpty(Session["VerifyCLR_returnValue"].ToString()))
            {
                //if (Session["VerifyCLR_returnValue"].ToString() == "OK")
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_valido.png');", true);
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateValid")) + "');", true);
                //}
                //else
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateUnvalid")) + "');", true);
                //}

                switch (Session["VerifyCLR_returnValue"].ToString())
                {
                    case "OK":
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_valido.png');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateValid")) + "');", true);
                        break;
                    case "VerifyCRLValid":
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_valido.png');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLValid")) + "');", true);
                        break;
                    case "VerifyCRLExpired":
                         ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                         ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLExpired")) + "');", true);
                        break;
                    case "VerifyCRLRevoked":
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLRevoked")) + "');", true);
                        break;
                    case "VerifyCRLNotValid":
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCRLNotValid")) + "');", true);
                        break;
                    case "VerifyCLRko" :
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCLRko")) + "');", true);
                        break;
                    case "VerifyCLRNoConnection":
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("VerifyCLRNoConnection")) + "');", true);
                        break;
                    default :
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueImage", "$('.certificateImage').attr('src', '../Images/Icons/certificato_non_valido.png');", true);
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setVerifyCLRReturnValueDescription", "$('.certificateDescription').html('" + utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateUnvalid")) + "');", true);
                        break;
                }

                Session["VerifyCLR_returnValue"] = null;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('VerifyCLR','');", true);
            }

            if (requestType.Equals("originalDocument") && Session["VerifyCLR_returnValue"] != null && 
                !string.IsNullOrEmpty(Session["VerifyCLR_returnValue"].ToString()))
            {
                this.plcStatusSignedAndCRL.Visible = true;
                DocsPaWR.FileRequest fileRequest = null;
                if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
                {
                    fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
                }
                else // ho aggiunto il file al documento principale
                {
                    fileRequest = FileManager.getSelectedFile();
                }
                FileInformation = DocumentManager.GetFileInformation(fileRequest, UserManager.GetInfoUser());
                string language = UserManager.GetUserLanguage();

                //Data della verifica
                if (!FileInformation.CrlRefDate.Equals(DateTime.MinValue))
                {
                    this.lblDateCheckUltimateText.Text = FileInformation.CrlRefDate.ToShortDateString();
                }

                //Verifica firma
                this.lblCheckSigned.Text = Utils.Languages.GetLabelFromCode(FileInformation.Signature.ToString(), language);
                if (!string.IsNullOrEmpty(GetImageCheckDetails(FileInformation.Signature)))
                {
                    this.imgCheckSignedResult.Visible = true;
                    this.imgCheckSignedResult.ImageUrl = GetImageCheckDetails(FileInformation.Signature);
                }
                else
                    this.imgCheckSignedResult.Visible = false;

                //Verifica CRL
                this.lblCRL.Text = Utils.Languages.GetLabelFromCode(FileInformation.CrlStatus.ToString(), language);
                if (!string.IsNullOrEmpty(GetImageCheckDetails(FileInformation.CrlStatus)))
                {
                    this.imgCheckCRLResult.Visible = true;
                    this.imgCheckCRLResult.ImageUrl = GetImageCheckDetails(FileInformation.CrlStatus);
                }
                else
                    this.imgCheckCRLResult.Visible = false;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSize", "resizeTable();", true);

                Session["VerifyCLR_returnValue"] = null;
                this.UpPnlDetails.Update();
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            DocsPaWR.FileRequest fileRequest = null;

            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileRequest = FileManager.getSelectedFile();
            }

            //SE FILE è FIRMATO XADES CHIUDO IL POPUP
            /*
            if (Path.GetExtension(fileRequest.fileName).ToUpper().EndsWith("XML"))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "$(function() {ajaxDialogModal('WarningSignDetailsXADES', 'warning', null, null, null, null, 'parent.closeAjaxModal(\\'DigitalSignDetails\\',\\'\\');');});", true);
                return;
            }
            */

            FileInformation = DocumentManager.GetFileInformation(fileRequest, UserManager.GetInfoUser());

            this.BuildTreeView();
            this.BuildTreeViewFirmaElettronica();
            this.BtnVerifyCRL.Enabled = false;
            this.UpPnlButtons.Update();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnView.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsBtnView", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.BtnVerifyCRL.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsBtnVerifyCRL", language);
            this.DocumentViewer.Title = Utils.Languages.GetLabelFromCode("TitleDocumentViewerPopup", language);
            this.VerifyCLR.Title = Utils.Languages.GetLabelFromCode("DigitalSignDetailsBtnVerifyCRL", language);
            this.lblMainHeader.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsMainHeader", language);
            this.lblMainDocStatus.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsMainDocStatus", language);
            this.lblMainDocType.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsMainDocType", language);
            this.lblMainDocOriginalFilename.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsMainDocOriginalFilename", language);
            this.lblMainDocSize.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsMainDocSize", language);
            this.lblMainDocFilename.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsMainDocFilenameP7M", language);
            this.ltCheckSigned.Text = Utils.Languages.GetLabelFromCode("InformationFileLtCheckSigned", language);
            this.ltCheckCRL.Text = Utils.Languages.GetLabelFromCode("InformationFileLtCheckCRL", language);
            this.ltDateCheckUltimate.Text = Utils.Languages.GetLabelFromCode("InformationFileDateCheckUltimate", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "EnableOnlyPreviousToday", "EnableOnlyPreviousToday();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('DigitalSignDetails', '" + retValue + "');", true);
        }

        private void BuildTreeView()
        {
            FileDocumento signedDocument = this.GetSignedDocument();
            if (signedDocument != null && signedDocument.signatureResult!=null && signedDocument.signatureResult.StatusCode != -100)
            {
                VerifySignatureResult signatureResult = signedDocument.signatureResult;

                //this.trvDettagliFirma.Nodes.Add(this.GetNodeCheckDocumento(signatureResult));

                int documentIndex = 0;

                TreeNode documentNode = null;
                TreeNode parentNode = null;
                TreeNode originalDocument = null;

                // Aggiunta del nodo relativo al file originale
                originalDocument = this.GetNodeDocumentoOriginale(signedDocument);
                originalDocument.Selected = true;
                this.trvDettagliFirma.Nodes.Add(originalDocument);
                parentNode = this.trvDettagliFirma.Nodes[0];

                foreach (PKCS7Document document in signatureResult.PKCS7Documents)
                {
                    // Aggiunta del nodo solo se il documento è p7m
                    documentNode = this.GetNodesFirmeDigitali(document, documentIndex, null);
                    parentNode.ChildNodes.Add(documentNode);
                    parentNode = documentNode;

                    documentIndex++;
                }

                //Questo Codice commentato visualizza la marca temporale sulla ROOT del certificato
                //Per visualizzarla è necessario decommentarla
                /*
                if (signatureResult.tsInfo != null)
                {
                    int idx = 0;
                    foreach (TSInfo ts in signatureResult.tsInfo)
                    {
                        Microsoft.Web.UI.WebControls.TreeNode tsNode = new Microsoft.Web.UI.WebControls.TreeNode();
                        tsNode.NavigateUrl = DETAIL_PAGE + "?type=timestamp";
                        tsNode.Target = TARGET;
                        string tsText = "Marca Temporale";
                        tsNode.Text = tsText;
                        this.trvDettagliFirma.Nodes.Add(tsNode);
                    }
                }
                */

                this.trvDettagliFirma_SelectedNodeChanged(null, null);
            }
        }
      

        //meccanismo di caching per evitare di fare la getFile tutte le volte, con conseguente controllo del certificato
        private FileDocumento DocumentoGetFileCached(DocsPaWR.FileRequest fileRequest)
        {
            FileDocumento retval = null;
            if (HttpContext.Current.Session["FileRequest_Cached"] == null)
                HttpContext.Current.Session["FileDocumento_Cached"] = null;

            if (HttpContext.Current.Session["FileDocumento_Cached"] == null)
            {
                retval = DocumentManager.DocumentoGetFile(fileRequest);
                HttpContext.Current.Session["FileRequest_Cached"] = fileRequest;
                HttpContext.Current.Session["FileDocumento_Cached"] = retval;
            }
            else
            {
                if (HttpContext.Current.Session["FileRequest_Cached"] != fileRequest)
                {
                    //filerequest è cambiato
                    retval = DocumentManager.DocumentoGetFile(fileRequest);
                    HttpContext.Current.Session["FileRequest_Cached"] = fileRequest;
                    HttpContext.Current.Session["FileDocumento_Cached"] = retval;

                }
                else
                {
                    retval = (FileDocumento)HttpContext.Current.Session["FileDocumento_Cached"];
                }
            }
            return retval;
        }
        

        private FileDocumento GetSignedDocument()
        {
            DocsPaWR.FileRequest fileRequest = null;
            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileRequest = FileManager.getSelectedFile();
            }
            //Aggiunto per le versioni
            if (!IsForwarded)
            {
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    fileRequest = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }
            }
            DocsPaWR.FileDocumento signedDocument = null;
            if (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != "")
            {
                signedDocument = this.DocumentoGetFileCached(fileRequest);

                if (FileInformation!=null) {
                    if (FileInformation.CrlStatus == VerifyStatus.Valid)
                        this.VerifyStatusResult = true;
                    else if (FileInformation.CrlStatus == VerifyStatus.Invalid)
                        this.VerifyStatusResult = true;
                    else
                        this.VerifyStatusResult = null;
                }
                else
                    this.VerifyStatusResult = null;
            }

            return signedDocument;
        }

        protected void trvDettagliFirma_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
        }

        private TreeNode GetNodeDocumentoOriginale(FileDocumento originalDocument)
        {
            bool isSignedDocument = (originalDocument.signatureResult != null);

            TreeNode node = new TreeNode();
            node.Value = "originalDocument";
            node.Target = "right";
            node.Text = this.GetLabel("DigitalSignDetailsOriginalDoc");

            string documentFileName = string.Empty;
            if (isSignedDocument)
                documentFileName = originalDocument.signatureResult.FinalDocumentName;
            else
                documentFileName = originalDocument.name;

            System.IO.FileInfo info = new System.IO.FileInfo(documentFileName);
            string extFileOrignale = info.Extension;
            if (extFileOrignale != "")
                extFileOrignale = extFileOrignale.Replace(".", "").ToLower();
            info = null;

            string imageUrl =  ResolveUrl(FileManager.getFileIcon(this, extFileOrignale));
            node.ImageUrl = imageUrl.Replace("icon_", "small_");

            return node;
        }

        private TreeNode GetNodesFirmeDigitali(PKCS7Document document, int documentIndex, string signerLevel)
        {
            TreeNode rootNode = new TreeNode();
            rootNode.Text = this.GetLabel("DigitalSignDetailsSigns") + " (" + document.SignersInfo.Length + ")";
            rootNode.Value = string.Empty;

            string imageUrl =  ResolveUrl(FileManager.getFileIcon(this, "p7m"));
            if (document.SignatureType == SignType.PADES)
                imageUrl =  ResolveUrl(FileManager.getFileIcon(this, "pdf"));
            rootNode.ImageUrl = imageUrl.Replace("icon_", "small_");

            int index = 0;

            foreach (SignerInfo info in document.SignersInfo)
            {
                TreeNode signNode = new TreeNode();
                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;

                //ABBATANGELI - Nel caso di certificati di enti non abbiamo Nome e Cognome ma solo CommonName
                string nodeText = info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome;
                if (string.IsNullOrEmpty(nodeText.Trim()))
                {
                    nodeText = info.SubjectInfo.CommonName;
                }
                nodeText = nodeText + " " + document.SignatureType.ToString();

                if (nodeText.Trim() == string.Empty)
                    nodeText = this.GetLabel("DigitalSignDetailsNotAvailable");

                signNode.Text = nodeText;

                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        TreeNode tsNode = new TreeNode();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = this.GetLabel("DigitalSignDetailsTimestamp");
                        tsNode.Text = tsText;
                        tsNode.ImageUrl = "../Images/Icons/small_timestamp.png";
                        signNode.ChildNodes.Add(tsNode);
                    }
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();
                        signNode.ChildNodes.Add(this.GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel,index));
                        signLevels++;
                    }
                }

                rootNode.ChildNodes.Add(signNode);
                signNode = null;

                index++;
            }

            return rootNode;
        }

        private TreeNode GetNodesFirmeDigitali(SignerInfo[] signersInfo, int documentIndex, string signerLevel,int countersignLevel =0)
        {
            TreeNode rootNode = new TreeNode();
            rootNode.Text = this.GetLabel("DigitalSignDetailsSigns")+" (" + signersInfo.Length + ")";
            int index = countersignLevel;

            foreach (SignerInfo info in signersInfo)
            {
                TreeNode signNode = new TreeNode();
                signNode.Value = "sign&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;

                string nodeText = info.SubjectInfo.Cognome + " " + info.SubjectInfo.Nome;
                if (nodeText.Trim() == string.Empty)
                    nodeText = this.GetLabel("DigitalSignDetailsNotAvailable");

                signNode.Text = nodeText;

                if (info.SignatureTimeStampInfo != null)
                {
                    foreach (TSInfo ts in info.SignatureTimeStampInfo)
                    {
                        TreeNode tsNode = new TreeNode();
                        tsNode.Value = "timestamp&documentIndex=" + documentIndex.ToString() + "&index=" + index.ToString() + signerLevel;
                        string tsText = this.GetLabel("DigitalSignDetailsTimestamp");
                        tsNode.Text = tsText;
                        tsNode.ImageUrl = "../Images/Icons/small_timestamp.png";
                        signNode.ChildNodes.Add(tsNode);
                    }
                }

                if (info.counterSignatures != null)
                {
                    int signLevels = 0;
                    foreach (SignerInfo csigner in info.counterSignatures)
                    {
                        List<SignerInfo> tmpLst = new List<SignerInfo>();
                        tmpLst.Add(csigner);
                        signerLevel = ":" + signLevels.ToString();
                        signNode.ChildNodes.Add(GetNodesFirmeDigitali(tmpLst.ToArray(), documentIndex, signerLevel,index));
                        signLevels++;
                    }
                }

                rootNode.ChildNodes.Add(signNode);
                signNode = null;

                index++;
            }

            return rootNode;
        }

        #region TREEVIEW FIRMA ELETTRONICA

        private void BuildTreeViewFirmaElettronica()
        {
            DocsPaWR.FileRequest fileRequest = null;
            if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else // ho aggiunto il file al documento principale
            {
                fileRequest = FileManager.getSelectedFile();
            }
            //Aggiunto per le versioni
            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
            {
                fileRequest = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            }
            List<DocsPaWR.FirmaElettronica> listElectronicSignature = DocumentManager.GetElectronicSignatureDocument(fileRequest.docNumber, fileRequest.versionId);
            if (listElectronicSignature != null && listElectronicSignature.Count > 0)
            {
                this.pnlDetailsFirmaElettronica.Attributes.Add("style", "display:block");
                this.trvDettagliFirmaElettronica.Nodes.Add(BuildNodesElectronicSignature(listElectronicSignature));

                this.trvDettagliFirmaElettronica.ExpandAll();
            }
            else
            {
                this.pnlDetailsFirmaElettronica.Attributes.Add("style", "display:none");
            }
        }

        private TreeNode BuildNodesElectronicSignature(List<DocsPaWR.FirmaElettronica> listElectronicSignature)
        {
            TreeNode nodesElectronicSignature = null;
            TreeNode nodeDateAffixing = null;
            string lblDateAffixing = Utils.Languages.GetLabelFromCode("EsaminaLblDateAffixing", UserManager.GetUserLanguage());
            nodesElectronicSignature = new TreeNode() { Text = Languages.GetLabelFromCode("EsaminaLibroFirmaNodesElectronicSignature", UserManager.GetUserLanguage()) + " (" + listElectronicSignature.Count().ToString() + ")" };
            TreeNode parentNode = null;
            foreach (FirmaElettronica e in listElectronicSignature)
            {
                parentNode = new TreeNode() { Text = e.Firmatario };// + e.DateAffixing.ToString() };
                nodeDateAffixing = new TreeNode() { Text = lblDateAffixing + " " + e.DataApposizione.ToString() };
                parentNode.ChildNodes.Add(nodeDateAffixing);
                nodesElectronicSignature.ChildNodes.Add(parentNode);
            }
            return nodesElectronicSignature;
        }

        protected void trvDettagliFirmaElettronica_Collapsed(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

        }

        #endregion

        private void FillTable()
        {
            this.BtnVerifyCRL.Enabled = false;
            if (trvDettagliFirma.Nodes != null && trvDettagliFirma.Nodes.Count > 0)
            {
                string requestType = this.trvDettagliFirma.SelectedNode != null ? this.trvDettagliFirma.SelectedNode.Value : string.Empty;
                if (requestType.Contains("&")) requestType = requestType.Split('&')[0];

                switch (requestType)
                {
                    case "originalDocument":
                        this.FillOriginalDocument();
                        this.BtnVerifyCRL.Enabled = true;
                        break;

                    case "pk7mDocument":
                        this.FillP7MDocument();
                        break;

                    case "sign":
                        //MEV 2020: Non apro più il popup per la veirfica della firma ma la faccio in questa pagina
                        Session["VerifyCLR_date"] = DocumentManager.GetDataRiferimentoValitaDocumento();
                        Session["VerifyCLR_returnValue"] = null;
                        this.FillSign();
                        this.BtnVerifyCRL.Enabled = true;
                        // Verifica CRL alla selezione della firma
                        this.VerifyCRL = true;
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupVerifyCLR", "ajaxModalPopupVerifyCLR();", true);
                        break;

                    case "certificate":
                        this.FillCertificate();
                        break;

                    case "timestamp":
                        this.FillTimeStamp();
                        break;

                    case "invalid":
                        this.FillInvalid();
                        break;
                }
            }
            else
            {
                this.FillOriginalDocument();
                this.BtnVerifyCRL.Visible = false;
            }

            this.UpPnlDetails.Update();
            this.UpPnlButtons.Update();
        }

        private void FillOriginalDocument()
        {
            this.ShowPlaceHolder("originalDocument");
            
            FileDocumento signedDocument = this.GetSignedDocument();
            VerifySignatureResult signatureResult = signedDocument.signatureResult;
            bool isSignedDocument = (signatureResult != null);

            // img
            string documentFileName = string.Empty;
            if (isSignedDocument)
                documentFileName = signedDocument.signatureResult.FinalDocumentName;
            else
                documentFileName = signedDocument.name;

            System.IO.FileInfo info = new System.IO.FileInfo(documentFileName);
            string extFileOrignale = info.Extension;
            if (extFileOrignale != "")
                extFileOrignale = extFileOrignale.Replace(".", "").ToLower();
            info = null;

            string imageUrl = FileManager.getFileIconBig(this, extFileOrignale);
            this.litMainDocImg.Text = "<img src=\"" + imageUrl + "\" alt=\"\" />" + this.GetLabel("DigitalSignDetailsMainDocInfo");

            // status
            if (isSignedDocument)
            {
                this.litMainDocStatus.Text = this.GetLabel("DigitalSignDetailsMainDocSigned");

                this.plcStatusSignedAndCRL.Visible = true;
                DocsPaWR.FileRequest fileRequest = null;
                if (DocumentManager.getSelectedAttachId() != null) // ho aggiunto il file ad un allegato
                {
                    fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
                }
                else // ho aggiunto il file al documento principale
                {
                    fileRequest = FileManager.getSelectedFile();
                }

                string language = UserManager.GetUserLanguage();

                //Data della verifica
                if (!FileInformation.CrlRefDate.Equals(DateTime.MinValue))
                {
                    this.lblDateCheckUltimateText.Text = FileInformation.CrlRefDate.ToShortDateString();
                }

                //Verifica firma
                this.lblCheckSigned.Text = Utils.Languages.GetLabelFromCode(FileInformation.Signature.ToString(), language);
                if (!string.IsNullOrEmpty(GetImageCheckDetails(FileInformation.Signature)))
                {
                    this.imgCheckSignedResult.Visible = true;
                    this.imgCheckSignedResult.ImageUrl = GetImageCheckDetails(FileInformation.Signature);
                }
                else
                    this.imgCheckSignedResult.Visible = false;

                //Verifica CRL
                this.lblCRL.Text = Utils.Languages.GetLabelFromCode(FileInformation.CrlStatus.ToString(), language);
                if (!string.IsNullOrEmpty(GetImageCheckDetails(FileInformation.CrlStatus)))
                {
                    this.imgCheckCRLResult.Visible = true;
                    this.imgCheckCRLResult.ImageUrl = GetImageCheckDetails(FileInformation.CrlStatus);
                }
                else
                    this.imgCheckCRLResult.Visible = false;

                //Se il file è firmato sia CADES che PADES la verifica non è applicabile
                //bool isCadesAndPades = this.CheckIsPresentCadesAndPades();
                //if (isCadesAndPades)
                //{
                //    this.lblCheckSigned.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsNonApplicabile", language);
                //    this.imgCheckSignedResult.ImageUrl = string.Empty;
                //    this.lblCRL.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsNonApplicabile", language);
                //    this.imgCheckCRLResult.ImageUrl = string.Empty;
                //}
            }
            else
            {
                this.plcStatusSignedAndCRL.Visible = false;
                this.litMainDocStatus.Text = this.GetLabel("DigitalSignDetailsMainDocUnsigned");
            }

            // type
            this.litMainDocType.Text = signedDocument.contentType;

            // original filename
            if (isSignedDocument)
                this.litMainDocOriginalFilename.Text = signatureResult.FinalDocumentName;
            else
                this.litMainDocOriginalFilename.Text = signedDocument.name;

            // size
            this.litMainDocSize.Text = signedDocument.length + " KB";

            // filename
            if (isSignedDocument)
            {
                bool isPades = false;
                if(signatureResult.PKCS7Documents != null && signatureResult.PKCS7Documents.Length > 0)
                    isPades = signatureResult.PKCS7Documents[0].SignAlgorithm.Contains("PADES");
                this.litMainDocFilename.Text = signatureResult.FinalDocumentName;

                if (isPades)
                {
                    this.lblMainDocFilename.Text = this.GetLabel("DigitalSignDetailsMainDocFilename");
                }
                else
                {
                    this.litMainDocFilename.Text = this.getFinalFileName();

                }
            }
            /*
            if (this.VerifyStatusResult != null)
            {
                this.plcMainVerifyStatus.Visible = true;
                this.lblMainVerifyStatus.Text = this.GetLabel("DigitalSignDetailsMainVerifyStatus");

                if (this.VerifyStatusResult==true)
                    this.litMainVerifyStatus.Text = this.GetLabel("DigitalSignDetailsMainVerifyStatusOk");
                else
                    this.litMainVerifyStatus.Text = this.GetLabel("DigitalSignDetailsMainVerifyStatusKo");
            }
             * */
        }

        private string GetImageCheckDetails(VerifyStatus status)
        {
            string result = string.Empty;
            if (status.Equals(VerifyStatus.Valid))
                result = IMAGE_CHECK;
            else
                if (status.Equals(VerifyStatus.Invalid))
                    result = IMAGE_NO_CHECK;
            return result;
        }

        private string getFinalFileName()
        {
            string retVal = string.Empty;
            string pathAndName = FileManager.getSelectedFile().fileName;
            if (!string.IsNullOrEmpty(pathAndName))
            {
                string[] arrayString = pathAndName.Split('\\');
                if (arrayString.Length > 0)
                    retVal = arrayString[arrayString.Length - 1];
            }

            return retVal;
        }

        private void FillSign()
        {
            this.ShowPlaceHolder("table");

            // Dati controllo
            this.AppendRowsStatus();
            isAlternateRow = !isAlternateRow;

            // Dati certificato
            this.AppendRowsCertificate();
            isAlternateRow = !isAlternateRow;

            // Dati soggetto
            this.AppendRowsSoggetto();
            isAlternateRow = !isAlternateRow;

            // Dati relativi all'algoritmo e firma digitale
            this.AppendRowsDocumentSign();
            isAlternateRow = !isAlternateRow;
        }

        private void FillP7MDocument()
        {
            PKCS7Document document = this._signatureResult.PKCS7Documents[this._requestDocumentIndex];

            TableRow row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsFilename");
            row.Cells[2].Text = document.DocumentFileName;
            if (!document.SignAlgorithm.Contains("PADES"))
            {
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsFilenameP7M");
                row.Cells[2].Text += ".P7M";
            }
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsLevel");
            row.Cells[2].Text = document.Level.ToString();
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsAlgorithm");
            row.Cells[2].Text = document.SignAlgorithm;
            row = null;
            /*
            if (this._signatureResult.tsInfo != null)
                this.AppendRowsTimeStamp(this._signatureResult.tsInfo);
            */
            document = null;
        }

        private void FillCertificate()
        {
            this.AppendRowsCertificate();
            isAlternateRow = !isAlternateRow;
        }

        private void FillTimeStamp()
        {
            TSInfo[] tsinfo = null;
            if (this._requestDocumentIndex == -1 && this._requestSignerIndex=="-1")
            {
                tsinfo = this._signatureResult.DocumentTimeStampInfo;
            }
            else
            {
                //SignerInfo signerInfo = this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
                SignerInfo signerInfo = this.getSignerInfo(this._requestDocumentIndex, this._requestSignerIndex);
                tsinfo = signerInfo.SignatureTimeStampInfo;

            }
            this.AppendRowsTimeStamp(tsinfo);
            isAlternateRow = !isAlternateRow;
        }

        private void FillInvalid()
        {
            TableRow row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsDocStatus");
            row.Cells[2].Text = this.GetLabel("DigitalSignDetailsSignInvalid");
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsErrCode");
            row.Cells[2].Text = this._signatureResult.StatusCode.ToString();
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsErrDescr");
            row.Cells[2].Text = this._signatureResult.StatusDescription;
            row = null;
        }

        private void CreateHeaderTable()
        {
            TableHeaderCell cell1 = new TableHeaderCell();
            cell1.Text = this.GetLabel("DigitalSignDetailsTableParameter");

            TableHeaderCell cell2 = new TableHeaderCell();
            cell2.Text = this.GetLabel("DigitalSignDetailsTableValue");
            cell2.ColumnSpan = 3;
            
            TableRow row = new TableRow();
            row.Cells.Add(cell1);
            row.Cells.Add(cell2);

            this.tblSignedDocument.Rows.Add(row);
        }

        private void AppendRowsStatus_old()
        {
            //SignerInfo signerInfo = this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, this._requestSignerIndex);
            TableRow row = this.CreateHeaderTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsVerifyResult").ToUpper();

            #region STATO DELLA FIRMA
            //Emanuela 06-05-2014: non facciamo più vedere qui lo stato della firma
            /*
            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsSignStatus");
            row.Cells[1].Controls.Add(this.GetStatusFirmaImage(signerInfo != null));
            */
            // Per verificare la validità della firma,
            // verifica la presenza di almeno un firmatario
            /*
            if (signerInfo.CertificateInfo.ThumbPrint != null && signerInfo.CertificateInfo.ThumbPrint != "")
            {
                row.Cells[2].Text = this.GetLabel("DigitalSignDetailsValid");
                //controlliamo l'aa
                if (this._signatureResult.StatusCode == -5)
                    row.Cells[2].Text = this.GetLabel("DigitalSignDetailsCadesNonCompliant");
            }
            else
                row.Cells[2].Text = this.GetLabel("DigitalSignDetailsInvalid");
            */
            // nuova gestione validità firma
            //if (this._signatureResult.StatusCode==0)
            //    row.Cells[2].Text = this.GetLabel("DigitalSignDetailsValid");
            //else if (this._signatureResult.StatusCode == -5)
            //    row.Cells[2].Text = this.GetLabel("DigitalSignDetailsCadesNonCompliant");
            //else
            //    row.Cells[2].Text = this.GetLabel("DigitalSignDetailsInvalid");

            /*
            if (this._signatureResult.ErrorMessages!=null)
                foreach (string msg in this._signatureResult.ErrorMessages)
                    row.Cells[2].Text += "<p>"+msg+"</p>\n";
            */
            #endregion

            row = this.CreateStandardTableRow();
            row.ID = "DigitalSignDetailsCertificateStatus";
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateStatus");
            if (this.VerifyStatusResult==true)
                row.Cells[1].Controls.Add(this.GetStatusImage(true));
            else
                row.Cells[1].Controls.Add(this.GetStatusImage(signerInfo.CertificateInfo.RevocationStatus == 0));
            row.Cells[2].Text = "<span class=\"certificateDescription\">"+signerInfo.CertificateInfo.RevocationStatusDescription+"</span>";
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsVerifyCLR");
            if (this._signatureResult.CRLOnlineCheck)
                row.Cells[2].Text = this.GetLabel("DigitalSignDetailsVerifyCLROnline");
            else
                row.Cells[2].Text = this.GetLabel("DigitalSignDetailsVerifyCLRLocal");
            row = null;

            row = this.CreateStandardTableRow();
            CustomTextArea textbox = new CustomTextArea();
            textbox.ID = "txtDate";
            textbox.ClientIDMode = ClientIDMode.Static;
            textbox.CssClass = "txt_textdata datepicker";
            textbox.CssClassReadOnly = "txt_textdata_disabled";
            textbox.Text = utils.formatDataDocsPa(DocumentManager.GetDataRiferimentoValitaDocumento());
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsVerifyCLRDate");
            row.Cells[2].Controls.Add(textbox);

            //se presente, inserisco la data della marca temporale
            DocsPaWR.FileRequest fileRequest = null;
            if (DocumentManager.getSelectedAttachId() != null)
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else 
            {
                fileRequest = FileManager.getSelectedFile();
            }
            //Aggiunto per le versioni
            if (!IsForwarded)
            {
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    fileRequest = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }
            }
            List<DocsPaWR.TimestampDoc> timestamp = DocumentManager.getTimestampsDoc(UserManager.GetInfoUser(), fileRequest);
            if (timestamp != null && timestamp.Count > 0)
            {
                row = this.CreateStandardTableRow();
                row.ID = "DigitalSignDetailsDateTimestamp";
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsDateTimestamp");
                row.Cells[2].Text = "<p><span>" + timestamp[0].DTA_CREAZIONE + "</span></p>";
                row = null;
            }
        }

        private void AppendRowsStatus()
        {
            string status = CertificateGetInfo();
            string extraInfo = string.Empty;
            if (!status.Equals("VerifyCRLValid"))
            {
                if (!String.IsNullOrEmpty(this.CertificateInfo.RevocationStatusDescription))
                    extraInfo = ": " + this.CertificateInfo.RevocationStatusDescription;
            }

            TableRow row = this.CreateHeaderTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsVerifyResult").ToUpper();

            row = this.CreateStandardTableRow();
            row.ID = "DigitalSignDetailsCertificateStatus";
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateStatus");
            row.Cells[1].Controls.Add(this.GetCertificateImage(status));
            row.Cells[2].Text = "<span class=\"certificateDescription\">" + GetCertificateDescription(status) + extraInfo + "</span>";

            row = null;
            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsVerifyCLR");
            row.Cells[2].Text = this._signatureResult.CRLOnlineCheck ? this.GetLabel("DigitalSignDetailsVerifyCLROnline") : this.GetLabel("DigitalSignDetailsVerifyCLRLocal");

            row = null;
            row = this.CreateStandardTableRow();
            CustomTextArea textbox = new CustomTextArea();
            textbox.ID = "txtDate";
            textbox.ClientIDMode = ClientIDMode.Static;
            textbox.CssClass = "txt_textdata datepicker";
            textbox.CssClassReadOnly = "txt_textdata_disabled";
            textbox.Text = utils.formatDataDocsPa(DocumentManager.GetDataRiferimentoValitaDocumento());
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsVerifyCLRDate");
            row.Cells[2].Controls.Add(textbox);

            //se presente, inserisco la data della marca temporale
            DocsPaWR.FileRequest fileRequest = null;
            if (DocumentManager.getSelectedAttachId() != null)
            {
                fileRequest = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId());
            }
            else
            {
                fileRequest = FileManager.getSelectedFile();
            }
            //Aggiunto per le versioni
            if (!IsForwarded)
            {
                if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                {
                    fileRequest = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                }
            }
            List<DocsPaWR.TimestampDoc> timestamp = DocumentManager.getTimestampsDoc(UserManager.GetInfoUser(), fileRequest);
            if (timestamp != null && timestamp.Count > 0)
            {
                row = this.CreateStandardTableRow();
                row.ID = "DigitalSignDetailsDateTimestamp";
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsDateTimestamp");
                row.Cells[2].Text = "<p><span>" + timestamp[0].DTA_CREAZIONE + "</span></p>";
                row = null;
            }
        }

        private string CertificateGetInfo()
        {
            string retValue = "VerifyCLRko";
            this.CertificateInfo = getSignerInfo(this._requestDocumentIndex, this._requestSignerIndex).CertificateInfo;
            this.CertificateInfo = DocumentManager.CertificateGetInfo(CertificateInfo);
            if (this.CertificateInfo != null)
            {
                retValue = "VerifyCLRok";
                DateTime date = (DateTime)Session["VerifyCLR_date"];
                //SE LA DATA INSERITA è PRECEDENTE ALLA DATA DI INIZIO VALIDITà
                if (this.CertificateInfo.RevocationStatus == -500)
                {
                    retValue = "VerifyCLRNoConnection";
                    return retValue;
                }

                if (!utils.verificaIntervalloDate(date.ToString(), CertificateInfo.ValidFromDate.ToString()))
                {
                    retValue = "VerifyCRLNotValid";
                }
                else
                {
                    //SE IL CERTIFICATO HA UNA DATA DI REVOCA
                    if (!CertificateInfo.RevocationDate.Equals(DateTime.MinValue))
                    {
                        if (utils.verificaIntervalloDate(CertificateInfo.RevocationDate.ToString(), date.ToString()))
                        {
                            retValue = utils.verificaIntervalloDate(CertificateInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLValid" : "VerifyCRLExpired";
                        }
                        else
                        {
                            retValue = utils.verificaIntervalloDate(CertificateInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLRevoked" : "VerifyCRLExpired";
                        }
                    }
                    else
                    {

                        bool valid = utils.verificaIntervalloDate(CertificateInfo.ValidToDate.ToString(), date.ToString());
                        if (valid && (this.CertificateInfo.RevocationStatus == CODE_CERTIFICATO_SCADUTO || this.CertificateInfo.RevocationStatus == 0))
                        {
                            retValue = "VerifyCRLValid";
                        }
                        else
                        {
                            //è presente un errore..
                            retValue = this.CertificateInfo.RevocationStatus != 0 ? "VerifyCRLInvalid" : "VerifyCRLExpired";
                        }

                    }
                }
            }

            return retValue;
        }

        private HtmlImage GetCertificateImage(string statusCerticate)
        {
            string imageUrl = string.Empty;
            HtmlImage img = this.CreateImage();
            img.Attributes["class"] = "certificateImage";

            switch (statusCerticate)
            {
                case "OK":
                    imageUrl = "../Images/Icons/certificato_valido.png";
                    break;
                case "VerifyCRLValid":
                    imageUrl = "../Images/Icons/certificato_valido.png";
                    break;
                case "VerifyCRLExpired":
                    imageUrl = "../Images/Icons/certificato_non_valido.png";
                    break;
                case "VerifyCRLRevoked":
                    imageUrl = "../Images/Icons/certificato_non_valido.png";
                    break;
                case "VerifyCRLNotValid":
                    imageUrl = "../Images/Icons/certificato_non_valido.png";
                    break;
                case "VerifyCLRko":
                    imageUrl = "../Images/Icons/certificato_non_valido.png";
                    break;
                case "VerifyCLRNoConnection":
                    imageUrl = "../Images/Icons/certificato_non_valido.png";
                    break;
                default:
                    imageUrl = "../Images/Icons/certificato_non_valido.png";
                    break;
            }
            img.Src = imageUrl;

            return img;
        }

        private string GetCertificateDescription(string statusCerticate)
        {
            string retValue = string.Empty;
            switch (statusCerticate)
            {
                case "OK":
                    retValue = utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateValid"));
                    break;
                case "VerifyCRLValid":
                    retValue = utils.FormatJs(this.GetLabel("VerifyCRLValid"));
                    break;
                case "VerifyCRLExpired":
                    retValue = utils.FormatJs(this.GetLabel("VerifyCRLExpired"));
                    break;
                case "VerifyCRLRevoked":
                    retValue = utils.FormatJs(this.GetLabel("VerifyCRLRevoked"));
                    break;
                case "VerifyCRLNotValid":
                    retValue = utils.FormatJs(this.GetLabel("VerifyCRLNotValid"));
                    break;
                case "VerifyCLRko":
                    retValue = utils.FormatJs(this.GetLabel("VerifyCLRko"));
                    break;
                case "VerifyCLRNoConnection":
                    retValue = utils.FormatJs(this.GetLabel("VerifyCLRNoConnection"));
                    break;
                default:
                    retValue = utils.FormatJs(this.GetLabel("DigitalSignDetailsCertificateUnvalid"));
                    break;
            }

            return retValue;
        }

        private void AppendRowsCertificate()
        {
            //SignerInfo signerInfo = this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, this._requestSignerIndex);

            CertificateInfo certInfo = signerInfo.CertificateInfo;

            TableRow row = this.CreateHeaderTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificate").ToUpper();
            row = null;

            //Mev Firma1 < aggiunta info ente certificatore
            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateAgency");
            row.Cells[2].Text = utils.getEnteCertificatore(certInfo.IssuerName);
            row = null;
            //>
            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateSN");
            row.Cells[2].Text = certInfo.SerialNumber;
            row = null;

            //Mev Firma1 < posizionameto orizzontale delle info date dal - al
            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateValidFrom");
            row.Cells[2].Text = string.Format("{0}&nbsp; &nbsp; &nbsp;&nbsp; " + this.GetLabel("DigitalSignDetailsCertificateValidTo") + ": {1}", certInfo.ValidFromDate.ToLongDateString(), certInfo.ValidToDate.ToLongDateString()); ;
            row = null;
            //>

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateAlgorithm");
            row.Cells[2].Text = certInfo.SignatureAlgorithm;
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateSigner");

            //ABBATANGELI - Nel caso di certificati di enti non abbiamo Nome e Cognome ma solo CommonName
            //row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.Cognome + " " + signerInfo.SubjectInfo.Nome);
            string firmatario = signerInfo.SubjectInfo.Cognome + " " + signerInfo.SubjectInfo.Nome;
            row.Cells[2].Text = this.GetItemValue((string.IsNullOrEmpty(firmatario.Trim())?signerInfo.SubjectInfo.CommonName:firmatario));
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCertificateThumbprint");
            row.Cells[2].Text = certInfo.ThumbPrint;
            row = null;

            certInfo = null;
        }

        private void AppendRowsSoggetto()
        {
            //SignerInfo signerInfo = this._signatureResult.PKCS7Documents[this._requestDocumentIndex].SignersInfo[this._requestSignerIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, this._requestSignerIndex);

            TableRow row = this.CreateHeaderTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsObject").ToUpper();
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsName");
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.Nome);
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsSurname");
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.Cognome);
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTaxId");
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.CodiceFiscale);
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsBirthday");
            row.Cells[2].Text = this.GetDateValue(signerInfo.SubjectInfo.DataDiNascita);
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsOrganization");
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.Organizzazione);
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsRole");
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.Ruolo);
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsCountry");
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.Country);
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsIDHolder");
            row.Cells[2].Text = this.GetItemValue(signerInfo.SubjectInfo.CertId);
            row = null;
        }

        private void AppendRowsDocumentSign()
        {
            PKCS7Document document = this._signatureResult.PKCS7Documents[this._requestDocumentIndex];
            //SignerInfo signerInfo = document.SignersInfo[this._requestSignerIndex];
            SignerInfo signerInfo = getSignerInfo(this._requestDocumentIndex, this._requestSignerIndex);

            CertificateInfo certInfo = signerInfo.CertificateInfo;

            TableRow row = this.CreateHeaderTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsDocSign").ToUpper();
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsAlgorithm");
            //row.Cells[2].Text = document.SignAlgorithm;
            row.Cells[2].Text = signerInfo.SignatureAlgorithm;
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsImprint");
            row.Cells[2].Text = document.SignHash;
            row = null;

            row = this.CreateStandardTableRow();
            row.Cells[0].Text = this.GetLabel("DigitalSignDetailsBackSigner");
            row.Cells[2].Text = signerInfo.isCountersigner ? this.GetLabel("DigitalSignDetailsBackSignerTrue") : this.GetLabel("DigitalSignDetailsBackSignerFalse");
            row = null;

            if (signerInfo.SigningTime != DateTime.MinValue)
            {
                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsBackSignDate");
                row.Cells[2].Text = signerInfo.SigningTime.ToLocalTime().ToString();
                row = null;
            }
        }

        private void AppendRowsTimeStamp(TSInfo[] tsInfo)
        {
            foreach (TSInfo tsi in tsInfo)
            {
                TableRow row = this.CreateHeaderTableRow();
                row.Cells[0].Text = String.Format(this.GetLabel("DigitalSignDetailsTimestamp").ToUpper()/*, tsi.TSType.ToString()*/);

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTimestampIssuer");
                row.Cells[2].Text = tsi.TSANameIssuer;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTimestampIssuingAuthority");
                row.Cells[2].Text = tsi.TSANameSubject;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTimestampDateMarking");
                row.Cells[2].Text = tsi.TSdateTime.ToLocalTime().ToString();
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTimestampSerialMarking");
                row.Cells[2].Text = tsi.TSserialNumber;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTimestampImprintMarking");
                row.Cells[2].Text = tsi.TSimprint;
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTimestampDateStartMarking");
                row.Cells[2].Text = tsi.dataInizioValiditaCert.ToLocalTime().ToString();
                row = null;

                row = this.CreateStandardTableRow();
                row.Cells[0].Text = this.GetLabel("DigitalSignDetailsTimestampDateEndMarking");
                row.Cells[2].Text = tsi.dataFineValiditaCert.ToLocalTime().ToString();
                row = null;
            }
        }

        private TableRow CreateHeaderTableRow()
        {
            TableRow row = new TableRow();
            if (isAlternateRow)
                row.CssClass = "AlternateRow";
            
            this.AppendHeaderTableCell(row);
            this.AppendHeaderTableCell(row);
            this.AppendHeaderTableCell(row);
            this.tblSignedDocument.Rows.Add(row);
            return row;
        }

        private void AppendHeaderTableCell(TableRow row)
        {
            TableCell cell = new TableCell();
            if (row.Cells.Count == 1)
                cell.CssClass = "header-image";
            else
                cell.CssClass = "header";
            row.Cells.Add(cell);
        }

        private TableRow CreateStandardTableRow()
        {
            TableRow row = new TableRow();
            if (isAlternateRow)
                row.CssClass = "AlternateRow";

            this.AppendStandardTableCell(row);
            this.AppendStandardTableCell(row);
            this.AppendStandardTableCell(row);
            this.tblSignedDocument.Rows.Add(row);

            return row;
        }

        private void AppendStandardTableCell(TableRow row)
        {
            TableCell cell = new TableCell();
            if (row.Cells.Count==1)
                cell.CssClass = "image";
            else
                cell.CssClass = "normal";
            row.Cells.Add(cell);
        }

        private string GetItemValue(string item)
        {
            if (item == null || item.Trim() == string.Empty)
                return string.Empty;
            else
                return item;
        }

        private string GetDateValue(string item)
        {
            if (item == null || item.Trim() == string.Empty)
            {
                return string.Empty;
            }
            else
            {
                if (utils.isDate(item))
                    return utils.formatStringToDate(item).ToLongDateString();
                else
                    return string.Empty;
            }
        }

        private HtmlImage CreateImage()
        {
            HtmlImage img = new HtmlImage();
            img.ID = "ImageStatus";
            img.Align = "center";
            return img;
        }

        private HtmlImage GetStatusFirmaImage(bool statusValid)
        {
            string imageUrl = @"..\Images\Icons\";

            HtmlImage img = this.CreateImage();
            if (statusValid)
                imageUrl += "flag_ok.png";
            else
                imageUrl += "no_check_grid.gif";
            img.Src = imageUrl;

            return img;
        }

        private HtmlImage GetStatusImage(bool statusValid)
        {
            string imageUrl = @"..\images\Icons\";

            HtmlImage img = this.CreateImage();
            img.Attributes["class"] = "certificateImage";
            if (statusValid)
                imageUrl += "certificato_valido.png";
            else if (this.VerifyStatusResult==false)
                imageUrl += "certificato_non_valido.png";
            else
                imageUrl += "certificato_np.png";
            img.Src = imageUrl;

            return img;
        }

        private void ShowPlaceHolder(string p)
        {
            this.plcOriginalDocument.Visible = false;
            this.plcTable.Visible = false;

            switch (p)
            {
                case "originalDocument":
                    this.plcOriginalDocument.Visible = true;
                    break;
                case "table":
                    this.plcTable.Visible = true;
                    this.tblSignedDocument.Controls.Clear();
                    this.CreateHeaderTable();
                    break;
            }
        }

        private bool CheckIsPresentCadesAndPades()
        {
            bool result = false;
            FileDocumento signedDocument = this.GetSignedDocument();
            bool cades = false;
            bool pades = false;
            if (signedDocument != null && signedDocument.signatureResult != null)
            {
                VerifySignatureResult signatureResult = signedDocument.signatureResult;
                foreach (PKCS7Document document in signatureResult.PKCS7Documents)
                {
                    if (document.SignatureType == SignType.PADES)
                        pades = true;
                    if (document.SignatureType == SignType.CADES)
                        cades = true;
                    if (cades && pades)
                        break;
                }
            }
            result = cades && pades;
            return result;
        }

        private Control GetControlById(Control owner, string controlID)
        {
            Control myControl = null;
            // cycle all controls
            if (owner.Controls.Count > 0)
            {
                foreach (Control c in owner.Controls)
                {
                    myControl = GetControlById(c, controlID);
                    if (myControl != null) return myControl;
                }
            }
            if (controlID.Equals(owner.ID)) return owner;
            return null;
        }

        SignerInfo getSignerInfo(int documentIndex, string sIndex)
        {
            string[] singerIndexes = sIndex.Split(':');
            List<Int32> intLst = new List<int>();

            foreach (string sIdx in singerIndexes)
            {
                int idx;
                Int32.TryParse(sIdx, out idx);
                intLst.Add(idx);
            }


            SignerInfo[] rootSI = _signatureResult.PKCS7Documents[documentIndex].SignersInfo;

            if (intLst.Count == 1)
                return rootSI[intLst.FirstOrDefault()];

            //voglio i controfirmatari
            int count = 1;
            foreach (int sIdx in intLst)
            {
                if (count == intLst.Count)
                    break;

                rootSI = rootSI[sIdx].counterSignatures;
                count++;

            }

            int singerIndex = intLst.LastOrDefault();
            return rootSI[singerIndex];
        }

        #endregion

    }
}
