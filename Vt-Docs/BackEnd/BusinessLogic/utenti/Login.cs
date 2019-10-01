using System;
using System.Data;
using System.Collections;
using System.Xml;
using log4net;


namespace BusinessLogic.Utenti
{
	/// <summary>
	/// </summary>
	public class Login
	{
        private static ILog logger = LogManager.GetLogger(typeof(Login));
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objLogin"></param>
		/// <param name="loginResult"></param>
		/// <param name="forcedLogin">
		/// Imposta se la connessione deve essere forzata, ossia 
		/// una eventuale connessione esistente viene annullata).
		/// </param>
		/// <returns></returns>
		public static DocsPaVO.utente.Utente loginMethod(DocsPaVO.utente.UserLogin objLogin, 
			out DocsPaVO.utente.UserLogin.LoginResult loginResult, bool forcedLogin, 
			string webSessionId, out string ipaddress) 
		{
			DocsPaVO.utente.Utente utente = null;
			loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
			ipaddress = string.Empty;
            bool multiAmm = false;

            try
            {
                if (DocsPaUtils.Security.SSOAuthTokenHelper.IsAuthToken(objLogin.Password))
                {

                    string ssoTok = DocsPaUtils.Security.SSOAuthTokenHelper.Restore(objLogin.UserName, objLogin.Password);
                    objLogin.SSOLogin = true;

                }
            }
            catch { }

			try
			{
				// Ricerca dell'utente in amministrazione
                if (string.IsNullOrEmpty(objLogin.IdAmministrazione))
				{
                    try
                    {
                        ArrayList listaAmmin = UserManager.getListaIdAmmUtente(objLogin);

                        if (listaAmmin != null && listaAmmin.Count > 0)
                        {
                            if (listaAmmin.Count == 1)
                                objLogin.IdAmministrazione = listaAmmin[0].ToString();
                            else
                            {
                                //loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                                multiAmm = true;
                                objLogin.IdAmministrazione = listaAmmin[0].ToString();
                            }
                        }
                        if (listaAmmin == null) logger.Debug("Attenzione, la query S_People in GetIdAmmUtente non ha dato alcun risultato.");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug("Errore di connessione al DB durante la procedura di login");
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.DB_ERROR;
                    }
                }

                // Se l'amministrazione è stata impostata
                if (!string.IsNullOrEmpty(objLogin.IdAmministrazione))
                {
                    // Get User
                    DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();
                    
                    if (userManager.LoginUser(objLogin, out utente, out loginResult))
                    {
                        // Impostazione id sessione utente
                        utente.sessionID = webSessionId;

                        if (!forcedLogin) // Gestione delle connessioni esistenti da amministrazione
                        {
                            //login concesso all'utente
                            //si verifica la tabella DPA_LOGIN per unicità della connessione
                            //la funzione torna True se l'utente è già collegato
                            DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                            
                            if (!gestioneUtenti.CheckUserLogin(utente.userId, utente.idAmministrazione))
                            {
                                utente.ruoli = UserManager.getRuoliUtente(utente.idPeople);
                                utente.dominio = getDominio(utente.idPeople);

                                if (utente.ruoli.Count == 0 && DocsPaUtils.Moduli.ModuliAuthManager.RolesRequired(objLogin.Modulo))
                                {
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI;
                                    utente = null;
                                }
                                else
                                {
                                    gestioneUtenti.LockUserLogin(utente.userId, utente.idAmministrazione, webSessionId, objLogin.IPAddress, utente.dst);
                                    if(!multiAmm)
                                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                                    else
                                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                                }
                            }
                            else
                            {
                                if (!multiAmm)
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                                else
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_AMMIN;
                                //loginResult = DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                                ipaddress = gestioneUtenti.GetUserIPAddress(utente.userId, utente.idAmministrazione);
                                utente = null;
                            }
                        }
                        else
                        {
                            // Gestione utente delle connessioni esistenti  
                            DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                            // Cancella eventuali connessioni esistenti
                            gestioneUtenti.UnlockUserLogin(utente.userId, utente.idAmministrazione);

                            // Assegna connessione
                            gestioneUtenti.LockUserLogin(utente.userId, utente.idAmministrazione,
                                webSessionId, objLogin.IPAddress, utente.dst);
                            utente.ruoli = UserManager.getRuoliUtente(utente.idPeople);
                            utente.dominio = getDominio(utente.idPeople);
                            if (utente.ruoli.Count == 0 && DocsPaUtils.Moduli.ModuliAuthManager.RolesRequired(objLogin.Modulo))
                            {
                                loginResult = DocsPaVO.utente.UserLogin.LoginResult.NO_RUOLI;
                                utente = null;
                            }

                            // Qualifiche Utente
                            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "GESTIONE_QUALIFICHE");
                            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                            {
                                utente.qualifiche = new ArrayList(utenti.QualificheManager.GetPeopleGroupsQualificheByIdPeople(utente.idPeople));
                            }

                        }
                    }
                }
			} 
			catch (Exception e) 
			{
				logger.Debug(e.Message);
				loginResult = DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
				logger.Debug("Errore nella gestione degli utenti (loginMethod)");
				utente=null;
			}

