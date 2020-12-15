using System;
using System.Configuration;
using System.Linq;

namespace DocsPAWA.SmartClient
{
    /// <summary>
    /// Classe per la gestione delle configurazioni relative ai componenti smartclient
    /// </summary>
    public sealed class Configurations
    {
        private Configurations()
        {
        }

        /// <summary>
        /// Attivazione componenti smartclient
        /// </summary>
        /// <returns></returns>
        public static bool IsActive()
        {
            // Verifica se all'utente è stato consentito da amministrazione l'utilizzo dei componenti smart client
            // dalla propria postazione di lavoro e, inoltre, se il sistema operativo installato in quest'ultima 
            // supporta tale tecnologia
            return (GetConfigurationsPerUser().IsEnabled && IsClientComputerCompliant());
        }

        /// <summary>
        /// Reperimento delle informazioni di configurazione dei componenti SmartClient per l'utente corrente
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.SmartClientConfigurations GetConfigurationsPerUser()
        {
            DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();

            return ws.GetSmartClientConfigurationsPerUser(UserManager.getInfoUtente());
        }

        /// <summary>
        /// Determina se il computer client supporta i requisiti minimi per
        /// utilizzare la piattaforma smartclient
        /// </summary>
        /// <returns></returns>
        public static bool IsClientComputerCompliant()
        {
            bool retValue = true;

            Version currentCrlVersion = System.Web.HttpContext.Current.Request.Browser.ClrVersion;

            //// Determina se nel computer client è installata o meno la versione 3.5 del .net framework
            //if (currentCrlVersion.Major >= 3 && currentCrlVersion.Minor >= 5)
            //{

            if (currentCrlVersion != null)
            {
                // Reperimento della la lista dei sistemi operativi che supportano l'utilizzo della piattaforma smartclient
                string supportList = System.Configuration.ConfigurationManager.AppSettings["SMART_CLIENT_OS_SUPPORT_LIST"];

                if (!string.IsNullOrEmpty(supportList))
                {
                    string userAgent = System.Web.HttpContext.Current.Request.UserAgent;

                    bool osFound = false;

                    foreach (string os in supportList.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (userAgent.Contains(os))
                        {
                            // Sistema operativo del client supportato
                            osFound = true;
                            break;
                        }
                    }

                    retValue = osFound;
                }
                else
                    // Chiave di configurazione non definita, per default il sistema operativo è supportato
                    retValue = true;
            }

            //else
            //{
            //    // Non è installata la versione corretta del .net framework, smartclient non supportati
            //    retValue = false;
            //}

            return retValue;
        }
    }
}