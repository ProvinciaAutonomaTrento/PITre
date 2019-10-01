using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using VtDocsWS.Services;
using log4net;


namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la verifica di connessione dei servizi
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class CheckWS : ICheckWS
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckWS));

        public VtDocsWS.Services.CheckWS.CheckConnection.CheckConnectionResponse CheckConnection(VtDocsWS.Services.CheckWS.CheckConnection.CheckConnectionRequest request)
        {
            logger.Info("BEGIN");
            logger.Info("Metodo CheckConnection");
            
            Services.CheckWS.CheckConnection.CheckConnectionResponse response = Manager.CheckConnectionManager.CheckConnection(request);
            logger.Info("Esito Connection: " + response.EsitoConnection);

            Utils.CheckFaultException(response);

            logger.Info("END");

            return response;
        }
    }
}
