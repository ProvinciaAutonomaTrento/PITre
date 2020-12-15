using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ConservazioneWA.Utils;
using Debugger = ConservazioneWA.Utils.Debugger;

namespace ConservazioneWA
{
    public partial class Login : ConservazioneWA.CssPage
    {
        bool logged = false;

        //
        // MEV CS 1.4 - Esibizione
        string ProfiloUtente = string.Empty;
        // End MEV CS 1.4 - Esibizione
        //

        DocsPaWR.Utente user;
        DocsPaWR.Ruolo ruolo;
        
        bool isAuthorized = true;
        
        protected System.Web.UI.WebControls.Image img_logologinente2;
        protected System.Web.UI.WebControls.Image img_logologinente1;
        protected System.Web.UI.WebControls.Image img_logologin;


        protected string GenerateAuthenticationToken(string userId)
        {
            return DocsPaUtils.Security.SSOAuthTokenHelper.Generate(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetInputToken()
        {
            if (!string.IsNullOrEmpty(this.Request.QueryString["token"]))
            {
                string token = this.Request.QueryString["token"];
                //Andrea 
                // Problema codifica-decodifica del +; all'atto dell'encode metto __ al posto di +
                token = token.Replace("__", "+");
                //End Andrea
                if (!string.IsNullOrEmpty(token))
                {
                    // Decryption del token
                    DocsPaUtils.Security.CryptoString cripto = new DocsPaUtils.Security.CryptoString("PGU");

                    string encryptedToken = cripto.Decrypt(token);

                    encryptedToken = encryptedToken.Replace("\0", "");

                    return encryptedToken;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
                return string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string tema = this.Page.Theme;
            if (tema.ToUpper().Equals("TEMAROSSO"))
            {
                this.img_logologinente1.Visible = false;
                this.img_logologinente2.Visible = false;
            }
            else if (tema.ToUpper().Equals("TEMAMILANO"))
            {
                this.img_logologinente1.Visible = false;
                this.img_logologinente2.Visible = false;
                this.img_logologin.Width = 232;
            }
            this.btn_accedi.ImageUrl = "App_Themes\\" + this.Page.Theme + "\\Butt_Accedi.jpg";

            string inputToken = this.GetInputToken();
            
            //
            // Modifica Per PGU
            // il Token Passato dal PGU
            string idAmm = string.Empty;

            //
            // MEV CS 1.4 - Esibizione
            if(!string.IsNullOrEmpty(inputToken))
                Session["PGU"] = "PGU";
            // End MEV CS 1.4 - Esibizione
            //

            //
            // Provengo dal PGU
            if (inputToken.Contains('_'))
            {
                idAmm = inputToken.Split('_')[1];
                inputToken = inputToken.Split('_')[0];
            }
            // End Modifica
            //

            // 
            // Mev Cs 1.4 - esibizione
            if (this.pnl_profili.Visible && !string.IsNullOrEmpty(this.hf_profiloUtente.Value))
            {
                if (ddl_profili.SelectedValue.Equals("0"))
                    Session["ProfiloUtente"] = "CONSERVAZIONE";
                if (ddl_profili.SelectedValue.Equals("1"))
                    Session["ProfiloUtente"] = "ESIBIZIONE";
            }
            // End Mev cs 1.4
            //


            if (!string.IsNullOrEmpty(inputToken) || this.hd_forceLogin.Value == "true")
            {
                string pwd = this.hd_pwd.Value;
                string userId = string.Empty;

                if (string.IsNullOrEmpty(inputToken))
                    userId = this.hd_userId.Value;
                else
                    userId = inputToken;
                
                DocsPaWR.UserLogin lgn = this.CreateUserLogin(pwd, userId);

                //
                // Modifica per PGU
                if (!string.IsNullOrEmpty(idAmm))
                    lgn.IdAmministrazione = idAmm;
                // End Modifica
                //

                if (this.ForcedLogin(lgn))
                {
                    user = (DocsPaWR.Utente)Session["userData"];

                    Session["DbType"] = ConservazioneManager.getDbType();

                    this.LaunchApplication();
                }
                else
                {
                    this.lbl_error.Visible = true;
                    this.lbl_error.Text = "Nome utente o password non validi.";
                }
            }

        }

        private bool ForcedLogin(DocsPaWR.UserLogin lgn)
        {
            bool result = false;

            DocsPaWR.LoginResult loginResult;
            DocsPaWR.Utente utente = UserManager.ForcedLogin(this, lgn, out  loginResult);

            if (loginResult == DocsPaWR.LoginResult.OK)
            {
                //PROVA SIGALOT per selezionare l'utente del centro servizi
                /*
                utente.extApplications = new DocsPaWR.ExtApplication[1];
                utente.extApplications[0] = new DocsPaWR.ExtApplication();
                utente.extApplications[0].codice = "CS";
                */

                if(utente != null)
                //MODIFICATO PER GESTIONE APPLICAZIONE CENTRO SERVIZI            
                utente.codWorkingApplication = "CS";

                result = true; // L'utente e' stato connesso
               // utente.urlWA = Utils.getHttpFullPath(this);
                Session["userData"] = utente;
            }

            return result;
        }

        protected void btn_accedi_Click(object sender, ImageClickEventArgs e)
        {
            if (string.IsNullOrEmpty(this.hd_forceLogin.Value))
            {
                if (string.IsNullOrEmpty(this.hd_ddlRuolicaricato.Value))
                {
                    try
                    {
                        string message = string.Empty;

                        if (!loginAction(this.CreateUserLogin(this.txt_pass.Text, this.txt_userId.Text), out message))
                        {
                            this.lbl_error.Text = message;
                            this.lbl_error.Visible = true;
                        }
                    }
                    catch (Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine(exception.ToString());

                        this.lbl_error.Text = "Errore di connessione con il Server";
                        this.lbl_error.Visible = true;
                        //focus
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_userId.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);

                        Debugger.Write("Errore nella login: " + exception.Message);
                    }
                }
                else
                {
                    string dbType = ConservazioneManager.getDbType();
                    if (dbType != string.Empty)
                    {
                        Session["DbType"] = dbType;
                        this.LaunchApplication();
                    }
                    else
                    {
                        this.lbl_error.Text = "Errore di comunicazione con il server";
                        this.lbl_error.Visible = true;
                    }
                }
            }
            else
            {
                if (isAuthorized)
                {
                    string dbType = ConservazioneManager.getDbType();
                    if (dbType != string.Empty)
                    {
                        Session["DbType"] = dbType;
                        this.LaunchApplication();
                    }
                    else
                    {
                        this.lbl_error.Text = "Errore di comunicazione con il server";
                        this.lbl_error.Visible = true;
                    }
                }
                else
                {
                    this.lbl_error.Text = "Utente non abilitato";
                    this.lbl_error.Visible = true;
                }

            }

            //
            // Adeguamento login Centro servizi dopo mev multi amministrazione
            if (this.hf_loginResult.Value.Equals("NO_AMMIN"))
            {
                //
                // Carico le amministrazioni buone per l'user id inserito.
                DocsPaWR.Amministrazione[] amministrazioni = null;
                string returnMsg = string.Empty;
                amministrazioni = UserManager.getListaAmministrazioniByUser(this, this.txt_userId.Text, true, out returnMsg);

                if (amministrazioni != null && amministrazioni.Length > 0) 
                {
                    
                    for (int i = 0; i < amministrazioni.Length; i++) 
                    {
                        if (!ddl_Amministrazioni.Items.Contains(new ListItem(amministrazioni[i].descrizione, amministrazioni[i].systemId)))
                            this.ddl_Amministrazioni.Items.Add(new ListItem(amministrazioni[i].descrizione, amministrazioni[i].systemId));
                    }

                    //
                    // La lista delle amministrazioni viene resa visibile
                    this.pnl_ddlAmm.Visible = true;
                    this.ddl_Amministrazioni.Visible = true;
                }
                else
                    this.lbl_error.Text = "Nessuna amministrazione disponibile";

            }
            //
            // End Adeguamento login Centro servizi dopo mev multi amministrazione
        }

        private DocsPaWR.UserLogin CreateUserLogin(string pwd, string userId)
        {
            DocsPaWR.UserLogin userLogin = new DocsPaWR.UserLogin();
            userLogin.UserName = userId;

            if (!string.IsNullOrEmpty(this.Request.QueryString["token"]))
            {
                //
                // Modifica per PGU
                // Dal PGU il token  viene passato a partire dal UserId_idAmm

                //Old CODE
                //userLogin.Password = this.GenerateAuthenticationToken(this.GetInputToken());
                
                string usrID = GetInputToken();
                if (usrID.Contains('_'))
                    usrID = usrID.Split('_')[0];
                userLogin.Password = this.GenerateAuthenticationToken(usrID);
                // End PGU
                //
            }
            else
                userLogin.Password = pwd;

            userLogin.IdAmministrazione = "";
            userLogin.IPAddress = this.Request.UserHostAddress;
            // Abilitazione dell'utente al modulo centro servizi
            userLogin.Modulo = DocsPaUtils.Moduli.ListaModuli.CENTRO_SERVIZI;
            this.hd_pwd.Value = pwd;
            this.hd_userId.Value = userId;

            //
            // Popolo valori per multiAmm
            if (this.pnl_ddlAmm.Visible && this.ddl_Amministrazioni.Visible) 
            {
                userLogin.IdAmministrazione = this.ddl_Amministrazioni.SelectedValue;
                if (this.hd_userId != null)
                    userLogin.UserName = this.hd_userId.Value;
                if (this.hd_pwd != null)
                    userLogin.Password = this.hd_pwd.Value;
            }
            //
            // End multiAmm

            return userLogin;
        }

        private void LaunchApplication()
        {
            if (this.user != null)
            {
                if (this.user.ruoli != null && this.user.ruoli.Length > 0)
                {
                    this.ruolo = (DocsPaWR.Ruolo)this.user.ruoli[0];
                }
                else
                {
                    DocsPaWR.Ruolo ruoloFinto = new DocsPaWR.Ruolo();
                    ruoloFinto.idAmministrazione = this.user.idAmministrazione;
                    ruoloFinto.idGruppo = "-1";
                    ruoloFinto.systemId = "-1";
                    this.ruolo = ruoloFinto;
                }
            }

            //
            // Old Code - MEV CS 1.4 - Esibizione
            //creaInfoUtente(this.user);
            //ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
            // End OldCode
            
            // New Code - MEV cs 1.4 - esibizione
            WSConservazioneLocale.InfoUtente infoUt = creaInfoUtente(this.user);

            if (infoUt != null) 
            {
                

                if (!string.IsNullOrEmpty(infoUt.idAmministrazione)
                    && !string.IsNullOrEmpty(infoUt.idPeople)
                    )
                {

                    // Lancio applicazione esibizione/conservazione
                    if (string.IsNullOrEmpty(ProfiloUtente))
                        ProfiloUtente = ConservazioneManager.CalcolaProfiloUtente(infoUt.idPeople, infoUt.idAmministrazione);

                    // Se provengo da PGU il Profilo è quello di conservazione
                    if(Session["PGU"] != null && Session["PGU"].Equals("PGU"))
                    {
                        ProfiloUtente = "CONSERVAZIONE";
                    }

                    if (!string.IsNullOrEmpty(ProfiloUtente))
                    {
                        switch (ProfiloUtente.ToUpper())
                        {
                            case "ESIBIZIONE":
                                Session["GestioneEsibizione"] = "ESIBIZIONE"; //per la pagina di gestione esibizione
                                ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('Esibizione/HomePageEsibizione.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                                break;
                            case "CONSERVAZIONE":
                                Session["GestioneEsibizione"] = "CONSERVAZIONE"; //per la pagina di gestione esibizione
                                ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                                break;
                            case "CONSERVAZIONE_ESIBIZIONE":
                                // Valore in sessione, che se popolato in pageLoad gestisce la combo per l'accesso al giusto modulo
                                // Rendo visibile il Panel della scelta del profilo CONSERVAZIONE / ESIBIZIONE
                                {
                                    this.pnl_profili.Visible = true;
                                    this.ddl_profili.Visible = true;

                                    this.lbl_error.Text = "Selezionare un profilo di accesso";
                                    this.lbl_error.Visible = true;

                                    // La combo profili è visibile 
                                    this.hf_profiloUtente.Value = ProfiloUtente;

                                    if (Session["ProfiloUtente"] != null && Session["ProfiloUtente"].Equals("ESIBIZIONE")) 
                                    {
                                        // Pulisco la sessione
                                        this.hf_profiloUtente.Value = Session["ProfiloUtente"].ToString();
                                        Session["ProfiloUtente"] = "";
                                        this.lbl_error.Text = string.Empty;
                                        this.lbl_error.Visible = false;
                                        Session["GestioneEsibizione"] = "ESIBIZIONE"; //per la pagina di gestione esibizione
                                        ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('Esibizione/HomePageEsibizione.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                                    }

                                    if (Session["ProfiloUtente"] != null && Session["ProfiloUtente"].Equals("CONSERVAZIONE")) 
                                    {
                                        this.hf_profiloUtente.Value = Session["ProfiloUtente"].ToString();
                                        Session["ProfiloUtente"] = "";
                                        this.lbl_error.Text = string.Empty;
                                        this.lbl_error.Visible = false;
                                        Session["GestioneEsibizione"] = "CONSERVAZIONE"; //per la pagina di gestione esibizione
                                        ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                                        
                                    }
                                    break;
                                }
                            default:
                                ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                                break;
                        }
                    }

                    //if(!string.IsNullOrEmpty(ProfiloUtente) && ProfiloUtente.ToUpper().Equals("ESIBIZIONE"))
                    //    ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('../Esibizione/HomePageEsibizione.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                    //else
                    //    ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                }
                else 
                {
                    // Lancio applicazione Conservazione
                    ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
                }
                
            }
            else
                // Default, Conservazione
                ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
            //
            // End Mev cs 1.4 - esibizione

           // Response.Write("<script>var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}</script>");
            //ClientScript.RegisterStartupScript(this.GetType(), "openApplicazione", "var w=window.open('HomePageNew.aspx','HomePage','scrollbars=yes,location=0,resizable=yes');w.moveTo(0,0);w.resizeTo(screen.availWidth,screen.availHeight);if(w!=self){window.opener=null;self.close();}", true);
        }

        private WSConservazioneLocale.InfoUtente creaInfoUtente(DocsPaWR.Utente utente)
        {
            WSConservazioneLocale.InfoUtente infoUtente = new ConservazioneWA.WSConservazioneLocale.InfoUtente();

            infoUtente.idPeople = utente.idPeople;

            if (this.ruolo != null)
            {
                infoUtente.idCorrGlobali = ruolo.systemId;
                infoUtente.idGruppo = ruolo.idGruppo;
            }
            
            infoUtente.dst = utente.dst;
            infoUtente.idAmministrazione = utente.idAmministrazione;
            infoUtente.userId = utente.userId;
            infoUtente.sede = utente.sede;

            if (utente != null && utente.urlWA != null)
                infoUtente.urlWA = utente.urlWA;
            //if (utente.extApplications != null && utente.extApplications.Length > 0)
            //{
            //    infoUtente.extApplications = new WSConservazioneLocale.ExtApplication[1];
            //    infoUtente.extApplications[0] = new WSConservazioneLocale.ExtApplication();
            //    infoUtente.extApplications[0].codice = utente.extApplications[0].codice;
            //}


            //MODIFICATO PER GESTIONE APPLICAZIONE CENTRO SERVIZI
            if (utente != null && utente.codWorkingApplication !=null)
                infoUtente.codWorkingApplication = utente.codWorkingApplication;

            Session.Add("infoutCons", infoUtente);

            // Modifica scrittura Registro di Conservazione e Log per il login in Conservazione
            if (infoUtente.codWorkingApplication == "CS")
            {

                // Inserisce nel DPA_LOG il login di Conservazione
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "LOGIN_CONSERVAZIONE", infoUtente.idAmministrazione, "Login al sistema di conservazione dell'utente userid " + utente.userId, WSConservazioneLocale.Esito.OK);

            }

            return infoUtente;
        }

        private bool loginAction(DocsPaWR.UserLogin lgn, out string message)
        {
            bool resLogin = false;
            message = string.Empty;
            string ipaddress = "";

            DocsPaWR.LoginResult loginResult;
            DocsPaWR.Utente utente = UserManager.login(this, lgn, out loginResult, out ipaddress);
            this.user = utente;
          if (user != null)
            {
                 //PROVA SIGALOT per selezionare l'utente del centro servizi
              /*
                user.extApplications = new DocsPaWR.ExtApplication[1];
                user.extApplications[0] = new DocsPaWR.ExtApplication();
                user.extApplications[0].codice = "CS";
               */

                //MODIFICATO PER GESTIONE APPLICAZIONE CENTRO SERVIZI
                user.codWorkingApplication = "CS";
            }
 
            switch (loginResult)
            {
                case DocsPaWR.LoginResult.OK:

                    //PROVA SIGALOT per selezionare l'utente del centro servizi
                    //user.extApplications = new DocsPaWR.ExtApplication[1];
                    //user.extApplications[0] = new DocsPaWR.ExtApplication();
                    //user.extApplications[0].codice = "CS";

                    resLogin = true;
                    
                    Session["DbType"] = ConservazioneManager.getDbType();
                    
                    this.LaunchApplication();


                    break;

                case DocsPaWR.LoginResult.UNKNOWN_USER:
                    message = "Nome utente o password non validi.";

                    break;

                case DocsPaWR.LoginResult.USER_ALREADY_LOGGED_IN:
                        // Gestione autonoma da parte dell'utente
                        message = "L'utente ha gia' una connessione aperta.";

                        //        // Store login object
                        //        Session.Add("loginData", lgn);
                        string script = "<script>forceLogin('" + ipaddress + "');</script>";
                        Page.RegisterStartupScript("NotifyUser", script);

                //    }

                    break;

                case DocsPaWR.LoginResult.NO_RUOLI:
                    message = "L'utente non ha un ruolo associato.";
                    break;


                case DocsPaWR.LoginResult.DISABLED_USER:
                    message = "Utente non abilitato";
                    break;

                case DocsPaWR.LoginResult.PASSWORD_EXPIRED:
                    message = "La password dell'utente risulta scaduta. Immettere una nuova password.";
                    break;

                case DocsPaWR.LoginResult.NO_AMMIN:
                    this.hf_loginResult.Value = DocsPaWR.LoginResult.NO_AMMIN.ToString();
                    message = "Selezionare un'amministrazione";
                    break;

                default:
                    // Application Error
                    message = "Errore nella procedura di Login. Contattare l'amministrazione.";
                    break;
            }

            if (resLogin)
            {
                //utente.urlWA = Utils.getHttpFullPath(this);
                Session["userData"] = utente;
            }

            return resLogin;
        }

    }
}
