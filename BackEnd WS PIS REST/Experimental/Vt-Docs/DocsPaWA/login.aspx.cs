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
using System.Configuration;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using System.Linq;
using log4net;
using System.Security.Principal;

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class login : System.Web.UI.Page
	{
        private ILog logger = LogManager.GetLogger(typeof(login));
		protected System.Web.UI.WebControls.ImageButton btn_login;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label Label3;
     
		protected System.Web.UI.WebControls.TextBox userid;
		protected System.Web.UI.WebControls.Image img_logologinente2;
        protected System.Web.UI.WebControls.Image img_logologinente1;
        protected System.Web.UI.WebControls.Image img_logologin;
        //protected System.Web.UI.WebControls.Panel pnl_Login;
		protected System.Web.UI.WebControls.Panel pnl_SceltaAmministrazione;
		protected System.Web.UI.WebControls.DropDownList ddl_Amministrazioni;
		protected System.Web.UI.HtmlControls.HtmlInputHidden sel_amministrazioni;

        protected System.Web.UI.WebControls.Label lblPassword;
        protected System.Web.UI.WebControls.Label lblPasswordConfirm;
        protected System.Web.UI.WebControls.TextBox password;
        protected System.Web.UI.WebControls.TextBox passwordConfirm;   

		// my var
		private string m_idAmministrazione="";
		protected System.Web.UI.WebControls.Panel pnl_error;
		protected System.Web.UI.WebControls.Label lbl_error;
		protected System.Web.UI.WebControls.Panel pnl_ddlAmm;
		protected System.Web.UI.WebControls.Label lbl_Amm;
		protected System.Web.UI.WebControls.Panel pnl_login;

        protected System.Web.UI.WebControls.MultiView loginMultiView;

        //Pec multi-amm
        //<
        protected System.Web.UI.WebControls.HiddenField hflLoginResult;
        //>
        // protected string appTitle;
        
        bool logged = false;
		// end my var

        /// <summary>
        /// Caricamento modalità di avvio dell'applicazione:
        /// - se presente il parametro "useStaticPath" valorizzato a "true",
        ///   l'applicazione utilizza, per il reperimento dei path, 
        ///   il percorso statico configurato nel file web.config
        /// </summary>
        private void LoadStaticRootPath()
        {
            if (Session["useStaticRootPath"] == null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["useStaticPath"]))
                {
                    if (Session["useStaticRootPath"] == null)
                    {
                        if (Request.QueryString["useStaticPath"].ToString().ToLower() == "true")
                        {
                            Session["useStaticRootPath"] = "true";

                            logger.Debug("useStaticPath = 'true'");
                        }
                    }
                }
            }
        } 

		private void Page_Load(object sender, System.EventArgs e) 
		{
            if (GetShibSSOEnabled())
            {
                LoginShibboleth();
            } else 
            if (this.IsActiveCAS)
            {
                this.loginMultiView.ActiveViewIndex = 1;

                this.LoginCAS();
            }
            else 
            {
                //Interazione IAM-GFD
                string policyAgent = null;
                policyAgent = ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED);

                //Autenticazione Windows
                
                bool windows_auth = Convert.ToBoolean( ConfigSettings.getKey(ConfigSettings.KeysENUM.AUTENTICAZIONE_WINDOWS));
                if (windows_auth)
                {
                    RegisterStartupScript("nascondiSitoAccessibile", "<script>nascondiSitoAccessibile();</script>");
                }
                else if ((policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                {
                    RegisterStartupScript("nascondiSitoAccessibile", "<script>nascondiSitoAccessibile();</script>");
                }
                //Fine Integrazione IAM-GFD

                if (Request.QueryString["ForceLogin"] != null && Request.QueryString["ForceLogin"].ToUpper() == Boolean.TrueString.ToUpper())
                {
                    // Gestione login forzato 
                    Page.RegisterStartupScript("NotifyUser", null);	// Fa in modo che lo script non venga lanciato
                    string administeredLogin = null;

                    try
                    {
                        administeredLogin = ConfigSettings.getKey(ConfigSettings.KeysENUM.ADMINISTERED_LOGIN_MODE);
                    }
                    catch (Exception) { }

                    if (administeredLogin == null || administeredLogin.ToUpper() == Boolean.FalseString.ToUpper())
                    {
                        if (Session["loginData"] == null || !this.ForcedLogin((DocsPAWA.DocsPaWR.UserLogin)Session["loginData"]))
                        {
                            this.lbl_error.Text = "Errore durante la chiusura della connessione esistente.";
                        }
                    }
                }
                else
                {
                    Session.RemoveAll();
                }

                // Se la query string contiene groupId, docNumber o idProfile...
                if (!(String.IsNullOrEmpty(Request["groupId"]) ||
                      String.IsNullOrEmpty(Request["docNumber"]) ||
                      String.IsNullOrEmpty(Request["idProfile"])))
                    // ...è stata richiesta la visualizzazione di un oggetto dall'esterno
                    // quindi viene immesso in sessione la query string
                    Session["visualizzaLink"] = Request.QueryString.ToString();

                // Se la query string contiene idAmministrazione, tipoOggetto, idObj o tipoProto...
                if (!(String.IsNullOrEmpty(Request["idAmministrazione"]) ||
                      String.IsNullOrEmpty(Request["tipoOggetto"]) ||
                      String.IsNullOrEmpty(Request["idObj"])))
                    // ...è stata richiesta la visualizzazione di un oggetto dall'esterno
                    // quindi viene immesso in sessione la query string
                    Session["visualizzaOggetto"] = Request.QueryString.ToString();

                m_idAmministrazione = this.sel_amministrazioni.Value;
                getQueryStringParameter();

                this.caricaLoghi();

                this.LoadStaticRootPath();

                this.caricaLogoAmministrazione();



                //Inizio integrazione IAM-GFD            

                if ((policyAgent != null && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                {
                    #region iam
                    RegisterStartupScript("nascondiSitoAccessibile", "<script>nascondiSitoAccessibile();</script>");
                    this.lblPassword.Visible = false;
                    this.passwordConfirm.Visible = false;
                    this.userid.Visible = false;
                    this.Label2.Visible = false;
                    this.lblPasswordConfirm.Visible = false;
                    this.btn_login.Visible = false;
                    this.password.Visible = false;
                    if ((Request.QueryString["ForceLogin"] == null || !(Request.QueryString["ForceLogin"].ToUpper() == Boolean.TrueString.ToUpper())))
                    {
                        vic.Integra VIC = new vic.Integra(Context, "");
                        this.userid.Text = VIC.amData.account.ToString();

                        if (this.ddl_Amministrazioni.SelectedItem != null)
                        {
                            m_idAmministrazione = this.ddl_Amministrazioni.SelectedItem.Value;
                        }

                        this.sel_amministrazioni.Value = m_idAmministrazione;

                        string message = string.Empty;

                        DocsPaWR.UserLogin userLogin = this.CreateUserLogin();
                        userLogin.Token = VIC.amData.account.ToString() + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();
                        //DocsPaVO.utente.
                        string appo = VIC.amData.account.ToString() + "&" + ConfigSettings.getKey(ConfigSettings.KeysENUM.CHIAVE_TOKEN).ToString() + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();
                        byte[] bt_datiInput = ASCIIEncoding.ASCII.GetBytes(appo);
                        userLogin.Token = userLogin.Token + '&' + DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(bt_datiInput);
                        //userLogin.Token = "aaa";
                        if (String.IsNullOrEmpty(VIC.amData.account.ToString()) || String.IsNullOrEmpty(VIC.amData.codFiscale.ToString()) || String.IsNullOrEmpty(VIC.amData.matricola.ToString()))
                        {
                            this.lbl_error.Text = "L'utente non è autorizzato ad accedere all'applicazione";
                            this.lbl_error.Visible = true;
                            this.pnl_error.Visible = true;
                            this.logged = false;
                        }
                        else
                        {
                            if (!loginAction(userLogin, out message))
                            {
                                this.lbl_error.Text = message;
                                this.lbl_error.Visible = true;
                                this.pnl_error.Visible = true;
                            }
                            else
                                this.logged = true;
                        }
                    }
                    #endregion iam
                }
                else if (windows_auth)
                {
                    #region windows auth

                    WindowsPrincipal windows_user = (HttpContext.Current.User as WindowsPrincipal);

                    var dati_user = windows_user.Identity.Name.Split('\\');
                    var dominio = dati_user[0].ToString().ToUpper();
                    var userN = dati_user[1].ToString().ToUpper();
                    string tipo_auth = windows_user.Identity.AuthenticationType;


                    RegisterStartupScript("nascondiSitoAccessibile", "<script>nascondiSitoAccessibile();</script>");
                    this.lblPassword.Visible = false;
                    this.passwordConfirm.Visible = false;
                    this.userid.Visible = false;
                    this.Label2.Visible = false;
                    this.lblPasswordConfirm.Visible = false;
                    this.btn_login.Visible = false;
                    this.password.Visible = false;
                    if ((Request.QueryString["ForceLogin"] == null || !(Request.QueryString["ForceLogin"].ToUpper() == Boolean.TrueString.ToUpper())))
                    {

                        this.userid.Text = userN;

                        if (this.ddl_Amministrazioni.SelectedItem != null)
                        {
                            m_idAmministrazione = this.ddl_Amministrazioni.SelectedItem.Value;
                        }

                        this.sel_amministrazioni.Value = m_idAmministrazione;

                        string message = string.Empty;

                        DocsPaWR.UserLogin userLogin = this.CreateUserLogin();
                        userLogin.Token = userN + '&' + dominio + '&' + tipo_auth;
                        //DocsPaVO.utente.
                        string appo = userN + "&" + ConfigSettings.getKey(ConfigSettings.KeysENUM.CHIAVE_TOKEN).ToString() + '&' + dominio + '&' + tipo_auth;
                        byte[] bt_datiInput = ASCIIEncoding.ASCII.GetBytes(appo);
                        userLogin.Token = userLogin.Token + '&' + DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(bt_datiInput);
                        //userLogin.Token = "aaa";
                        if (String.IsNullOrEmpty(userN) || String.IsNullOrEmpty(dominio) || String.IsNullOrEmpty(tipo_auth))
                        {
                            this.lbl_error.Text = "L'utente non è autorizzato ad accedere all'applicazione";
                            this.lbl_error.Visible = true;
                            this.pnl_error.Visible = true;
                            this.logged = false;
                        }
                        else
                        {
                            if (!loginAction(userLogin, out message))
                            {
                                this.lbl_error.Text = message;
                                this.lbl_error.Visible = true;
                                this.pnl_error.Visible = true;
                            }
                            else
                                this.logged = true;
                        }
                    }

                    #endregion windows auth
                }

                //Fine Integrazione IAM-GFD
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
			this.btn_login.Click += new System.Web.UI.ImageClickEventHandler(this.btn_login_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.login_PreRender);

		}
		#endregion

		private void login_PreRender(object sender, System.EventArgs e) 
		{
            if (!this.IsActiveCAS)
            {
                bool res;

                if (!logged)
                {
                    this.lblPasswordConfirm.Visible = this.OnPasswordExpired;
                    this.passwordConfirm.Visible = this.lblPasswordConfirm.Visible;

                    if (m_idAmministrazione.Equals("") || m_idAmministrazione.Equals("-1"))
                    {
                        /*AUTENTICAZIONE CENTRALIZZATA*/
                        //Verifico se provengo da una autenticazione centralizzata.
                        //Questa verifica viene fatta una sola volta e solo se si 
                        //proviene dalla pagina di autenticazione centralizzata.
                        //"AutenticazioneCentralizzata.aspx"
                        //In caso positivo si simula il click sul pulsante login
                        //avendo valorizzato i campi username e password.
                        //Così facendo lasciamo invariata tutta la logica di questa pagina
                        if (!IsPostBack)
                        {
                            if (this.PreviousPage != null)
                            {
                                if (this.PreviousPage.FindControl("txt_userID") != null && this.PreviousPage.FindControl("txt_password") != null)
                                {
                                    userid.Text = ((HtmlInputHidden)PreviousPage.FindControl("txt_userID")).Value;
                                    password.Text = ((HtmlInputHidden)PreviousPage.FindControl("txt_password")).Value;
                                    btn_login_Click(null, null);
                                    return;
                                }
                            }
                        }
                        /*FINE AUTENTICAZIONE CENTRALIZZATA*/

                        //Pec multi-amm
                        //<
                        //Popola la lista delle amministrazioni solo se l'utente appartiene a piu' amministrazioni
                        if (hflLoginResult.Value == DocsPAWA.DocsPaWR.LoginResult.NO_AMMIN.ToString())
                        {
                            res = caricaComboAmministrazioni();
                        }
                        else
                        {
                            res = true;
                        }
                        //>

                        if (!res)
                        {
                            pnl_login.Visible = false;
                            pnl_error.Visible = true;
                            this.lbl_error.Visible = true;
                            return;
                        }
                        else if (res && m_idAmministrazione.Equals(""))
                        {
                            pnl_login.Visible = true;
                            if (this.lbl_error.Text.Equals(""))
                                pnl_error.Visible = false;
                            else
                                pnl_error.Visible = true;
                            this.lbl_error.Visible = true;
                            return;
                        }
                        else if (res && m_idAmministrazione.Equals("-1"))
                        {
                            pnl_login.Visible = true;
                            pnl_error.Visible = true;
                            this.lbl_error.Visible = true;

                            if (ViewState["pwdMultiAmm"] != null)
                            {
                                this.password.Visible = false;
                                this.lblPassword.Visible = false;
                            }

                            return;
                        }
                    }
                }
            }
		}

		private void getQueryStringParameter()
		{
			if (Request.QueryString["sel_amministrazioni"]!=null)
			{
				m_idAmministrazione=(string)Request.QueryString["sel_amministrazioni"];
				Response.Cookies["sel_amministrazioni"].Value=m_idAmministrazione;
			}
			else
			{
				if (Request.Cookies.Count!=0 && Request.Cookies["CK_idAmministrazione"]!=null)
				{
					if (Utils.isNumeric((string)Request.Cookies["CK_idAmministrazione"].Value))
					{
						m_idAmministrazione=(string)Request.Cookies["CK_idAmministrazione"].Value;
					}
				}
			}
		}

		/// <summary>
		/// Lancia l'applicazione
		/// </summary>
		private void LaunchApplication()
		{
			string fullScreen = "true";			

			try 
			{
				fullScreen = ConfigSettings.getKey(ConfigSettings.KeysENUM.FULLSCREEN);
			} 
			catch(Exception) {}

			fullScreen = fullScreen.ToLower();

            // Se nella sessione è presente visualizzaLink...
            if (Session["visualizzaLink"] != null)
            {
                // ...significa che si è arrivati a questa pagina dalla pagina 
                // visualizzaLink...                
                // ...è quindi necessatio prelevare i parametri della query string memorizzati 
                // nella sessione e richiamare la pagina visualizzaLink...
                string url = Utils.getHttpFullPath() + "/visualizzaLink.aspx?" + Session["visualizzaLink"];
                string script = String.Empty; // "<script>";
                script += "var popup = window.open('" + url + "','Index',";
                script += "'location=0,resizable=yes');";
                script += " if(popup!=self) {window.opener=null;self.close();}";
                //script += "</script>";
                ClientScript.RegisterStartupScript(this.GetType(), "Index", script, true);
                //Page.RegisterStartupScript("Index", script);

                // Infine si rimuove dalla sessione il parametro visualizzaLink
                Session.Remove("visualizzaLink");
                return;

            }           

            if(fullScreen.Equals("false"))
			{
				Response.Redirect("index.aspx");
                return;
			}
			else
			{
                string script = string.Empty; //"<script>";
				script += "var popup = window.open('index.aspx','Index',";
                // script += "'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');";
                //ie7:
                script += "'location=0,resizable=yes');"; 
                script += "popup.moveTo(0,0); popup.resizeTo(screen.availWidth,screen.availHeight);";
				script += " if(popup!=self) {window.opener=null;self.close();}";
				//script += "</script>";
                //Page.RegisterStartupScript("Index", script);
                ClientScript.RegisterStartupScript(this.GetType(), "Index", script, true);
                //return;
			}

            // Se nella sessione è presente visualizzaOggetto...
            if (Session["visualizzaOggetto"] != null)
            {
                string url = Utils.getHttpFullPath() + "/visualizzaOggetto.aspx?" + Session["visualizzaOggetto"];
                string script = "<script>";
                script += "var popup = window.open('" + url + "','Index',";
                script += "'location=0,resizable=yes');";
                script += " if(popup!=self) {window.opener=null;self.close();}";
                script += "</script>";
                //ClientScript.RegisterStartupScript(this.GetType(), "Index", script, true);
                //Page.RegisterStartupScript("Index", script);
                Response.Write(script);

                // Infine si rimuove dalla sessione il parametro visualizzaLink
                Session.Remove("visualizzaOggetto");

                return;
                
                /*
                // ...significa che si è arrivati a questa pagina dalla pagina 
                // visualizzaOggetto...                
                // ...è quindi necessatio prelevare i parametri della query string memorizzati 
                // nella sessione e richiamare la pagina visualizzaOggetto...
                string url = Utils.getHttpFullPath() + "/visualizzaOggetto.aspx?" + Session["visualizzaOggetto"];

                // Si rimuove dalla sessione il parametro visualizzaOggetto
                Session.Remove("visualizzaOggetto");

                // E si redireziona la response alla pagina visualizzaOggetto
                Response.Redirect(url, false);
                return;
                */
            }

            
           
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lgn"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private bool ForcedLogin(DocsPAWA.DocsPaWR.UserLogin lgn)
		{
			bool result = false; 
			
			DocsPaWR.LoginResult loginResult;			
			DocsPaWR.Utente utente = UserManager.ForcedLogin(this, lgn, out loginResult);

			if(loginResult == DocsPAWA.DocsPaWR.LoginResult.OK)
			{
				result = true; // L'utente e' stato connesso
				utente.urlWA= Utils.getHttpFullPath(this);
				Session["userData"] = utente;
				
				this.LaunchApplication();
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lgn"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		private bool loginAction(DocsPAWA.DocsPaWR.UserLogin lgn, out string message)
		{
			bool resLogin = false; 
			message = string.Empty;
			string ipaddress="";
			
			DocsPaWR.LoginResult loginResult;			
			DocsPaWR.Utente utente = UserManager.login(this, lgn, out loginResult, out ipaddress);

			switch(loginResult)
			{
				case DocsPAWA.DocsPaWR.LoginResult.OK:					
					resLogin = true; // L'utente e' stato connesso
					this.LaunchApplication();
					break;

				case DocsPAWA.DocsPaWR.LoginResult.UNKNOWN_USER:
                    if ((ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.POLICY_AGENT_ENABLED).ToUpper() == Boolean.TrueString.ToUpper()))
                        message = "L'utente non è autorizzato ad accedere all'applicazione";
                    else
                        message = "Nome o password errati";
					break;

				case DocsPAWA.DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
					string loginMode = null;
				
					try 
					{
						loginMode = ConfigSettings.getKey(ConfigSettings.KeysENUM.ADMINISTERED_LOGIN_MODE);
					} 
					catch(Exception) {}

					if(loginMode == null || loginMode.ToUpper() == Boolean.TrueString.ToUpper())
					{
						// Gestione tramite tool di amministrazione
						message = "L'utente ha gia' una connessione aperta. Contattare l'amministrazione.";
					}
					else
					{
						// Gestione autonoma da parte dell'utente
						message = "L'utente ha gia' una connessione aperta.";

						// Store login object
						Session.Add("loginData", lgn);

                        // Lettura del valore della chiave di configurazione AUTO_FORCE_LOGIN
                        bool autoForce;
                        Boolean.TryParse(System.Configuration.ConfigurationManager.
                            AppSettings["AUTO_FORCE_LOGIN"], out autoForce);

                        // Se è attiva la chiave di configurazione AUTO_FORCE_LOGIN  
                        if (autoForce)
                            // ...allora redizioniamo la response aggiungendo forceLogin=True alla
                            // request
                            Response.Redirect("login.aspx?forceLogin=True", false);
                        else
                        {
                            // ...altrimenti si procede nel modo classico
                            string script = "<script>forceLogin('" + ipaddress + "');</script>";
                            Page.RegisterStartupScript("NotifyUser", script);
                        }
					}

					break;

				case DocsPAWA.DocsPaWR.LoginResult.DISABLED_USER:
					message="Utente non abilitato";
					break;

				case DocsPAWA.DocsPaWR.LoginResult.NO_RUOLI:
					message="L'utente non può accedere al sistema perché non è<br>associato a nessun ruolo";
                    break;

                case DocsPAWA.DocsPaWR.LoginResult.NO_AMMIN:  //aggiunto sabrina
				    m_idAmministrazione = "-1";                 
                    message = "Selezionare un'amministrazione";
                    break;

                case DocsPAWA.DocsPaWR.LoginResult.PASSWORD_EXPIRED:
                    // Gestione scadenza password
                    this.OnPasswordExpired = true;
                    this.OldPassword = lgn.Password;
                    message = "La password dell'utente risulta scaduta. Immettere una nuova password.";
                    break;

                    case DocsPAWA.DocsPaWR.LoginResult.DTCM_SERVICE_NO_CONTACT:
                   
                    message = "Errore, I Servizi del Documentale DTCM non sono raggiungibili.";
                    break;

                      case DocsPAWA.DocsPaWR.LoginResult.UNKNOWN_AMMIN:
                                      message = "Errore, Amministrazione inesistente.";
                    break;

                case DocsPAWA.DocsPaWR.LoginResult.UNKNOWN_DTCM_USER:
                    
                    message = "Errore, L'utente non risulta presente nel Documentale DTCM.";
                    break;

                case DocsPAWA.DocsPaWR.LoginResult.DB_ERROR:
                    message = "Errore nella connessione al Database durante la procedura di login.";
                    break;

                default: 
                    // Application Error
					message = "Errore nella procedura di Login. Contattare l'amministrazione.";
					break;
			}

            //Pec multi-amm
            //<
            hflLoginResult.Value = loginResult.ToString();
            //>
			if (resLogin)
			{
				utente.urlWA= Utils.getHttpFullPath(this);
				Session["userData"] = utente;
			}
			return resLogin;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool caricaComboAmministrazioni() 
		{
            DocsPaWR.Amministrazione[] amministrazioni = null;
			string returnMsg = string.Empty;
			this.ddl_Amministrazioni.Items.Clear();
            //MEV utente multi-amministrazione
            if (string.IsNullOrEmpty(userid.Text)) 
			    amministrazioni = UserManager.getListaAmministrazioni(this, out returnMsg);
            else
			    amministrazioni = UserManager.getListaAmministrazioniByUser(this, userid.Text, false, out returnMsg);
            

			if(!returnMsg.Equals(string.Empty))
			{
				this.lbl_error.Text= "Impossibile contattare il Server";
				this.lbl_error.ToolTip = returnMsg;
				return false;
			}

			if(amministrazioni == null || amministrazioni.Length == 0)
			{
				this.lbl_error.Text= "Nessuna amministrazione disponibile";				
				return false;
			}
			else
			{
				if(amministrazioni.Length == 1) 
				{
					m_idAmministrazione = amministrazioni[0].systemId;
					this.sel_amministrazioni.Value = m_idAmministrazione;
					pnl_ddlAmm.Visible = false;
					this.lbl_Amm.Text = amministrazioni[0].descrizione;
					this.lbl_Amm.Visible = true;
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + userid.ID + "').focus() </SCRIPT>";
					RegisterStartupScript("focus", s);
				}
                else if (amministrazioni.Length > 1 && m_idAmministrazione.Equals("-1"))
                {
                    this.ddl_Amministrazioni.Items.Add(new ListItem("", ""));
                    for (int i = 0; i < amministrazioni.Length; i++)
                        this.ddl_Amministrazioni.Items.Add(new ListItem(amministrazioni[i].descrizione, amministrazioni[i].systemId));
                    pnl_ddlAmm.Visible = true;
                    this.lbl_Amm.Visible = false;
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + ddl_Amministrazioni.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);

                }
                else 
                {
                    //focus
                    string s = "<SCRIPT language='javascript'>document.getElementById('" + userid.ID + "').focus() </SCRIPT>";
                    RegisterStartupScript("focus", s);
                }
			}
			return true;
		}

		private void caricaLogoAmministrazione()
		{
            //if (m_idAmministrazione != "" && m_idAmministrazione != "-1") 
            //{
            //    /*const string LOGO_BASE_NAME="LogoAmm";
            //    const string LOGO_EXT=".gif";
            //    const string LOGO_FOLDER="images/loghiAmministrazioni";
            //    string newUrlLogo=LOGO_FOLDER+"/"+LOGO_BASE_NAME+"_"+m_idAmministrazione+LOGO_EXT;*/
            //    string imgPath = "./images/loghiAmministrazioni/LogoAmm_"+m_idAmministrazione+".gif";
            //    //controllo se esiste l'immagine dell'amministrazione
            //    if (System.IO.File.Exists(Server.MapPath(imgPath)))
            //    {
            //        this.img_logologinente2.ImageUrl = imgPath;
            //        this.img_logologinente2.Visible = true;
            //    }
            //    else
            //        this.img_logologinente2.Visible = true;
            //} 
            //else 
            //{
            //    this.img_logologinente2.Visible = true;
            //}

		}

        private void caricaLoghi()
        {
                if (fileExist("logo.gif", "LoginFE"))
                    this.img_logologin.ImageUrl = "images/loghiAmministrazioni/logo.gif";
                else
                    this.img_logologin.ImageUrl = "images/logo.gif";

                if (fileExist("logoenteloginFE1.gif", "LoginFE"))
                    this.img_logologinente1.ImageUrl = "images/loghiAmministrazioni/logoenteloginFE1.gif";
                else
                    this.img_logologinente1.Visible = false;

                if (fileExist("logoenteloginFE2.gif", "LoginFE"))
                    this.img_logologinente2.ImageUrl = "images/loghiAmministrazioni/logoenteloginFE2.gif";
                else
                    this.img_logologinente2.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="passwordConfirm"></param>
        /// <param name="failDetails"></param>
        /// <returns></returns>
        protected virtual bool SetNewPassword(string userID, string pwd, string pwdConfirm, 
                              out string failDetails)
        {
            bool retValue = false;
            failDetails = string.Empty;

            // Verifica della validità della nuova password immessa
            retValue = (pwd.Trim().Length > 0 && pwdConfirm.Trim().Length > 0);

            if (!retValue)
            {
                failDetails = "Immettere la nuova password";
            }
            else if (string.Compare(pwd, pwdConfirm, false) != 0)
            {
                failDetails = "I valori immessi nei campi password e conferma password non coincidono";
                retValue = false;
            }

            if (retValue)
            {
                // Inserimento nuova password
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                DocsPaWR.ValidationResultInfo result = ws.UserChangePassword(this.CreateUserLogin(), this.OldPassword);

                retValue = (result.Value);

                if (!retValue)
                {
                    failDetails = result.BrokenRules[0].Description;
                }
            }

            return retValue;
        }
		
		private void btn_login_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            try
			{
                bool continueLogon = true;

                // Verifica se la password è stata impostata come scaduta
                if (this.OnPasswordExpired)
                {
                    string failDetails;
                    if (this.SetNewPassword(this.userid.Text, this.password.Text, this.passwordConfirm.Text, out failDetails))
                    {
                        this.OnPasswordExpired = false;
                    }
                    else
                    {
                        continueLogon = false;

                        this.lbl_error.Text = failDetails;
                        this.lbl_error.Visible = true;
                        this.pnl_error.Visible = true;
                    }
                }

                if (continueLogon)
                {
                    if (this.ddl_Amministrazioni.SelectedItem != null)
                    {
                        m_idAmministrazione = this.ddl_Amministrazioni.SelectedItem.Value;
                    }

                    this.sel_amministrazioni.Value = m_idAmministrazione;

                    string message = string.Empty;

                    if (!loginAction(this.CreateUserLogin(), out message))
                    {
                        if (message.Equals("Selezionare un'amministrazione"))
                            ViewState["pwdMultiAmm"] = this.password.Text;

                        this.lbl_error.Text = message;
                        this.lbl_error.Visible = true;
                        this.pnl_error.Visible = true;
                    }
                    else
                        this.logged = true;
                }
			}
			catch(Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception.ToString());

				this.lbl_error.Text ="Errore di connessione con il Server";
				this.lbl_error.Visible=true;
				this.pnl_error.Visible=true;
                //focus
                string s = "<SCRIPT language='javascript'>document.getElementById('" + userid.ID + "').focus() </SCRIPT>";
                RegisterStartupScript("focus", s);
			}
        }

        /// <summary>
        /// Creazione oggetto UserLogin con le credenziali per l'accesso a docspa
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.UserLogin CreateUserLogin()
        {
            return this.CreateUserLogin(this.userid.Text, this.password.Text);
        }

        /// <summary>
        /// Creazione oggetto UserLogin con le credenziali per l'accesso a docspa
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>        
        private DocsPaWR.UserLogin CreateUserLogin(string userId, string password)
        {
            return CreateUserLogin(userId, password, this.Session.SessionID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private DocsPaWR.UserLogin CreateUserLogin(string userId, string password, string sessionId)
        {
            //if (string.IsNullOrEmpty(password) && Session["multiPwd"] != null)
            if (string.IsNullOrEmpty(password) && ViewState["pwdMultiAmm"] != null)
            {
                password = ViewState["pwdMultiAmm"].ToString(); 
                ViewState.Remove("pwdMultiAmm");
            }
            DocsPaWR.UserLogin userLogin = new DocsPAWA.DocsPaWR.UserLogin();
            userLogin.UserName = userId;
            userLogin.Password = password;
            userLogin.IdAmministrazione = this.m_idAmministrazione;
            userLogin.IPAddress = this.Request.UserHostAddress;
            userLogin.SessionId = sessionId;
            
            DocsPaWR.BrowserInfo bra = new DocsPaWR.BrowserInfo();
            bra.activex = Request.Browser.ActiveXControls.ToString();
            bra.browserType = Request.Browser.Browser;
            bra.browserVersion = Request.Browser.Version;
            string clientIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(clientIP))
                clientIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            bra.ip = clientIP;
            bra.javaApplet = Request.Browser.JavaApplets.ToString();
            bra.javascript = Request.Browser.JavaScript.ToString();
            bra.userAgent = Request.UserAgent;
            
            userLogin.BrowserInfo = bra; 

            return userLogin;
        }

        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }

        #region Gestione scadenza password

        /// <summary>
        /// Verifica che sia stata immessa la nuova password
        /// </summary>
        /// <returns></returns>
        private bool CheckNewPasswordValidity()
        {
            bool valid = false;

            if (this.OnPasswordExpired)
            {
                valid = (this.password.Text.Trim().Length > 0 && this.passwordConfirm.Text.Trim().Length > 0);

                if (valid)
                    valid = (string.Compare(this.password.Text, this.passwordConfirm.Text, false) == 0);
            }
            else
                valid = true;

            return valid;
        }

        /// <summary>
        /// Solo se la password risulta scaduta, 
        /// mantiene la vecchia password per i postback successivi
        /// </summary>
        protected string OldPassword
        {
            get
            {
                if (this.ViewState["OldPassword"] != null)
                    return this.ViewState["OldPassword"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["OldPassword"] = value;
            }
        }

        /// <summary>
        /// Flag, indica che la password immessa dall'utente
        /// è scaduta; è necessario gestire l'inserimento della nuova password
        /// </summary>
        private bool OnPasswordExpired
        {
            get
            {
                if (this.ViewState["OnPasswordExpired"] != null)
                    return (Convert.ToBoolean(this.ViewState["OnPasswordExpired"]));
                else
                    return false;
            }
            set
            {
                this.ViewState["OnPasswordExpired"] = value;
            }
        }

        #endregion

        #region CAS Integration (Single Sign On)

        protected System.Web.UI.WebControls.Panel pnlCasAuthentication;
        protected System.Web.UI.WebControls.Label lblCasAuthenticationMessage;

        /// <summary>
        /// Reperimento dell'url dei servizi di SSO
        /// </summary>
        /// <returns></returns>
        protected string GetCASServiceUrl()
        {
            return ConfigurationManager.AppSettings["SSOURL"];
        }
        
        protected bool GetShibSSOEnabled()
        {
            bool retval = false;
            if (Request.Headers["ShibSpoofCheck"] == null)
                return false;

            
            if ((Request.Headers["Shib-Application-ID"] != null) || (Request.Headers["ShibApplicationID"] != null))
                retval = true;
            else
                retval = false;
            
            if ((Request.Headers["Shib-Session-ID"] != null)||(Request.Headers["ShibSessionID"] != null))
                retval = true;
            else
                retval = false;

            if ((Request.Headers["Shib-Identity-Provider"] != null)||(Request.Headers["ShibIdentityProvider"] != null))
                retval = true;
            else
                retval = false;

            if (retval)
            {
                string cfgVal = ConfigurationManager.AppSettings["SHIBSSO"];
                retval = false;

                Boolean.TryParse(cfgVal, out retval);
            }
            return retval;
        }
        /// <summary>
        /// Verifica se è attiva l'integrazione CAS
        /// </summary>
        protected bool IsActiveCAS
        {
            get
            {
                return (!string.IsNullOrEmpty(this.GetCASServiceUrl()));
            }
        }

        /// <summary>
        /// Login tramite servizi CAS
        /// </summary>
        protected void LoginCAS()
        {
            //DocsPAWA.Logger.log("INIT - LoginCAS");

            string CASHOST = this.GetCASServiceUrl();
            string tkt = Request.QueryString["ticket"];
            string service = Request.Url.GetLeftPart(UriPartial.Path);

            //DocsPAWA.Logger.log(string.Format("CASHOST: {0}; tkt: {1}; service: {2};", CASHOST, tkt, service));

            if (tkt == null || tkt.Length == 0)
            {
                this.Session["SSO_ReturnURL"] = service;
                string redir = (CASHOST + "login?" + "service=") + service;

                //DocsPAWA.Logger.log(string.Format("RedirectTo: {0};", redir));

                Response.Redirect(redir);
                return;
            }

            string returnUrl = Session["SSO_ReturnUrl"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Session["SSO_ReturnUrl"] = "";
                string redir = Request.Url.GetLeftPart(UriPartial.Query) + "&ReturnUrl=" + returnUrl;

                //DocsPAWA.Logger.log(string.Format("RedirectTo: {0};", redir));

                Response.Redirect(redir);
            }

            string validateUrl = CASHOST + "serviceValidate?" + "ticket=" + tkt + "&" + "service=" + service;

            //DocsPAWA.Logger.log(string.Format("validateUrl: {0};", validateUrl));

            StreamReader RespRead = new StreamReader(new WebClient().OpenRead(validateUrl));
            string Resp = RespRead.ReadToEnd();
            NameTable NT = new NameTable();
            XmlNamespaceManager NSMgr = new XmlNamespaceManager(NT);
            XmlParserContext Context = new XmlParserContext(null, NSMgr, null, XmlSpace.None);
            XmlTextReader ReadXML = new XmlTextReader(Resp, XmlNodeType.Element, Context);

            string netId = null;
            string sessionName = null;

            while (ReadXML.Read())
            {
                if (ReadXML.IsStartElement())
                {
                    string Tag = ReadXML.LocalName;
                    if (Tag == "user")
                    {
                        netId = ReadXML.ReadString();
                    }
                    if (Tag == "name")
                    {
                        sessionName = ReadXML.ReadString();
                    }
                    if (Tag == "value")
                    {
                        this.Session["SSO_" + sessionName] = ReadXML.ReadString();
                    }
                }
            }

            ReadXML.Close();

            if (string.IsNullOrEmpty(netId))
            {
                this.lblCasAuthenticationMessage.Text = "Si e' verificato un problema nell'autenticazione con il Single Sign On. Chiudere il browser e riprovare. Se il problema persiste, contattatare l'amministratore.";
            }
            else
            {
                //DocsPAWA.Logger.log(string.Format("netId: {0}; sessionName: {1}", netId, sessionName));

                netId = netId.Substring(1);

                // Generazione del token di autenticazione
                string token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(netId);

                DocsPaWR.UserLogin login = this.CreateUserLogin(netId, token, token);

                // Login utente al sistema documentale
                if (!this.ForcedLogin(login))
                {
                    // Errore di autenticazione al sistema
                    this.lblCasAuthenticationMessage.Text = "Errore nell'autenticazione al sistema documentale: utente non riconosciuto";
                }
            }

            //DocsPAWA.Logger.log("END - LoginCAS");
        }

        /// <summary>
        /// Login tramite servizi CAS
        /// </summary>
        protected bool LoginShibboleth()
        {

            string netId = string.Empty;
            netId = Request.Headers["idada"];
            // modifica unitn - shibboleth
            string adaroles = string.Empty;
            adaroles = Request.Headers["adaroles"];

            logger.DebugFormat("netID {0}, adaroles {1}", netId, adaroles);

            if (string.IsNullOrEmpty(netId) || (string.IsNullOrEmpty(adaroles) || !adaroles.ToUpper().Contains("ADAP3USER(6873)")))
            {
                logger.Debug("Si e' verificato un problema nell'autenticazione con il Single Sign On. Chiudere il browser e riprovare. Se il problema persiste, contattatare l'amministratore.");
                this.lblCasAuthenticationMessage.Text = "Si e' verificato un problema nell'autenticazione con il Single Sign On. Chiudere il browser e riprovare. Se il problema persiste, contattatare l'amministratore.";
                return false;
            }
            else
            {
                //DocsPAWA.Logger.log(string.Format("netId: {0}; sessionName: {1}", netId, sessionName));

               

                // Generazione del token di autenticazione
                string token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(netId);

                DocsPaWR.UserLogin login = this.CreateUserLogin(netId, token, token);
                login.SSOLogin = true;
              
                // Login utente al sistema documentale
                if (!this.ForcedLogin(login))
                {
                    // Errore di autenticazione al sistema
                    this.lblCasAuthenticationMessage.Text = "Errore nell'autenticazione al sistema documentale: utente non riconosciuto";
                    return false;
                }
                return true;
            }

            //DocsPAWA.Logger.log("END - LoginCAS");
        }

        #endregion
    }
}
