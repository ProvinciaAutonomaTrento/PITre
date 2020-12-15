using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using Emc.Documentum.FS.Runtime.Services;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Context;

namespace DocsPaDocumentale_DOCUMENTUM.DctmServices
{
    /// <summary>
    /// Class factory per la crezione di un oggetto Service di documentum
    /// </summary>
    public sealed class DctmServiceFactory
    {
        #region Module constants

        /// <summary>
        /// Identifica il nome del module predefinito per la creazione dei servizi documentum
        /// </summary>
        private const string CORE_SERVICE_MODULE = "core";

        /// <summary>
        /// Identifica il nome del module predefinito per la creazione dei servizi cusotm creati per documentum
        /// </summary>
        private const string CUSTOM_SERVICE_MODULE = "etnoteam";

        #endregion

        #region Factory methods

        /// <summary>
        /// Creazione di un oggetto Service di documentum,
        /// a partire da un token di autenticazione utente
        /// </summary>
        /// <param name="authenticationToken">Token di autenticazione utente</param>
        /// <returns></returns>
        public static T GetServiceInstance<T>(string authenticationToken)
        {
            return GetServiceInstance<T>(authenticationToken, CORE_SERVICE_MODULE);
        }

        /// <summary>
        /// Creazione di un oggetto Service custom creati per documentum,
        /// a partire da un token di autenticazione utente
        /// </summary>
        /// <param name="authenticationToken">Token di autenticazione utente</param>
        /// <returns></returns>
        public static T GetCustomServiceInstance<T>(string authenticationToken)
        {
            return GetServiceInstance<T>(authenticationToken, CUSTOM_SERVICE_MODULE);
        }

        /// <summary>
        /// Creazione di un oggetto Service di documentum, a partire da un token di autenticazione utente
        /// </summary>
        /// <param name="authenticationToken">Token di autenticazione utente</param>
        /// <param name="serviceModule"></param>
        /// <returns></returns>
        private static T GetServiceInstance<T>(string authenticationToken, string serviceModule)
        {
            IServiceContext context = DctmServiceContextHelper.GetCurrent();

            //ContentTransferProfile contentTransferProfile = new ContentTransferProfile();
            //contentTransferProfile.TransferMode = ContentTransferMode.BASE64;
            //OperationOptions streamOptions = new OperationOptions();
            //ContentProfile contentProfile = streamOptions.ContentProfile;
            //contentProfile.FormatFilter = FormatFilter.ANY;
            //contentProfile.formatFilterSpecified = true;
            //context.SetProfile(contentProfile);
            
            // Reperimento oggetto identity dal token di autenticazione
            context.AddIdentity(DctmRepositoryIdentityHelper.GetIdentity(authenticationToken));

            // Reperimento url del servizio
            string serviceUrl = DctmConfigurations.GetDocumentumServerUrl();

            return ServiceFactory.Instance.GetRemoteService<T>(context, serviceModule, serviceUrl);
        }

        #endregion
    }
}