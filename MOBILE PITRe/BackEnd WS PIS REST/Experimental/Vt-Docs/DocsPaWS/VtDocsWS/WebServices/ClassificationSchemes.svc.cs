using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using log4net;



namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione dei titolari
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class ClassificationSchemes : IClassificationSchemes
    {

        private ILog logger = LogManager.GetLogger(typeof(ClassificationSchemes));

        /// <summary>
        /// Servizio per il reperimento del titolario attivo
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeResponse GetActiveClassificationScheme(Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeRequest request)
        {
            logger.Info("BEGIN");
            Services.ClassificationScheme.GetActiveClassificationScheme.GetActiveClassificationSchemeResponse response = Manager.ClassificationSchemesManager.GetActiveClassificationScheme(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutti i titolari
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesResponse GetAllClassificationSchemes(Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesRequest request)
        {
            logger.Info("BEGIN");
            Services.ClassificationScheme.GetAllClassificationSchemes.GetAllClassificationSchemesResponse response = Manager.ClassificationSchemesManager.GetAllClassificationSchemes(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un titolario
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdResponse GetClassificationSchemeById(Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdRequest request)
        {
            logger.Info("BEGIN");
            Services.ClassificationScheme.GetClassificationSchemeById.GetClassificationSchemeByIdResponse response = Manager.ClassificationSchemesManager.GetClassificationSchemeById(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            
            return response;
        }
    }
}
