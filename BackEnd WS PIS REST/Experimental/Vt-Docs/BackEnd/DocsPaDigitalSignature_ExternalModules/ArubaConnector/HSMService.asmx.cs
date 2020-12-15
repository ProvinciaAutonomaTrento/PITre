using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using System.Configuration;
using ArubaConnector.ArubaSignServices;

namespace ArubaConnector
{
    /// <summary>
    /// Summary description for HSMService
    /// </summary>
    [WebService(Namespace = "http://nttdata.com/2013/HSMRsSvc")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class HSMService : System.Web.Services.WebService
    {
        public enum SignType
        {
            CADES,
            PADES
        }

        private static ILog logger = LogManager.GetLogger(typeof(HSMService));

        static object client = null;

        /// <summary>
        /// Richiede un OTP via SMS
        /// </summary>
        /// <param name="aliasCertificato"> Alias del certificato</param>
        /// <param name="dominioCertificato">Dominio Certificato (se vuoto preso dalla configurazione)</param>
        /// <returns></returns>
        [WebMethod]
        public bool RichiediOTP(string aliasCertificato, string dominioCertificato)
        {
            OpenWsChannel();
            I_HSMConnector h = new HsmConnectorFactory();
            return h.richiediOTP(aliasCertificato, dominioCertificato, client);
        }

        /// <summary>
        /// Richiede il certificato associato all'ALIAS e al dominio
        /// </summary>
        /// <param name="aliasCertificato"> Alias del certificato</param>
        /// <param name="dominioCertificato">Dominio Certificato (se vuoto preso dalla configurazione)</param>
        /// <returns></returns>
        [WebMethod]
        public string GetCertificatoHSM(string aliasCertificato, string dominioCertificato)
        {
            OpenWsChannel();
            I_HSMConnector h = new HsmConnectorFactory();
            return h.VisualizzaCertificatoHSM(aliasCertificato, dominioCertificato, client);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliasCertificato"></param>
        /// <param name="dominioCertificato"></param>
        /// <param name="pinCertificato"></param>
        /// <param name="otpFirma"></param>
        /// <param name="fileDafirmare"></param>
        /// <param name="marcaTemporale"></param>
        /// <returns></returns>
        [WebMethod]
        public byte[] FirmaFilePADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale)
        {
            OpenWsChannel();
            I_HSMConnector h = new HsmConnectorFactory();
            return h.FirmaFilePADES(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale, client);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aliasCertificato"></param>
        /// <param name="dominioCertificato"></param>
        /// <param name="pinCertificato"></param>
        /// <param name="otpFirma"></param>
        /// <param name="fileDafirmare"></param>
        /// <param name="marcaTemporale"></param>
        /// <param name="cofirma"></param>
        /// <returns></returns>
        [WebMethod]
        public byte[] FirmaFileCADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, bool cofirma)
        {
            OpenWsChannel();
            I_HSMConnector h = new HsmConnectorFactory();
            return h.FirmaFileCADES(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale, cofirma, client);
        }

        [WebMethod]
        public byte[] ControFirmaFileCADES(byte[] fileDafirmare, string aliasCertificatoDaControfirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale)
        {
            OpenWsChannel();
            I_HSMConnector h = new HsmConnectorFactory();
            return h.ControFirmaFileCADES(fileDafirmare, aliasCertificatoDaControfirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale, client);
        }

        //Gestione Firma Massiva
        #region Firma Massiva

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string Session_OpenMultiSign(bool cosign, bool timestamp, SignType Type)
        {
            I_HSMConnector h = new HsmConnectorFactory();
            return  h.OpenMultiSignSession(cosign, timestamp, (int)Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public bool Session_CloseMultiSign(string SessionToken)
        {
            I_HSMConnector h = new HsmConnectorFactory();
            return h.Session_CloseMultiSign(SessionToken);
        }


        [WebMethod]
        public string Session_GetSessions()
        {
            I_HSMConnector h = new HsmConnectorFactory();
            return h.Session_GetSessions();
        }

        [WebMethod]
        public string Session_GetManifest(string SessionToken)
        {
            I_HSMConnector h = new HsmConnectorFactory();
            return h.Session_GetManifest(SessionToken);
        }

        [WebMethod]
        public bool Session_RemoteSign(string SessionToken, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma)
        {
            OpenWsChannel();
            I_HSMConnector h = new HsmConnectorFactory();
            return h.Session_RemoteSign(SessionToken, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, client);
        }

        [WebMethod]
        public bool Session_RemoteSignDummy(string SessionToken, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma)
        {
            I_HSMConnector h = new HsmConnectorFactory();
            return h.Session_RemoteSign(SessionToken, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, null);
        }

        [WebMethod]
        public string Session_PutFileToSign(string SessionToken, byte[] FileDafirmare, string FileName)
        {
            I_HSMConnector h = new HsmConnectorFactory();
            return h.Session_PutFileToSign(SessionToken, FileDafirmare, FileName);
        }


        [WebMethod]
        public byte[] Session_GetSignedFile(string SessionToken, string hashFileDaFirmare)
        {
            I_HSMConnector h = new HsmConnectorFactory();
            return h.Session_GetSignedFile(SessionToken, hashFileDaFirmare);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string ConnessioneServizio()
        {
            string retval = "KO";
            try
            {
                OpenWsChannel();
                retval = "OK";
            }
            catch (Exception e)
            {
                retval = "Execption : " + e.Message;

            }
            return retval;
        }

        private void OpenWsChannel()
        {
            string serviceUrl = ConfigurationManager.AppSettings["HSMSERVICE_URL"];

            if (client == null)
            {
                logger.Debug("Il canale è null, apro un nuovo canale di comunicazione");
                I_HSMConnector h = new HsmConnectorFactory();
                client = h.createClient(serviceUrl);
            }
        }
    }
}
