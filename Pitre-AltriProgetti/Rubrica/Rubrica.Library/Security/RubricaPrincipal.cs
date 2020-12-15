using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using Rubrica.Library.Data;

namespace Rubrica.Library.Security
{
    public class RubricaPrincipal : IPrincipal
    {
        /// <summary>
        /// 
        /// </summary>
        private IIdentity _identity = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        private RubricaPrincipal(IIdentity identity)
        {
            this._identity = identity;
        }

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public IIdentity Identity
        {
            get
            {
                return this._identity;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>        
        /// <returns></returns>
        public static bool Login(SecurityCredentials credentials)
        {
            return SetPrincipal(RubricaIdentity.GetIdentity(credentials));
        }

        /// <summary>
        /// Crezione identity non autenticata
        /// </summary>
        public static void Logout()
        {
            Current = new RubricaPrincipal(RubricaIdentity.UnauthenticatedIdentity());
        }

        /// <summary>
        /// Impostazione oggetto Principal
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        private static bool SetPrincipal(RubricaIdentity identity)
        {
            RubricaPrincipal.Current = new RubricaPrincipal(identity);

            return identity.IsAuthenticated;
        }

        /// <summary>
        /// Oggetto Principal corrente
        /// </summary>
        public static RubricaPrincipal Current
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return System.Threading.Thread.CurrentPrincipal as Security.RubricaPrincipal;
                else
                    return System.Web.HttpContext.Current.User as Security.RubricaPrincipal;
            }
            set
            {
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.User = value;

                System.Threading.Thread.CurrentPrincipal = value;
            }
        }

        #endregion
    }
}