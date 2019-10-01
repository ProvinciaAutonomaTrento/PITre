using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
using System.Linq;
using NttDataWA.Utils;
using NttDataWA.SmartClient;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class DigitalSignDialog : Page
    {
        private string language;
        public string lblExpired = string.Empty;
        private bool forzaCofirma;

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                Response.Expires = -1;

                this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
                this.LoadTextLanguage();
                //if (!this.IsPostBack)
                //{
                string totalDocForSign = Languages.GetLabelFromCode("DigitalSignDialogTotalDocForSign", language);
                //this.lblDocumentCount.Text = string.Format("{0} " + totalDocForSign, this.GetSelectedDocuments().Count);

                DocsPaWR.Ruolo ruoloCorrente = NttDataWA.UIManager.RoleManager.GetRoleInSession();

                DocsPaWR.Funzione funzioneFirma256 = ruoloCorrente.funzioni.FirstOrDefault(itm => itm.codice == "DO_DOC_FIRMA_256");

                UserControl ctrl = null;

                //if (funzioneFirma256 != null)
                //{
                    // Attivazione usercontrol smartclient
                    ctrl = (UserControl)this.LoadControl("~/SmartClient/DigitalSignWrapper.ascx");
                //}

                this.Form.Controls.Add(ctrl);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void LoadTextLanguage()
        {
            this.lblListaCertificati.Text = Languages.GetLabelFromCode("DigitalSignDialogCertificateList", language);
            if (this.chkConverti.Enabled)
                this.chkConverti.Text = Languages.GetLabelFromCode("DigitalSignDialogPdfConvert", language);
            else
                this.chkConverti.Text = Languages.GetLabelFromCode("DigitalSignDialogPdfAConvert", language);
            this.grdResult.Columns[0].HeaderText = Languages.GetLabelFromCode("DigitalSignDialogIdDoc", language);
            this.grdResult.Columns[1].HeaderText = Languages.GetLabelFromCode("DigitalSignDialogResult", language);
            this.lblExpired = Languages.GetLabelFromCode("DigitalSignDialogCertificateExpired", language);
            this.optFirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptFirma", language);
            this.optFirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptFirmaTo", language);
            this.optCofirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirma", language);
            this.optCofirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirmaTo", language);
        }

        protected void DigitalSignDialogBtnSign_Click(object sender, EventArgs e)
        {
            try {
                this.ShowReport();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
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

                if (!isPdf && !fileReq.firmato.Equals("1"))
                {
                    this.chkConverti.Checked = true;

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

                //
                this.chkConverti.Enabled = false;

                if (!signHash) // se non posso firmare con l'HASH allora non posso neanche firmare pades
                    this.chkPades.Visible = false;
                //if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString())) || Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString()).Equals("0"))

               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "clickconvertpdf", "$(function() {OnClickConvertPDF();});", true);
            }
            else
            {
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

                this.chkConverti.Attributes.Add("onClick", "OnClickConvertPDF();");
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
        #endregion

        /// <summary>
        /// Visualizzazione dell'esito della firma applicata ad uno o più documenti
        /// </summary>
        protected void ShowReport()
        {
            this.grdResult.DataSource = FirmaDigitaleResultManager.CurrentData;
            this.grdResult.DataBind();
            this.pnlGridResultContainer.Update();

            FirmaDigitaleResultManager.ClearData();
        }

        /// <summary>
        /// Lettura delle configurazioni per verificare
        /// se la gestione di conversione in pdf di un file firmato è attiva o meno
        /// </summary>
        protected bool ConvertPdfOnSign
        {
            get
            {
                bool retValue = false;

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ENABLE_CONVERT_PDF_ON_SIGN"]))
                    bool.TryParse(ConfigurationManager.AppSettings["ENABLE_CONVERT_PDF_ON_SIGN"], out retValue);

                return retValue;
            }
        }

        protected bool signHash
        {
            get
            {
                try
                {
                    //controlla se globale
                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SIGN_WITH_HASH_SM.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SIGN_WITH_HASH_SM.ToString()).Equals("1"))
                        return true;

                    //se no controlla se per idAmm
                    if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_SIGN_WITH_HASH_SM.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_SIGN_WITH_HASH_SM.ToString()).Equals("1"))
                        return true;
                }
                catch
                { }

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

        /// <summary>
        /// 
        /// </summary>
        protected bool IsEnabledConvPDFSincrona
        {
            get
            {
                return new NttDataWA.DocsPaWR.DocsPaWebService().IsEnabledConversionePdfLatoServerSincrona();
                //return true;
            }
        }

        /// <summary>
        /// Reperimento estensione del file da firmare
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected List<MassiveOperationTarget> GetSelectedDocuments()
        {
            List<MassiveOperationTarget> selectedItems = MassiveOperationUtils.GetSelectedItems();

            if (selectedItems.Count == 0)
            {
                DocsPaWR.SchedaDocumento schedaDocumento = NttDataWA .UIManager.DocumentManager.getSelectedRecord();

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
                    if(fileReq == null || fileReq.docNumber.Equals(schedaDocumento.docNumber))
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

        /// <summary>
        /// Reperimento degli id dei documenti selezionati da firmare digitalmente
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
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

                return tipoFirma;
            }
        }

        public string GetMaskTitle()
        {
            string retValue = string.Empty;

            if (this.TipoFirma == "cosign")
                retValue = "Co-Firma documento";
            else
                retValue = "Firma documento";

            return retValue;
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

    }

}