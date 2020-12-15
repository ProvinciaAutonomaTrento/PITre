using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaUtils.LogsManagement;
using log4net;

namespace DocsPaDocumentale_CDC.Documentale
{
    public class UserManager : IUserManager
    {
        private ILog logger = LogManager.GetLogger(typeof(UserManager));
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private IUserManager _userManagerETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private IUserManager _userManagerOCS = null;

        /// <summary>
        /// </summary>
        public UserManager()
        {
        }

        #endregion

         /// <summary>
        /// Connessione dell'utente al sistema documentale
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {

            bool result = false;
            utente = null;
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;

            try
            {
                string name = string.Empty;
                string password = string.Empty;

                if (!string.IsNullOrEmpty(userLogin.UserName))
                    name = userLogin.UserName;

                if (!string.IsNullOrEmpty(userLogin.Password))
                    password = userLogin.Password;

                DocsPaDB.Query_DocsPAWS.Utenti user = new DocsPaDB.Query_DocsPAWS.Utenti();

                result = user.IsUtenteDisabled(userLogin.UserName, userLogin.IdAmministrazione);

                if (result)
                {
                        loginResult = DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER;
                        utente = null;
                        result = false;
                }
                else
                    {

                        // Connessione al sistema OCS
                        DocsPaVO.utente.Utente utenteOCS;
                        result = this.UserManagerOCS.LoginUser(userLogin, out utenteOCS, out loginResult);

                        // Assegnazione dell'id di sessione OCS
                        if (result)
                        {
                            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                            utente = utenti.GetUtente(name, userLogin.IdAmministrazione);
                            utente.dst = utenteOCS.dst;
                        }
                        else 
                        {
                            result = false;
                            loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                        }

                    }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore nella login.", exception);
                result = false;
                utente = null;
            }

            return result;

            #region OLD
            /* OLD
            try
            {
                // Connessione al sistema ETDOCS
                bool connected = this.UserManagerETDOCS.LoginUser(userLogin, out utente, out loginResult);
                
                if (connected)
                {
                    DocsPaVO.utente.Utente utenteOCS;

                    // Connessione al sistema OCS
                    connected = this.UserManagerOCS.LoginUser(userLogin, out utenteOCS, out loginResult);

                    // Assegnazione dell'id di sessione OCS
                    if (connected)
                        utente.dst = utenteOCS.dst;
                }

                return connected;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella login dell'utente al sistema documentale: {0}", ex.Message);
                logger.Debug(errorMessage);
                throw new ApplicationException(errorMessage, ex);
            }
 */
            #endregion
        }
        public virtual bool Checkconnection()
        {
            return true;

        }

        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="newPassword"/></param>
        /// <param name="utente"></param>
        ///// <returns></returns>
        public DocsPaVO.Validations.ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            return this.UserManagerOCS.ChangeUserPwd(user, oldPassword);

            //DocsPaVO.Validations.ValidationResultInfo result = null;

            //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            //{
            //    result = this.UserManagerETDOCS.ChangeUserPwd(user, oldPassword);

            //    if (result.Value)
            //        result = this.UserManagerOCS.ChangeUserPwd(user, oldPassword);

            //    if (result.Value)
            //        transactionContext.Complete();
            //}
            //return result;
        }

        /// <summary>
        /// Disconnessione dell'utente dal sistema documentale
        /// </summary>
        /// <param name="dst">Identificativo univoco della sessione utente</param>
        /// <returns></returns>
        public bool LogoutUser(string dst)
        {
            try
            {
                // Connessione al sistema ETDOCS
                bool disconnected = this.UserManagerETDOCS.LogoutUser(dst);

                if (disconnected)
                {
                    // Connessione al sistema OCS
                    disconnected = this.UserManagerOCS.LogoutUser(dst);
                }

                return disconnected;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella logout dell'utente al sistema documentale: {0}", ex.Message);
                logger.Debug(errorMessage);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Effettua il login di un utente amministratore
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginAdminUser(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            utente = null;
            loginResult = UserLogin.LoginResult.UNKNOWN_USER;

            try
            {
                DocsPaVO.amministrazione.InfoUtenteAmministratore infoAdmin = null;
                
                using (DocsPaDB.Query_DocsPAWS.Amministrazione amministrazioneDb = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                {
                    // Reperimento metadati dell'utente amministratore (senza gestione password criptate)
                    infoAdmin = amministrazioneDb.GetDatiAmministratore(userLogin.UserName);

                    if (infoAdmin != null)
                    {
                        // Connessione al sistema CDC se altro tipo di amministratore
                        if (this.UserManagerOCS.LoginAdminUser(userLogin, forceLogin, out utente, out loginResult))
                        {
                            // L'utente si è connesso con successo al sistema mediante i servizi forniti da CDC,
                            // pertanto vengono fatte le opportune operazioni in etdocs per validare la connessione
                            bool userAlreadyConnected;

                            if (!amministrazioneDb.LoginAmministratore(utente, userLogin.IPAddress, userLogin.SessionId, forceLogin, out userAlreadyConnected))
                            {
                                utente.dst = null;

                                if (userAlreadyConnected)
                                    // Utente già connesso
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                                else
                                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                            }
                            else
                            {
                                utente.idPeople = infoAdmin.idPeople;
                                utente.tipoAmministratore = infoAdmin.tipoAmministratore;
                                utente.nome = infoAdmin.nome;
                                utente.cognome = infoAdmin.cognome;
                                utente.idAmministrazione = infoAdmin.idAmministrazione;
                                utente.idCorrGlobali = infoAdmin.idCorrGlobali;

                                loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                            }
                        }
                    }
                    else
                    {
                        loginResult = UserLogin.LoginResult.UNKNOWN_USER;
                    }
                }

                return (loginResult == UserLogin.LoginResult.OK);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella login dell'utente al sistema documentale: {0}", ex.Message);
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage, ex);
            }
        }


        #region Protected methods

        /// <summary>
        /// Documentale etdocs
        /// </summary>
        protected IUserManager UserManagerETDOCS
        {
            get
            {
                if (this._userManagerETDOCS == null)
                    this._userManagerETDOCS = new DocsPaDocumentale_ETDOCS.Documentale.UserManager();
                return this._userManagerETDOCS;
            }
        }

        /// <summary>
        /// Documentale OCS
        /// </summary>
        protected IUserManager UserManagerOCS
        {
            get
            {
                if (this._userManagerOCS == null)
                    this._userManagerOCS = new DocsPaDocumentale_OCS.Documentale.UserManager();
                return this._userManagerOCS;
            }
        }

        /// <summary>
        /// Aggiornamento token di autenticazione del documentale esterno in ETDOCS
        /// </summary>
        /// <param name="infoUtente"></param>
        protected virtual bool UpdateUserToken(string newDst, string oldDst)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("UPDATE DPA_LOGIN SET DST = '{0}' WHERE DST = '{1}'", newDst, oldDst);

                int rowsAffected;
                if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = (rowsAffected == 1);
            }

            return retValue;
        }

        #endregion


        public string GetSuperUserAuthenticationToken()
        {
            return this.UserManagerOCS.GetSuperUserAuthenticationToken();
        }
    
    }
}
