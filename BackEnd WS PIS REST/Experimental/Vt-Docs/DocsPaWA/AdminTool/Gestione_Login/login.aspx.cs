using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Amministrazione.Gestione_Login
{
	/// <summary>
	/// Summary description for login.
	/// </summary>
	public class login : System.Web.UI.Page
	{
		#region WebControls e variabili
		protected System.Web.UI.WebControls.TextBox txt_userid;
		protected System.Web.UI.WebControls.Button btn_accedi;
		protected System.Web.UI.WebControls.Label lbl_error;
		protected System.Web.UI.WebControls.TextBox txt_pwd;
		protected System.Web.UI.WebControls.Label lbl_version;
		protected System.Web.UI.WebControls.Panel pnl_login;
		protected System.Web.UI.WebControls.Button btn_new_session;
		protected System.Web.UI.WebControls.Panel pnl_exist_login;
		protected System.Web.UI.WebControls.Button btn_annulla;
        protected System.Web.UI.WebControls.Image img_logo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_userid;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_pwd;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipoAmm;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected string _userID = string.Empty;
		protected string _userPWD = string.Empty;
        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore datiAmministratore = new DocsPAWA.DocsPaWR.InfoUtenteAmministratore();
        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;
        #endregion

		#region Page Load ed impostazioni interfaccia grafica
		/// <summary>
		/// Page_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				SetFocus(this.txt_userid);

				//gestione della versione
				this.gestioneVersione();
                
                if (fileExist("logoAmm.gif", "LoginFE"))
                    this.img_logo.ImageUrl = "../../images/loghiAmministrazioni/logoAmm.gif";
                else
                    this.img_logo.ImageUrl = "../Images/logo.gif";

            }
        }

        private bool fileExist(string fileName, string type)
        {
            return DocsPAWA.FileManager.fileExist(fileName, type);
        }
        
        /// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		private void GUI(string key)
		{
			switch (key)
			{
				case "start": // normale
					this.lbl_error.Text = "";
					this.lbl_error.Visible = false;
					this.pnl_login.Visible = true;
					this.txt_userid.Text = "";
					this.txt_pwd.Text = "";
					this.SetFocus(this.txt_userid);
					this.pnl_exist_login.Visible = false;
					break;

				case "unknown": // utente non riconosciuto
					this.lbl_error.Text = "Credenziali errate!";
					this.lbl_error.Visible = true;
					this.pnl_login.Visible = true;
					this.SetFocus(this.txt_userid);
					this.pnl_exist_login.Visible = false;
					break;

				case "locked": // utente già loggato
					this.lbl_error.Text = "";
					this.lbl_error.Visible = false;
					this.pnl_login.Visible = false;
					this.pnl_exist_login.Visible = true;
					this.SetFocus(this.btn_new_session);
					this.hd_userid.Value = "";
					this.hd_pwd.Value = "";
					break;

				case "error": // errore generico
					this.lbl_error.Text = "Errore di sistema!";
					this.lbl_error.Visible = true;
					this.pnl_login.Visible = false;
					this.pnl_exist_login.Visible = false;
					break;
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
			this.btn_accedi.Click += new System.EventHandler(this.btn_accedi_Click);
			this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
			this.btn_new_session.Click += new System.EventHandler(this.btn_new_session_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Tasti
		/// <summary>
		/// btn_accedi
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void btn_accedi_Click(object sender, System.EventArgs e)
		{
			//this.Login();
			this.LoginProfilata();
		}

		/// <summary>
		/// btn_annulla
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_annulla_Click(object sender, System.EventArgs e)
		{
			this.GUI("start");
		}

		/// <summary>
		/// btn_new_session
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_new_session_Click(object sender, System.EventArgs e)
		{
			//this.AccessOnNewSession(this.hd_userid.Value, this.hd_pwd.Value);
			this.AccessOnNewSessionProfilato();
		}
		#endregion

		#region Accesso al sistema di amministrazione



		/// <summary>
		/// Nuova gestione della Login con gli utenti amm.ri profilati
		/// </summary>
		private void LoginProfilata()
		{
			try
			{
                string userId = this.txt_userid.Text.Trim();
                string userPassword = this.txt_pwd.Text.Trim();

				if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userPassword))
				{
					DocsPAWA.AdminTool.Manager.AmministrazioneManager manager = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
					DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();

                    DocsPAWA.DocsPaWR.UserLogin userLogin = new DocsPAWA.DocsPaWR.UserLogin();
                    userLogin.UserName = userId;
                    userLogin.Password = userPassword;
                    userLogin.SessionId = Session.SessionID;
                    userLogin.IPAddress = this.Request.UserHostAddress;

                    esito = manager.Login(userLogin, false, out datiAmministratore);

					// gestione della sessione dei dati dell'utente amministratore
					DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
					session.setUserAmmSession(datiAmministratore);
                    
					switch (esito.Codice)
					{
						case 0: // tutto ok
							this._userID = this.txt_userid.Text.Trim();
							this._userPWD = this.txt_pwd.Text.Trim();
							this.gotoHomePageProfilata();
							break;

						case 1: // errore generico
							this.GUI("error");
							session.removeUserAmmSession();
							break;

						case 99: // utente non riconosciuto
							this.GUI("unknown");
							session.removeUserAmmSession();
							break;

						case 100: // utente già connesso
							this.GUI("locked");
                            this.hd_pwd.Value = userPassword;
							break;

						case 200: // ....NON GESTITO!... utente presente su più amministrazioni (non vale per il SYSTEM ADMIN [tipo = 1])
							break;
					}

				}
			}
			catch
			{
				this.GUI("error");
			}
		}

		/// <summary>
		/// reindirizzamento rispetto alla tipologia utente
		/// </summary>
		private void gotoHomePageProfilata()
		{
			try
			{
				/*
				* GESTIONE DELLA SESSIONE:
				* -----------------------------------------------------------------------------
				* sia il tool di amministrazione sia Docspa si trovano sotto lo stesso progetto 
				* quindi hanno in comune il presente Global.asax .
				* 
				* Esiste una sessione denominata "AppWA" che all'accesso del tool di amm.ne 
				* viene impostata a "ADMIN"; all'accesso di Docspa viene impostata a "DOCSPA".
				* 
				* Vedi >>>>>>>     Global.asax.cs > Session_End(Object sender, EventArgs e)
				*/
				Session["AppWA"] = "ADMIN";
				Session["UserIdAdmin"] = datiAmministratore.userId; //utile per la gestione del cambia password				
				// -----------------------------------------------------------------------------

				string script = string.Empty;
				string tipoAmministratore = string.Empty;
				string redirectUrl = string.Empty;

				tipoAmministratore = datiAmministratore.tipoAmministratore;

				switch (tipoAmministratore)
				{
					case "3":
						if (!this.setCurrAmmAndMenu())
						{
							this.GUI("error");
							return;
						}
						redirectUrl = "../Gestione_Homepage/Home2.aspx";
						break;
					default:
						redirectUrl = "../Gestione_Homepage/Home.aspx";
						break;
				}

				// apre la homepage dell'amministrazione
                script = "<script>; var popup = window.open('" + redirectUrl + "','Home'," +
                    "'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes,scrollbars=yes');" +
                    "popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);" +
                    " if(popup!=self) {window.opener=null;self.close();}" +
                    "</script>";

                //script = "<script>";
                //script += "var popup = window.open('" + redirectUrl + "','Home',";
                //// script += "'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');";
                ////ie7:
                //script += "'location=0,resizable=yes');";
                //script += "popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);";
                //script += " if(popup!=self) {window.opener=null;self.close();}";
                //script += "</script>";
                
				this.scriptJP(script);
			}
			catch
			{
				this.GUI("error");
			}
		}

		/// <summary>
		/// nuovo per la gestione degli utenti profilati:
		/// se l'utente è di tipo USER ADMIN, imposta l'amministrazione corrente
		/// </summary>
		/// <returns></returns>
		private bool setCurrAmmAndMenu()
		{
			bool retValue = true;

			try
			{
				// reperimento dati dell'amministrazione alla quale appartiene l'utente loggato
				DocsPAWA.AdminTool.Manager.AmministrazioneManager manager = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
				manager.GetAmmAppartenenza(this._userID, this._userPWD);
				if (manager.getCurrentAmm() != null)
				{
					DocsPAWA.DocsPaWR.InfoAmministrazione amm = manager.getCurrentAmm();

					string codice = amm.Codice;
					string descrizione = amm.Descrizione;
					string dominio = "";
					string idAmm = amm.IDAmm;

					// imposta la sessione come se l'utente fosse passato dalla homepage ed avesse impostato l'amministrazione da gestire
					Session["AMMDATASET"] = codice + "@" + descrizione + "@" + dominio + "@" + idAmm;

					// prende le voci di menu associate a questo USER ADMIN
					manager.GetAmmListaVociMenu(datiAmministratore.idCorrGlobali, amm.IDAmm);
					DocsPAWA.DocsPaWR.Menu[] listaVociMenu = manager.getListaVociMenu();

					if (listaVociMenu != null && listaVociMenu.Length > 0)
					{
						datiAmministratore.VociMenu = listaVociMenu;

						DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
						session.removeUserAmmSession();
						session.setUserAmmSession(datiAmministratore);
					}
				}
				else
				{
					retValue = false;
				}
			}
			catch
			{
				retValue = false;
			}

			return retValue;
		}

		/// <summary>
		/// 						[[[[[[....DEPRECATED....]]]]]]
		/// 
		/// reindirizzamento rispetto alla tipologia utente
		/// </summary>
		/// <param name="tipoAmministratore"></param>
		private void gotoHomePage(int tipoAmministratore)
		{
			/*
			* GESTIONE DELLA SESSIONE:
			* -----------------------------------------------------------------------------
			* sia il tool di amministrazione sia Docspa si trovano sotto lo stesso progetto 
			* quindi hanno in comune il presente Global.asax .
			* 
			* Esiste una sessione denominata "AppWA" che all'accesso del tool di amm.ne 
			* viene impostata a "ADMIN"; all'accesso di Docspa viene impostata a "DOCSPA".
			* 
			* Vedi >>>>>>>     Global.asax.cs > Session_End(Object sender, EventArgs e)
			*/
			Session["AppWA"] = "ADMIN";
			Session["UserIdAdmin"] = this._userID; //utile per la gestione del cambia password

			string script = string.Empty;

			if (tipoAmministratore == 2) // super-amministratore
			{
				// apre la homepage dell'amministrazione
				script = "<script>; var popup = window.open('../Gestione_Homepage/Home.aspx','Home'," +
					"'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes,scrollbars=yes');" +
					"popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);" +
					" if(popup!=self) {window.opener=null;self.close();}" +
					"</script>";
			}

			// ************************************
			//		GESTIONE RESTRICTED AREA
			// ************************************
			if (tipoAmministratore == 3) // utente amministratore di titolario
			{
				// reperimento dati dell'amministrazione alla quale appartiene l'utente loggato
				DocsPAWA.AdminTool.Manager.AmministrazioneManager manager = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
				manager.GetAmmAppartenenza(this._userID, this._userPWD);
				if (manager.getCurrentAmm() != null)
				{
					DocsPAWA.DocsPaWR.InfoAmministrazione amm = manager.getCurrentAmm();
					Session["Restricted"] = "Y";

					string codice = amm.Codice;
					string descrizione = amm.Descrizione;
					string dominio = "";
					string idAmm = amm.IDAmm;

					// imposta la sessione come se l'utente fosse passato dalla homepage ed avesse impostato l'amministrazione da gestire
					Session["AMMDATASET"] = codice + "@" + descrizione + "@" + dominio + "@" + idAmm;

					// apre direttamente la pagina del titolario
					script = "<script>; var popup = window.open('../Gestione_Titolario/Titolario.aspx?from=TI',''," +
						"'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes,scrollbars=yes');" +
						"popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);" +
						" if(popup!=self) {window.opener=null;self.close();}" +
						"</script>";
				}
				else
				{
					this.GUI("error");
				}
			}

			this.scriptJP(script);
		}

		private void AccessOnNewSessionProfilato()
		{			
			try
			{
				DocsPAWA.AdminTool.Manager.AmministrazioneManager manager = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
				DocsPAWA.AdminTool.Manager.SessionManager session = new DocsPAWA.AdminTool.Manager.SessionManager();
				DocsPAWA.DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
				
				datiAmministratore = session.getUserAmmSession();

                DocsPAWA.DocsPaWR.UserLogin userLogin = new DocsPAWA.DocsPaWR.UserLogin();
                userLogin.UserName = datiAmministratore.userId;
                userLogin.Password = this.hd_pwd.Value;
                userLogin.SessionId = Session.SessionID;
                userLogin.IPAddress = this.Request.UserHostAddress;

                esito = manager.Login(userLogin, true, out datiAmministratore);
                
				if (esito.Codice.Equals(0))
				{
                    session.setUserAmmSession(datiAmministratore);

                    this._userID = datiAmministratore.userId;
                    this._userPWD = this.hd_pwd.Value;
					this.gotoHomePageProfilata();
				}
				else
				{
					this.GUI("error");
				}
			}
			catch
			{
				this.GUI("error");
			}
		}
		
		#endregion

		#region Gestione della versione del software
		/// <summary>
		/// gestione Versione
		/// </summary>
		private void gestioneVersione()
		{
            this.lbl_version.Text = this.appTitleProvider.ApplicationNameVersion;
		}
		#endregion

		#region Utility
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus();</SCRIPT>";
			this.scriptJP(s);
		}

		private void AlertJS(string msg)
		{
			if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
			{
				string scriptString = "<SCRIPT>alert('" + msg.Replace("'", "\\'") + "');</SCRIPT>";
				//this.Page.RegisterStartupScript("alertJavaScript", scriptString);
				this.ClientScript.RegisterStartupScript(this.GetType(), "alertJavaScript", scriptString);
			}
		}

		private void scriptJP(string script)
		{
			if (!this.Page.IsStartupScriptRegistered("scriptJavaScript"))
				this.ClientScript.RegisterStartupScript(this.GetType(), "scriptJavaScript", script);
			//this.Page.RegisterStartupScript("scriptJavaScript", script);				
		}
		#endregion
	}
}
