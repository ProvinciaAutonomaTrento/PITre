using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using DocsPaVO.utente;
using DocsPaVO.Ldap;
using DocsPaVO.Validations;
using log4net;

namespace BusinessLogic.Ldap
{
    /// <summary>
    /// Classe per la gestione delle configurazioni per l'accesso da docspa ai servizi LDAP per un'amministrazione
    /// </summary>
    public sealed class LdapConfigurationsServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(LdapConfigurationsServices));

        #region Public members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static LdapConfig GetLdapConfig(InfoUtente infoUtente, string idAmministrazione)
        {
            return DocsPaLdapServices.LdapConfigurations.GetLdapConfig(idAmministrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="ldapInfo"></param>
        public static void SaveLdapConfig(InfoUtente infoUtente, string idAmministrazione, LdapConfig ldapInfo)
        {
            DocsPaLdapServices.LdapConfigurations.SaveLdapConfig(idAmministrazione, ldapInfo);
        }

        #endregion

        #region Private members

        /// <summary>
        /// 
        /// </summary>
        private LdapConfigurationsServices()
        { }

        #endregion 
    }
}
