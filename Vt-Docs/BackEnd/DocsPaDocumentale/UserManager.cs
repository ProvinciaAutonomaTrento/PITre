using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DocsPaDocumentale.Interfaces;

namespace DocsPaDocumentale.Documentale
{
    /// <summary>
    /// Classe per l'interazione con il motore documentale corrente.
    /// Internamente istanzia e utilizza l'oggetto del motore documentale
    /// che implementa l'interfaccia "IUserManager"
    /// </summary>
    public class UserManager : IUserManager
    {
        #region Ctros, variables, constants

        /// <summary>
        /// Tipo documentale corrente
        /// </summary> 
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IUserManager _instance = null;

        /// <summary>
        /// Reperimento del tipo relativo al documentale corrente
        /// </summary>
        static UserManager()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["documentale"]))
            {
                string documentale = ConfigurationManager.AppSettings["documentale"].ToLower();

                if (documentale.Equals(TipiDocumentaliEnum.Etnoteam.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.UserManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Hummingbird.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_HUMMINGBIRD.Documentale.UserManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Filenet.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_FILENET.Documentale.UserManager);
                else if (documentale.Equals(TipiDocumentaliEnum.Pitre.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_PITRE.Documentale.UserManager);
                else if (documentale.Equals(TipiDocumentaliEnum.CDC.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC.Documentale.UserManager);
                else if (documentale.Equals(TipiDocumentaliEnum.GFD.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_GFD.Documentale.UserManager);
             
                //Giordano Iacozzilli  08/10/2012 Aggiunta strato SharePoint
                else if (documentale.Equals(TipiDocumentaliEnum.SharePoint.ToString().ToLower()))
                    _type = typeof(DocsPaDocumentale_CDC_SP.Documentale.UserManager);
                //Fine
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UserManager()
        {
            this._instance = (IUserManager)Activator.CreateInstance(_type);
        }

        #endregion
        
        #region Protected methods

        /// <summary>
        /// Reperimento istanza oggetto "IUserManager"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected IUserManager Instance
        {
            get
            {
                return this._instance;
            }
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
            return this.Instance.LoginAdminUser(userLogin, forceLogin, out utente, out loginResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            return this.Instance.LoginUser(userLogin, out utente, out loginResult);
        }

        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="newPassword"/></param>
        /// <param name="utente"></param>
        ///// <returns></returns>
        public DocsPaVO.Validations.ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            return this.Instance.ChangeUserPwd(user, oldPassword);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        public bool LogoutUser(string dst)
        {
            return this.Instance.LogoutUser(dst);
        }
        
        /// <summary>
        /// Reperimento del token di autenticazione per il superuser del documentale 
        /// </summary>
        /// <returns></returns>
        public string GetSuperUserAuthenticationToken()
        {
            return this.Instance.GetSuperUserAuthenticationToken();
        }

        #endregion

        public bool Checkconnection()
        {
            return this.Instance.Checkconnection();
        }
    }
}
