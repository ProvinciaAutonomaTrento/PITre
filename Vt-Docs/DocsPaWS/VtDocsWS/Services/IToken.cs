using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per la gestione dei token di autenticazione dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IToken
    {
        [OperationContract]
        VtDocsWS.Services.Token.GetAuthenticationToken.GetAuthenticationTokenResponse GetAuthenticationToken(VtDocsWS.Services.Token.GetAuthenticationToken.GetAuthenticationTokenRequest request);

        [OperationContract]
        VtDocsWS.Services.Token.GetToken.GetTokenResponse GetToken(VtDocsWS.Services.Token.GetToken.GetTokenRequest request);
    }
}
