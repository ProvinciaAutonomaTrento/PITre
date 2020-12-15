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
    /// 
    /// </summary>
    public sealed class LdapUserConfigurationServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(LdapUserConfigurationServices));
        /// <summary>
        /// 
        /// </summary>
        private LdapUserConfigurationServices()
        { }

        #region Public members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public static LdapUserConfig GetLdapUserConfig(DocsPaVO.utente.InfoUtente infoUtente, string idUser)
        {
            return DocsPaLdapServices.LdapUserConfigurations.GetLdapUserConfig(idUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static LdapUserConfig GetLdapUserConfigByName(string userName)
        {
            return DocsPaLdapServices.LdapUserConfigurations.GetLdapUserConfigByName(userName);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="infoUtente"></param>
        ///// <param name="idUser"></param>
        ///// <param name="ldapInfo"></param>
        //public static void SaveLdapUserConfig(DocsPaVO.utente.InfoUtente infoUtente, string idUser, LdapConfig ldapInfo)
        //{
        //    DocsPaLdapServices.LdapUserConfigurations.SaveLdapUserConfig(idUser, ldapInfo);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool UserCanConnectToLdap(string userName)
        {
            return DocsPaLdapServices.LdapUserConfigurations.UserCanConnectToLdap(userName);
        }

        #endregion
    }
}