			return utente;
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objLogin"></param>
		/// <param name="loginResult"></param>
		/// <param name="forcedLogin">
		/// Imposta se la connessione deve essere forzata, ossia 
		/// una eventuale connessione esistente viene annullata).
		/// </param>
		/// <returns></returns>
		public static DocsPaVO.utente.UserLogin.ValidationResult ValidateLogin(string userName, string idAmministrazione, string webSessionId) 
		{
			DocsPaVO.utente.UserLogin.ValidationResult result = DocsPaVO.utente.UserLogin.ValidationResult.OK;

			try 
			{
				DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
				result = gestioneUtenti.CheckUserLogin(userName, idAmministrazione, webSessionId);
			} 
			catch (Exception exception) 
			{
				logger.Debug("Errore durante la validazione della sessione web.", exception);
				result = DocsPaVO.utente.UserLogin.ValidationResult.APPLICATION_ERROR;
			}

			return result;
		}

		public static string ElencoUtentiConnessi(string codiceAmm)
		{
			string result=null;
			DataSet elencoUtenti=null;
			DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti=new DocsPaDB.Query_DocsPAWS.Utenti ();
			elencoUtenti=gestioneUtenti.GetUtentiConnessi(codiceAmm);
			if(elencoUtenti!=null)
			{
				XmlDocument xmlUtenti=new XmlDocument();

				XmlNode utenti=xmlUtenti.AppendChild (xmlUtenti.CreateElement("UTENTI"));
				
				foreach(DataRow utente in elencoUtenti.Tables[0].Rows )
				{
					XmlNode nodoUtente=utenti.AppendChild (xmlUtenti.CreateElement("UTENTE"));
					string userId=utente["USER_ID"].ToString().ToUpper();
					nodoUtente.AppendChild(xmlUtenti.CreateElement("USERID")).InnerText = userId;
					string nomeUtente=gestioneUtenti.GetNomeUtente(userId);
					nodoUtente.AppendChild(xmlUtenti.CreateElement("DESCRIZIONE")).InnerText = nomeUtente;
					nodoUtente.AppendChild(xmlUtenti.CreateElement("DATAORA")).InnerText = utente["DTA_CONNESSIONE"].ToString ();
				}
				result=xmlUtenti.OuterXml;
			}
			return result;
		}

        public static int NumUtentiConnessi(string codiceAmm)
        {
            int result = 0;
            DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            result = gestioneUtenti.GetNumUtentiConnessi(codiceAmm);
            if (result == null)
                result = 0;
            return result;
        }

        public static int NumUtentiAttivi(string codiceAmm)
        {
            int result = 0;
            DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            result = gestioneUtenti.GetNumUtentiAttivi(codiceAmm);
            if (result == null)
                result = 0;
            return result;
        }

		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static string getLibrary()
		{
			#region Codice Commentato
			/*logger.Debug("getLibrary");
			string queryString = 
				"SELECT LIBRARY_NAME FROM REMOTE_LIBRARIES"; 
			logger.Debug (queryString);		
			string library = null;
			try 
			{
				library = db.executeScalar(queryString).ToString();
			} 
			catch (Exception e) {
				logger.Debug(e.Message);
				throw new DocsPaWSException("F_WrongLogin");
			}*/
			#endregion 

			string library=null;
			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
			utenti.GetLibrary(out library);												   
			return library;
		}

		/// <summary>
		/// </summary>
		/// <param name="db"></param>
		/// <param name="idPeople"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static string getDominio(string idPeople)
		{
			#region Codice Commentato
			/*
			logger.Debug("getLibrary");
			string queryString = 
				" SELECT NETWORK_ID FROM NETWORK_ALIASES WHERE PERSONORGROUP= " + idPeople;

			logger.Debug (queryString);		
			string dominio = null;
			try 
			{
				dominio = db.executeScalar(queryString).ToString();
			} 
			catch (Exception) {}
			*/
			#endregion 

			string dominio=null;
			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
			utenti.GetDominio(out dominio,idPeople);												   
			return dominio;
		}

