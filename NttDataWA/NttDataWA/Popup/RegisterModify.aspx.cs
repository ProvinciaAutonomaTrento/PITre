using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class RegisterModify : System.Web.UI.Page
    {

        #region Property

        /// <summary>
        /// Registro selezionato nella griglia
        /// </summary>
        private Registro SelectedRegister
        {
            get
            {
                if (HttpContext.Current.Session["selectedRegister"] != null)
                    return (Registro)HttpContext.Current.Session["selectedRegister"];
                else
                    return null;
            }
        }

        #endregion

        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    InitializeLanguage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.RegisterModifyBtnSavePw.Text = Utils.Languages.GetLabelFromCode("RegisterModifyBtnSavePw", language);
            this.RegisterModifyBtnClose.Text = Utils.Languages.GetLabelFromCode("RegisterModifyBtnClose", language);
            this.RegisterModifyOldPw.Text = Utils.Languages.GetLabelFromCode("RegisterModifyOldPw", language);
            this.RegisterModifyNewPw.Text = Utils.Languages.GetLabelFromCode("RegisterModifyNewPw", language);
            this.RegisterModifyConfirmPw.Text = Utils.Languages.GetLabelFromCode("RegisterModifyConfirmPw", language);
        }

        #endregion

        #region Event Handler

        protected void RegisterModifyBtnSavePw_Click(object sender, EventArgs e)
        {
            OrgRegistro register = GestManager.GetRegisterAmm(SelectedRegister.systemId);
            CasellaRegistro[] caselle = GestManager.GetMailRegistro(register.IDRegistro);
            CasellaRegistro casellaDaAggiornare = new CasellaRegistro();

            if (caselle != null && caselle.Length > 0)
            {
                casellaDaAggiornare = (from c in caselle
                                       where c.EmailRegistro.ToUpper().Equals(SelectedRegister.email.ToUpper())
                                       select c).FirstOrDefault();
            }

            if (casellaDaAggiornare != null)
            {
                register.Mail = BuildMailRegistro(casellaDaAggiornare);
                if (checkField(register.Mail.Password))
                {
                    register.Mail.Password = this.RegisterModifyNewPwTxt.Text;
                    register.Mail.PasswordSMTP = this.RegisterModifyNewPwTxt.Text;
                    casellaDaAggiornare.PwdMail = this.RegisterModifyNewPwTxt.Text;
                    casellaDaAggiornare.PwdSMTP = this.RegisterModifyNewPwTxt.Text;
                    ValidationResultInfo result = GestManager.UpdateRegistro(register);
                    if (result.Value)
                    {
                        result = GestManager.UpdateMailRegistro(register.IDRegistro, new CasellaRegistro[] { casellaDaAggiornare });
                    }
                    if (result.Value)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
                        Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('RegisterModify','up');</script></body></html>");
                        Response.End();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErrorRegisterModify', 'error', '');", true);
                        return;
                    }
                }
                else
                    return;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErrorRegisterModify', 'error', '');", true);
                return;
            }
        }

        private MailRegistro BuildMailRegistro(CasellaRegistro casella)
        {
            MailRegistro mail = new MailRegistro();

            mail.Email = casella.EmailRegistro;
            mail.UserID = casella.UserMail;
            mail.Password = casella.PwdMail;
            mail.ServerSMTP = casella.ServerSMTP;
            mail.ServerPOP = casella.ServerPOP;
            mail.inbox = casella.IboxIMAP;
            mail.mailElaborate = casella.BoxMailElaborate;
            mail.serverImap = casella.ServerIMAP;
            mail.tipoPosta = casella.TipoConnessione;
            mail.mailNonElaborate = casella.MailNonElaborate;
            mail.IMAPssl = casella.ImapSSL;
            mail.soloMailPec = casella.SoloMailPEC;
            // Per gestione pendenti tramite PEC
            mail.MailRicevutePendenti = casella.MailRicevutePendenti;

            try
            {
                mail.PortaSMTP = casella.PortaSMTP;
                mail.PortaPOP = casella.PortaPOP;
                mail.portaIMAP = casella.PortaIMAP;
            }
            catch (Exception e)
            {
            }
            mail.UserSMTP = casella.UserSMTP;
            mail.PasswordSMTP = casella.PwdSMTP;
            mail.POPssl = casella.PopSSL;
            mail.SMTPssl = casella.SmtpSSL;
            mail.SMTPsslSTA = casella.SmtpSta;
            mail.IMAPssl = casella.ImapSSL;
            mail.pecTipoRicevuta = casella.RicevutaPEC;

            return mail;
        }

        protected void RegisterModifyBtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('RegisterModify','');</script></body></html>");
            Response.End();
        }

        #endregion

        #region Auxiliary Methods


        private bool checkField(string password)
        {
            string msg = string.Empty;
            if (this.RegisterModifyOldPwTxt.Text.Equals(""))
            {
                msg = "ErrorRegisterModifyOldPwEmpty";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg + "', 'error', '');", true);
                return false;
            }
            if (this.RegisterModifyNewPwTxt.Text.Equals(""))
            {
                msg = "ErrorRegisterModifyNewPwEmpty";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg + "', 'error', '');", true);
                return false;
            }
            if (this.RegisterModifyConfirmPwTxt.Text.Equals(""))
            {
                msg = "ErrorRegisterModifyConfirmPwEmpty";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg + "', 'error', '');", true);
                return false;
            }
            if (!this.RegisterModifyOldPwTxt.Text.Equals(password))
            {
                msg = "ErrorRegisterModifyOldPwError";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg + "', 'error', '');", true);
                return false;
            }
            if (!this.RegisterModifyNewPwTxt.Text.Equals(this.RegisterModifyConfirmPwTxt.Text))
            {
                msg = "ErrorRegisterModifyConfirmPwFailed";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + msg + "', 'error', '');", true);
                return false;
            }
            return true;
        }

        #endregion
    }
}