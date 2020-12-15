using System;
using DocsPaDocumentale.Interfaces;
using log4net;

namespace DocsPaDocumentale_HUMMINGBIRD.Documentale
{
	/// <summary>
	/// Classe per la gestione degli utenti tramite il documentale Hummingbird
	/// </summary>
	public class UserManager : IUserManager
    {
        private ILog logger = LogManager.GetLogger(typeof(UserManager));
		#region Costruttori
		/// <summary>
		/// Inizializza l'istanza della classe acquisendo i dati relativi al login
		/// ed alla libreria per la connessione al documentale.
		/// </summary>
		/// <param name="login">Dati relativi al login</param>
		/// <param name="library">Libreria per la connessione al documentale</param>
		public UserManager()
		{	
		}

		#endregion
	
		#region Metodi
        public virtual bool Checkconnection()
        {
            return true;

        }
        protected string GetLibrary(string idAmministrazione)
        {
            return DocsPaDB.Utils.Personalization.getInstance(idAmministrazione).getLibrary();
        }

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
        /// Reperimento del token di autenticazione per il superuser del documentale 
        /// </summary>
        /// <returns></returns>
        public string GetSuperUserAuthenticationToken()
        {
            return string.Empty;
        }

		/// <summary>
		/// Effettua il login di un utente
		/// </summary>
		/// <param name="utente">Oggetto Utente connesso</param>
		/// <returns>True = OK; False = Si è verificato un errore</returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult) 
		{
			bool result = false;
			utente = null;
			loginResult=DocsPaVO.utente.UserLogin.LoginResult.OK;

			try 
			{
                string name = string.Empty;
                string password = string.Empty;
				
                if (!string.IsNullOrEmpty(userLogin.UserName)) 
                    name = userLogin.UserName;
				
                if (!string.IsNullOrEmpty(userLogin.Password)) 
                    password = userLogin.Password;
				
				//ANAS modifiche per integrazione con il portale
				if (!string.IsNullOrEmpty(userLogin.DST))
				{
					DocsPaDB.Query_DocsPAWS.Utenti utenti=new DocsPaDB.Query_DocsPAWS.Utenti();
					utente = utenti.GetUtente(name, userLogin.IdAmministrazione);
					utente.dst = userLogin.DST;
                    //luluciani 11/03/2008 dopo rilascio 3.7.5 con bug su portale in anas.
                    result = true;
				}
				else
				{
					DocsPaDB.Query_DocsPAWS.Utenti user=new DocsPaDB.Query_DocsPAWS.Utenti();
                    
                    result = user.IsUtenteDisabled(userLogin.UserName, userLogin.IdAmministrazione);
					
                    if (result)
					{
						loginResult = DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER;
						utente = null;
                        result = false;
					}
                    else
                    {
                        string library = this.GetLibrary(userLogin.IdAmministrazione);

                        DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Login login = new DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Login(0, library, name, password);
                        login.Execute();

                        if (login.GetErrorCode() != 0)
                        {
                            result = false;
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;                            
                        }
                        else
                        {
                            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                            utente = utenti.GetUtente(name, userLogin.IdAmministrazione);
                            utente.dst = login.GetDST();
                            result = true;
                        }
                    }
				}
			} 
			catch (Exception exception) 
			{
				logger.Error("Errore nella login.", exception);
				result = false;
				utente = null;
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
                result.BrokenRules.Add(new DocsPaVO.Validations.BrokenRule("ChangePassword_ERROR", "Errore nella modifica della password per il documentale HUMMINGBIRD", DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error));

            result.Value = (result.BrokenRules.Count == 0);

            return result;
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
	
		#endregion
	}
}