		/// <summary>
		/// </summary>
		public static void logoff(string userId,string idAmm, string sessionId, string dst)
		{
			DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti=new DocsPaDB.Query_DocsPAWS.Utenti ();
			gestioneUtenti.UnlockUserLogin (userId ,idAmm, sessionId);
			DocsPaDocumentale.Documentale.UserManager userManager=new DocsPaDocumentale.Documentale.UserManager();
			userManager.LogoutUser(dst);
		}

		public static void logoff_RDE(string userId, string dst)
		{
			DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti=new DocsPaDB.Query_DocsPAWS.Utenti ();
			gestioneUtenti.RDE_UnlockUserLogin(userId);
			DocsPaDocumentale.Documentale.UserManager userManager=new DocsPaDocumentale.Documentale.UserManager();
			userManager.LogoutUser(dst);
		}

		public static void logoff(string userId,string idAmm, string dst)
		{
			DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti=new DocsPaDB.Query_DocsPAWS.Utenti ();
			gestioneUtenti.UnlockUserLogin (userId ,idAmm);
			DocsPaDocumentale.Documentale.UserManager userManager=new DocsPaDocumentale.Documentale.UserManager();
            userManager.LogoutUser(dst);
		}

		public static void disconnettiUtente(string userId,string codiceAmm)
		{
			DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti=new DocsPaDB.Query_DocsPAWS.Utenti ();
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazione=new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
			string idAmm=amministrazione.GetAdminByName(codiceAmm);
			if(idAmm!=null)
			{
				gestioneUtenti.UnlockUserLogin (userId ,idAmm);
			}
			return;
		}

        /// <summary>
        /// Modifica della password per l'utente
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static DocsPaVO.Validations.ValidationResultInfo ChangePassword(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            DocsPaVO.Validations.ValidationResultInfo result = userManager.ChangeUserPwd(user, oldPassword);

            // Se l'esito è positivo...
            if (result.Value == true)
                UserLog.UserLog.WriteLog(user.UserName, "0", "0",
                    user.IdAmministrazione, "MODUSER", "0",
                    string.Format("Password dell'utente {0} modificata dall'utente stesso",
                        user.UserName), DocsPaVO.Logger.CodAzione.Esito.OK, null);
            
            return result;
        }

