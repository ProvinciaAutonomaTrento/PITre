using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DocsPaWS.BusinessServices
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VtDocs/Business/BusinessServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService()]
    public class BusinessServices : System.Web.Services.WebService, VtDocs.BusinessServices.IBusinessService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Authentication.GetLoginResponse LoginUser(VtDocs.BusinessServices.Entities.Authentication.GetLoginRequest request)
        {
            VtDocs.BusinessServices.Entities.Authentication.GetLoginResponse response = new VtDocs.BusinessServices.Entities.Authentication.GetLoginResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    DocsPaVO.utente.Utente user;
                    string ipAddress;

                    response.Result = ws.Login(request.Login,
                                                out user,
                                                request.Forced,
                                                request.WebSessionId,
                                                out ipAddress);

                    response.User = user;
                    response.IpAddress = ipAddress;
                }

            }
            catch (Exception ex)
            {
                System.Web.Services.Protocols.SoapException soapEx = DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);

                if (request.TrowOnError)
                    throw soapEx;
                else
                {
                    response.Exception = soapEx.ToString();
                    response.User = null;
                    response.Result = DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR;
                    response.IpAddress = null;
                }
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Response LogoutUser(VtDocs.BusinessServices.Entities.Request request)
        {
            VtDocs.BusinessServices.Entities.Response response = new VtDocs.BusinessServices.Entities.Response();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    if (ws.Logoff(request.InfoUtente.userId, request.InfoUtente.idAmministrazione, string.Empty, request.InfoUtente.dst))
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                System.Web.Services.Protocols.SoapException soapEx = DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);

                if (request.TrowOnError)
                    throw soapEx;
                else
                {
                    response.Exception = soapEx.ToString();
                }
            }

            return response;
        }

        /// <summary>
        /// Reperimento valore chiave di configurazione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.GetConfigurazioneResponse GetConfigurazione(VtDocs.BusinessServices.Entities.GetConfigurazioneRequest request)
        {
            VtDocs.BusinessServices.Entities.GetConfigurazioneResponse response = new VtDocs.BusinessServices.Entities.GetConfigurazioneResponse();

            try
            {
                DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(string.IsNullOrEmpty(request.IdAmministrazione) ? "0" : request.IdAmministrazione);

                if (chiaviAmm.ListaChiavi != null)
                {
                    DocsPaVO.amministrazione.ChiaveConfigurazione[] keys = (DocsPaVO.amministrazione.ChiaveConfigurazione[])
                        chiaviAmm.ListaChiavi.ToArray(typeof(DocsPaVO.amministrazione.ChiaveConfigurazione));

                    var keyValuePair = keys.FirstOrDefault(e => e.Codice.Equals(request.CodiceChiave, StringComparison.InvariantCultureIgnoreCase));

                    if (keyValuePair == null)
                        throw new ApplicationException(string.Format("Chiave di configurazione con codice '{0}' non trovata per l'amministrazione richiesta.", request.CodiceChiave));

                    response.ChiaveConfigurazione = keyValuePair;
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.GetConfigurazioneResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }
    }
}
