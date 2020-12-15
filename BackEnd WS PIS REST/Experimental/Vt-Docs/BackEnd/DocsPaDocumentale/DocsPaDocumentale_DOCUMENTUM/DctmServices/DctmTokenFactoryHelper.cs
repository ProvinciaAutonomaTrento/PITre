using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.DctmServices
{
    /// <summary>
    /// Helper class per la gestione del token di autenticazione a documentum
    /// </summary>
    public sealed class DctmTokenFactoryHelper
    {
        private static ILog logger = LogManager.GetLogger(typeof(DctmTokenFactoryHelper));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string Generate(string userName)
        {
            try
            {
                string token = string.Empty;

                string serviceUrl = DctmConfigurations.GetDocumentumTokenFactoryServiceUrl();

                if (string.IsNullOrEmpty(serviceUrl))
                    throw new ApplicationException("Chiave di configurazione 'DocumentumTokenFactoryServiceUrl' non definita");

                using (Custom.TicketFactory tokenFactory = new Custom.TicketFactory(serviceUrl))
                {
                    // Utilizzo del servizio web per la creazione del token di autenticazione per l'utente richiesto.
                    // NB: é necessario fornire le credenziali dell'utente superamministratore e il nome del repository.
                    token = tokenFactory.getTicket(DctmConfigurations.GetDocumentumSuperUser(),
                                                    DctmConfigurations.GetDocumentumSuperUserPwd(),
                                                    userName,
                                                    DctmConfigurations.GetRepositoryName());
                }

                return token;
            }
            catch (Exception ex)
            {
                string message = string.Format("Errore nella generazione del DM_TOKEN per l'autenticazione dell'utente '{0}' in documentum", userName);
                logger.Error(message, ex);

                throw new ApplicationException(message, ex);
            }
        }
    }
}
