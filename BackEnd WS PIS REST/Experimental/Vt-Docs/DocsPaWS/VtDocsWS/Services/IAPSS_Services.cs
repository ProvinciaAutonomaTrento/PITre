using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VtDocsWS.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAPSS_Services" in both code and config file together.
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IAPSS_Services
    {
        [OperationContract]
        VtDocsWS.Services.APSS_Services.SendPublicationResult.SendPublicationResultResponse SendPublicationResult(VtDocsWS.Services.APSS_Services.SendPublicationResult.SendPublicationResultRequest request);
    }
}
