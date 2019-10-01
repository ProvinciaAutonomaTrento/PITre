using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per la Check dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface ICheckWS
    {
        [OperationContract]
        VtDocsWS.Services.CheckWS.CheckConnection.CheckConnectionResponse CheckConnection(VtDocsWS.Services.CheckWS.CheckConnection.CheckConnectionRequest request);
    }
}
