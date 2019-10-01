using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using log4net;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione della rubrica
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Authentication : IAuthentication
    {

        private ILog logger = LogManager.GetLogger(typeof(Authentication));

        /// <summary>
        /// Metodo di login
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Authentication.LogIn.LogInResponse LogIn(Services.Authentication.LogIn.LogInRequest request)
        {
            logger.Info("BEGIN");
            Services.Authentication.LogIn.LogInResponse response = Manager.AuthenticationManager.LogIn(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio di logout
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Authentication.LogOut.LogOutResponse LogOut(Services.Authentication.LogOut.LogOutRequest request)
        {
            logger.Info("BEGIN");
            Services.Authentication.LogOut.LogOutResponse response = Manager.AuthenticationManager.LogOut(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        public Services.Authentication.Authenticate.AuthenticateResponse Authenticate(Services.Authentication.Authenticate.AuthenticateRequest request)
        {
            logger.Info("BEGIN");
            Services.Authentication.Authenticate.AuthenticateResponse response = Manager.AuthenticationManager.Authenticate(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }
    }
}
