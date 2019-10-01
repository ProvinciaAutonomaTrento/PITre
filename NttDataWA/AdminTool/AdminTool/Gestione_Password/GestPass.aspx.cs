using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_Password
{
    public partial class GestPass : System.Web.UI.Page
    {

        //protected System.Web.UI.HtmlControls.HtmlTableRow tblRowPasswordMng;
        //protected System.Web.UI.WebControls.TextBox txtPasswordValidityDays;
        //protected System.Web.UI.WebControls.Button btnExpireAll;
        //protected System.Web.UI.WebControls.Button btnSave;
        //protected System.Web.UI.WebControls.Label lblPasswordDescription;
        protected System.Web.UI.WebControls.TextBox txtPasswordMinLength;
        //protected System.Web.UI.WebControls.TextBox txtPasswordSpecialChars;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                this.LoadPasswordConfigurations();
            }
            }
        /// <summary>
        /// Inizializzazione campi scadenza password 
        /// </summary>
        private void LoadPasswordConfigurations()
        {
            //// Gestione abilitazione / disabilitazione campi scadenza password
            this.EnablePasswordControls();

            if (this.IsSupportedPasswordConfig())
            {
                SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();

                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

                SAAdminTool.DocsPaWR.PasswordConfigurations pwdConfigurations = ws.AdminGetPasswordConfigurations(sessionManager.getUserAmmSession(), this.GetIdAmministrazione());

                if (pwdConfigurations.MinLength > 0)
                    this.txtPasswordMinLength.Text = pwdConfigurations.MinLength.ToString();
                else
                    this.txtPasswordMinLength.Text = string.Empty;

                this.txtPasswordSpecialChars.Text = new string(pwdConfigurations.SpecialCharacters);

                if (pwdConfigurations.ExpirationEnabled && pwdConfigurations.ValidityDays > 0)
                    this.txtPasswordValidityDays.Text = pwdConfigurations.ValidityDays.ToString();
                else
                    this.txtPasswordValidityDays.Text = string.Empty;

                this.btnExpireAll.Attributes.Add("onclick", "return OnClickExpireAllPassword();");
            }
        }

        private void EnablePasswordControls()
        {
            this.tblRowPasswordMng.Visible = this.IsSupportedPasswordConfig();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SavePasswordConfigurations();
        }

        /// <summary>
        /// Save dei dati della password
        /// </summary>
        protected void SavePasswordConfigurations()
        {
            if (this.IsSupportedPasswordConfig())
            {
                int idAmministrazione = this.GetIdAmministrazione();

                SAAdminTool.DocsPaWR.PasswordConfigurations pwdConfigurations = new SAAdminTool.DocsPaWR.PasswordConfigurations();

                pwdConfigurations.IdAmministrazione = this.GetIdAmministrazione();

                int validationDaysIfEnabled;
                if (Int32.TryParse(this.txtPasswordValidityDays.Text, out validationDaysIfEnabled))
                    pwdConfigurations.ValidityDays = validationDaysIfEnabled;
                pwdConfigurations.ExpirationEnabled = (validationDaysIfEnabled > 0);

                int pwdMinLenght;
                if (Int32.TryParse(this.txtPasswordMinLength.Text, out pwdMinLenght))
                    pwdConfigurations.MinLength = pwdMinLenght;
                else
                    pwdConfigurations.MinLength = 0;

                pwdConfigurations.SpecialCharacters = this.txtPasswordSpecialChars.Text.ToCharArray();

                SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();

                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

                if (!ws.AdminSavePasswordConfigurations(sessionManager.getUserAmmSession(), pwdConfigurations))
                {
                    // Aggiornamento dei dati non andato a buon fine
                    this.Page.Response.Write("<script>alert('Si è verificato un errore nell\\'aggiornamento delle configurazioni delle password.');</script>");
                }
                else
                {
                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "SavePasswordConfigCompleted"))
                        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SavePasswordConfigCompleted", "<script>SavePasswordConfigCompleted();</script>");
                }
            }
        }
        /// <summary>
        /// Verifica se, per l'amministrazione, la gestione scadenza password è abilitata o meno
        /// </summary>
        /// <returns></returns>
        protected bool IsSupportedPasswordConfig()
        {
            if (this.ViewState["IsSupportedPasswordConfig"] == null)
            {
                SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                this.ViewState["IsSupportedPasswordConfig"] = ws.AdminIsSupportedPasswordConfig();
            }

            return Convert.ToBoolean(this.ViewState["IsSupportedPasswordConfig"]);
        }

        protected int GetIdAmministrazione()
        {
            return Convert.ToInt32(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3"));
        }

        protected void btnExpireAll_Click(object sender, EventArgs e)
        {
            this.ExpireAllPassword();
        }

        /// <summary>
        /// Forza la scadenza di tutte le password di tutti gli utenti dell'amministrazione
        /// </summary>
        protected void ExpireAllPassword()
        {
            if (this.IsSupportedPasswordConfig())
            {
                SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();

                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

                ws.AdminExpireAllPassword(sessionManager.getUserAmmSession(), this.GetIdAmministrazione());
            }
        }


    }
}