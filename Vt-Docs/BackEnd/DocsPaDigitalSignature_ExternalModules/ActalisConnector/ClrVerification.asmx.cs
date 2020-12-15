using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using System.Configuration;

namespace ActalisConnector
{
    /// <summary>
    /// Summary description for ClrVerification
    /// </summary>
    [WebService(Namespace = "http://nttdata.com/2013/CRLSvc")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ClrVerification : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(ClrVerification));

        static VerificaRemota.VerificationServiceClient client = null;

        [WebMethod]
        public string[] BatchDettaglioCertificato(string[] CertificateBase64)
        {
            List<String> retval = new List<string>();
            OpenWcfChannel();
            string serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            CrlVerificationService clr = new CrlVerificationService();
            CrlVerificationService.EsitoVerifica retvalCert = null;

            byte[] CertificateCAPEM = null;
            foreach (string certStr in CertificateBase64)
            {
                //converto il certificato base64 in binario
                byte[] CertificateDer = Convert.FromBase64String(certStr);
                try
                {
                    retvalCert = clr.VerificaCertificato(CertificateDer, CertificateCAPEM, client);
                    logger.Debug("Verifica cerificato effettuato con successo.");
                    retval.Add(Utils.SerializeObject<DocsPaVO.documento.CertificateInfo>(clr.GetCertificateInfoFromEv(retvalCert)));
                }
                catch (Exception e)
                {
                    logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
                    //return e.Message;
                }
            }
            return retval.ToArray();
        }

        [WebMethod]
        public bool FileLocale(string file)
        {
            if ((System.IO.File.Exists(file)))
                return true;

            return false;
        }


        [WebMethod]
        public string ProvaServizio()
        {
            string file = Server.MapPath("~") + "\\test.p7m";
            return VerificaCertificato(System.IO.File.ReadAllBytes(file), DateTime.Now, false);
        }

        [WebMethod]
        public string ProvaServizioCERT()
        {
            string file = Server.MapPath("~") + "\\cert.der";
            OpenWcfChannel();
            CrlVerificationService clr = new CrlVerificationService();
            logger.Debug("Effettuo la verifica certificato");
            CrlVerificationService.EsitoVerifica retval = null;
            try
            {
                byte[] ca = new byte[0];
                //byte[] cer = new System.Text.ASCIIEncoding().GetBytes(Convert.ToBase64String(System.IO.File.ReadAllBytes(file)));
                byte[] cer = System.IO.File.ReadAllBytes(file);
                string b64Cer = Convert.ToBase64String(System.IO.File.ReadAllBytes(file));
                retval = clr.VerificaCertificato(cer, ca, client);
                logger.Debug("Verifica cerificato effettuato.");
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
                return e.Message;
            }


            return Utils.SerializeObject<CrlVerificationService.EsitoVerifica>(retval);

        }


        [WebMethod]
        public string DettaglioCertificato(byte[] CertificateDer, byte[] CertificateCAPEM)
        {

            OpenWcfChannel();
            CrlVerificationService clr = new CrlVerificationService();
            CrlVerificationService.EsitoVerifica retval = null;
            try
            {
                retval = clr.VerificaCertificato(CertificateDer, CertificateCAPEM, client);
                logger.Debug("Verifica cerificato effettuato con successo.");
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
                return e.Message;
            }

            return Utils.SerializeObject<CrlVerificationService.EsitoVerifica>(retval);
        }


        [WebMethod]
        public string VerificaCertificatoConFileOriginale(byte[] content, byte[] fileOriginale, DateTime? dataverificaDT, bool ancheFile)
        {
            return VerificaCertificato(content, dataverificaDT, ancheFile);
        }


        [WebMethod]
        public string VerificaCertificatoFileLocale(string file, DateTime? dataverificaDT, bool ancheFile)
        {
            return VerificaCertificato(System.IO.File.ReadAllBytes(file), dataverificaDT, ancheFile);
        }

        [WebMethod]
        public string VerificaCertificato(byte[] content, DateTime? dataverificaDT, bool ancheFile)
        {
            OpenWcfChannel();
            ActalisConnector.CrlVerificationService cvs = new CrlVerificationService ();
            return cvs.verifica(content, dataverificaDT, ancheFile,client);
        }

        [WebMethod]
        public string ConnessioneServizio()
        {
            string retval = "KO";
            try
            {
                OpenWcfChannel();
                retval = "OK";
            }
            catch (Exception e)
            {
                retval = "Execption : " + e.Message;
            }
            return retval;
        }


        private void OpenWcfChannel()
        {
            string serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            if (client == null)
            {
                logger.Debug("Il canale è null, apro un nuovo canale di comunicazione");
                client = CrlVerificationService.createClient(serviceUrl);
            }

            if (client.State != System.ServiceModel.CommunicationState.Opened)
            {
                logger.Debug("Il canale è chiuso, lo apro nuovamente");
                client = CrlVerificationService.createClient(serviceUrl);
            }
        }

    }
}
