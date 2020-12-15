using log4net;
using Newtonsoft.Json;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace NttDataWA.Utils
{
    /// <summary>
    /// Descrizione di riepilogo per ValidateCertificateHandler
    /// </summary>
    public class ValidateCertificateHandler : IHttpHandler, IRequiresSessionState 
    {
        private ILog logger = LogManager.GetLogger(typeof(ValidateCertificateHandler));
        public enum EsitoVerificaStatus
        {
            Valid = 0,         //OK
            NotTimeValid = 1,  //Scaduto
            Revoked = 4,       //Revocato
            CtlNotTimeValid = 131072, //Data non corretta
            ErroreGenerico = -1,
            SHA1NonSupportato = -2
        }

        public void ProcessRequest(HttpContext context)
        {

            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            try
            {
                logger.Debug("Inizio Verifica certificato");
                string cert = new StreamReader(context.Request.InputStream).ReadToEnd();
                CertificateInfoJSON certificateInfoJSON = JsonConvert.DeserializeObject<CertificateInfoJSON>(cert);
                CertificateInfo certificateInfo = certificateInfoJSON.getCertificateInfo();
                string revocationDate = string.Empty;
                string formatDate = "dd/MM/yyy HH:mm:ss";
                string revocationStatus = string.Empty;
                response.StatusCode = 200;
                certificateInfo = DocumentManager.VerifyCertificateExpired(certificateInfo);
                logger.Debug("Verifica certificato - certificato : "+cert);
                if (certificateInfo != null && certificateInfo.RevocationStatus != (int)EsitoVerificaStatus.Valid)
                {

                    if (certificateInfo.RevocationDate != null && !certificateInfo.RevocationDate.Equals(DateTime.MinValue))
                        revocationDate = certificateInfo.RevocationDate.ToString(formatDate);

                    else if (certificateInfo.ValidToDate != null)
                        revocationDate = certificateInfo.ValidToDate.ToString(formatDate); ;

                    response.Write(JsonConvert.SerializeObject(new
                        {
                            revocationDate = revocationDate,
                            revocationStatus = certificateInfo.RevocationStatus
                        }));
                }
                else
                {
                    response.Write(JsonConvert.SerializeObject(new
                    {
                        revocationDate = string.Empty,
                        revocationStatus = (int)EsitoVerificaStatus.Valid
                    }));
                }
                logger.Debug("Fine Verifica certificato");
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel controllo del certificato",ex);
                response.Write(JsonConvert.SerializeObject(new
                {
                    revocationDate = string.Empty,
                    revocationStatus = (int)EsitoVerificaStatus.ErroreGenerico
                }));
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}