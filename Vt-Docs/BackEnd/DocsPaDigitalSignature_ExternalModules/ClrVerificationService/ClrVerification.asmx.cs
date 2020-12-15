using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using log4net;
using System.Configuration;

namespace ClrVerificationService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    [WebService(Namespace = "http://nttdata.com/2013/CRLSvc")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class ClrVerification : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(ClrVerification));
 
        static FirmaDigitale.FirmaDigitalePortTypeClient client = null;

        [WebMethod]
        public string[] BatchDettaglioCertificato(string[] CertificateBase64)
        {
            List<String> retval = new List<string>();
            OpenWcfChannel();
            string serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            Verifica clr = new Verifica();
            EsitoVerifica retvalCert = null;

            byte[] CertificateCAPEM=null;
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
        public bool FileLocale( string file)
        {
            if ((System.IO.File.Exists (file)))
                return true;

            return false;
        }

        
        [WebMethod]
        public string ProvaServizio()
        {
            string file =  Server.MapPath("~")+"\\test.p7m";
            return VerificaCertificato(System.IO.File.ReadAllBytes(file), DateTime.Now, false, false);
        }

        [WebMethod]
        public string ProvaServizioCERT()
        {
            string file = Server.MapPath("~") + "\\cert.der";
            string serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            OpenWcfChannel();
            Verifica clr = new Verifica();
            logger.Debug("Effettuo la verifica certificato");
            EsitoVerifica retval = null;
            try
            {
                byte[] ca = new  byte[0];
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


            return Utils.SerializeObject<EsitoVerifica>(retval);

        }

      
        [WebMethod]
        public string DettaglioCertificato(byte[] CertificateDer, byte[] CertificateCAPEM)
        {
            OpenWcfChannel();
            string serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            Verifica clr = new Verifica();
            EsitoVerifica retval = null;
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

                return Utils.SerializeObject<EsitoVerifica>(retval);
        }

  


        [WebMethod]
        public string VerificaCertificatoConFileOriginale(byte[] content, byte[] fileOriginale, DateTime? dataverificaDT, bool ancheFile)
        {
            OpenWcfChannel();
            Verifica clr = new Verifica();
            EsitoVerifica retval = null;
            try
            {

                retval = clr.VerificaByteEV(content, fileOriginale, dataverificaDT, client);
                logger.Debug("Verifica cerificato effettuato con successo.");
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
            }

            if (!ancheFile)
                retval.content = null;


            return Utils.SerializeObject<EsitoVerifica>(retval);
        }


        [WebMethod]
        public string VerificaCertificatoFileLocale(string file, DateTime? dataverificaDT, bool ancheFile, bool isCadesPades)
        {
            return VerificaCertificato(System.IO.File.ReadAllBytes(file), dataverificaDT, ancheFile, isCadesPades);
        }

        [WebMethod]
        public string VerificaCertificato(byte[] content, DateTime? dataverificaDT,bool ancheFile, bool isCadesPades)
        {
            OpenWcfChannel();
            Verifica clr = new Verifica();
            EsitoVerifica retval = null;
            try
            {
                if(!isCadesPades)
                    retval = clr.VerificaByteEV(content,null, dataverificaDT, client);
                else
                    retval = clr.VerificaByteEVCompleta(content, dataverificaDT, client);
                logger.Debug("Verifica cerificato effettuato con successo.");
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
            }

            if (!ancheFile)
                retval.content = null;


            return Utils.SerializeObject<EsitoVerifica>( retval);
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
                client = Verifica.createClient(serviceUrl);
            }

            if (client.State != System.ServiceModel.CommunicationState.Opened)
            {
                logger.Debug("Il canale è chiuso, lo apro nuovamente");
                client = Verifica.createClient(serviceUrl);
            }
        }

    }
}