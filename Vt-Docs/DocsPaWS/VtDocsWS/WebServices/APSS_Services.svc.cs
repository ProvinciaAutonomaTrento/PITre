using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using System.Diagnostics;
using System.Reflection;
using log4net;
using DocsPaWS.VtDocsWS;

namespace VtDocsWS.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "APSS_Services" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class APSS_Services : IAPSS_Services
    {
        private ILog logger = LogManager.GetLogger(typeof(APSS_Services));

        public VtDocsWS.Services.APSS_Services.SendPublicationResult.SendPublicationResultResponse SendPublicationResult(VtDocsWS.Services.APSS_Services.SendPublicationResult.SendPublicationResultRequest request)
        {
            logger.Info("BEGIN");

            Services.APSS_Services.SendPublicationResult.SendPublicationResultResponse response = VtDocsWS.Manager.APSS_ServicesManager.SendPublicationResult(request);

            Utils.CheckFaultException(response);

            logger.Info("END");
            
            return response;

        }
    }
}
