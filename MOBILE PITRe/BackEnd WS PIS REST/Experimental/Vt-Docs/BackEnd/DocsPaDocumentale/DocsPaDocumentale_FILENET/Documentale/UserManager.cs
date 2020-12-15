using System;
using System.Web;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Principal;
using DocsPaDB.Utils;
using DocsPaDocumentale_FILENET.FilenetLib;
using DocsPaDocumentale.Interfaces;
using log4net;

namespace DocsPaDocumentale_FILENET.Documentale
{
	/// <summary>
	/// Classe per la gestione degli utenti tramite il documentale FileNet
	/// </summary>
	public class UserManager : IUserManager
	{
        private ILog logger = LogManager.GetLogger(typeof(UserManager));
		#region Costruttori

		public UserManager()   
		{
		}

		#endregion
	
		#region Metodi

        /// <summary>
        /// Effettua il login di un utente amministratore
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginAdminUser(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            // Per la login in amministrazione, la gestione è delegata al documentale etdocs
            IUserManager etdocsUserManager = new DocsPaDocumentale_ETDOCS.Documentale.UserManager();

            return etdocsUserManager.LoginAdminUser(userLogin, forceLogin, out utente, out loginResult);
        }

		/// <summary>
		/// Effettua il login di un utente
		/// </summary>
		/// <param name="utente">Oggetto Utente connesso</param>
		/// <returns>True = OK; False = Si è verificato un errore</returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult) 
		{
			bool result=true;
			utente = null;
			loginResult=DocsPaVO.utente.UserLogin.LoginResult.OK;

			try 
			{	
				DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

				string name=System.String.Empty;
				string password=System.String.Empty;
				string idAmm=null;

				if (userLogin.UserName != null) 
                    name = userLogin.UserName;
				if (userLogin.Password != null) 
                    password = userLogin.Password;
				idAmm = userLogin.IdAmministrazione;

                string peopleId = string.Empty;

				if (utenti.IsUtenteDisabled(userLogin.UserName, idAmm))
				{
					loginResult=DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER;
                    result = false;
				}

				//verifica userId su tabella utenti
                if (result && !utenti.UserLogin(out peopleId, name, idAmm, userLogin.Modulo))
				{
					loginResult=DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                    result = false;
				}

				if (result && !string.IsNullOrEmpty(peopleId))
				{
					string utenteDominio;

					userLogin.SystemID=peopleId;

					//verifica se deve bisogna eseguire l'autenticazione su dominio;
					utenti.GetDominio(out utenteDominio, peopleId);

                    if (!string.IsNullOrEmpty(utenteDominio))
					{
						string[] arr = this.separaDominio(utenteDominio);
						if (arr.GetLength(0) < 2)
						{
							loginResult=DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
                            result = false;
                            logger.Debug("Firma dello user sul dominio non valida.");
						}

                        if (result)
                        {
                            string userName = arr[1];
                            string dominio = arr[0];
                            userLogin.Dominio = dominio;

                            //esegue l'autenticazione su dominio;
                            if (!this.loginOnDomain(dominio, userName, password))
                            {
                                loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                                result = false;
                                logger.Debug("Utente non autenticato sul dominio");
                            }
                        }
					}
					else 
					{
						//autenticazione completa su tabella utenti
						if (!utenti.UserLogin(out peopleId, name, password,idAmm, userLogin.Modulo))
						{
							loginResult=DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                            result = false;
							logger.Debug("Utente non trovato");
						}
					}

                    if (!string.IsNullOrEmpty(peopleId))
                        utente = utenti.GetUtente(name, userLogin.IdAmministrazione);
                    else
                        result = false;
				}
			} 
			catch (Exception ex) 
			{
				logger.Debug("Errore nella login.", ex);
				result = false;
				utente = null;
			}

            if (result)
            {
                string dst = string.Empty;
                
                try
                {
                    string gruppoFilenet = ConfigurationManager.AppSettings["FNET_userGroup"].ToString();

                    if (string.IsNullOrEmpty(gruppoFilenet))
                    {
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
                        result = false;
                        logger.Debug("Gruppo Filenet non indicato");
                    }
                    else
                    {
                        dst = LoginToFileNet(userLogin, gruppoFilenet);
                        result = true;
                        utente.dst = dst;
                        userLogin.DST = dst;
                    }
                }
                catch (Exception e)
                {
                    if (e.Message == "Non essendo abilitato, l'utente non può connettersi ai servizi del documento IDM.")
                    {
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER;
                    }
                    logger.Debug("Errore nella login Filenet", e);
                    utente = null;
                    result = false;
                }
            }

			return result;
        }

        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="newPassword"/></param>
        /// <param name="utente"></param>
        ///// <returns></returns>
        public DocsPaVO.Validations.ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            DocsPaVO.Validations.ValidationResultInfo result = new DocsPaVO.Validations.ValidationResultInfo();

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            if (!utenti.CambiaPassword(user, oldPassword))
                result.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("ChangePassword_ERROR", "Errore nella modifica della password per il documentale FILENET", DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error));

