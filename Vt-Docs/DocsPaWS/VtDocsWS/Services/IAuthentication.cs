using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per l'autenticazione dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IAuthentication
    {
        [OperationContract]
        VtDocsWS.Services.Authentication.LogIn.LogInResponse LogIn(VtDocsWS.Services.Authentication.LogIn.LogInRequest request);

        [OperationContract]
        VtDocsWS.Services.Authentication.LogOut.LogOutResponse LogOut(VtDocsWS.Services.Authentication.LogOut.LogOutRequest request);

        [OperationContract]
        VtDocsWS.Services.Authentication.Authenticate.AuthenticateResponse Authenticate(VtDocsWS.Services.Authentication.Authenticate.AuthenticateRequest request);

    }
}