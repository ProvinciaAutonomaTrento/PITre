using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Options
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        #region Standard Method

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();

                this.TxtOldPassword.Focus();
            }
            else
            {
                if (!string.IsNullOrEmpty(this.HiddenChangePassword.Value))
                {
                    this.HiddenChangePassword.Value = string.Empty;
                    ExecuteChangePassword();
                }
                if (!string.IsNullOrEmpty(this.HiddenChangePasswordMultiAministrator.Value))
                {
                    this.HiddenChangePasswordMultiAministrator.Value = string.Empty;
                    ChangePasswordOperation(true);
                    this.hdnMultiAdmNewPass.Value = string.Empty;
                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.ChangePasswordLbl.Text = Utils.Languages.GetLabelFromCode("ChangePasswordLbl", language);
            this.BtnChangePasswordConfirm.Text = Utils.Languages.GetLabelFromCode("BtnChangePasswordConfirm", language);
            this.lt_oldPassword.Text = Utils.Languages.GetLabelFromCode("ChangePasswordConfirmLtOldPassword", language);
            this.lt_NewPassword.Text = Utils.Languages.GetLabelFromCode("ChangePasswordConfirmLtNewPassword", language);
            this.lt_ConfirmPassword.Text = Utils.Languages.GetLabelFromCode("ChangePasswordConfirmLtConfirmPassword", language);
        }

        #endregion

        #region Event

        protected void BtnChangePasswordConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string error = CheckChangePassword();
                if (!string.IsNullOrEmpty(error))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('" + error + "', 'warning', '','');", true);
                    return;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal2", "ajaxConfirmModal('ConfirmChangePassword', 'HiddenChangePassword', '');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Change password

        private string CheckChangePassword()
        {
            string errorMessage = string.Empty;
            if (string.IsNullOrEmpty(this.TxtOldPassword.Text)
               || string.IsNullOrEmpty(this.TxtNewPassword.Text)
               || string.IsNullOrEmpty(this.TxtConfirmPassword.Text))
            {
                errorMessage = "WarningChangePassword";
                return errorMessage;
            }

            //if (this.TxtNewPassword.Text.Trim().Length < 8)
            //{
            //    errorMessage = "WarningErrorChangePasswordNewLength";
            //    return errorMessage;
            //}

            //i campi sono uguali
            if (!this.TxtConfirmPassword.Text.Trim().Equals(this.TxtNewPassword.Text.Trim()))
            {
                errorMessage = "WarningErrorCheckConfirmPassword";
                return errorMessage;
            }

            DocsPaWR.LoginResult loginResult;
            string ipaddress = string.Empty;
            DocsPaWR.Utente utente = LoginManager.Login(this, this.CreateUserLogin(), out loginResult, out ipaddress);

            if (loginResult == LoginResult.UNKNOWN_USER)
            {
                errorMessage = "WarningErrorOldPassword";
                return errorMessage;
            }
            return errorMessage;
        }

        private void ExecuteChangePassword()
        {
             UserLogin ut = LoginManager.CreateUserLoginCurrentUser(this.TxtConfirmPassword.Text, true);

             if (VerificaUtenteMultiAmministrazioneMod(ut.UserName))
             {
                 this.hdnMultiAdmNewPass.Value = TxtConfirmPassword.Text;
                 ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxConfirmModal1", "ajaxConfirmModal('ConfirmChangePasswordMultiAministrator', 'HiddenChangePasswordMultiAministrator', '');", true);
             }
             else
             {
                 ChangePasswordOperation();
             }
        }

        private void ChangePasswordOperation(bool multiAdministrator = false)
        {
            DocsPaWR.ValidationResultInfo result = null;
            if(!multiAdministrator)
                result = LoginManager.UserChangePassword(LoginManager.CreateUserLoginCurrentUser(this.TxtConfirmPassword.Text, false), string.Empty);
            else
                result = LoginManager.UserChangePassword(LoginManager.CreateUserLoginCurrentUser(this.hdnMultiAdmNewPass.Value.ToString(), true), string.Empty);
            string errorMessage = string.Empty;
            if (!result.Value)
            {
                errorMessage = result.BrokenRules[0].Description;
                for (int i = 1; result.BrokenRules.Count() > i; i++)
                {
                    errorMessage += " - " + result.BrokenRules[i].Description;
                }
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal3", "ajaxDialogModal('ErrorChangePassword', 'error', '','" + errorMessage + "');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal3", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('ErrorChangePassword', 'error', '','" + HttpUtility.UrlEncode(errorMessage) + "');} else {parent.parent.ajaxDialogModal('ErrorChangePassword', 'error', '','" + HttpUtility.UrlEncode(errorMessage) + "');}", true);
               
            }
            else
            {
                UserLogin ut = LoginManager.CreateUserLoginCurrentUser(this.TxtConfirmPassword.Text, true);
                // Il metodo utilizzato di seguito CANCELLA le password se non è presente un utente con id_amm = 0, quindi lo tolgo
                //if (multiAdministrator)
                //{
                //    this.ModificaPasswordUtenteMultiAmm(ut.UserName.ToUpper(), ut.IdAmministrazione);
                //}
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal2", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('CheckChangePassword', 'check');} else {parent.parent.ajaxDialogModal('CheckChangePassword', 'check');}", true);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal2", "ajaxDialogModal('CheckChangePassword', 'check', '','');", true);
            }     
        }

        private DocsPaWR.UserLogin CreateUserLogin()
        {
            DocsPaWR.Utente utente = UserManager.GetUserInSession();
            DocsPaWR.UserLogin userLogin = new UserLogin();
            userLogin.UserName = utente.userId;
            userLogin.Password = this.TxtOldPassword.Text;
            userLogin.IdAmministrazione = utente.idAmministrazione;
            userLogin.IPAddress = this.Request.UserHostAddress;
            userLogin.SessionId = this.Session.SessionID;
            return userLogin;
        }

        private bool VerificaUtenteMultiAmministrazioneMod(string userId)
        {
            string returnMsg = string.Empty;
            Amministrazione[] listaAmm = UserManager.getListaAmministrazioniByUser(this, userId, true, out returnMsg);
            return (listaAmm.Length > 1) ? true : false;
        }

        /// <summary>
        /// Update del record in people con l'inserimento dell'encrypted password
        /// </summary>
        private bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm)
        {
            bool resultValue = false;
            resultValue = LoginManager.ModificaPasswordUtenteMultiAmm(this, userId, idAmm);
            return resultValue;
        }


        #endregion
    }
}