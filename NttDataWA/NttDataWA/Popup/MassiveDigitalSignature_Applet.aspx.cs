using System;
using System.Collections.Generic;
using System.Configuration;
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
using NttDataWA.DigitalSignature;
using NttDataWA.SmartClient;


namespace NttDataWA.Popup
{

    public partial class MassiveDigitalSignature_Applet : System.Web.UI.Page
    {

        #region Fields

        private const string TYPE_NONE = "0";
        private const string TYPE_ACTIVEX = "1";
        private const string TYPE_SMARTCLIENT = "2";
        private const string TYPE_APPLET = "3";

        public static string componentType = TYPE_SMARTCLIENT;
        public static string randomVersion = "";
        public static string CommandType = "";
        public static string lblExpired = string.Empty;

        private string language;

        #endregion

        #region Properties

        private bool IsFasc
        {
            get
            {
                return Request.QueryString["objType"].Equals("D") ? false : true;
            }
        }

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }


        protected Dictionary<String, FileToSign> ListToSign
        {
            get
            {
                Dictionary<String, FileToSign> result = null;
                if (HttpContext.Current.Session["listToSign"] != null)
                {
                    result = HttpContext.Current.Session["listToSign"] as Dictionary<String, FileToSign>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listToSign"] = value;
            }
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
        /// 
        /// </summary>
        protected string TipoFirma
        {
            get
            {
                string tipoFirma = Request.QueryString["TipoFirma"];

                if (tipoFirma == null || tipoFirma == string.Empty)
                    tipoFirma = "sign";

                return tipoFirma;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool FirmaPades
        {
            get
            {
                bool retval = false;
                string firmaPades = Request.QueryString["FirmaPades"];
                if (!String.IsNullOrEmpty(firmaPades))
                {
                    if (Boolean.TryParse(firmaPades, out retval))
                    {
                        return retval;
                    }
                    if (firmaPades == "1")
                        return true;
                }
                return retval;
            }
        }


        protected bool signHash
        {
            get
            {
                return false; // interrutore per disabilitare la firma hash
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

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

        private bool IsLF
        {
            get
            {
                string daLibroFirma = Request.QueryString["LF"];
                if (string.IsNullOrEmpty(daLibroFirma))
                    return false;
                else
                    return Request.QueryString["LF"].Equals("1") ? true : false;
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
            else
            {
                this.ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Session["CommandType"] = "close";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.ShowReport();

            if (IsLF)
            {
                HttpContext.Current.Session["CommandType"] = "close";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
                this.CloseMask(true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveDigitalSignatureApplet', 'UP');", true);
            }
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }

        #endregion

        #region Methods

        private void ReadRetValueFromPopup()
        {
            if (!string.IsNullOrEmpty(this.MassiveReport.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveReport','');", true);
            }
        }

        protected void InitializePage()
        {
            if (HttpContext.Current.Session["CommandType"] != null)
            {
                CommandType = HttpContext.Current.Session["CommandType"].ToString();
            }
            else
                CommandType = "";

            if (CommandType.ToUpper() != "CLOSE")
            {
                this.InitializeKeys();
                this.InitializeLanguage();
                this.InitializeList();

                this.BtnReport.Visible = false;

                this.plcMessage.Visible = false;
                this.UpPnlMessage.Update();
                this.lblDocumentCount.Text = string.Format("{0} documenti da firmare", this.GetSelectedDocuments().Count);

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openApplet", "$(function () {openApplet();});", true);
            }
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            lblExpired = Languages.GetLabelFromCode("DigitalSignDialogCertificateExpired", language);
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnConfirm", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnClose", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveReport.Title = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.litMessage.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserAskConfirm", language);
            this.grdReport.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridResult", language);
            this.grdReport.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserGridDetails", language);
            this.lblListaCertificati.Text = Utils.Languages.GetLabelFromCode("MassiveDigitalSignatureCertificatesList", language);
            
            if (this.chkConverti.Enabled)
                this.chkConverti.Text = Utils.Languages.GetLabelFromCode("MAssiveDigitalSignatureChkConverti", language);
            else
                this.chkConverti.Text = Utils.Languages.GetLabelFromCode("MAssiveDigitalSignatureChkConvertiA", language);


            this.optCades.Text = Utils.Languages.GetLabelFromCode("HsmLitP7M", language);
            this.optPades.Text = Utils.Languages.GetLabelFromCode("HsmLitPades", language);

            if (this.signHash)
            {
                this.optPades.Visible = true;
                this.optCades.Visible = true;
            }
            else
            {
                this.optPades.Visible = false;
                this.optCades.Visible = false;
            }
            
            this.optLocale.Text = Utils.Languages.GetLabelFromCode("MAssiveDigitalSignatureOptLocale", language);
            this.optCentrale.Text = Utils.Languages.GetLabelFromCode("MAssiveDigitalSignatureOptCentrale", language);

            this.optFirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptFirma", language);
            this.optCofirma.Text = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirma", language);
            this.optCofirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptCofirmaTo", language);
            this.optFirma.ToolTip = Languages.GetLabelFromCode("DigitalSignSelector_OptFirmaTo", language);
        }

        public void InitializeList()
        {
            Dictionary<String, MassiveOperationTarget> temp = new Dictionary<string, MassiveOperationTarget>();

            // Inizializzazione della mappa con i system id degli oggetti e lo stato
            // di checking (in fase di inizializzazione tutti gli item sono deselezionati)
            foreach (KeyValuePair<string, string> item in this.ListCheck)
                if (!temp.Keys.Contains(item.Key))
                    temp.Add(item.Key, new MassiveOperationTarget(item.Key, item.Value));

            // Salvataggio del dizionario
            MassiveOperationUtils.ItemsStatus = temp;
        }

        private void InitializeKeys()
        {
            if (IsLF)
            {
                this.chkConverti.Checked = false;
                this.chkConverti.Enabled = false;
            }
            else
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
                {
                    this.chkConverti.Checked = true;
                    this.chkConverti.Enabled = false;
                }

            //ABBATANGELI - Nuova gestione firma/cofirma
            //if (string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString())) || Utils.InitConfigurationKeys.GetValue("0", DBKeys.LOCK_COFIRMA.ToString()).Equals("0"))
            //Valore della Chiave FE_SET_TIPO_FIRMA
            //  0: Annidata
            //  1: Parallela
            //  2: Annidata non modificabile
            //  3: Parallela non modificabile
            string setTipoFirma = string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString())) ? "0" : Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_SET_TIPO_FIRMA.ToString());
            if (setTipoFirma.Equals("0") || setTipoFirma.Equals("2"))
            {
                //forzaCofirma = false;
                if (TipoFirma.ToUpper().Trim() == "COSIGN")
                {
                    this.optCofirma.Checked = true;
                    this.optCofirma.Enabled = true;
                    this.optFirma.Enabled = true;
                }
                else
                {
                    this.optFirma.Checked = true;
                    this.optCofirma.Enabled = true;
                    this.optFirma.Enabled = true;
                }
            }
            else
            {
                //forzaCofirma = true;
                this.optCofirma.Checked = true;
                this.optCofirma.Enabled = false;
                this.optFirma.Enabled = false;
            }

            bool enableChangeRadio = setTipoFirma.Equals("0") || setTipoFirma.Equals("1");
            this.optCofirma.Enabled = enableChangeRadio;
            this.optFirma.Enabled = enableChangeRadio;
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            if(IsLF)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('MassiveDigitalSignatureApplet', '" + retValue + "', parent);", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.parent.closeAjaxModal('MassiveDigitalSignatureApplet', '" + retValue + "');", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('MassiveDigitalSignatureApplet', '" + retValue + "');", true);
        }

        protected void generateReport(MassiveOperationReport report, string titolo)
        {
            this.generateReport(report, titolo, IsFasc);
        }

        public void generateReport(MassiveOperationReport report, string titolo, bool isFasc)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            string template = (isFasc) ? "../xml/massiveOp_formatPdfExport_fasc.xml" : "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.plcMessage.Visible = false;
            this.UpPnlMessage.Update();

            this.plcSign.Visible = false;
            this.UnPnlSign.Update();

            this.BtnConfirm.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }

        /// <summary>
        /// Visualizzazione dell'esito della firma applicata ad uno o più documenti
        /// </summary>
        protected void ShowReport()
        {
            List<FirmaDigitaleResultStatus> res = FirmaDigitaleResultManager.CurrentData;
            MassiveOperationReport report = new MassiveOperationReport();
            foreach (FirmaDigitaleResultStatus temp in res)
            {
                MassiveOperationTarget target = MassiveOperationUtils.getItem(temp.IdDocument);
                MassiveOperationReport.MassiveOperationResultEnum result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                if (!temp.Status) result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                report.AddReportRow(target.Codice, result, temp.StatusDescription);
            }

            if (!IsLF)
            {
                string[] pars = new string[] { "" + report.Worked, "" + report.NotWorked };
                report.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);
                this.generateReport(report, "Firma digitale massiva");
                
            }
            else
            {
                HttpContext.Current.Session["massiveSignReport"] = report;

            }
            //PDZ 17/01/2017 
            //Spostata da dentro if (!IsLF)
            FirmaDigitaleResultManager.ClearData();
        }

        /// <summary>
        /// Reperimento estensione del file da firmare
        /// </summary>
        /// <returns></returns>
        protected string GetFileExtension()
        {
            NttDataWA.DocsPaWR.FileRequest fileRequest = NttDataWA.UIManager.FileManager.getSelectedFile(this);

            if (fileRequest != null)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileRequest.fileName);
                return fileInfo.Extension.ToLower();
            }
            else
                return string.Empty;
        }

        protected List<MassiveOperationTarget> GetSelectedDocuments()
        {
            List<MassiveOperationTarget> selectedItems = MassiveOperationUtils.GetSelectedItems();

            if (selectedItems.Count == 0)
            {
                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getSelectedRecord();

                if (schedaDocumento != null)
                {
                    string codice = (schedaDocumento.protocollo != null) ? schedaDocumento.protocollo.numero : schedaDocumento.docNumber;
                    MassiveOperationTarget mot = new MassiveOperationTarget(schedaDocumento.systemId, codice);
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

        protected string GetSelectedDocumentsJSON()
        {
            StringBuilder sb = new StringBuilder();

            List<MassiveOperationTarget> selectedItems = this.GetSelectedDocuments();

            foreach (MassiveOperationTarget temp in selectedItems)
            {
                if (sb.Length > 0)
                    sb.Append("|");
                sb.Append("{ \\\"idDocumento\\\":\\\"");
                sb.Append(temp.Id);
                sb.Append("\\\", \\\"isSigned\\\":\\\"");
                sb.Append(IsSigned(temp.Id));
                sb.Append("\\\", \\\"fileExtension\\\":\\\"");
                sb.Append(GetFileExtension(temp.Id));
                sb.Append("\\\"}");
            }
            return sb.ToString();
        }

        private string IsSigned(String idDocumento)
        {
            //String idDocumento = this.idDocumentoCorrente.Value;
            //Signed = 1 Not Signed = 0
            String signed = "0";
            FileToSign file = null;
            if (!String.IsNullOrEmpty(idDocumento))
            {
                if (this.ListToSign != null && this.ListToSign.ContainsKey(idDocumento))
                {
                    file = this.ListToSign[idDocumento];
                    if (file != null)
                    {
                        file.signType = UIManager.DocumentManager.GetTipoFirmaDocumento(idDocumento.Replace("C", "").Replace("P", ""));
                        file.signed = (!string.IsNullOrEmpty(file.signType) && !file.signType.Equals(NttDataWA.Utils.TipoFirma.NESSUNA_FIRMA)) ? "1" : "0";

                        if (!String.IsNullOrEmpty(file.signed) && file.signed.Trim() == "1")
                            signed = "1";
                        if (!String.IsNullOrEmpty(file.signType) && file.signType.Equals(NttDataWA.Utils.TipoFirma.ELETTORNICA))
                            signed = "0";
                    }
                }

            }

            return signed;
        }

        private string GetFileExtension(String idDocumento)
        {
            //String idDocumento = this.idDocumentoCorrente.Value;
            //Signed = 1 Not Signed = 0
            String exstention = "";
            FileToSign file = null;
            if (!String.IsNullOrEmpty(idDocumento))
            {
                if (this.ListToSign != null && this.ListToSign.ContainsKey(idDocumento))
                {
                    file = this.ListToSign[idDocumento];
                    if (file != null && file.fileExtension != null)
                        exstention = file.fileExtension;
                }
            }

            return exstention;
        }
        #endregion

    }

}