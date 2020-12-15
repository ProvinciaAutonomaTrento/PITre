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
    /// Metodi per la gestione della rubrica
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Administration : IAdministration
    {

        private ILog logger = LogManager.GetLogger(typeof(Administration));

        /// <summary>
        /// Metodo di GetAdministrations
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Administration.GetAdministrations.GetAdministrationsResponse GetAdministrations(Services.Administration.GetAdministrations.GetAdministrationsRequest request)
        {
            logger.Info("BEGIN");
            Services.Administration.GetAdministrations.GetAdministrationsResponse response = Manager.AdministrationManager.GetAdministrations(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }
    }
}
