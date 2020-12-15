using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using log4net;

namespace DocsPaDocumentale_ETDOCS.Documentale
{
	/// <summary>
	/// Classe per la gestione degli utenti tramite il documentale ETDoc
	/// </summary>
	public class UserManager : DocsPaDocumentale.Interfaces.IUserManager
	{
        private ILog logger = LogManager.GetLogger(typeof(UserManager));
//        private const string HASH_ALGORITHM = "System.Security.Cryptography.SHA1CryptoServiceProvider";

		#region Costruttori
		/// <summary>
		/// Inizializza l'istanza della classe acquisendo i dati relativi al login
		/// ed alla libreria per la connessione al documentale.
		/// </summary>
		/// <param name="login">Dati relativi al login</param>
		/// <param name="library">Libreria per la connessione al documentale</param>
		/// 

		public UserManager() {}

		#endregion
	
		#region Public methods
        public virtual bool Checkconnection()
        {
            bool retValue = false;

            //loginResult = UserLogin.LoginResult.APPLICATION_ERROR;
            System.Data.DataSet ds = null;
            try
            {


                DocsPaDB.Query_DocsPAWS.Documentale d = new DocsPaDB.Query_DocsPAWS.Documentale();
                d.ExecuteQuery(out ds, "SELECT SYSTEM_ID FROM DPA_AMMINISTRA");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0
                    && ds.Tables[0].Rows[0][0] != null)
                    retValue = true;
                else retValue = false;



            }
            catch (Exception ex)
            {
                //AuthenticationException - Exception in com.emc.documentum.fs.rt
                //Exception which is raised when authentication errors occur
                //  loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_DTCM_USER;
                retValue = false;

                logger.Error("errore in checkconnetcion" + ex.Message);
            }

            return retValue;
        }


