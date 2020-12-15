using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per la firma dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface ISignature
    {
        [OperationContract]
        VtDocsWS.Services.Signature.VerifySignature.VerifySignatureResponse VerifySignature(VtDocsWS.Services.Signature.VerifySignature.VerifySignatureRequest request);
    }
}
