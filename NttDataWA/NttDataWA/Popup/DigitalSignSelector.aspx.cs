using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using NttDataWA.Utils;
using NttDataWA.SmartClient;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class DigitalSignSelector : Page
    {
        public static string componentType = Constans.TYPE_SMARTCLIENT;
        public static string randomVersion = "";
        public static string CommandType = "";
        public static string lblExpired = string.Empty;

        private string language;

        private bool forzaCofirma;

        protected void Page_Load(object sender, EventArgs e)
        {
                Response.Expires = -1;
                randomVersion = "?v=" + Guid.NewGuid().ToString("N");
                string componentCall = string.Empty;
                
                componentType = UserManager.getComponentType(Request.UserAgent);

                if (!IsPostBack)
                {
                    this.InitializeKeys();
                    if (HttpContext.Current.Session["CommandType"] != null)
                    {
                        CommandType = HttpContext.Current.Session["CommandType"].ToString();
                    }
                    else
                        CommandType = "";
                }

                switch (componentType)
                {
                    case Constans.TYPE_ACTIVEX:
                    case Constans.TYPE_SMARTCLIENT:
                        componentCall = "return signSmartClient();";
                        this.pnlApplet.Visible = false;
                        break;
                    case Constans.TYPE_SOCKET:
                        componentCall = "signWithSocket(function(){ $('#hdnDigitalSignDialogBtnSign').click(); }); return false;";
                        this.DigitalSignDialogBtnClose.Attributes.Add("OnClick", "return CloseApplet();");
                        this.ShowSocketForm();
                        break;
                    default:
                        componentCall = "return signWithApplet();";
                        this.DigitalSignDialogBtnClose.Attributes.Add("OnClick", "return CloseApplet();");
                        this.ShowAppletForm();
                        break;
                }
                this.DigitalSignDialogBtnSign.Attributes.Add("OnClick", componentCall);
                //Prendere da language
                this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
                lblExpired = Languages.GetLabelFromCode("DigitalSignDialogCertificateExpired", language);
                this.optFirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptFirma", language);
                this.optFirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptFirmaTo", language);    
                this.optCofirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirma", language);
                this.optCofirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirmaTo", language);
        }

        private void InitializeKeys()
        {
            DocsPaWR.FileRequest fileReq = null;

            if (FileManager.GetSelectedAttachment() == null)
                fileReq = UIManager.FileManager.getSelectedFile();
            else
            {
                fileReq = FileManager.GetSelectedAttachment();
            }

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
            {

                bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");

                //if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString())) || Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_LIBRO_FIRMA.ToString()).Equals("0"))
                //{
                //    //E' spento il libro firma e quindi rimane la normale gestione della co-firma
                //} //Modifica condizioni Libro Firma per APSS: se il libro firma è attivo ma è abilitata UNLOCK_COFIRMA ripristino la selezione del tipo firma(firma/cofirma)   
                //else if (!string.IsNullOrEmpty(fileReq.firmato) && fileReq.firmato.Trim() == "1" &&
                //    (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.UNLOCK_COFIRMA.ToString())) || Utils.InitConfigurationKeys.GetValue("0", DBKeys.UNLOCK_COFIRMA.ToString()).Equals("0")))
                //{
                //    //E' attivo il libro firma e quindi se è già firmato, appone sempre co-firma
                //    forzaCofirma = true;
                //}

                if (!isPdf && !fileReq.firmato.Equals("1"))
                {
                    this.chkConverti.Checked = true;
                    this.chkConverti.Enabled = false;
                    if (UIManager.FileManager.IsEnabledSupportedFileTypes())
                    {
                        this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));

                        bool retVal = true;

                        int count = this.FileTypes.Count(e => e.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant() &&
                                                                e.FileTypeUsed && e.FileTypeSignature);
                        retVal = (count > 0);

                        this.chkConverti.Checked = !retVal;
                    }
                    this.chkPades.Visible = false;
                }
                else
                {
                    this.chkPades.Visible = true;
                    this.chkConverti.Enabled = false;

                    //Se il documento è in libro firma non posso scegliere il tipo firma, ma di pende dal passo.
                    if (fileReq.inLibroFirma)
                    {
                        this.chkPades.Enabled = false;
                        string typeSignature = LibroFirmaManager.GetTypeSignatureToBeEntered(fileReq);
                        if (typeSignature.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
                        {
                            this.chkPades.Checked = true;
                        }
                    }
                }
            }

            if (!signHash) // se non posso firmare con l'HASH allora non posso neanche firmare pades
                this.chkPades.Visible = false;


            //Se il documento è in libro firma non posso scegliere il tipo firma, ma di pende dal passo.
            if (fileReq.inLibroFirma)
            {
                this.chkPades.Enabled = false;
                string typeSignature = LibroFirmaManager.GetTypeSignatureToBeEntered(fileReq);
                if (typeSignature.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES))
                {
                    this.chkPades.Checked = true;
                }
            }

            //PDZ - Nuova gestione firma/cofirma
            //Valore della Chiave FE_SET_TIPO_FIRMA
            //  0: Annidata
            //  1: Parallela
            //  2: Annidata non modificabile
            //  3: Parallela non modificabile

            if (fileReq.firmato.Equals("0") || fileReq.tipoFirma == NttDataWA.Utils.TipoFirma.ELETTORNICA)
            {
                this.tipoFirmaH.Value = "true";
                this.optFirma.Visible = false;
                this.optCofirma.Visible = false;
            }
            string setTipoFirma = string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString())) ? "0" : Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString());
            switch (setTipoFirma)
            {
                //Annidata default ma modificabile
                case ("0"):
                    this.optCofirma.Checked = false;
                    this.optFirma.Checked = true;
                    this.optCofirma.Enabled = true;
                    this.optFirma.Enabled = true;
                    break;
                //Parallela default ma modificabile
                case ("1"):
                    this.optCofirma.Checked = true;
                    this.optFirma.Checked = false;
                    this.optFirma.Enabled = true;
                    this.optFirma.Enabled = true;
                    break;
                //Annidata non modificabile
                case ("2"):
                    this.optCofirma.Checked = false;
                    this.optFirma.Checked = true;
                    this.optCofirma.Enabled = false;
                    this.optFirma.Enabled = false;
                    break;
                //Parallela non modificabile
                case ("3"):
                    this.optCofirma.Checked = true;
                    this.optFirma.Checked = false;
                    this.optFirma.Enabled = false;
                    this.optFirma.Enabled = false;
                    break;
            }
           
        }

        private void ShowAppletForm()
        {
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            this.lblListaCertificati.Text = Languages.GetLabelFromCode("DigitalSignDialogCertificateList", language);
            if (this.chkConverti.Enabled)
                this.chkConverti.Text = Languages.GetLabelFromCode("DigitalSignDialogPdfConvert", language);
            else
                this.chkConverti.Text = Languages.GetLabelFromCode("DigitalSignDialogPdfAConvert", language);
            this.grdResult.Columns[0].HeaderText = Languages.GetLabelFromCode("DigitalSignDialogIdDoc", language);
            this.grdResult.Columns[1].HeaderText = Languages.GetLabelFromCode("DigitalSignDialogResult", language);
            
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "open", "$(function () {openApplet();});", true);
            
            this.pnlApplet.Visible = true;
        }

        private void ShowSocketForm()
        {
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            this.lblListaCertificati.Text = Languages.GetLabelFromCode("DigitalSignDialogCertificateList", language);
            if (this.chkConverti.Enabled)
                this.chkConverti.Text = Languages.GetLabelFromCode("DigitalSignDialogPdfConvert", language);
            else
                this.chkConverti.Text = Languages.GetLabelFromCode("DigitalSignDialogPdfAConvert", language);
            this.grdResult.Columns[0].HeaderText = Languages.GetLabelFromCode("DigitalSignDialogIdDoc", language);
            this.grdResult.Columns[1].HeaderText = Languages.GetLabelFromCode("DigitalSignDialogResult", language);

            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "open", "$(function () {openSocket();});", true);

            this.pnlApplet.Visible = true;
        }

        protected string Caller
        {
            get
            {
                return Request.QueryString["Caller"];
            }
        }

        protected string TipoFirma
        {
            get
            {
                string tipoFirma = Request.QueryString["TipoFirma"];

                if (tipoFirma != null && tipoFirma.ToUpper().Trim() == "COSIGN")
                {
                    tipoFirma = "cosign";
                }
                else
                {
                    if (forzaCofirma)
                        tipoFirma = "cosign";
                    else if (tipoFirma != null)
                        tipoFirma = "sign";
                    else
                        tipoFirma = "unselected";
                }

                //if (tipoFirma == null || tipoFirma == string.Empty)
                //    tipoFirma = "sign";

                //if (forzaCofirma)
                //    tipoFirma = "cosign";

                return tipoFirma;
            }
        }

        protected bool signHash
        {
            get
            {
                try
                {
                    //controlla se globale
                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SIGN_WITH_HASH.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SIGN_WITH_HASH.ToString()).Equals("1"))
                        return true;

                    //se no controlla se per idAmm
                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_SIGN_WITH_HASH.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_SIGN_WITH_HASH.ToString()).Equals("1"))
                        return true;
                }
                catch
                {
                }
                return false;
            }
        }


        /// <summary>
        /// Reperimento estensione del file da firmare
        /// </summary>
        /// <returns></returns>
        protected string GetSignType()
        {
            if (signHash)
            {
                return "1";
            }
            return "0";
        }

        /// <summary>
        /// Tipi di file
        /// </summary>
        private SupportedFileType[] FileTypes
        {
            get
            {
                if (this.ViewState["SupportedFileType"] == null)
                    return null;
                else
                    return this.ViewState["SupportedFileType"] as SupportedFileType[];
            }
            set
            {
                if (this.ViewState["SupportedFileType"] == null)
                    this.ViewState.Add("SupportedFileType", value);
                else
                    this.ViewState["SupportedFileType"] = value;
            }
        }

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

        protected string GetSelectedDocumentsIds()
        {
            StringBuilder sb = new StringBuilder();

            List<MassiveOperationTarget> selectedItems = this.GetSelectedDocuments();

            foreach (MassiveOperationTarget temp in selectedItems)
            {
                if (sb.Length > 0)
                    sb.Append("|");
                sb.Append(temp.Id);
            }

            return sb.ToString();
        }

        protected List<MassiveOperationTarget> GetSelectedDocuments()
        {
            // commentato per forzare l'istanziamento di una nuova lista altrimenti se si arriva dalla firma massiva
            // rimangono in sessione i documenti lì selezionati
            //List<MassiveOperationTarget> selectedItems = MassiveOperationUtils.GetSelectedItems();
            List<MassiveOperationTarget> selectedItems = new List<MassiveOperationTarget>();

            if (selectedItems.Count == 0)
            {
                DocsPaWR.SchedaDocumento schedaDocumento = NttDataWA.UIManager.DocumentManager.getSelectedRecord();

                if (schedaDocumento != null)
                {
                    FileRequest fileReq = new FileRequest();
                    if (UIManager.DocumentManager.getSelectedAttachId() != null)
                    {
                        fileReq = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                    }
                    else
                    {
                        fileReq = FileManager.GetFileRequest();
                    }
                    if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                    {
                        fileReq = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
                    }
                    string codice = string.Empty;
                    MassiveOperationTarget mot;
                    if (fileReq == null || fileReq.docNumber.Equals(schedaDocumento.docNumber))
                    {
                        codice = (schedaDocumento.protocollo != null) ? schedaDocumento.protocollo.numero : schedaDocumento.docNumber;
                        mot = new MassiveOperationTarget(schedaDocumento.systemId, codice);
                    }
                    else
                    {
                        codice = fileReq.docNumber;
                        mot = new MassiveOperationTarget(codice, codice);
                    }
                    mot.Checked = true;
                    selectedItems.Add(mot);
                }
            }

            return selectedItems;
        }

        protected string GetFileExtension()
        {
            //NttDataWA.DocsPaWR.FileRequest fileRequest = NttDataWA.UIManager.FileManager.getSelectedFile(this);
            NttDataWA.DocsPaWR.FileRequest fileRequest = null;
            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                fileRequest = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
            }
            else
            {
                fileRequest = FileManager.GetFileRequest();
            }
            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
            {
                fileRequest = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            }
            if (fileRequest != null)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);
                return fileInfo.Extension.ToLower();
            }
            else
                return string.Empty;
        }

        protected string IsSigned()
        {
            //NttDataWA.DocsPaWR.FileRequest fileRequest = NttDataWA.UIManager.FileManager.getSelectedFile(this);
            NttDataWA.DocsPaWR.FileRequest fileRequest = null;
            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                fileRequest = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
            }
            else
            {
                fileRequest = FileManager.GetFileRequest();
            }
            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
            {
                fileRequest = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v).FirstOrDefault();
            }
            if (fileRequest != null)
            {
                if (!string.IsNullOrEmpty(fileRequest.firmato) && fileRequest.firmato.Trim() == "1")
                    return "1";
                return "0";
            }
            else
                return "0";
        }

        protected void DigitalSignDialogBtnClose_Click(object sender, EventArgs e)
        {
            if (Caller == "cosign")
                ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DigitalCosignSelector','up');", true);
            else
                ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('DigitalSignSelector','up');", true);

            HttpContext.Current.Session["CommandType"] = "close";

            FirmaDigitaleResultManager.ClearData();
        }

        protected void DigitalSignDialogBtnSign_Click(object sender, EventArgs e)
        {
            //try {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
                this.grdResult.DataSource = FirmaDigitaleResultManager.CurrentData;
                this.grdResult.DataBind();
                this.pnlGridResultContainer.Update();

                FirmaDigitaleResultManager.ClearData();
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }
    }
}
