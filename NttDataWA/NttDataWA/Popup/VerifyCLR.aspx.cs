using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class VerifyCLR : System.Web.UI.Page
    {

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            this.InitializePage();
            if (Session["VerifyCLR_OriginalDocument"] != null && Session["VerifyCLR_OriginalDocument"].Equals("originalDocument"))
            {
                HttpContext.Current.Session.Remove("VerifyCLR_OriginalDocument");
                this.BtnCheck.Visible = true;
                this.plcDate.Visible = true;
                this.txt_DateCheck.Text = DocumentManager.GetDataRiferimentoValitaDocumento().ToShortDateString();
            }
            else
            {
                if (Session["VerifyCLR_returnValue"] == null || Session["VerifyCLR_returnValue"].Equals(string.Empty))
                {
                    this.BtnCheck.Visible = false;
                    this.plcDate.Visible = false;
                    CertificateGetInfo();
                }
            }
            }
            RefreshScript();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "EnableOnlyPreviousToday", "EnableOnlyPreviousToday();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            if(this.BtnCheck.Visible)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('VerifyCLR', '', parent);", true);
            else
                this.CloseMask(true);
        }

        protected void BtnCheck_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            VerificaValiditaFirmaAllaData();
            this.plcMessage.Visible = true;
            this.UpPnlMessage.Update();
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

        #endregion

        private const int CODE_CERTIFICATO_SCADUTO = 5127;

        #region Methods

        protected void InitializePage()
        {
            this.InitializeLanguage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.LtlDateCheck.Text = Utils.Languages.GetLabelFromCode("DigitalSignDetailsVerifyCLRDate", language);
            this.BtnCheck.Text = Utils.Languages.GetLabelFromCode("VerifyCLRBtnCheck", language);
            this.litMandateNewHour.Text = Utils.Languages.GetLabelFromCode("MandateNewHour", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('VerifyCLR', '" + retValue + "', parent);", true);
        }

        private void VerificaValiditaFirmaAllaData()
        {
            if (!string.IsNullOrEmpty(this.txt_DateCheck.Text))
            {
                DateTime date = utils.formatStringToDate(this.txt_DateCheck.Text);

                if (!utils.verificaIntervalloDateSenzaOra(System.DateTime.Today.ToShortDateString(),date.ToShortDateString()))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningVerifyCLRDate', 'warning', '', '');} else {parent.ajaxDialogModal('WarningVerifyCLRDate', 'warning', '', '');}", true);
                    return;
                }

                if (!string.IsNullOrEmpty(this.ddlHourFrom.SelectedValue))
                    date = date.AddHours(Convert.ToDouble(this.ddlHourFrom.SelectedValue));
                FileDocumento fileDoc = DocumentManager.VerificaValiditaFirmaAllaData(date);
                if (fileDoc.signatureResult.StatusCode == 0)
                {
                    this.litMessage.Text = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_check.gif") + "\" alt=\"\" />"
                    + this.GetLabel("VerifyCLRok");
                    Session["VerifyCLR_returnValue"] = "OK";

                }
                else
                {
                    string label = this.GetLabel("VerifyCLRko");
                    if (fileDoc.signatureResult.ErrorMessages != null && fileDoc.signatureResult.ErrorMessages.Count() > 0)
                        label += ": " + fileDoc.signatureResult.ErrorMessages[0];

                    /*
                    // decommentando questo codice ricaviamo il messaggio d'errore del controllo CRL esterno
                    if (fileDoc.signatureResult.ErrorMessages.Length > 0)
                    {
                        string errs="";
                        foreach (string err in fileDoc.signatureResult.ErrorMessages)
                        {
                            if (err.Contains("EXTCHECK"))
                            {
                                if (err.Contains("-"))
                                    errs += err.Split('-')[0]+":";
                                else 
                                    errs += err + ":";
                            }
                        }
                        if (errs.Length > 0)
                            label += " :" + errs;
                    }
                    */

                    if (fileDoc.signatureResult.StatusCode == -100)
                        label =this.GetLabel("VerifyCLRNoConnection");

                    if (revokedCertArePresent(fileDoc))
                        label = this.GetLabel("VerifyCLRRevoked");

                  
                    this.litMessage.Text = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_error.gif") + "\" alt=\"\" />"
                    + label;
                    Session["VerifyCLR_returnValue"] = "KO";

                    
                }

                this.UpPnlMessage.Update();
                Session["VerifyCLR_date"] = null;
            }
        }

        private static bool revokedCertArePresent(FileDocumento filedoc)
        {
            if (filedoc != null && filedoc.signatureResult != null && filedoc.signatureResult.PKCS7Documents != null)
            {
                foreach (PKCS7Document p7md in filedoc.signatureResult.PKCS7Documents)
                {
                    foreach (SignerInfo siinfo in p7md.SignersInfo)
                    {
                        if (siinfo.CertificateInfo.RevocationDate != DateTime.MinValue)
                            return true;
                    }
                }
            }
            return false;
        }


        private void CertificateGetInfo()
        {
            this.CertificateInfo = DocumentManager.CertificateGetInfo(CertificateInfo);
            if (this.CertificateInfo != null)
            {
                string label = "VerifyCLRok";
                DateTime date = (DateTime)Session["VerifyCLR_date"];
                //SE LA DATA INSERITA è PRECEDENTE ALLA DATA DI INIZIO VALIDITà
                if (this.CertificateInfo.RevocationStatus == -500)
                {

                    label = "VerifyCLRNoConnection";
                    this.litMessage.Text = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_error.gif") + "\" alt=\"\" />"
                        + this.GetLabel(label);
                    Session["VerifyCLR_returnValue"] = "VerifyCLRNoConnection";
                    return;
                }


                if (!utils.verificaIntervalloDate(date.ToString(), CertificateInfo.ValidFromDate.ToString()))
                {
                    label = "VerifyCRLNotValid";
                }
                else
                {
                    //SE IL CERTIFICATO HA UNA DATA DI REVOCA
                    if (!CertificateInfo.RevocationDate.Equals(DateTime.MinValue))
                    {
                        if (utils.verificaIntervalloDate(CertificateInfo.RevocationDate.ToString(), date.ToString()))
                        {
                            label = utils.verificaIntervalloDate(CertificateInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLValid" : "VerifyCRLExpired";
                        }
                        else
                        {
                            label = utils.verificaIntervalloDate(CertificateInfo.ValidToDate.ToString(), date.ToString()) ? "VerifyCRLRevoked" : "VerifyCRLExpired";
                        }
                    }
                    else
                    {

                        bool valid = utils.verificaIntervalloDate(CertificateInfo.ValidToDate.ToString(), date.ToString());
                        if (valid && (this.CertificateInfo.RevocationStatus == CODE_CERTIFICATO_SCADUTO || this.CertificateInfo.RevocationStatus == 0))
                        {
                            label = "VerifyCRLValid";
                        }
                        else
                        {
                            //è presente un errore..
                            if (this.CertificateInfo.RevocationStatus != 0)
                            {
                                label = "VerifyCRLInvalid";
                            }
                            else
                            {
                                label = "VerifyCRLExpired";
                            }
                        }
                        
                    }
                }

                if (label.Equals("VerifyCRLValid"))
                    this.litMessage.Text = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_check.gif") + "\" alt=\"\" />"
                        + this.GetLabel(label);
                else
                {
                    string extraInfo = "";
                    if (!String.IsNullOrEmpty(this.CertificateInfo.RevocationStatusDescription))
                        extraInfo = ": "+this.CertificateInfo.RevocationStatusDescription;


                    this.litMessage.Text = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_error.gif") + "\" alt=\"\" />"
                        + this.GetLabel(label) + extraInfo;

                }

                Session["VerifyCLR_returnValue"] = label;
            }
            else
            {
                this.litMessage.Text = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_error.gif") + "\" alt=\"\" />"
                    + this.GetLabel("VerifyCLRko");
                Session["VerifyCLR_returnValue"] = "VerifyCLRko";
            }
        }


        #endregion

    }
}