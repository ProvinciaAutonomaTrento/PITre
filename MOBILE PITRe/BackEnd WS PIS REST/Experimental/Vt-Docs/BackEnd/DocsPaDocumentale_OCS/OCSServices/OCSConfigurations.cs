using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_OCS.OCSServices
{
    /// <summary>
    /// Classe per la gestione delle configurazioni relative all'utilizzo dei servizi OCS
    /// </summary>
    public sealed class OCSConfigurations
    {
        /// <summary>
        /// 
        /// </summary>
        private OCSConfigurations()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetServiceUrl()
        {
            return GetOCSConfig("OCSServiceUrl");
        }

        /// <summary>
        /// Reperimento nome superutente per ocs
        /// </summary>
        /// <returns></returns>
        public static string GetSuperUser()
        {
            return GetOCSConfig("OCSSuperUser");
        }

        /// <summary>
        /// Reperimento password superutente per ocs
        /// </summary>
        /// <returns></returns>
        public static string GetSuperUserPwd()
        {
            return GetOCSConfig("OCSSuperUserPwd");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetGroupUsers()
        {
            return GetOCSConfig("OCSGroupUsers");
        }
        
        /// <summary>
        /// Reperimento path per i documenti inseriti in cestino nel documentale ocs
        /// </summary>
        /// <returns></returns>
        public static string GetPathCestino()
        {
            return GetOCSConfig("OCSPathCestino");
        }

        /// <summary>
        /// Reperimento del path root in cui sono inseriti i documenti in ocs
        /// </summary>
        /// <returns></returns>
        public static string GetDocRootFolder()
        {
            return GetOCSConfig("OCSDocRootFolder");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetDocPathPattern()
        {
            return GetOCSConfig("OCSDocPathPattern");
        }

        /// <summary>
        /// Reperimento configurazione per OCS
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        private static string GetOCSConfig(string configKey)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[configKey] != null)
                return System.Configuration.ConfigurationManager.AppSettings[configKey];
            else
                return string.Empty;
        }
    }
}