        /// <summary>
        /// Effettua il login di un utente amministratore
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginAdminUser(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            bool retValue = false;
            utente = null;
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
            
            AdminPasswordConfig pwdConfig = new AdminPasswordConfig();

            DocsPaDB.Query_DocsPAWS.Amministrazione amministrazioneDb = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            // Verifica se è supportata la gestione delle configurazioni password (con supporto per encryption)
            if (pwdConfig.IsSupportedPasswordConfig())
            {
                // Verifica della validità delle credenziali utente
                if (DocsPaPwdServices.UserPasswordServices.IsValidForLogon(userLogin.UserName, userLogin.Password))
                {
                    // Reperimento metadati dell'utente amministratore (con gestione password criptate)
                    utente = amministrazioneDb.GetDatiAmministratoreEncrypted(userLogin.UserName, userLogin.Password);
                }
            }
            else
            {
                // Reperimento metadati dell'utente amministratore (senza gestione password criptate)
                utente = amministrazioneDb.GetDatiAmministratore(userLogin.UserName);
            }

            if (utente == null)
            {
                // Utente non riconosciuto
                loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;

                logger.Debug(string.Format("Utente {0} non riconosciuto", userLogin.UserName));
            }
            else
            {
                bool userAlreadyConnected;

                // Creazione token di autenticazione
                utente.dst = this.CreateUserToken();
                
                // Connessione come utente amministratore
                if (!amministrazioneDb.LoginAmministratore(utente, userLogin.IPAddress, userLogin.SessionId, forceLogin, out userAlreadyConnected))
                {
                    utente.dst = null;

                    if (userAlreadyConnected)
                    {
                        // Utente già connesso
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                    }
                    else
                    {
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                    }
                }
                else
                {
                    retValue = true;
                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        public bool LogoutUser(string dst)
        {
            DocsPaDB.Query_DocsPAWS.Utenti userDb = new DocsPaDB.Query_DocsPAWS.Utenti();
            
            return userDb.RemoveUserLoginLock(dst);
        }

        /// <summary>
        /// Effettua il login di un utente
        /// </summary>
        /// <param name="utente">Oggetto Utente connesso</param>
        /// <returns>True = OK; False = Si è verificato un errore</returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            bool result = true;
            utente = null;
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;

            try
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                string name = System.String.Empty;
                string password = System.String.Empty;
                int idAmm = 0;

                if (!string.IsNullOrEmpty(userLogin.UserName))
                    name = userLogin.UserName;
                if (!string.IsNullOrEmpty(userLogin.Password))
                    password = userLogin.Password;
                if (!string.IsNullOrEmpty(userLogin.IdAmministrazione))
                    idAmm = Convert.ToInt32(userLogin.IdAmministrazione);

                if (utenti.IsUtenteDisabled(userLogin.UserName, userLogin.Modulo, userLogin.IdAmministrazione))
                {
                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER;
                    result = false;
                    logger.Debug("Utente disabilitato");
                }

                //verifica userId su tabella utenti
                string peopleId = string.Empty;

                if (result && !utenti.UserLogin(out peopleId, name, idAmm.ToString(), userLogin.Modulo))
                {
                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                    result = false;
                    logger.Debug("Utente sconosciuto");
                }

                if (result && !string.IsNullOrEmpty(peopleId))
				{
                    //controlla se deve eseguire la login usando il 'token'
                    if (!string.IsNullOrEmpty(userLogin.Token))
                    {
                        //esegue l'autenticazione con token;
                        string message = string.Empty;

                        if (!Documentale.SSOLogin.loginWithToken(userLogin.Token, out message))
                        {
                            result = false;
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                            logger.Debug("Utente non autenticato con token. --" + message);
                        }
                    }
                    else
                    {
                        if (DocsPaUtils.Security.SSOAuthTokenHelper.IsAuthToken(userLogin.Password))
                        {
                            try
                            {
                                // Verifica la validità del token di autenticazione
                                string token = DocsPaUtils.Security.SSOAuthTokenHelper.Restore(userLogin.UserName, userLogin.Password);

                                // Il token è l'id della sessione utente:
                                // se nella dpa_login è già stata assegnata una sessione con quest'id, il token non è valido
                                result = true;
                            }
                            catch (Exception ex)
                            {
                                logger.Debug("Errore nell'autenticazione dell'utente tramite Token Single Sign On", ex);
                                loginResult = DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
                                result = false;
                            }
                        }
                        else
                        {
                            // Verifica se l'utente è configurato per connettersi ad un archivio LDAP
                            if (DocsPaLdapServices.LdapUserConfigurations.UserCanConnectToLdap(userLogin.UserName))
                            {
                                // Autenticazione utente in LDAP
                                DocsPaLdapServices.Core.BaseLdapUserServices ldapServices = DocsPaLdapServices.Core.LdapUserServicesFactory.GetConfiguredInstance(userLogin.IdAmministrazione);

                                // Reperimento del nome dell'utente corrispondente in LDAP
                                string userNameLdap = DocsPaLdapServices.LdapUserConfigurations.GetLdapUserConfigByName(userLogin.UserName).LdapIdSync;

                                if (!ldapServices.AuthenticateUser(userNameLdap, userLogin.Password))
                                {
                                    result = false;
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                                    logger.Debug("Utente non valido o non trovato");
                                }
                            }
                            else
                            {
                                string utenteDominio;

                                //verifica se deve bisogna eseguire l'autenticazione su dominio;
                                utenti.GetDominio(out utenteDominio, peopleId);

                                if (!string.IsNullOrEmpty(utenteDominio))
                                {
                                    string[] arr = this.separaDominio(utenteDominio);

                                    if (arr.GetLength(0) < 2)
                                    {
                                        result = false;
                                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
                                        logger.Debug("Firma dell'utente sul dominio non valida.");
                                    }

                                    string userName = arr[1];
                                    string dominio = arr[0];

                                    //esegue l'autenticazione su dominio;
                                    if (result && !this.loginOnDomain(dominio, userName, password))
                                    {
                                        result = false;
                                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                                        logger.Debug("Utente non autenticato sul dominio");
                                    }
                                }
                                else
                                {
                                    AdminPasswordConfig pwdConfig = new AdminPasswordConfig();
                                    if (pwdConfig.IsSupportedPasswordConfig())
                                    {
                                        // Nuova gestione password
                                        logger.Debug("inizio controllo se passowrd è valida");
                                        //MEV utenti multi-amministrazione - agginto parametro codice amministrazione
                                        if (!DocsPaPwdServices.UserPasswordServices.IsValidForLogon(name, password, userLogin.IdAmministrazione))
                                        {
                                            logger.Debug("la passowrd è differente da quella registrata.");
                                            result = false;
                                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                                            logger.Debug("Utente non valido o non trovato");
                                        }

                                        if (userLogin != null && userLogin.Modulo != null &&
                                           DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(userLogin.Modulo))
                                        {
                                            //Se accedo dal centro servizi con un utente abilitato solo al centro servizi        
                                        }
                                        else
                                        {

                                            // Verifica se è attiva la gestione delle scadenze password
                                            if (result && DocsPaPwdServices.UserPasswordServices.PasswordExpirationEnabled(Convert.ToInt32(userLogin.IdAmministrazione), name))
                                            {
                                                // Verifica se è presente la password predefinita assegnata dall'amministratore
                                                if (DocsPaPwdServices.UserPasswordServices.IsPasswordExpired(name,userLogin.IdAmministrazione))
                                                {
                                                    // Se la password è scaduta, richiede l'immissione di una nuova password
                                                    result = false;
                                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.PASSWORD_EXPIRED;
                                                    logger.Debug("La password definita per l'utente è scaduta");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Vecchia gestione password, se il documentale non la supporta
                                        //autenticazione completa su tabella utenti
                                        if (!utenti.UserLogin(out peopleId, name, password, idAmm.ToString()))
                                        {
                                            result = false;
                                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                                            logger.Debug("Utente non valido o non trovato");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (result)
                    {
                        // Reperimento metadati dell'utente
                        utente = utenti.GetUtente(name, userLogin.IdAmministrazione, userLogin.Modulo);
                       
                        // Associazione token di autenticazione
                        utente.dst = this.CreateUserToken();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella login.", ex);
                result = false;
                utente = null;
            }

            return result;
        }

        /// <summary>
        /// Reperimento del token di autenticazione per il superuser del documentale 
        /// </summary>
        /// <returns></returns>
        public string GetSuperUserAuthenticationToken()
        {
            return this.CreateUserToken();
        }

        #region autenticazione su dominio

        /// <summary>
        /// Autenticazione su Dominio. Metodo Valido solo per ETDOC
        /// </summary>
        /// <param name="utenteDominio">DOMINIO\USERNAME</param>
        /// <param name="password"></param>
        /// <returns>true o false</returns>
        internal bool loginOnDomain(string dominio, string utente, string password)
        {
            bool result = true;

            try
            {
                IntPtr token;
                WindowsIdentity wi = WindowsIdentity.GetCurrent();

                if (LogonUser(utente, dominio, password, 3, 0, out token))
                {
                    CloseHandle(token);
                }
                else
                {
                    int err = GetLastError();
                    if (err != 1326) // faccio il log solo se non si tratta di un errore di scrittura
                        logger.Debug("Errore di autenticazione sul dominio (utente: " + utente + " - err: " + err.ToString() + " ): ");
                    throw new System.Exception(err.ToString());

                }

            }
            catch (Exception ex)
            {
                result = false;
                logger.Debug("Errore di autenticazione sul dominio (utente: " + utente + "): ", ex);
            }

            return result;






        }



        internal string[] separaDominio(string utenteDominio)
        {

            string[] dominio = null;
            if (utenteDominio.Contains("\\"))
            {
                dominio = utenteDominio.Split('\\');
            }
            if (utenteDominio.Contains("@"))
            {
                dominio = new string[2];
                dominio[0] = "";
                dominio[1] = utenteDominio;
            }
            return dominio;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("Kernel32.dll")]
        internal static extern bool CloseHandle(IntPtr phToken);


        [DllImport("Kernel32.dll")]
        internal static extern int GetLastError();

        #endregion autenticazione su dominio

        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="newPassword"/></param>
        /// <param name="utente"></param>
        ///// <returns></returns>
        public DocsPaVO.Validations.ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            DocsPaVO.Validations.ValidationResultInfo result = null;

            AdminPasswordConfig pwdConfig = new AdminPasswordConfig();

            if (pwdConfig.IsSupportedPasswordConfig())
            {
                // Se è abilitata la gestione configurazioni password
                int idAmministrazione = 0;
                if (!string.IsNullOrEmpty(user.IdAmministrazione))
                    idAmministrazione = Convert.ToInt32(user.IdAmministrazione);

                //result = DocsPaPwdServices.UserPasswordServices.SetPassword(user.UserName, user.Password, false);
                result = DocsPaPwdServices.UserPasswordServices.SetPassword(user, false);
            }
            else
            {
                result = new DocsPaVO.Validations.ValidationResultInfo();

                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                if (!utenti.CambiaPassword(user, oldPassword))
                    result.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("ChangePassword_ERROR", "Errore nella modifica della password per il documentale ETDOCS", DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error));

                result.Value = (result.BrokenRules.Count == 0);
            }

            return result;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Creazione token di autenticazione utente per etdocs
        /// </summary>
        /// <returns></returns>
        protected virtual string CreateUserToken()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        #endregion
    }
}

