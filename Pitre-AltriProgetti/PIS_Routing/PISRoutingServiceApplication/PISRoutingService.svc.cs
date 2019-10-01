using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

namespace PISRoutingServiceApplication
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class PISRoutingService : IPISRoutingService
    {
        public string GetEndpoint(string administrationCode)
        {
            return Manager.PISRoutingManager.GetEndpoint(administrationCode);
        }

        public string[] GetEndpointAndApps(string administrationCode)
        {
            return Manager.PISRoutingManager.GetEndpointAndApps(administrationCode);
        }
    }
}
