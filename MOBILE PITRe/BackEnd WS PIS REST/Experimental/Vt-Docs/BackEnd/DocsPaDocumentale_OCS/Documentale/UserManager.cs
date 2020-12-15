using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using DocsPaUtils.LogsManagement;
using DocsPaDocumentale_OCS.CorteContentServices;
using DocsPaDocumentale_OCS.OCSServices;
using log4net;

namespace DocsPaDocumentale_OCS.Documentale
{
    /// <summary>
    /// Gestione connessione utente al documentale OCS
    /// </summary>
    public class UserManager : IUserManager
    {
        private ILog logger = LogManager.GetLogger(typeof(UserManager));
        /// <summary>
        /// Istanza webservice user
        /// </summary>
        public CorteContentServices.UserSOAPHTTPBinding _wsUserInstance = null;

        /// <summary>
        /// 
        /// </summary>
        public UserManager()
        {}

        #region Protected methods

        /// <summary>
        /// Reperimento istanza webservice User
        /// </summary>
        protected CorteContentServices.UserSOAPHTTPBinding WsUserInstance
        {
            get
            {
                if (this._wsUserInstance == null)
                    this._wsUserInstance = OCSServices.OCSServiceFactory.GetDocumentServiceInstance<CorteContentServices.UserSOAPHTTPBinding>();
                return this._wsUserInstance;
            }
        }

        /// <summary>
        /// Creazione token di autenticazione a partire dalla userid e password dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected virtual string GetAuthenticationToken(string userId, string password)
        {
            // Formattazione 
            return OCSTokenHelper.Encrypt(string.Format("{0}|{1}", userId, password));
        }

        #endregion

        #region Public methods
        public virtual bool Checkconnection()
        {
            return true;

        }
        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="oldPassword"/></param>
        /// <param name="user"></param>
        ///// <returns></returns>
        public ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();
            retValue.Value = false;
            retValue.BrokenRules.Add(new BrokenRule("CHANGE_PWD_NOT_SUPPORTED", "Cambio password non supportato dal documentale CDC", BrokenRule.BrokenRuleLevelEnum.Error));
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
            loginResult = UserLogin.LoginResult.UNKNOWN_USER;

            DocsPaVO.utente.Utente utenteDocsPa;
            retValue = (this.LoginUser(userLogin, out utenteDocsPa, out loginResult));

            if (retValue)
            {
                loginResult = UserLogin.LoginResult.OK;

                utente = new DocsPaVO.amministrazione.InfoUtenteAmministratore();
                utente.dst = utenteDocsPa.dst;
                utente.userId = userLogin.UserName;
            }

            return retValue;
        }

        /// <summary>
        /// Login al sistema documentale
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            bool retValue = false;
            utente = null;
            loginResult = UserLogin.LoginResult.UNKNOWN_USER;
            try
            {
                utente = new Utente();

                CorteContentServices.UserCredentialsRequestType userCred = new CorteContentServices.UserCredentialsRequestType();
                CorteContentServices.ResultType result = new CorteContentServices.ResultType();
                userCred.userCredentials = new CorteContentServices.UserCredentialsType();
                userCred.userCredentials.userId = userLogin.UserName;
                userCred.userCredentials.password = userLogin.Password;

                result = this.WsUserInstance.Login(userCred);

                if (OCSUtils.isValidServiceResult(result))
                {
                    //definizione del dst
                    // il dst tiene in memoria le informazioni necessarie per l'autenticazione dell'utente
                    // ci sono operazioni in cui non viene utilizzato l'utente applicativo ma è necessario 
                    // utilizzare le credenziali effettive dell'utente che si connette al sistema. 
                    // Per esempio nelle ricerche
                    utente = new Utente();
                    utente.dst = this.GetAuthenticationToken(userLogin.UserName, userLogin.Password);

                    retValue = true;
                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                    logger.Debug(string.Format("OCS.LoginUser: utente {0} connesso", userLogin.UserName));
                }
                else
                {
                    retValue = false;
                    utente = null;
                    loginResult = UserLogin.LoginResult.UNKNOWN_USER;

                    logger.Debug(string.Format("Errore in OCS.Login:\n{0}", result.message));
                }
            }
            catch (Exception ex)
            {
                retValue = false;
                utente = null;
                loginResult = UserLogin.LoginResult.UNKNOWN_USER;

                logger.Error(string.Format("Errore in OCS.Login:\n{0}", ex.ToString()));
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
            return true;
        }


        /// <summary>
        /// Reperimento del token di autenticazione per il superuser del documentale 
        /// </summary>
        /// <returns></returns>
        public string GetSuperUserAuthenticationToken()
        {   
            UserCredentialsType credentials = OCSServices.OCSUtils.getApplicationUserCredentials();

            // Creazione token di autenticazione
            return this.GetAuthenticationToken(credentials.userId, credentials.password);
        }

        #endregion
    }
}
