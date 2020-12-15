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
    /// Metodi per la gestione delle trasmissioni
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class PluginHash : IPluginHash
    {
        private ILog logger = LogManager.GetLogger(typeof(PluginHash));

        public Services.PluginHash.GetHashMail.GetHashMailResponse GetHashMail(Services.PluginHash.GetHashMail.GetHashMailRequest request)
        {
            logger.Info("BEGIN");
            Services.PluginHash.GetHashMail.GetHashMailResponse response = Manager.PluginHashManager.GetHashMail(request);

            Utils.CheckFaultException(response);
            logger.Info("END");

            return response;
        }

        public Services.PluginHash.NewHashMail.NewHashMailResponse NewHashMail(Services.PluginHash.NewHashMail.NewHashMailRequest request)
        {
            logger.Info("BEGIN");
            Services.PluginHash.NewHashMail.NewHashMailResponse response = Manager.PluginHashManager.NewHashMail(request);

            Utils.CheckFaultException(response);
            logger.Info("END");

            return response;
        }
    }
}
