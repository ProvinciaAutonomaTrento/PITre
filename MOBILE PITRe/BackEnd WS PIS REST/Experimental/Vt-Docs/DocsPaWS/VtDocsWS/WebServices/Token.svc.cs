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
    /// Metodi per la gestione del token di autenticazione.
    /// </summary>

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Token : IToken
    {
        private ILog logger = LogManager.GetLogger(typeof(Token));

        public VtDocsWS.Services.Token.GetAuthenticationToken.GetAuthenticationTokenResponse GetAuthenticationToken(VtDocsWS.Services.Token.GetAuthenticationToken.GetAuthenticationTokenRequest request)
        {
            logger.Info("BEGIN");

            Services.Token.GetAuthenticationToken.GetAuthenticationTokenResponse response = Manager.TokenManager.GetAuthenticationToken(request);

            //log
            //DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            //DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request);
            //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "GETAUTHENTICATIONTOKEN ", request.UserName, "Prelievo del token per l'utente " + request.UserName, esito);
            // Utilizzo questa forma per evitare che esegua il login.
            logger.Info("Metodo GetAuthenticationToken nei WS PIS: Prelievo del token per l'utente "+request.UserName);
            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public VtDocsWS.Services.Token.GetToken.GetTokenResponse GetToken(VtDocsWS.Services.Token.GetToken.GetTokenRequest request)
        {
            logger.Info("BEGIN");
            Services.Token.GetToken.GetTokenResponse response = Manager.TokenManager.GetToken(request);
            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;
        }
    
    }

    
}
