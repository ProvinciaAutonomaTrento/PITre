using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using log4net;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace ArubaConnector
{
    public class RemotePdfStamp
    {
        private static ILog logger = LogManager.GetLogger(typeof(RemotePdfStamp));

        static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            // Ignore errors
            return true;
        }
        
        public byte[] Stamp(int Page, int LeftX, int LeftY, int RightX, int RightY, byte[] PdfContent, String StampText)
        {

            try
            {
                string IGNORESSL = string.Empty;
                IGNORESSL = ConfigurationManager.AppSettings["IGNORESSL"];
                if (!string.IsNullOrEmpty(IGNORESSL) && IGNORESSL.ToLower() == "true")
                    //ingora errori ssl
                    ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;

                ArubaSignServices.ArubaSignServiceService arss = new ArubaSignServices.ArubaSignServiceService();

                string PDFSTAMPSERVICE_URL = ConfigurationManager.AppSettings["PDFSTAMPSERVICE_URL"];
                if (string.IsNullOrEmpty(PDFSTAMPSERVICE_URL))
                {
                    logger.Error("PDFSTAMPSERVICE_URL non valorizzato!");
                    return null;
                }

                arss.Url = PDFSTAMPSERVICE_URL;

                string AUTOSIGN_TYPE_OTP_AUTH = ConfigurationManager.AppSettings["AUTOSIGN_TYPE_OTP_AUTH"];
                string AUTOSIGN_OTP_PWD = ConfigurationManager.AppSettings["AUTOSIGN_OTP_PWD"];

                string AUTOSIGN_USER = ConfigurationManager.AppSettings["AUTOSIGN_USER"];
                string AUTOSIGN_USERPWD = ConfigurationManager.AppSettings["AUTOSIGN_USERPWD"];
                string AUTOSIGN_PROFILE = ConfigurationManager.AppSettings["AUTOSIGN_PROFILE"];

                string AUTOSIGN_DELEGATED_DOMAIN = ConfigurationManager.AppSettings["AUTOSIGN_DELEGATED_DOMAIN"];
                string AUTOSIGN_DELEGATED_USER = ConfigurationManager.AppSettings["AUTOSIGN_DELEGATED_USER"];
                string AUTOSIGN_DELEGATED_PASSWORD = ConfigurationManager.AppSettings["AUTOSIGN_DELEGATED_PASSWORD"];

                string AUTOSIGN_REASON = ConfigurationManager.AppSettings["AUTOSIGN_REASON"];
                string AUTOSIGN_LOCATION = ConfigurationManager.AppSettings["AUTOSIGN_LOCATION"];


                if (string.IsNullOrEmpty(AUTOSIGN_TYPE_OTP_AUTH))
                {
                    logger.Error("AUTOSIGN_TYPE_OTP_AUTH non valorizzato!");
                    return null;
                }


                ArubaSignServices.signRequestV2 signRequest = new ArubaSignServices.signRequestV2
                {
                    transport = ArubaSignServices.typeTransport.BYNARYNET,
                    transportSpecified = true,
                    binaryinput = PdfContent,
                    certID = "AS0",
                    requiredmark = false,

                    identity = new ArubaSignServices.auth
                    {
                        typeHSM = "COSIGN",
                        typeOtpAuth = AUTOSIGN_TYPE_OTP_AUTH,
                        ext_authtype = ArubaSignServices.credentialsType.PAPERTOKEN,
                    },
                };

                if (!String.IsNullOrEmpty(AUTOSIGN_PROFILE))
                    signRequest.profile = AUTOSIGN_PROFILE;

                if (!String.IsNullOrEmpty(AUTOSIGN_DELEGATED_DOMAIN))
                    signRequest.identity.delegated_domain = AUTOSIGN_DELEGATED_DOMAIN;

                if (!String.IsNullOrEmpty(AUTOSIGN_DELEGATED_USER))
                    signRequest.identity.delegated_user = AUTOSIGN_DELEGATED_USER;

                if (!String.IsNullOrEmpty(AUTOSIGN_DELEGATED_PASSWORD))
                    signRequest.identity.delegated_password = AUTOSIGN_DELEGATED_PASSWORD;

                if (!String.IsNullOrEmpty(AUTOSIGN_USERPWD))
                    signRequest.identity.userPWD = AUTOSIGN_USERPWD;

                if (!String.IsNullOrEmpty(AUTOSIGN_USER))
                    signRequest.identity.user = AUTOSIGN_USER;

                if (!String.IsNullOrEmpty(AUTOSIGN_OTP_PWD))
                    signRequest.identity.otpPwd = AUTOSIGN_OTP_PWD;

                ArubaSignServices.pdfSignApparence appearance = new ArubaSignServices.pdfSignApparence
                {
                    page = Page,
                    leftx = LeftX,
                    lefty = LeftY,
                    rightx = RightX,
                    righty = RightY,
                    testo = StampText
                };

                if (!String.IsNullOrEmpty(AUTOSIGN_REASON))
                    appearance.reason = AUTOSIGN_REASON;

                if (!String.IsNullOrEmpty(AUTOSIGN_LOCATION))
                    appearance.location = AUTOSIGN_LOCATION;

                ArubaSignServices.signReturnV2 retval = arss.pdfsignatureV2(signRequest, appearance, ArubaSignServices.pdfProfile.PADESBES, false, "");
                if (retval.status != "OK")
                {
                    logger.ErrorFormat("Errore firmando {0} - {1} ", retval.return_code, retval.description);
                    return null;
                }
                else
                {
                    logger.Info("File Firmato OK");
                }
                return retval.binaryoutput;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore : msg {0} stk {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

    }
}