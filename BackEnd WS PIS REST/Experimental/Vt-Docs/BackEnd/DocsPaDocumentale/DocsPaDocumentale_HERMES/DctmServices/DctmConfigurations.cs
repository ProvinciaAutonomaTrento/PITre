using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale_DOCUMENTUM.DctmServices
{
    /// <summary>
    /// Classe per la gestione delle configurazioni relative al repository corrente di documentum
    /// </summary>
    public sealed class DctmConfigurations
    {
        /// <summary>
        /// 
        /// </summary>
        private DctmConfigurations()
        { }

        /// <summary>
        /// Reperimento nome repository dctm
        /// </summary>
        /// <returns></returns>
        public static string GetRepositoryName()
        {
            return GetDocumentumConfig("DocumentumRepositoryName");
        }

        /// <summary>
        /// Reperimento url webservice documentum
        /// </summary>
        /// <returns></returns>
        public static string GetDocumentumServerUrl()
        {
            return GetDocumentumConfig("DocumentumServiceUrl");
        }

        /// <summary>
        /// Reperimento userid utente superuser
        /// </summary>
        /// <returns></returns>
        public static string GetDocumentumSuperUser()
        {
            return GetDocumentumConfig("DocumentumSuperUser");
        }

        /// <summary>
        /// Reperimento password utente superuser
        /// </summary>
        /// <returns></returns>
        public static string GetDocumentumSuperUserPwd()
        {
            return GetDocumentumConfig("DocumentumSuperUserPwd");
        }

        /// <summary>
        /// Reperimento url servizio web per la creazione del token di autenticazione documentum
        /// </summary>
        /// <returns></returns>
        public static string GetDocumentumTokenFactoryServiceUrl()
        {
            return GetDocumentumConfig("DocumentumTokenFactoryServiceUrl");
        }

        /// <summary>
        /// Reperimento configurazione dctm
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        private static string GetDocumentumConfig(string configKey)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[configKey] != null)
                return System.Configuration.ConfigurationManager.AppSettings[configKey];
            else
                return string.Empty;
        }
    }
}