using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using log4net;

namespace DocsPaDocumentale_GFD.Documentale
{
    /// <summary>
    /// 
    /// </summary>
    public class UserManager : IUserManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserManager));

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private IUserManager _userManagerETDOCS = null;

        

        /// <summary>
        /// </summary>
        public UserManager()
        {
        }

        #endregion

        #region Public methods

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
                // Connessione al sistema ETDOCS
                bool connected = this.UserManagerETDOCS.LoginAdminUser(userLogin, forceLogin, out utente, out loginResult);

                

                return connected;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella login dell'utente al sistema documentale: {0}", ex.Message);
                logger.Debug(errorMessage);

                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Connessione dell'utente al sistema documentale
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            try
            {
                // Connessione al sistema ETDOCS
                bool connected = this.UserManagerETDOCS.LoginUser(userLogin, out utente, out loginResult);

               

                return connected;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella login dell'utente al sistema documentale: {0}", ex.Message);
                logger.Debug(errorMessage);
                throw new ApplicationException(errorMessage, ex);
            }
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
            DocsPaVO.Validations.ValidationResultInfo result = null;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                result = this.UserManagerETDOCS.ChangeUserPwd(user, oldPassword);

                

                if (result.Value)
                    transactionContext.Complete();
            }
            return result;
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

               

                return disconnected;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella logout dell'utente al sistema documentale: {0}", ex.Message);
                logger.Debug(errorMessage);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        #endregion

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

        public string GetSuperUserAuthenticationToken()
        {
            return this.UserManagerETDOCS.GetSuperUserAuthenticationToken();
        }

        #endregion
    }
}
