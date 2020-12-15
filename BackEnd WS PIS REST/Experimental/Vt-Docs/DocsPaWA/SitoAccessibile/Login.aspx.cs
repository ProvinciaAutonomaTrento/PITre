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
using System.Text;

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class Login : WebPage
	{
        /*Messaggi Integrazione IAM*/
        private const string UNKNOWN_USER_MSG_POLICY_AGENT = "L'utente non è autorizzato ad accedere all'applicazione";
        private const string INCORRECT_DATA_FORMAT_ACCESS_MANAGER = "Formato dati da AM non corretto";
        /*Fine messaggi Integrazione IAM*/
		private const string UNKNOWN_USER_MSG = "Nome utente o password errati";
		private const string USER_ALREADY_LOGGED_MSG = "Utente già connesso";
		private const string USER_ALREADY_LOGGED_ASK_FOR_FORCE_MSG = "Utente già connesso. Vuoi procedere comunque?";
		private const string DISABLED_USER_MSG = "Utente non abilitato";
		private const string USER_WITH_NO_RULE_MSG = "L'utente non è associato ad alcun ruolo";
		private const string GENERIC_ERROR_MSG = "Errore generico nella procedura di login";
		private const string NO_ORGANIZATION_MSG = "Impossibile completare l'autenticazione perchè non risulta configurata alcuna amministrazione";
        private const string NO_AMM_DEFINED = "E' necessario scegliere un'amministrazione";
        private const string PASSWORD_EXPIRED = "La password dell'utente risulta scaduta";

		public enum PageType { authentication, choice, authentication_ammin}
		private enum Actions { ShowForm, DoLogin, AskForForcedLogin, DoForcedLogin }
		private InnerLogin login = null;
		private DocsPAWA.DocsPaWR.Utente utente = null;
		private Actions myAction = Actions.ShowForm;
		private PageType pgType = PageType.authentication;
        private string logoutMessageText = String.Empty;
        private vic.Integra VICLoad;

		private bool session = false;
		private bool force = false;       
		private string action = null;
        private bool forced = false;

		public Login()
		{
			#region recupero del parametro Session

			if (this.Context.Request.Params["session"]!=null)
			{
				try
				{
					session = System.Boolean.Parse((string)this.Context.Request.Params["session"]);
				} 
				catch (Exception) {}
			}
			#endregion recupero del parametro Session

			#region recupero del parametro Force
			if (this.Context.Request.Params["force"]!=null)
			{
				try
				{
					force = System.Boolean.Parse((string)this.Context.Request.Params["force"]);
					if (force)
						session = true;
				} 
				catch (Exception) {}
			}
			#endregion recupero del parametro Force

            #region recupero parametro forced
            if (this.Context.Request.Params["forced"] != null)
            {
                try
                {
                    forced = System.Boolean.Parse((string)this.Context.Request.Params["forced"]);
                }
                catch (Exception) { }
            }
            #endregion recupero del parametro forced

			#region recupero del parametro Action
			action = (string)this.Context.Request.Params["action"];
			#endregion recupero del parametro Force
		}


		public override void Create(System.Web.SessionState.HttpSessionState mySession)
		{
			base.Create(mySession);
		
			#region recupera le informazioni dalla sessione
			login =  new InnerLogin();
			if (session && pgSession["login"]!=null)
			{
				login = ((InnerLogin)pgSession["login"]);
				pgSession.Remove("login");
			}
			#endregion recupera le informazioni dalla sessione

			#region calcola operazione
			if (action!=null && action=="login")
			{
				if (force)
				{
					myAction = Actions.DoForcedLogin;
				}
				else
				{
					login.IdAmministrazione = (string)this.Context.Request.Params["cmbamministrazioni"];
					login.UserId = (string)this.Context.Request.Params["userid"];
					login.UserPwd = (string)this.Context.Request.Params["password"];
					myAction = Actions.DoLogin;
				}
			}
			else
			{
				if (force)
				{
					myAction = Actions.AskForForcedLogin;
				}
				else
				{
					login.ErrorMessage = (string)this.Context.Request.Params["error"];
					myAction = Actions.ShowForm;
				}
			}
			#endregion calcola operazione

			#region esegue operazione
			switch (myAction)
			{
				case Actions.ShowForm:
                    /*Integrazione IAM*/
                    string policyAgent = null;
                    policyAgent = ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED);
                    if (policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper())
                    {
                        /*caso query string*/
                        //if (Request.QueryString["exit"] != null && (Request.QueryString["exit"].ToString().ToUpper() == Boolean.TrueString.ToUpper()))
                        /*fine caso query string*/
                        if (Session["exit"] != null && Convert.ToBoolean(Session["exit"].ToString()))
                        {
                            Session.Clear();
                            LogoutMessage = "Logout effettuato con successo";
                            UserId = VICLoad.amData.account.ToString();
                        }
                        else
                        {
                            if (forced)
                            {
                                string url = EnvironmentContext.RootPath + "exit.aspx";                                
                                Response.Redirect(url, true);       
                            }
                            else
                                this.ExecuteLogin();
                        }
                        /*fine Integrazione IAM*/
                    }
					//delegato alla componente aspx
					break;
				case Actions.DoLogin:
					//esegue l'operazione di login e predispone il risultato
					ExecuteLogin();
					break;
				case Actions.AskForForcedLogin:
					//delegato alla componente aspx
					break;
				case Actions.DoForcedLogin:
					//esegue l'operazione di login e predispone il risultato
					ExecuteForcedLogin();
					break;
			}
			#endregion esegue operazione
		}

		public override void AddToSession()
		{
			pgSession["login"]=login;
		}
		public override void RemoveFromSession()
		{
			if (pgSession["login"]!=null)
				pgSession.Remove("login");
			login = new InnerLogin();
		}

		public string IdAmministrazione
		{
			get { return login.IdAmministrazione; }
			set { login.IdAmministrazione = value; }
		}

        public bool existsLogoAmm
        {
            get 
            {
                return fileExist("logo.gif", "LoginFE");
            }
           // set { login.IdAmministrazione = value; }
        }
 
        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }

		public string UserId
		{
			get { return login.UserId; }
			set { login.UserId = value; }
		}

		public string UserPwd
		{
			get { return login.UserPwd; }
			set { login.UserPwd = value; }
		}

		public string ErrorMessage
		{
			get { return login.ErrorMessage; }
			set { login.ErrorMessage = value; }
		}

        public string LogoutMessage
        {
            get { return logoutMessageText; }
            set { logoutMessageText = value; }
        }

		public PageType Type
		{
			get { return pgType; }
			set { pgType = value; }
		}

		public DocsPAWA.DocsPaWR.Utente Utente
		{
			get { return utente; }
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
            string policyAgent = null;
            policyAgent = ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED);
            if ((policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
            {
                VICLoad = new vic.Integra(Context, "");                
            }
			// Put user code to initialize the page here
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
		}
		#endregion

		public DocsPAWA.DocsPaWR.Amministrazione[] GetAmministrazioni()
		{
			string returnMsg = string.Empty;
			DocsPaWR.Amministrazione[] amministrazioni = null;
			try
			{
				amministrazioni = DocsPAWA.UserManager.getListaAmministrazioni(this,out returnMsg);

				if (amministrazioni==null)
				{
					ErrorMessage = NO_ORGANIZATION_MSG;
				}
			}
			catch (Exception)
			{
				ErrorMessage = GENERIC_ERROR_MSG;
			}

			return amministrazioni;
		}

		public void ExecuteLogin()
		{
			try
			{
				DocsPaWR.UserLogin lgn = new DocsPAWA.DocsPaWR.UserLogin();
                /*Integrazione IAM*/
                string policyAgent = null;
                policyAgent = ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED);
                if ((policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                {
                    vic.Integra VIC = new vic.Integra(Context, "");                   
                    lgn.UserName = VIC.amData.account.ToString();
                    UserId = VIC.amData.account.ToString();
                    lgn.Token = VIC.amData.account.ToString() + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();
                    string appo = VIC.amData.account.ToString() + "&" + ConfigSettings.getKey(ConfigSettings.KeysENUM.CHIAVE_TOKEN).ToString() + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();
                    byte[] bt_datiInput = ASCIIEncoding.ASCII.GetBytes(appo);
                    lgn.Token = lgn.Token + '&' + DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(bt_datiInput);
                    if (String.IsNullOrEmpty(VIC.amData.account.ToString()) || String.IsNullOrEmpty(VIC.amData.codFiscale.ToString()) || String.IsNullOrEmpty(VIC.amData.matricola.ToString()))
                    {
                        this.ErrorMessage = INCORRECT_DATA_FORMAT_ACCESS_MANAGER;
                        return;
                    }                   
                }
                else
                {
				lgn.UserName = UserId;
				lgn.Password = UserPwd;
                }
                /*Fine integrazione IAM*/

				lgn.IdAmministrazione = IdAmministrazione;
				lgn.IPAddress=GetCallerAddress();

				DocsPaWR.LoginResult loginResult;	
				string ipaddress="";
				utente = UserManager.login(this.Page, lgn, out loginResult, out ipaddress);
				switch(loginResult)
				{
					case DocsPAWA.DocsPaWR.LoginResult.OK:					
						ErrorMessage = null;
						break;
					case DocsPAWA.DocsPaWR.LoginResult.UNKNOWN_USER:
                        if ((ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED).ToUpper() == Boolean.TrueString.ToUpper()))
                            ErrorMessage = UNKNOWN_USER_MSG_POLICY_AGENT;
                        else   
						    ErrorMessage = UNKNOWN_USER_MSG;
						pgType = PageType.authentication;
						break;
					case DocsPAWA.DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
						ErrorMessage = USER_ALREADY_LOGGED_MSG;
						string loginMode = null;
						try 
						{
							loginMode = ConfigSettings.getKey(ConfigSettings.KeysENUM.ADMINISTERED_LOGIN_MODE);
						} 
						catch(Exception) {}

						if(loginMode == null || loginMode.ToUpper() == Boolean.TrueString.ToUpper())
						{
							// Gestione tramite tool di amministrazione
							pgType = PageType.authentication;
						}
						else
						{
							ErrorMessage = USER_ALREADY_LOGGED_ASK_FOR_FORCE_MSG;
							pgType = PageType.choice;
                            Session["forced"] = true;
						}
						break;
					case DocsPAWA.DocsPaWR.LoginResult.DISABLED_USER:
						ErrorMessage=DISABLED_USER_MSG;
						pgType = PageType.authentication;
						break;
					case DocsPAWA.DocsPaWR.LoginResult.NO_RUOLI:
						ErrorMessage=USER_WITH_NO_RULE_MSG;
						pgType = PageType.authentication;
						break;
                    case DocsPAWA.DocsPaWR.LoginResult.NO_AMMIN:
                        ErrorMessage = NO_AMM_DEFINED;
                        pgType = PageType.authentication_ammin;
                        break;
                    case DocsPAWA.DocsPaWR.LoginResult.PASSWORD_EXPIRED:
                        ErrorMessage = PASSWORD_EXPIRED;
                        pgType = PageType.authentication;
                        break;
					default: // Application Error
						ErrorMessage=GENERIC_ERROR_MSG;
						pgType = PageType.authentication;
						break;
				}
			}
			catch (Exception)
			{
				ErrorMessage = GENERIC_ERROR_MSG;
				pgType = PageType.authentication;
			}

			if (utente==null)
			{
				AddToSession();
			}
			else
			{
				Session["AppWA"]="DOCSPA";

				if (utente.ruoli.Length>0)
					utente.ruoli[0].selezionato = true;
				UserManager.setUtente(this.Page,utente);
				//Session["userData"] = utente;
				UserManager.setRuolo(this.Page,utente.ruoli[0]);
				
				if (utente.ruoli[0].registri!=null && utente.ruoli[0].registri.Length>0)
					UserManager.setRegistroSelezionato(this.Page,utente.ruoli[0].registri[0]);
		
				this.GoToHomePage();		
			}
		}

		private void GoToHomePage()
		{
			Response.Redirect(this.GetBaseUrl()+"/Home.aspx");
		}
				
		public void ExecuteForcedLogin()
		{
			try
			{
				DocsPaWR.UserLogin lgn = new DocsPAWA.DocsPaWR.UserLogin();
                /* Integrazione IAM*/
                string policyAgent = null;
                policyAgent = ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED);
                if ((policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                {
                    vic.Integra VIC = new vic.Integra(Context, "");                    
                    lgn.UserName = VIC.amData.account.ToString();
                    lgn.Token = VIC.amData.account.ToString() + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();
                    string appo = VIC.amData.account.ToString() + "&" + ConfigSettings.getKey(ConfigSettings.KeysENUM.CHIAVE_TOKEN).ToString() + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();
                    byte[] bt_datiInput = ASCIIEncoding.ASCII.GetBytes(appo);
                    lgn.Token = lgn.Token + '&' + DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(bt_datiInput);
                    if (String.IsNullOrEmpty(VIC.amData.account.ToString()) || String.IsNullOrEmpty(VIC.amData.codFiscale.ToString()) || String.IsNullOrEmpty(VIC.amData.matricola.ToString()))
                    {
                        this.ErrorMessage = this.ErrorMessage = INCORRECT_DATA_FORMAT_ACCESS_MANAGER;
                        return;                                               
                    }                                 
                }
                else
                {
				lgn.UserName = UserId;
				lgn.Password = UserPwd;
                }
                /*fine Integrazione IAM*/
				lgn.IdAmministrazione = IdAmministrazione;

				DocsPaWR.LoginResult loginResult;	
				utente = UserManager.ForcedLogin(this.Page, lgn, out loginResult);
				switch(loginResult)
				{
					case DocsPAWA.DocsPaWR.LoginResult.OK:					
						ErrorMessage = null;
						break;
					case DocsPAWA.DocsPaWR.LoginResult.UNKNOWN_USER:
						ErrorMessage = UNKNOWN_USER_MSG;
						pgType = PageType.authentication;
						break;
					case DocsPAWA.DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
						ErrorMessage = USER_ALREADY_LOGGED_MSG;
						string loginMode = null;
						try 
						{
							loginMode = ConfigSettings.getKey(ConfigSettings.KeysENUM.ADMINISTERED_LOGIN_MODE);
						} 
						catch(Exception) {}

						if(loginMode == null || loginMode.ToUpper() == Boolean.TrueString.ToUpper())
						{
							// Gestione tramite tool di amministrazione
							pgType = PageType.authentication;
						}
						else
						{
							ErrorMessage = USER_ALREADY_LOGGED_ASK_FOR_FORCE_MSG;
							pgType = PageType.choice;
						}
						break;
					case DocsPAWA.DocsPaWR.LoginResult.DISABLED_USER:
						ErrorMessage=DISABLED_USER_MSG;
						pgType = PageType.authentication;
						break;
					case DocsPAWA.DocsPaWR.LoginResult.NO_RUOLI:
						ErrorMessage=USER_WITH_NO_RULE_MSG;
						pgType = PageType.authentication;
						break;
					default: // Application Error
						ErrorMessage=GENERIC_ERROR_MSG;
						pgType = PageType.authentication;
						break;
				}
			}
			catch (Exception)
			{
				ErrorMessage = GENERIC_ERROR_MSG;
				pgType = PageType.authentication;
			}

			if (utente==null)
			{
				AddToSession();
			}
			else
			{
				if (utente.ruoli.Length>0)
					utente.ruoli[0].selezionato = true;
				UserManager.setUtente(this.Page,utente);
				//Session["userData"] = utente;
				UserManager.setRuolo(this.Page,utente.ruoli[0]);

				if (utente.ruoli[0].registri!=null && utente.ruoli[0].registri.Length>0)
					UserManager.setRegistroSelezionato(this.Page,utente.ruoli[0].registri[0]);
				
				this.GoToHomePage();
			}
		}


		class InnerLogin
		{
			private string idamm = null;
			private string userid = null;
			private string userpwd = null;
			private string errormsg = null;

			public string IdAmministrazione
			{
				get { return idamm; }
				set { idamm = value; }
			}

			public string UserId
			{
				get { return userid; }
				set { userid = value; }
			}

			public string UserPwd
			{
				get { return userpwd; }
				set { userpwd = value; }
			}

			public string ErrorMessage
			{
				get { return errormsg; }
				set { errormsg = value; }
			}
		}

	}

}
