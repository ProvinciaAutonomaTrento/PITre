using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Data;
using Rubrica.Library.Data;

namespace Rubrica.Library.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class RubricaIdentity : IIdentity
    {
        private string _name = string.Empty;
        private bool _isAuthenticated = false;
        private bool _isAdminRole = false;

        /// <summary>
        /// 
        /// </summary>
        private RubricaIdentity()
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        private RubricaIdentity(SecurityCredentials credentials)
        {
            // Validazione credenziali utente
            this.ValidateCredentials(credentials);
        }

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public string AuthenticationType
        {
            get 
            {
                return "Rubrica";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAuthenticated
        {
            get 
            {
                return this._isAuthenticated;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get 
            {
                return this._name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAdminRole
        {
            get
            {
                return this._isAdminRole;
            }
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static RubricaIdentity UnauthenticatedIdentity()
        {
            return new RubricaIdentity();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        internal static RubricaIdentity GetIdentity(SecurityCredentials credentials)
        {
            return new RubricaIdentity(credentials);
        }

        #endregion

        #region DataAccess methods

        /// <summary>
        /// 
        /// </summary>
        private const string SP_GET = "R.GetUserCredentials";

        /// <summary>
        /// Validazione credenziali utente
        /// </summary>
        /// <returns></returns>
        protected virtual void ValidateCredentials(SecurityCredentials credentials)
        {
            SecurityCredentialsResult result = SecurityHelper.ValidateCredentials(credentials);

            this._isAuthenticated = (result != null);

            if (this._isAuthenticated)
            {
                this._name = result.UserName;
                this._isAdminRole = result.Amministratore;
            }
        }

        #endregion
    }
}