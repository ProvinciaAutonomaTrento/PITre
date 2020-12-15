using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Cambia Password dell'utente.
	/// </summary>
	public class cambiaPassword : DocsPAWA.CssPage
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.Label lbl_oggetto;
		protected System.Web.UI.WebControls.TextBox txt_nuovaPWD;
        protected System.Web.UI.WebControls.TextBox txt_vecchiaPWD;
        protected System.Web.UI.WebControls.Label Label3;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.TextBox txt_confermaPWD;
		protected System.Web.UI.WebControls.Button btn_OK;
		protected System.Web.UI.WebControls.Panel pnl_PWD;
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Label Label1;
        protected Utilities.MessageBox msg_modifica;
        #endregion

		#region eventi Page
		/// <summary>
		/// Evento Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
            this.Response.Expires = -1;

			if(!IsPostBack)
			{
				DocsPaWR.Utente utente = UserManager.getUtente(this);
		
				// se l'utente si autentica su dominio, allora non è possibile modificare nulla!
				if (utente.dominio != null && !utente.dominio.Equals(string.Empty))
				{
					this.pnl_PWD.Visible = false;
					this.btn_OK.Enabled = false;
					this.lbl_message.Visible = true; // avviso!
				}
			}
		}

		#endregion

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
			this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region eventi Tasti
		/// <summary>
		/// Tasto Conferma
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_OK_Click(object sender, System.EventArgs e)
		{
            this.ExecuteChangePassword();
		}

        protected void msg_modifica_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                //Session.Add("ok_multiAmm", true);
                ExecuteChangePassword();
            }
            else
                Session.Remove("ok_multiAmm");
        }

        private DocsPaWR.UserLogin CreateUserLogin()
        {
            DocsPaWR.Utente utente = UserManager.getUtente(this);
            //return this.CreateUserLogin(utente.userId, this.txt_vecchiaPWD.Text, this.Session.SessionID);
            DocsPaWR.UserLogin userLogin = new DocsPAWA.DocsPaWR.UserLogin();
            userLogin.UserName = utente.userId;
            userLogin.Password = this.txt_vecchiaPWD.Text;
            userLogin.IdAmministrazione = utente.idAmministrazione;
            userLogin.IPAddress = this.Request.UserHostAddress;
            userLogin.SessionId = this.Session.SessionID;
            return userLogin;
        }

        private bool VerificaUtenteMultiAmministrazioneMod(string userId)
        {
            string returnMsg = string.Empty;
            DocsPAWA.DocsPaWR.Amministrazione[] listaAmm = UserManager.getListaAmministrazioniByUser(this, userId, true, out returnMsg);
            return (listaAmm.Length > 1) ? true : false;
        }

        /// <summary>
        /// Update del record in people con l'inserimento dell'encrypted password
        /// </summary>
        private bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm)
        {
            bool resultValue = false;
            resultValue = UserManager.ModificaPasswordUtenteMultiAmm(this, userId, idAmm);
            return resultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void ExecuteChangePassword()
        {
            bool isUserMultiAmm = false;
            bool continua = true;
            string pwdUser = string.Empty;
            if (Session["ok_multiAmm"] != null)
            {
                this.txt_vecchiaPWD.Text = Session["oldPwd"].ToString();
                Session.Remove("oldPwd");
                this.txt_nuovaPWD.Text = Session["newPwd"].ToString();
                Session.Remove("newPwd");
                this.txt_confermaPWD.Text = Session["confPwd"].ToString();
                Session.Remove("confPwd");
            }
            //sono stati inseriti entrambi i campi 
            if (this.txt_vecchiaPWD.Text.Trim().Equals(string.Empty) || this.txt_nuovaPWD.Text.Trim().Equals(string.Empty) || this.txt_confermaPWD.Text.Trim().Equals(string.Empty))
            {
                if (!this.Page.IsStartupScriptRegistered("alertJS"))
                {
                    string scriptString = "<SCRIPT>alert('Inserire valori per tutti i campi!');</SCRIPT>";
                    this.Page.RegisterStartupScript("alertJS", scriptString);
                }
            }
            else
            {
                // inserita Vecchia Password errata
                DocsPaWR.LoginResult loginResult;
                string ipaddress = "";
                DocsPaWR.Utente utente = UserManager.login(this, this.CreateUserLogin(), out loginResult, out ipaddress);

                if (loginResult == DocsPAWA.DocsPaWR.LoginResult.UNKNOWN_USER)
                {
                    if (!this.Page.IsStartupScriptRegistered("alertJS"))
                    {
                        string scriptString = "<SCRIPT>alert('La vecchia password risulta essere errata!');</SCRIPT>";
                        this.Page.RegisterStartupScript("alertJS", scriptString);
                    }
                }
                else
                {

                    // i campi sono uguali
                    if (!this.txt_confermaPWD.Text.Trim().Equals(this.txt_nuovaPWD.Text.Trim()))
                    {
                        if (!this.Page.IsStartupScriptRegistered("alertJS"))
                        {
                            string scriptString = "<SCRIPT>alert('La nuova password e la sua conferma non coincidono!');</SCRIPT>";
                            this.Page.RegisterStartupScript("alertJS", scriptString);
                        }
                    }
                    else
                    {
                        DocsPaWR.UserLogin ut = UserManager.CreateUserLoginCurrentUser(this.txt_confermaPWD.Text);
                        isUserMultiAmm = (VerificaUtenteMultiAmministrazioneMod(ut.UserName));
                        if (Session["ok_multiAmm"] == null)
                        {
                            if (isUserMultiAmm)
                            {
                                //TODO SHOW ALERT MESSAGE....
                                continua = false;
                                Session.Add("ok_multiAmm", ut);
                                Session.Add("oldPwd", this.txt_vecchiaPWD.Text);
                                Session.Add("newPwd", this.txt_nuovaPWD.Text);
                                Session.Add("confPwd", this.txt_confermaPWD.Text);
                                msg_modifica.Confirm("E’ presente almeno un utente con stessa UserID in un’altra amministrazione, vuoi procedere con la modifica? Attenzione: l’eventuale modifica della password verrà ereditata dagli altri utenti.");
                            }
                        }
                        else
                            Session.Remove("ok_multiAmm");


                        if (continua)
                        {
                            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                            DocsPaWR.ValidationResultInfo result = ws.UserChangePassword(UserManager.CreateUserLoginCurrentUser(this.txt_confermaPWD.Text, true), string.Empty);

                            if (!result.Value)
                            {
                                string errorMessage = result.BrokenRules[0].Description;

                                // Se la modifica password non è andata a buon fine
                                this.lbl_message.Text = errorMessage;

                                //this.lbl_message.Text = "<br><font color='#ff0000'>attenzione!<br>errore durante la procedura di cambia password!</font><br>";
                            }
                            else
                            {
                                if (isUserMultiAmm)
                                    this.ModificaPasswordUtenteMultiAmm(ut.UserName.ToUpper(), ut.IdAmministrazione);

                                this.lbl_message.Text = "<br>Aggiornamento avvenuto con successo<br><br><br><br>";
                                this.lbl_oggetto.Visible = false;
                                this.Label3.Visible = false;
                                this.Label2.Visible = false;
                                this.txt_nuovaPWD.Visible = false;
                                this.txt_confermaPWD.Visible = false;
                                this.txt_vecchiaPWD.Visible = false;
                                this.btn_OK.Visible = false;
                            }

                            this.lbl_message.Visible = true;
                        }
                    }
                }
            }
        }

		#endregion
	}
}