            if (result.BrokenRules.Count == 0)
            {
                // gestione specifica per Filenet---------------------------------------------------------
                string oldPwd = utenti.GetPasswordUserFilenet(user.UserName);
                
                if (!this.ChangeUserFilenetPassword(user.UserName, oldPwd, user.UserName, user.IdAmministrazione))
                    result.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("ChangePassword_ERROR", "Errore nella modifica della password per il documentale FILENET", DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error));
            }

            result.Value = (result.BrokenRules.Count == 0);

            return result;
        }

        public bool LogoutUser(string dst)
		{
			try
			{
                DocsPaDB.Query_DocsPAWS.Utenti userDb = new DocsPaDB.Query_DocsPAWS.Utenti();

                if (userDb.RemoveUserLoginLock(dst))
                {
                    logoutToFilenet(dst);
                    return true;
                }
                else
                    return false;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

        /// <summary>
        /// Reperimento del token di autenticazione per il superuser del documentale 
        /// </summary>
        /// <returns></returns>
        public string GetSuperUserAuthenticationToken()
        {
            return string.Empty;
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
				WindowsIdentity  wi = WindowsIdentity.GetCurrent();

				if (LogonUser(utente, dominio, password, 3, 0, out token)) 
				{
					CloseHandle(token);
				}
				else 
				{
					int err = GetLastError();
					if (err != 1326) // faccio il log solo se non si tratta di un errore di scrittura
						logger.Debug("Errore di autenticazione sul dominio (utente: " + utente + " - err: " + err.ToString() + " ): " );
					throw new System.Exception(err.ToString());

				}

			}
			catch(Exception ex)
			{
				result = false;
				logger.Debug("Errore di autenticazione sul dominio (utente: " + utente + "): " , ex);
			}

			return result;

		}

		
		internal string[] separaDominio(string utenteDominio) 
		{
			return utenteDominio.Split('\\');
		}
		
		[DllImport("advapi32.dll" , SetLastError = true)]
		internal static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
			int dwLogonType, int dwLogonProvider, out IntPtr phToken);
		
		[DllImport("Kernel32.dll")]
		internal static extern bool CloseHandle(IntPtr phToken);
		
		
		[DllImport("Kernel32.dll")]
		internal static extern int GetLastError();

		#endregion autenticazione su dominio
		
		#endregion


		#region Metodi per il login al documentale FileNet

		public string LoginToFileNet(DocsPaVO.utente.UserLogin userLogin, string gruppoFilenet)
		{
			logger.Debug("init login");
			
			string password=userLogin.Password;

			if ( userLogin.Dominio != null && userLogin.Dominio.Length > 0)
			{
				DocsPaDB.Query_DocsPAWS.Utenti utenti=new DocsPaDB.Query_DocsPAWS.Utenti();
				password=utenti.GetPasswordUserFilenet(userLogin.SystemID, true);
			}

			string dst = "";
			IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();

			try
			{
				IDMObjects.Library oLibrary;
				oLibrary = getFileNETLib(userLogin);
				logger.Debug ("Library name=" + oLibrary.Name);
				if (oLibrary.Logon(userLogin.UserName, password, gruppoFilenet, 
					IDMObjects.idmLibraryLogon.idmLogonOptNoUI
					//IDMObjects.idmLibraryLogon.idmLogonOptUseNetworkNoUI 
					))
				{
					dst = oLibrary.LogonId.ToString();	
				}

			}
			catch(Exception e)
			{
				string msg = e.Message;	
				logger.Debug ("msg errore in fileNetLogon = " + msg);
				for (int i=1; i < idmErrorManager.Errors.Count; i++)
				{
					msg += " " + idmErrorManager.Errors[i].Description;
				}
				throw new Exception(msg);
			}

			return dst;
					
		}

		internal IDMObjects.Library getFileNETLib(DocsPaVO.utente.UserLogin login) 
		{
			logger.Debug("getFileNETLib");

			IDMObjects.Library oLibrary = new IDMObjects.Library();
			if(System.Web.HttpContext.Current.Application["SessionManager"] == null) 
			{
				logger.Debug ("prima di add context");
				HttpContext.Current.Application.Add("SessionManager", new IDMObjects.SessionManager());
			}
			logger.Debug ("prima di set session manager");
			oLibrary.SessionManager = (IDMObjects.SessionManager)HttpContext.Current.Application["SessionManager"];
			logger.Debug ("prima di getlib");
			oLibrary.Name =  Personalization.getInstance(login.IdAmministrazione).getLibrary();;
			return oLibrary;
		}

		internal IDMObjects.Library getFileNETLib(string idamministrazione) 
		{
			logger.Debug("getFileNETLib");

			IDMObjects.Library oLibrary = new IDMObjects.Library();
			if(System.Web.HttpContext.Current.Application["SessionManager"] == null) 
			{
				logger.Debug ("prima di add context");
				HttpContext.Current.Application.Add("SessionManager", new IDMObjects.SessionManager());
			}
			logger.Debug ("prima di set session manager");
			oLibrary.SessionManager = (IDMObjects.SessionManager)HttpContext.Current.Application["SessionManager"];
			logger.Debug ("prima di getlib");
			oLibrary.Name =  Personalization.getInstance(idamministrazione).getLibrary();;
			return oLibrary;
		}


		public void logoutToFilenet(string dst) 
		{
			IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
			try 
			{
				IDMObjects.Library oLibrary = getLibrary(dst);
				oLibrary.Logoff();
			} 
			catch(Exception e) 
			{			
				string msg = e.Message;	
				logger.Debug("msg errore in logout = " + msg);
				for (int i=1; i < idmErrorManager.Errors.Count; i++)
					msg += " " + idmErrorManager.Errors[i].Description;
				throw new Exception(msg);
			}
		}

		internal IDMObjects.Library getLibrary(string dst) 
		{
			IDMObjects.Library oLibrary = new IDMObjects.Library();
			oLibrary.LogonId = dst;
			return oLibrary;
		}

		#endregion

		#region Gestione utenti del documentale Filenet
        public virtual bool Checkconnection()
        {
            return true;

        }
		public bool AddUserFilenet(string userID, string userPwd, 
			string idAmministrazione, string userFullName, string userDefaultGroup)
		{
			string dst="";
			UserManager userManager=null;
			IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
			
			try
			{
				userManager = new UserManager();
				IDMObjects.Library oLibrary = userManager.getFileNETLib(idAmministrazione);
				string ArchivioFile = DocsPaDocumentale_FILENET.FilenetLib.DocumentManagement.checkFolderCompliance(oLibrary);
				DocsPaDB.Query_DocsPAWS.Utenti userFilenet = new DocsPaDB.Query_DocsPAWS.Utenti();
				if ( ! userFilenet.AddUserFilenet(userID, userDefaultGroup, ArchivioFile, userFullName))
					throw new Exception("Errore durante l'inserimento utente Filenet: " + userID);
				bool result = oLibrary.Logon(userID, "", "Administrators", IDMObjects.idmLibraryLogon.idmLogonOptNoUI);
	
				oLibrary.ChangePassword("", userPwd, IDMObjects.idmPasswordOptions.idmPasswordNoUI, userID);
				dst = oLibrary.LogonId.ToString();
				return true;
			}
			catch(Exception e)
			{
				string msg = e.Message;	
				logger.Debug("msg errore in AddUserFilenet = " + msg);
				for (int i=1; i < idmErrorManager.Errors.Count; i++)
					msg += " " + idmErrorManager.Errors[i].Description;
				throw new Exception(msg);
			}
			finally
			{
				if (dst.Length > 0)
					userManager.logoutToFilenet(dst);
			}
		}

		public bool DisableUserFilenet( string username)
		{
			DocsPaDB.Query_DocsPAWS.Utenti userFilenet = new DocsPaDB.Query_DocsPAWS.Utenti();
			return userFilenet.DisableUserFilenet(username);
		}

		public bool DeleteUserFilenet( string username)
		{
			DocsPaDB.Query_DocsPAWS.Utenti userFilenet = new DocsPaDB.Query_DocsPAWS.Utenti();
			return userFilenet.DeleteUserFilenet(username);
		}

        public bool ChangeUserFilenetPassword(string username, string oldpwd, string newPwd, string idAmministrazione)
        {
            bool changed = false;
            string dst = string.Empty;
            
            IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();

            try
            {
                IDMObjects.Library oLibrary = this.getFileNETLib(idAmministrazione);

                DocsPaDB.Query_DocsPAWS.Utenti userFilenet = new DocsPaDB.Query_DocsPAWS.Utenti();
                
                bool result = oLibrary.Logon(username, oldpwd, "Administrators", IDMObjects.idmLibraryLogon.idmLogonOptNoUI);
                oLibrary.ChangePassword(oldpwd, newPwd, IDMObjects.idmPasswordOptions.idmPasswordNoUI, username);
                dst = oLibrary.LogonId.ToString();

                changed = true;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                logger.Debug("msg errore in UpdateUserFilenet = " + msg);

                for (int i = 1; i < idmErrorManager.Errors.Count; i++)
                    msg += " " + idmErrorManager.Errors[i].Description;
                
                throw new Exception(msg);
            }
            finally
            {
                if (!string.IsNullOrEmpty(dst))
                    this.logoutToFilenet(dst);
            }

            return changed;
        }

		public bool UpdateUserFilenet( string username, string oldpwd, string newPwd, string userfullname, string idamministrazione)
		{
			string dst="";
			UserManager userManager=null;
			IDMError.ErrorManager idmErrorManager = new IDMError.ErrorManager();
			
			try
			{
				userManager = new UserManager();
				IDMObjects.Library oLibrary = userManager.getFileNETLib(idamministrazione);
				DocsPaDB.Query_DocsPAWS.Utenti userFilenet = new DocsPaDB.Query_DocsPAWS.Utenti();
				if ( ! userFilenet.UpdateUserFilenet( username, userfullname))
					throw new Exception("Errore durante la modifica dell'utente Filenet: " + username);
				bool result = oLibrary.Logon(username, oldpwd, "Administrators", IDMObjects.idmLibraryLogon.idmLogonOptNoUI);
				oLibrary.ChangePassword(oldpwd, newPwd, IDMObjects.idmPasswordOptions.idmPasswordNoUI, username);
				dst = oLibrary.LogonId.ToString();
				return true;
			}
			catch(Exception e)
			{
				string msg = e.Message;	
				logger.Debug("msg errore in UpdateUserFilenet = " + msg);
				for (int i=1; i < idmErrorManager.Errors.Count; i++)
					msg += " " + idmErrorManager.Errors[i].Description;
				throw new Exception(msg);
			}
			finally
			{
				if (dst.Length > 0)
					userManager.logoutToFilenet(dst);
			}
		}

		#endregion
	}
}
