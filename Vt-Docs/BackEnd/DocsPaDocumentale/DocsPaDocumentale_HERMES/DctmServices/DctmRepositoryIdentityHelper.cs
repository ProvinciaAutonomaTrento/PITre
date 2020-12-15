using System;
using System.Collections.Generic;
using System.Text;
using Emc.Documentum.FS.DataModel.Core.Context;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.DctmServices
{
    /// <summary>
    /// Classe di utilità per la creazione dell'oggetto RepositoryIdentity
    /// di documentum e per la generazione del token di autenticazione
    /// </summary>
    public sealed class DctmRepositoryIdentityHelper
    {
        private static ILog logger = LogManager.GetLogger(typeof(DctmRepositoryIdentityHelper));
        /// <summary>
        /// Creazione oggetto identity documentum
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static RepositoryIdentity GetIdentity(string repositoryName, string userName, string password, string domain)
        {
            return new RepositoryIdentity(repositoryName, userName, password, domain);
        }

        /// <summary>
        /// Creazione oggetto identity documentum dal token di autenticazione generato precedentemente
        /// </summary>
        /// <param name="authenticationToken"></param>
        /// <returns></returns>
        public static RepositoryIdentity GetIdentity(string authenticationToken)
        {
            RepositoryIdentity identity = null;

            try
            {
                string decryptedToken = DctmTokenHelper.Decrypt(authenticationToken);

                // Creazione oggetto RepositoryIdentity
                if (!string.IsNullOrEmpty(decryptedToken))
                {
                    string[] items = decryptedToken.Split('|');

                    if (items.Length == 5)
                        identity = new RepositoryIdentity(items[0], items[1], items[2], items[3]);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Documentum.GetIdentity: Si è verificato un errore nella decodifica del token di autenticazione";
                logger.Error(errorMessage);
                throw new ApplicationException(errorMessage, ex);
            }

            return identity; 
        }

        /// <summary>
        /// Creazione di un token di autenticazione a partire da un oggetto RepositoryIdentity documentum
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string CreateAuthenticationToken(RepositoryIdentity identity)
        {
            // Formattazione token di autenticazione
            string token = string.Format("{0}|{1}|{2}|{3}|{4}",
                        identity.RepositoryName, identity.UserName, identity.Password, identity.Domain, Guid.NewGuid().ToString());

            return DctmTokenHelper.Encrypt(token);
        }
    }
}
