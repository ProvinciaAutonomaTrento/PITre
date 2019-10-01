using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Security.Principal;
using NttDataWA.UIManager;

namespace NttDataWA
{
    public partial class Login : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(Login));
        private string sessionend = string.Empty;
        private string userid = string.Empty;
        private string tempPass = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.Form.Get("portale")) || (Request.Form.Get("portale").ToUpper() == "FALSE"))
            {
                string qs = Request.QueryString.ToString();
                debug.InnerHtml = "qs=" + qs;
                //Link da Word per accedere al documento 
                if (Session["directlinkOffice"] == null && ((!string.IsNullOrEmpty(Request.QueryString["idProfile"])
                    && !string.IsNullOrEmpty(Request.QueryString["groupId"])) || (!string.IsNullOrEmpty(Request.QueryString["idObj"]))))
                {
                    string strFrom = Request.QueryString["from"];
                    string docNumber = Request.QueryString["docNumber"];
                    string idProfile = Request.QueryString["idProfile"];
                    string groupId = Request.QueryString["groupId"];
                    string numVersion = string.IsNullOrEmpty(Request.QueryString["numVersion"]) ? "1" : Request.QueryString["numVersion"];

                    string idAmministrazione = Request.QueryString["idAmministrazione"];
                    string idObj = Request.QueryString["idObj"];
                    string tipoProto = Request.QueryString["tipoProto"];
                    if ((!string.IsNullOrEmpty(idProfile) && !string.IsNullOrEmpty(numVersion)) || (!string.IsNullOrEmpty(Request.QueryString["idObj"])))
                        Session["directlinkOffice"] = Utils.utils.getHttpFullPath() + "/CheckInOut/OpenDirectLink.aspx?from=" + strFrom + "&idProfile=" + idProfile + "&groupId=" + groupId + "&numVersion=" + numVersion + "&idAmministrazione=" + idAmministrazione + "&idObj=" + idObj + "&tipoProto=" + tipoProto;
                }

                if (!IsPostBack)
                {
                    sessionend = Request.QueryString["sessionend"];

                    ResetSession();
                    InitializesPage();

                    if (EnableBrowserControl() && !BrowserControl())
                    {
                        BrowserNotCompatible();
                        return;
                    }

                    string msgLogin = UIManager.AdministrationManager.getLoginMessage();
                    if (!string.IsNullOrWhiteSpace(msgLogin))
                    {
                        msgLoginText.Text = msgLogin;
                    }
                    else
                    {
                        msgLoginDiv.Visible = false;
                    }


                    if (GetShibSSOEnabled())
                    {
                        try
                        {
                            LoginShibboleth();
                        }
                        catch (Exception shibEx)
                        {
                            logger.Debug("Errore in LoginShibboleth: " + shibEx.Message);
                            Response.Redirect("login.aspx?lang=" + SelectedLanguage);
                        }
                    }
                    else if (GetAutFedEnabled())
                    {
                        try
                        {
                            LoginAutenticazioneFederata();
                        }
                        catch (Exception shibEx)
                        {
                            logger.Debug("Errore in LoginAutenticazioneFederata: " + shibEx.Message);
                            Response.Redirect("login.aspx?lang=" + SelectedLanguage);
                        }
                    }
                    else
                    {
                        if (UIManager.LoginManager.IsActiveCAS())
                        {
                            LoginCAS();
                        }
                        else
                        {
                            //Integration IAM-GFD
                            string policyAgent = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.POLICY_AGENT_ENABLED.ToString()];
                            //Windows Authentication
                            string windowsAuthentication = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.AUTENTICAZIONE_WINDOWS.ToString()];
                            if (!string.IsNullOrEmpty(windowsAuthentication) || !string.IsNullOrEmpty(policyAgent))
                            {
                                if ((windowsAuthentication.ToUpper() == Boolean.TrueString.ToUpper()) || (policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                                    LogInLinkAccessibleVersion.Visible = false;
                            }

                            if ((!string.IsNullOrEmpty(policyAgent) && policyAgent.ToUpper() == Boolean.TrueString.ToUpper()))
                            {

                                ShowHideInput(false);
                                vic.Integra VIC = new vic.Integra(Context, "");
                                TxtUserId.Text = VIC.amData.account.ToString();

                                if (ddl_Amministrazioni.SelectedItem != null)
                                {
                                    M_idAmministrazione = ddl_Amministrazioni.SelectedItem.Value;
                                }

                                string message = string.Empty;
                                DocsPaWR.UserLogin userLogin = CreateUserLogin(TxtUserId.Text, TxtPassword.Text);
                                userLogin.Token = VIC.amData.account.ToString() + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();

                                string appo = VIC.amData.account.ToString() + "&" + System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.CHIAVE_TOKEN.ToString()] + '&' + VIC.amData.codFiscale.ToString() + '&' + VIC.amData.matricola.ToString();
                                byte[] bt_datiInput = ASCIIEncoding.ASCII.GetBytes(appo);
                                userLogin.Token = userLogin.Token + '&' + Utils.CryptographyManager.CalculatesFingerprint(bt_datiInput);
                                if (String.IsNullOrEmpty(VIC.amData.account.ToString()) || String.IsNullOrEmpty(VIC.amData.codFiscale.ToString()) || String.IsNullOrEmpty(VIC.amData.matricola.ToString()))
                                {
                                    LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInUnauthorizedUser", SelectedLanguage);
                                    LblError.Visible = true;
                                }
                                else
                                {
                                    if (!LoginAction(userLogin, out message))
                                    {
                                        LblError.Text = message;
                                        LblError.Visible = true;
                                    }
                                }
                            }
                            else
                            {
                                if ((!string.IsNullOrEmpty(windowsAuthentication) && windowsAuthentication.ToUpper() == Boolean.TrueString.ToUpper()))
                                {
                                    WindowsPrincipal windows_user = (HttpContext.Current.User as WindowsPrincipal);
                                    string userDomain = windows_user.Identity.Name;
#if DEBUG
                                    userDomain = "valueteam\\user_test02";
#endif
                                    if (string.IsNullOrEmpty(userDomain))
                                        throw new Exception("Autenticazione windows non impostata");

                                    var dati_user = userDomain.Split('\\');
                                    var dominio = dati_user[0].ToString().ToUpper();
                                    var userN = dati_user[1].ToString().ToUpper();
                                    string tipo_auth = windows_user.Identity.AuthenticationType;
                                    string codice = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.CHIAVE_TOKEN.ToString()];

                                    ShowHideInput(false);
                                    TxtUserId.Text = userN;
                                    string message = string.Empty;
                                    DocsPaWR.UserLogin userLogin = CreateUserLogin(TxtUserId.Text, TxtPassword.Text);

                                    byte[] bt_datiInput = ASCIIEncoding.ASCII.GetBytes(userN + "&" + codice + "&" + dominio + "&" + tipo_auth);
                                    userLogin.Token = userN + "&" + dominio + "&" + tipo_auth + "&" + Utils.CryptographyManager.CalculatesFingerprint(bt_datiInput);

                                    if (String.IsNullOrEmpty(userN) || String.IsNullOrEmpty(dominio))
                                    {
                                        LblError.Text = Utils.Languages.GetMessageFromCode("ErrorLogInUserNotAuthorized", SelectedLanguage);
                                        LblError.Visible = true;
                                    }
                                    else
                                    {
                                        if (!LoginAction(userLogin, out message))
                                        {
                                            LblError.Text = message;
                                            LblError.Visible = true;
                                        }
                                    }
                                }
                            }
                            //End Integration IAM-GFD
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(HiForcerLogin.Value))
                    {
                        ForcedLogin(UserLoginForced);
                    }

                    RefreshScript();
                }
            }
            else
            {

            }
        }

        private bool EnableBrowserControl()
        {
            bool retVal = true;
            if ((!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.NO_BROWSER_CONTROL.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.NO_BROWSER_CONTROL.ToString()].ToUpper() == "1"))
            {
                retVal = false;
            }
            return retVal;
        }

        private bool BrowserControl()
        {
            bool retVal = true;

            if (Request.Browser.Browser.Trim().ToUpperInvariant().Equals("IE") && Request.Browser.MajorVersion <= 7)
            {
                retVal = false;
            }
            return retVal;
        }

        protected void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "refreshSelect", "refreshSelect();", true);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            string layout = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LAYOUT.ToString()];

            if (layout != null && layout.ToUpper().Equals("GFD"))
                img_logo_login.ImageUrl = "Images/Common/logo_login_gfd.jpg";
            else
                img_logo_login.ImageUrl = "Images/Common/logo_login.jpg";
        }

        private void ShowHideInput(bool boolean)
        {
            LogInLblUserId.Visible = boolean;
            TxtUserId.Visible = boolean;
            LogInLblPassword.Visible = boolean;
            TxtPassword.Visible = boolean;
            LoginBtnLogin.Visible = boolean;
            LogInLblConfirmPassword.Visible = boolean;
            TxtConfirmPassword.Visible = boolean;
        }

        /// <summary>
        /// Force Login
        /// </summary>
        /// <param name="lgn">UserLogin</param>
        /// <returns>bool</returns>
        private bool ForcedLogin(DocsPaWR.UserLogin lgn)
        {
            bool result = false;

            DocsPaWR.LoginResult loginResult;
            DocsPaWR.Utente user = UIManager.LoginManager.ForcedLogin(this, lgn, out loginResult);

            if (loginResult == DocsPaWR.LoginResult.OK)
            {
                userid = user.idPeople;
                result = true;
                UIManager.UserManager.SetUserInSession(user);
                UIManager.RoleManager.SetRoleInSession(user.ruoli[0]);
                UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(user.ruoli[0].systemId, "1", ""));
                UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(user.ruoli[0].systemId, "", ""));
                UIManager.UserManager.SetUserLanguage(SelectedLanguage);
                UIManager.DocumentManager.GetLettereProtocolli();
                LaunchApplication();
            }

            return result;
        }

        /// <summary>
        /// Redirect after correct Login
        /// </summary>
        protected void LaunchApplication()
        {
            if (Session["directLink"] != null || Session["directlinkOffice"]!=null)
            {
                string link = string.Empty;
                if (Session["directLink"] != null)
                {
                    link = Session["directLink"].ToString();
                }
                else
                {
                    link = Session["directlinkOffice"].ToString();
                    Session.Remove("directlinkOffice");
                }
                ScriptManager.RegisterStartupScript(Page, GetType(), "redirect", "sessionend('" + userid + "'); location.href='" + link + "';", true);
            }
            else
            {
                HttpContext.Current.Session["IsFirstTime"] = null;
                ScriptManager.RegisterStartupScript(Page, GetType(), "redirect", "sessionend('" + userid + "'); location.href='index.aspx';", true);
            }
            //ScriptManager.RegisterStartupScript(Page, GetType(), "redirect", "location.href='index.aspx';", true);
        }

        protected void BrowserNotCompatible()
        {
            Response.Redirect("BrowserNoCompatible.aspx?lang=" + SelectedLanguage);

        }

        /// <summary>
        /// If single sign on with Shibbolet is active return true
        /// </summary>
        private bool GetShibSSOEnabled()
        {
            bool retval = false;
            if (Request.Headers["ShibSpoofCheck"] == null)
                return false;

            if ((Request.Headers["Shib-Application-ID"] != null) || (Request.Headers["ShibApplicationID"] != null))
                retval = true;
            else
                retval = false;

            if ((Request.Headers["Shib-Session-ID"] != null) || (Request.Headers["ShibSessionID"] != null))
                retval = true;
            else
                retval = false;

            if ((Request.Headers["Shib-Identity-Provider"] != null) || (Request.Headers["ShibIdentityProvider"] != null))
                retval = true;
            else
                retval = false;

            if (retval)
            {
                string cfgVal = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.SHIBSSO.ToString()];
                retval = false;

                Boolean.TryParse(cfgVal, out retval);
            }

            return retval;
        }

        /// <summary>
        /// If single sign on with Shibbolet is active return true
        /// </summary>
        private bool GetAutFedEnabled()
        {
            bool retval = false;
            logger.Debug("Controllo autenticazione federata");
            
            if ((Request.Headers["Shib-Application-ID"] != null) || (Request.Headers["ShibApplicationID"] != null))
                retval = true;
            else
                retval = false;

            if ((Request.Headers["Shib-Session-ID"] != null) || (Request.Headers["ShibSessionID"] != null))
                retval = true;
            else
                retval = false;

            if ((Request.Headers["Shib-Identity-Provider"] != null) || (Request.Headers["ShibIdentityProvider"] != null))
                retval = true;
            else
                retval = false;

            if (retval)
            {
                logger.Debug("Header shibboleth presenti, controllo chiave");
                string cfgVal = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.AUTENTICAZIONE_FEDERATA_SSO.ToString()];
                logger.Debug("chiave " + cfgVal);
                retval = false;

                Boolean.TryParse(cfgVal, out retval);
            }

            return retval;
        }

        /// <summary>
        /// Login with services CAS
        /// </summary>
        private bool LoginShibboleth()
        {
            string netId = string.Empty;
            netId = Request.Headers["idada"];
            // Modifica: nascondo il form quando mi connetto da shibboleth
            LogInLblUserId.Visible = false;
            TxtUserId.Visible = false;
            LogInLblPassword.Visible = false;
            TxtPassword.Visible = false;
            LoginBtnLogin.Visible = false;

            // modifica unitn - shibboleth
            string adaroles = string.Empty;
            adaroles = Request.Headers["adaroles"];

            logger.DebugFormat("netID {0}, adaroles {1}", netId, adaroles);

            if (string.IsNullOrEmpty(netId))
            {
                logger.Debug("Si e' verificato un problema nell'autenticazione con il Single Sign On. Chiudere il browser e riprovare. Se il problema persiste, contattatare l'amministratore.");
                LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInSingleSignOn", SelectedLanguage);
                LblError.Visible = true;
                return false;
            }
            else if (string.IsNullOrEmpty(adaroles) || !adaroles.ToUpper().Contains("ADAP3USER(6873)"))
            {
                logger.Debug("L'utente non ha un ruolo abilitato all'utilizzo dell'applicazione.  Per informazioni rispetto all’accesso a PiTre contatti l’amministratore del sistema all’indirizzo uff.protocollocentrale@unitn.it");
                LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInSingleSignOnRole", SelectedLanguage);
                LblError.Visible = true;
                return false;
            }
            else
            {
                string token = Utils.SSOAuthTokenHelper.Generate(netId, SelectedLanguage);

                DocsPaWR.UserLogin login = CreateUserLogin(netId, token);
                login.SSOLogin = true;

                if (!ForcedLogin(login))
                {
                    // Authentication error                   
                    LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInUnknowUser", SelectedLanguage);
                    LblError.Visible = true;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Login with Autenticazione Federata
        /// </summary>
        private bool LoginAutenticazioneFederata()
        {
            string netId = string.Empty;
            netId = Request.Headers["upn"];
            if (!string.IsNullOrEmpty("netId: "+netId))
            {
                logger.Debug(netId);
                netId = netId.Substring(0, netId.IndexOf('@'));
            }
            LogInLblUserId.Visible = false;
            TxtUserId.Visible = false;
            LogInLblPassword.Visible = false;
            TxtPassword.Visible = false;
            LoginBtnLogin.Visible = false;

            logger.DebugFormat("UserId {0}", netId);

            if (string.IsNullOrEmpty(netId))
            {
                logger.Debug("Si e' verificato un problema nell'autenticazione con il Single Sign On. Chiudere il browser e riprovare. Se il problema persiste, contattatare l'amministratore.");
                LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInSingleSignOn", SelectedLanguage);
                LblError.Visible = true;
                return false;
            }
            else
            {
                string token = Utils.SSOAuthTokenHelper.Generate(netId, SelectedLanguage);

                DocsPaWR.UserLogin login = CreateUserLogin(netId, token);
                login.SSOLogin = true;

                if (!ForcedLogin(login))
                {
                    // Authentication error                   
                    LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInUnknowUser", SelectedLanguage);
                    LblError.Visible = true;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Initializes page
        /// </summary>
        private void InitializesPage()
        {
            UIManager.LoginManager.IniInitializesLanguages();
            if (UIManager.LoginManager.IsEnableMultiLanguages())
            {
                MenuLanguages.Visible = true;
                LoadLanguagesMenu();
            }
            else
            {
                InitializesSingleLanguages();
            }

            InitializesLabel();


            if (Request.QueryString["MessageType"] != null)
                GestLivelloChiusuraPopUp();
        }

        private void ResetSession()
        {
            M_idAmministrazione = string.Empty;
            UserLoginForced = null;
            SelectedLanguage = string.Empty;
            OnPasswordExpired = false;
            OldPassword = string.Empty;
        }

        /// <summary>
        /// Multilanguages no active
        /// </summary>
        private void InitializesSingleLanguages()
        {
            List<string> multiLanguages = UIManager.LoginManager.GetAvailableLanguages();
            foreach (string lang in multiLanguages)
            {
                SelectedLanguage = lang;
                break;
            }
        }

        /// <summary>
        /// Load MultiLanguages menu
        /// </summary>
        private void LoadLanguagesMenu()
        {
            List<string> multiLanguages = UIManager.LoginManager.GetAvailableLanguages();
            foreach (string lang in multiLanguages)
            {
                MenuItem menu = new MenuItem();
                menu.ImageUrl = "images/Languages/" + lang + ".jpg";
                menu.ToolTip = UIManager.LoginManager.GetDescriptionFromCode(lang);
                menu.Text = string.Empty; ;
                menu.Value = lang;
                MenuLanguages.Items.Add(menu);
                ListItem a = new ListItem();
            }
        }

        /// <summary>
        /// Initializes application labels
        /// </summary>
        private void InitializesLabel()
        {
            if (sessionend != null && sessionend == "1") LblError.Text = UIManager.LoginManager.GetLabelFromCode("LoginSessionLost", string.Empty);

            LoginBtnLogin.Text = UIManager.LoginManager.GetLabelFromCode("LoginBtnLogin", string.Empty);
            LogInLinkAccessibleVersion.Text = UIManager.LoginManager.GetLabelFromCode("LogInLinkAccessibleVersion", string.Empty);
            LogInLinkAccessibleVersion.ToolTip = LogInLinkAccessibleVersion.Text;
            LogInLblUserId.Text = UIManager.LoginManager.GetLabelFromCode("LogInTxtUserId", string.Empty);
            LogInLblPassword.Text = UIManager.LoginManager.GetLabelFromCode("LogInTxtPassword", string.Empty);
            Loading.Text = UIManager.LoginManager.GetLabelFromCode("Loading", string.Empty);
            LogInLblConfirmPassword.Text = UIManager.LoginManager.GetLabelFromCode("LogInLblConfirmPassword", string.Empty);
            string languageDirection = UIManager.LoginManager.GetLanguageDirectionFromCode(string.Empty);
            ddl_Amministrazioni.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("LoginAdministration", string.Empty));
            Html.Attributes.Add("dir", languageDirection);
            SetCssClass(languageDirection);

        }

        /// <summary>
        /// Set right or left Css
        /// </summary>
        /// <param name="languageDirection"></param>
        private void SetCssClass(string languageDirection)
        {
            string link = "~/Css/Left/Login.css";
            if (!string.IsNullOrEmpty(languageDirection) && languageDirection.Equals("rtl"))
            {
                link = "~/Css/Right/Login.css";
            }
            CssLayout.Attributes.Add("href", link);
        }

        /// <summary>
        /// Login with CAS Service
        /// </summary>
        private void LoginCAS()
        {
            string CASHOST = UIManager.LoginManager.GetCASServiceUrl();
            string tkt = Request.QueryString["ticket"];
            string service = Request.Url.GetLeftPart(UriPartial.Path);
            if (tkt == null || tkt.Length == 0)
            {
                Session["SSO_ReturnURL"] = service;
                string redir = (CASHOST + "login?" + "service=") + service;
                Response.Redirect(redir);
                return;
            }
            string returnUrl = Session["SSO_ReturnUrl"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Session["SSO_ReturnUrl"] = "";
                string redir = Request.Url.GetLeftPart(UriPartial.Query) + "&ReturnUrl=" + returnUrl;
                Response.Redirect(redir);
            }
            string validateUrl = CASHOST + "serviceValidate?" + "ticket=" + tkt + "&" + "service=" + service;
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
                        Session["SSO_" + sessionName] = ReadXML.ReadString();
                    }
                }
            }
            ReadXML.Close();

            if (string.IsNullOrEmpty(netId))
            {
                LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInSingleSignOn", SelectedLanguage);
                LblError.Visible = false;
            }
            else
            {
                netId = netId.Substring(1);
                string token = Utils.SSOAuthTokenHelper.Generate(netId, SelectedLanguage);
                DocsPaWR.UserLogin login = CreateUserLogin(netId, token);
                if (!ForcedLogin(login))
                {
                    LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInUnknowUser", SelectedLanguage);
                    LblError.Visible = false;
                }
            }
        }

        /// <summary>
        /// On Login click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            UserManager.setDelegato(null);
            UserManager.setUtenteDelegato(this, null);
            Session["ESERCITADELEGA"] = null;

            LblError.Visible = false;

            bool continueLogon = true;


            if (string.IsNullOrEmpty(TxtUserId.Text) || string.IsNullOrEmpty(TxtPassword.Text))
            {
                LblError.Visible = true;
                LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInNoData", SelectedLanguage);
            }
            else
            {
                this.tempPass = TxtPassword.Text;
                string failDetails = string.Empty;
                string message = string.Empty;
                try
                {
                    if (OnPasswordExpired)
                    {
                        if (SetNewPassword(TxtUserId.Text, TxtPassword.Text, TxtConfirmPassword.Text, out failDetails))
                        {
                            OnPasswordExpired = false;
                        }
                        else
                        {
                            continueLogon = false;
                            LblError.Visible = true;
                            LblError.Text = failDetails;
                        }
                    }

                    if (continueLogon)
                    {
                        if (ddl_Amministrazioni.SelectedItem != null)
                        {
                            M_idAmministrazione = ddl_Amministrazioni.SelectedItem.Value;
                        }

                        message = string.Empty;

                        if (!LoginAction(CreateUserLogin(TxtUserId.Text, TxtPassword.Text), out message))
                        {
                            LblError.Visible = true;
                            LblError.Text = message;
                        }
                    }
                }
                catch (Exception exception)
                {
                    LblError.Text = UIManager.LoginManager.GetMessageFromCode("ErrorLogInConnection", SelectedLanguage);
                    logger.Debug("LoginAction error - ", exception);
                }
            }
        }

        /// <summary>
        /// On change language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MenuLanguages_Click(object sender, EventArgs e)
        {
            LblError.Visible = false;
            SelectedLanguage = MenuLanguages.SelectedValue;
            ChangeLanguage();
        }

        private void ChangeLanguage()
        {
            LoginBtnLogin.Text = UIManager.LoginManager.GetLabelFromCode("LoginBtnLogin", SelectedLanguage);
            LoginBtnLogin.ToolTip = LoginBtnLogin.Text;
            LogInLinkAccessibleVersion.Text = UIManager.LoginManager.GetLabelFromCode("LogInLinkAccessibleVersion", SelectedLanguage);
            LogInLinkAccessibleVersion.ToolTip = UIManager.LoginManager.GetLabelFromCode("LogInLinkAccessibleVersion", SelectedLanguage);
            LogInLblUserId.Text = UIManager.LoginManager.GetLabelFromCode("LogInTxtUserId", SelectedLanguage);
            LogInLblPassword.Text = UIManager.LoginManager.GetLabelFromCode("LogInTxtPassword", SelectedLanguage);
            Loading.Text = UIManager.LoginManager.GetLabelFromCode("Loading", SelectedLanguage);
            LogInLblConfirmPassword.Text = UIManager.LoginManager.GetLabelFromCode("LogInLblConfirmPassword", SelectedLanguage);
            ddl_Amministrazioni.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("LoginAdministration", SelectedLanguage));
            GetLanguageDirectionFromCode();
            UpPnlLogin.Update();
        }

        private void GetLanguageDirectionFromCode()
        {
            string result = UIManager.LoginManager.GetLanguageDirectionFromCode(SelectedLanguage);
            ScriptManager.RegisterStartupScript(Page, GetType(), "changeLanguage", "changeLanguageDirection('" + result + "');", true);
        }

        /// <summary>
        /// Set New Password 
        /// </summary>
        /// <param name="userID">string</param>
        /// <param name="password">string</param>
        /// <param name="passwordConfirm">pwdConfirm</param>
        /// <param name="failDetails">failDetails</param>
        /// <returns>bool</returns>
        private bool SetNewPassword(string userID, string pwd, string pwdConfirm, out string failDetails)
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
                DocsPaWR.ValidationResultInfo result = UIManager.LoginManager.UserChangePassword(CreateUserLogin(TxtUserId.Text, TxtPassword.Text), OldPassword);
                if (!result.Value)
                {
                    failDetails = result.BrokenRules[0].Description;
                    for (int i = 1; result.BrokenRules.Count() > i; i++)
                    {
                        failDetails += " - " + result.BrokenRules[i].Description;
                    }
                    retValue = false;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Create Object UserLogin
        /// </summary>
        /// <param name="userId">string</param>
        /// <param name="password">string</param>
        /// <returns>UserLogin</returns>
        private DocsPaWR.UserLogin CreateUserLogin(string userId, string password)
        {
            DocsPaWR.UserLogin userLogin = new DocsPaWR.UserLogin();
            userLogin.UserName = userId;
            userLogin.Password = password;
            userLogin.IdAmministrazione = M_idAmministrazione;
            userLogin.IPAddress = Request.UserHostAddress;
            userLogin.SessionId = Session.SessionID;
            createBrowserInfo(userLogin);
            return userLogin;
        }

        /// <summary>
        /// Execute Login
        /// </summary>
        /// <param name="lgn">UserLogin</param>
        /// <param name="message">string</param>
        /// <returns>bool</returns>
        private bool LoginAction(DocsPaWR.UserLogin lgn, out string message)
        {
            bool resLogin = false;
            message = string.Empty;
            string ipaddress = string.Empty;
            this.TxtPassword.Text = string.Empty;
            this.TxtPassword.Attributes.Add("value", string.Empty);

            DocsPaWR.LoginResult loginResult;
            DocsPaWR.Utente utente = UIManager.LoginManager.Login(this, lgn, out loginResult, out ipaddress);

            switch (loginResult)
            {
                case DocsPaWR.LoginResult.OK:
                    userid = utente.idPeople;
                    resLogin = true;
                    LaunchApplication();
                    break;

                case DocsPaWR.LoginResult.UNKNOWN_USER:
                    if ((!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.POLICY_AGENT_ENABLED.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.POLICY_AGENT_ENABLED.ToString()].ToUpper() == Boolean.TrueString.ToUpper()))
                        message = Utils.Languages.GetMessageFromCode("ErrorLogInUserNotAuthorized", SelectedLanguage);
                    else
                        message = Utils.Languages.GetMessageFromCode("ErrorLogInUserPassword", SelectedLanguage);
                    break;

                case DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
                    string loginMode = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.ADMINISTERED_LOGIN_MODE.ToString()];

                    //Login with administration tool
                    if (!string.IsNullOrEmpty(loginMode) && loginMode.ToUpper() == Boolean.TrueString.ToUpper())
                    {
                        message = Utils.Languages.GetMessageFromCode("ErrorLogInAlreadyConnection", SelectedLanguage);
                    }
                    else
                    {
                        // Login with user
                        message = Utils.Languages.GetMessageFromCode("ErrorLogInAlreadyConnection", SelectedLanguage);
                        // Store login object
                        Session.Add("loginData", lgn);
                        bool autoForce = false;

                        // Gabriele Melini 10-11-2014
                        // sposto la chiave AUTO_FORCE_LOGIN nel DB
                        //if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.AUTO_FORCE_LOGIN.ToString()]))
                        if(!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_AUTO_FORCE_LOGIN.ToString())))
                        {
                            //Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.AUTO_FORCE_LOGIN.ToString()], out autoForce);
                            autoForce = Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_AUTO_FORCE_LOGIN.ToString()).Equals("1") ? true : false;
                        }
                        if (autoForce)
                        {
                            ForcedLogin(lgn);
                            // INC000000472586 
                            // se è attiva la chiave AUTO_FORCE_LOGIN non deve essere visualizzato il messaggio relativo alla connessione aperta
                            message = string.Empty;
                        }
                        else
                        {
                            UserLoginForced = lgn;
                            ScriptManager.RegisterStartupScript(Page, GetType(), "force", "forceLogin('" + ipaddress + "','" + Utils.Languages.GetMessageFromCode("WarningLogInForceStart", SelectedLanguage) + "','" + Utils.Languages.GetMessageFromCode("WarningLogInForceEnd", SelectedLanguage) + "');", true);
                        }
                    }
                    break;

                case DocsPaWR.LoginResult.DISABLED_USER:
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInUserDisabled", SelectedLanguage);
                    break;

                case DocsPaWR.LoginResult.NO_RUOLI:
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInUserNoRoles", SelectedLanguage);
                    break;

                case DocsPaWR.LoginResult.NO_AMMIN:
                    caricaComboAmministrazioni();
                    UpPnlDllAdmin.Update();
                    message = "Selezionare un'amministrazione";

                    if (!string.IsNullOrEmpty(this.tempPass))
                    {
                        this.TxtPassword.Text = this.tempPass;
                        this.TxtPassword.Attributes.Add("value", this.tempPass);
                    }
                    break;

                case DocsPaWR.LoginResult.PASSWORD_EXPIRED:
                    OnPasswordExpired = true;
                    OldPassword = lgn.Password;
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInChangePassword", SelectedLanguage);
                    if (string.IsNullOrEmpty(M_idAmministrazione) || M_idAmministrazione == "0")
                    {
                        string returnMsg = "";
                        DocsPaWR.Amministrazione[] amministrazioni = UserManager.getListaAmministrazioniByUser(this, TxtUserId.Text, ddl_Amministrazioni.Visible, out returnMsg);
                        if (amministrazioni.Length == 1)
                        {
                            M_idAmministrazione = amministrazioni[0].systemId;
                        }
                    }
                    SetChangePassword();
                    break;

                case DocsPaWR.LoginResult.DTCM_SERVICE_NO_CONTACT:
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInDocumentum", SelectedLanguage);
                    break;

                case DocsPaWR.LoginResult.UNKNOWN_AMMIN:
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInUnknownAdministration", SelectedLanguage);
                    break;

                case DocsPaWR.LoginResult.UNKNOWN_DTCM_USER:
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInDocumentumNoUser", SelectedLanguage);
                    break;

                case DocsPaWR.LoginResult.DB_ERROR:
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInDBConnection", SelectedLanguage);
                    break;

                default:
                    // Application Error
                    message = Utils.Languages.GetMessageFromCode("ErrorLogInGeneric", SelectedLanguage);
                    break;
            }

            if (resLogin)
            {
                UIManager.UserManager.SetUserInSession(utente);
                UIManager.RoleManager.SetRoleInSession(utente.ruoli[0]);
                UIManager.RegistryManager.SetRFListInSession(UIManager.UserManager.getListaRegistriWithRF(utente.ruoli[0].systemId,"1",""));
                UIManager.RegistryManager.SetRegAndRFListInSession(UIManager.UserManager.getListaRegistriWithRF(utente.ruoli[0].systemId, "", ""));
                UIManager.UserManager.SetUserLanguage(SelectedLanguage);
                UIManager.DocumentManager.GetLettereProtocolli();
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
            ddl_Amministrazioni.Items.Clear();
            //MEV utente multi-amministrazione
            if (string.IsNullOrEmpty(TxtUserId.Text))
                amministrazioni = UserManager.getListaAmministrazioni(this, out returnMsg);
            else
                amministrazioni = UserManager.getListaAmministrazioniByUser(this, TxtUserId.Text, ddl_Amministrazioni.Visible, out returnMsg);

            if (!returnMsg.Equals(string.Empty))
            {
                LblError.Text = "Impossibile contattare il Server";
                return false;
            }

            if (amministrazioni == null || amministrazioni.Length == 0)
            {
                LblError.Text = "Nessuna amministrazione disponibile";
                return false;
            }
            else
            {
                if (amministrazioni.Length == 1)
                {
                    M_idAmministrazione = amministrazioni[0].systemId;
                    pnl_ddlAmm.Visible = false;
                }
                else if (amministrazioni.Length > 1)
                {
                    ddl_Amministrazioni.Items.Add(new ListItem("", ""));
                    for (int i = 0; i < amministrazioni.Length; i++)
                        ddl_Amministrazioni.Items.Add(new ListItem(amministrazioni[i].descrizione, amministrazioni[i].systemId));
                    pnl_ddlAmm.Visible = true;
                }
            }
            return true;
        }


        private void SetChangePassword()
        {
            LogInLblConfirmPassword.Visible = true;
            TxtConfirmPassword.Visible = true;
            LogInLblPassword.Text = UIManager.LoginManager.GetLabelFromCode("LogInTxtPasswordNew", SelectedLanguage);
        }

        private void createBrowserInfo(DocsPaWR.UserLogin userLogin)
        {
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

        }

        private void GestLivelloChiusuraPopUp()
        {
            string MessageType = Request.QueryString["MessageType"];

            LblError.Visible = true;
            switch (MessageType)
            {
                case "1":
                    LblError.Text = "La sessione è scaduta.";
                    break;
                case "2":
                    LblError.Text = "L'utente si è connesso da un'altra postazione.";
                    break;
                default:
                    LblError.Text = "Si è verificato un problema.";
                    break;
            }

            if (Convert.ToString(Request.QueryString["param"]).Equals("StepLogin"))
            {
                Context.Response.Write("<script type='text/javascript'>window.open('Login.aspx', '_parent');</script>");
            }

            if (!string.IsNullOrEmpty(Request.QueryString["param"]))
            {
                switch (Convert.ToString(Request.QueryString["param"]))
                {
                    case "Step1":
                        Context.Response.Write("<script type='text/javascript'>window.open('Login.aspx?param=Step2&MessageType=" + MessageType + "', '_parent');</script>");
                        break;
                    case "Step2":
                        Context.Response.Write("<script type='text/javascript'>window.open('Login.aspx?param=StepStop&MessageType=" + MessageType + "', '_parent');</script>");
                        break;
                }
            }
            else
            {
                Context.Response.Write("<script type='text/javascript'>window.open('Login.aspx?param=Step1&MessageType=" + MessageType + "', '_parent');</script>");
            }
        }

        #region Page properties

        /// <summary>
        /// Old password
        /// </summary>
        protected string OldPassword
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["oldPassword"] != null)
                {
                    result = HttpContext.Current.Session["oldPassword"] as string;
                }

                return result;
            }
            set
            {
                HttpContext.Current.Session["oldPassword"] = value;
            }
        }

        /// <summary>
        /// True if password expired
        /// </summary>
        protected bool OnPasswordExpired
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["onPasswordExpired"] != null)
                {
                    Boolean.TryParse(HttpContext.Current.Session["onPasswordExpired"].ToString(), out result);
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["onPasswordExpired"] = value;
            }
        }

        /// <summary>
        /// Selected language
        /// </summary>
        protected string SelectedLanguage
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["selectedLanguage"] != null)
                {
                    result = HttpContext.Current.Session["selectedLanguage"] as string;
                }

                return result;
            }
            set
            {
                HttpContext.Current.Session["selectedLanguage"] = value;
            }
        }

        /// <summary>
        /// Selected language
        /// </summary>
        protected DocsPaWR.UserLogin UserLoginForced
        {
            get
            {
                DocsPaWR.UserLogin result = null;
                if (HttpContext.Current.Session["userLoginForced"] != null)
                {
                    result = HttpContext.Current.Session["userLoginForced"] as DocsPaWR.UserLogin;
                }

                return result;
            }
            set
            {
                HttpContext.Current.Session["userLoginForced"] = value;
            }
        }

        protected string M_idAmministrazione
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["m_idAmministrazione"] != null)
                {
                    result = HttpContext.Current.Session["m_idAmministrazione"] as string;
                }

                return result;
            }
            set
            {
                HttpContext.Current.Session["m_idAmministrazione"] = value;
            }
        }


        #endregion
    }
}
