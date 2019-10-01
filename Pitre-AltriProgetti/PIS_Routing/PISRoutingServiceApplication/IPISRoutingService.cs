using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PISRoutingServiceApplication
{
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IPISRoutingService
    {

        [OperationContract]
        string GetEndpoint(string administrationCode);

        [OperationContract]
        string[] GetEndpointAndApps(string administrationCode);

    }
}