        /// <summary>
        /// Reperimento del giorno di scadenza della password dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DateTime GetUserPasswordExpirationDate(string userId)
        {
            DocsPaDocumentale.Documentale.AdminPasswordConfig adminConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();
            if (adminConfig.IsSupportedPasswordConfig())
                return DocsPaPwdServices.UserPasswordServices.GetPasswordExpirationDate(userId);
            else
                throw new ApplicationException("Gestione password non supportata, impossibile reperire la data di scadenza");
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="newPassword"></param>
        ///// <param name="userName"></param>
        ///// <returns></returns>
        //public static DocsPaVO.Validations.ValidationResultInfo ChangePassword(string newPassword, string userName, string idAmministrazione)
        //{
        //    DocsPaVO.Validations.ValidationResultInfo result = null;

        //    DocsPaVO.utente.Utente user = BusinessLogic.Utenti.UserManager.getUtente(userName, idAmministrazione);

        //    if (user != null)
        //    {
        //        result = ChangePassword(user, );
        //    }
        //    else
        //    {
        //        // Utente non trovato
        //        result = new DocsPaVO.Validations.ValidationResultInfo();
        //        result.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("USER_NOT_FOUND", string.Format("Utente '{0}' non trovato", userName), DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error));
        //        result.Value = false;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// </summary>
        ///// <param name="login"></param>
        ///// <param name="debug"></param>
        //public static bool cambiaPassword(DocsPaVO.utente.UserLogin user, string oldPassword)
        //{
        //    bool result = false;

        //    DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
        //    result = utenti.CambiaPassword(user, oldPassword);

        //    string esito = (result)?"OK":"errore!";
        //    logger.Debug("Funzione: Cambia password utente... esito: " + esito);

        //    if(result)
        //    {
        //        // gestione specifica per Filenet---------------------------------------------------------
        //        string oldPwd = string.Empty;
        //        string documentType = System.Configuration.ConfigurationManager.AppSettings["documentale"];
        //        if (documentType.ToUpper().Equals("FILENET"))
        //        {	
        //            oldPwd = utenti.GetPasswordUserFilenet(user.userId);
					
        //            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager(null, null);
        //            result = userManager.UpdateUserFilenet(user.userId, oldPwd, newPassword, string.Format("{0} {1}", user.cognome, user.nome), user.idAmministrazione);					
        //            esito = (result)?"OK":"errore!";
        //            logger.Debug("segue: Cambia password utente su FILENET... esito: " + esito);
        //        }
        //        // fine filenet---------------------------------------------------------------------------
        //    }
			
        //    return result;
        //}

		public static DocsPaVO.utente.UserLogin VerificaUtente(string userName)
		{
			DataSet ds;
			logger.Debug("Verifica utente");
			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
			ds = utenti.VerificaUtente(userName);
			if (ds.Tables[0].Rows.Count == 0) return null;
			string idAmm = ds.Tables[0].Rows[0]["ID_AMM"].ToString();
			string pwdUtente = ""; // ds.Tables[0].Rows[0]["USER_PASSWORD"].ToString();

			DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin(userName, pwdUtente, idAmm);
            return userLogin;		
		}

        #region Login And Impersonate

        /// <summary>
        /// Funzione per effettuare login di un utente e impersonare un altro utente.
        /// </summary>
        /// <param name="userLogin">Oggetto con le informazioni da utilizzare per il login</param>
        /// <param name="userToImpersonate">User name dell'utente da impersonare</param>
        /// <returns>Info utente da utilizzare per effettuare delle operazioni impersonando un altro utente</returns>
        public static DocsPaVO.utente.InfoUtente LoginAndImpersonate(DocsPaVO.utente.UserLogin userLogin, String userToImpersonate)
        {
            // Risultato processo di login
            DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;

            // Indirizzo ip
            String ipAddress;

            // Login classico
            DocsPaVO.utente.Utente user = loginMethod(userLogin, out loginResult, true, userLogin.SessionId, out ipAddress);

            // Se il risultato non è OK, viene sollevata un'eccezione
            if (loginResult != DocsPaVO.utente.UserLogin.LoginResult.OK)
                throw new ApplicationException(loginResult.ToString());

            // Impersonificazione con l'utente userToImpersonate
            return Impersonate(userToImpersonate);
            
        }

        /// <summary>
        /// Funzione per l'impersonificazione di un utente
        /// </summary>
        /// <param name="userIdRichiedente">User id dell'utente da impersonificare</param>
        /// <returns>Info utente relativo all'utente impersonificato</returns>
        private static DocsPaVO.utente.InfoUtente Impersonate(String userIdRichiedente)
        {
            // Reperimento oggetto utente richiedente
            DocsPaVO.utente.Utente utente = UserManager.getUtente(userIdRichiedente, UserManager.getIdAmmUtente(userIdRichiedente));

            if (utente == null)
                throw new ApplicationException(String.Format("Utente {0} non trovato", userIdRichiedente));

            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, GetRoleForUserToImpersonate(utente.idPeople, utente.userId));
            
            // Reperimento token superutente
            infoUtente.dst = UserManager.getSuperUserAuthenticationToken();
            
            return infoUtente;
        }

        /// <summary>
        /// Funzione per il reperimento dei ruoli dell'utente impersonificato
        /// </summary>
        /// <param name="idPeople">Id dell'utente impersonificato</param>
        /// <param name="userName">User name dell'utente personificato</param>
        /// <returns>Ruolo da utilizzare</returns>
        private static DocsPaVO.utente.Ruolo GetRoleForUserToImpersonate(String idPeople, String userName)
        {
            DocsPaVO.utente.Ruolo ruoloPreferito = null;

            // Reperimento dei ruoli associati all'utente
            DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));

            // Se non ci sono ruoli, viene sollevata un'eccezione
            if(ruoli.Length == 0)
                throw new ApplicationException(String.Format("Nessun ruolo trovato per l'utente {0}", userName.ToUpper()));

            ruoloPreferito = ruoli[0];

            return ruoloPreferito;
        }

        #endregion

        public static bool AddBrowserInfo(DocsPaVO.utente.UserLogin objLogin, string idPeople)
        {
            bool retValue = false;
            DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            retValue = gestioneUtenti.AddBrowserInfo(objLogin.BrowserInfo, idPeople, objLogin.UserName, objLogin.IPAddress);
            return retValue;
        }

        /// <summary>
        /// </summary>
        public static void logoffOtherSessions(string userId, string idAmm, string sessionId)
        {
            DocsPaDB.Query_DocsPAWS.Utenti gestioneUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            gestioneUtenti.UnlockUserLoginOtherSessions(userId, idAmm, sessionId);
        }

	}
}
