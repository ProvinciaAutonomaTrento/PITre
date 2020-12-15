using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using log4net;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;

namespace CRLService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://nttdata.com/2013/CRLSvc")]
    //[WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class ClrVerification : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(ClrVerification));

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
            return VerificaCertificato(System.IO.File.ReadAllBytes(file), DateTime.Now, true);
        }

        [WebMethod]
        public string VerificaCertificatoFileLocale(string file, DateTime? dataverificaDT, bool ancheFile)
        {
            return VerificaCertificato(System.IO.File.ReadAllBytes(file), dataverificaDT, ancheFile);
        }
        [WebMethod]
        public string VerificaCertificatoNC(byte[] content, DateTime? dataverificaDT, bool ancheFile,bool noCache)
        {
            SignatureVerify.Verifica.EsitoVerifica retval = null;
            SignatureVerify.Verifica v = new SignatureVerify.Verifica();
            try
            {
                string forceDownload = noCache.ToString();
                string cachePath = HttpContext.Current.Server.MapPath("~/Cache");
                Object[] arg = { null, cachePath,forceDownload };
                if (dataverificaDT != null)
                    arg[0] = dataverificaDT.Value;

                retval = v.VerificaByteEV(content, "", arg);
                logger.Debug("Verifica cerificato effettuato con successo.");
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
            }

            if (!ancheFile)
                retval.content = null;


            return Utils.SerializeObject<SignatureVerify.Verifica.EsitoVerifica>(retval);

        }

        [WebMethod]
        public string VerificaCertificato(byte[] content, DateTime? dataverificaDT, bool ancheFile)
        {
            SignatureVerify.Verifica.EsitoVerifica  retval = null;
            SignatureVerify.Verifica v = new SignatureVerify.Verifica();
            try
            {
                string forceDownload = ConfigurationManager.AppSettings["FORCE_DOWNLOAD"];
                string cachePath = HttpContext.Current.Server.MapPath("~/Cache");
                Object[] arg = { null, cachePath ,forceDownload };
                if (dataverificaDT!=null)
                     arg[0] =  dataverificaDT.Value ;

                retval = v.VerificaByteEV(content, "", arg);
                logger.Debug("Verifica cerificato effettuato con successo.");

                
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
            }

            if (!ancheFile)
                retval.content = null;


            return Utils.SerializeObject<SignatureVerify.Verifica.EsitoVerifica>(retval);
        }

        /// <summary>
        /// Servizio di test per la verifica del certificato pretende un file certifificato cert.der (binario) nella cartella del WS
        /// </summary>
        /// <returns>stringa che ritorna lo status del certificato e le sue informazioni</returns>
        [WebMethod]
        public string ProvaServizioCERT()
        {
            string file = Server.MapPath("~") + "\\cert.der";
            string serviceUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            logger.Debug("Effettuo la verifica certificato");
            SignatureVerify.Verifica.EsitoVerifica retval = null;
            SignatureVerify.Verifica v = new SignatureVerify.Verifica();
            try
            {
                byte[] ca = new byte[0];
                byte[] cer = System.IO.File.ReadAllBytes(file);
                
                string forceDownload = ConfigurationManager.AppSettings["FORCE_DOWNLOAD"];
                string cachePath = HttpContext.Current.Server.MapPath("~/Cache");
                Object[] arg = { null, cachePath, forceDownload };

                retval = v.VerificaCertificato(cer, ca, arg);
                logger.Debug("Verifica cerificato effettuato.");
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
                return e.Message;
            }


            return Utils.SerializeObject<SignatureVerify.Verifica.EsitoVerifica>(retval);

        }


        /// <summary>
        /// Servizio che ritorna i dettagli del certificato
        /// </summary>
        /// <param name="CertificateDer">Certificato da verificare in Formato DER (binario)</param>
        /// <param name="CertificateCAPEM">Non utilizzato</param>
        /// <returns>Stringa che ritorna lo status del certificato e le sue informazioni</returns>
        [WebMethod]
        public string DettaglioCertificato(byte[] CertificateDer, byte[] CertificateCAPEM)
        {
            SignatureVerify.Verifica.EsitoVerifica retval = null;
            SignatureVerify.Verifica v = new SignatureVerify.Verifica();
            try
            {
                string forceDownload = ConfigurationManager.AppSettings["FORCE_DOWNLOAD"];
                string cachePath = HttpContext.Current.Server.MapPath("~/Cache");
                Object[] arg = { null, cachePath, forceDownload };

                //chiamata alla funzione CORE di verifica del certificato
                retval = v.VerificaCertificato(CertificateDer, CertificateCAPEM,arg);
                logger.Debug("Verifica cerificato effettuato con successo.");
            }
            catch (Exception e)
            {
                logger.Error("Verifica cerificato Errore :" + e.Message + e.StackTrace);
                return e.Message;
            }

            //trasforma in stringa l'oggetto retval
            return Utils.SerializeObject<SignatureVerify.Verifica.EsitoVerifica>(retval);

        }

        /// <summary>
        /// Data una lista di base64 con i certificati, controlla uno per uno i certificati
        /// </summary>
        /// <param name="CertificateBase64">lista di stringhe base 64 rappresentanti i cetificati</param>
        /// <returns>lista di stringhe con il risultato dei check</returns>
        [WebMethod]
        public string[] BatchDettaglioCertificato(string[] CertificateBase64)
        {
            List<String> retval = new List<string>();
            SignatureVerify.Verifica v = new SignatureVerify.Verifica();
            SignatureVerify.Verifica.EsitoVerifica retvalCert = null;

            byte[] CertificateCAPEM = null;
            foreach (string certStr in CertificateBase64)
            {
                //converto il certificato base64 in binario
                byte[] CertificateDer = Convert.FromBase64String(certStr);
                try
                {

                    string forceDownload = ConfigurationManager.AppSettings["FORCE_DOWNLOAD"];
                    string cachePath = HttpContext.Current.Server.MapPath("~/Cache");
                    Object[] arg = { null, cachePath, forceDownload };

                    //chiamata alla funzione CORE di verifica del certificato
                    retvalCert = v.VerificaCertificato(CertificateDer, CertificateCAPEM, arg);
                    logger.Debug("Verifica cerificato effettuato con successo.");
                    retval.Add(Utils.SerializeObject<DocsPaVO.documento.Internal.CertificateInfo>(v.GetCertificateInfoFromEv(retvalCert)));
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
        public string ConnessioneServizio()
        {
            //non ci sta connessione dato che è locale
            string retval = "OK";          
            return retval;
        }
    }

    public enum EsitoVerificaStatus
    {
        Valid = 0,         //OK
        NotTimeValid = 1,  //Scaduto
        Revoked = 4,       //Revocato
        CtlNotTimeValid = 131072, //Data non corretta
        ErroreGenerico = -1,
        SHA1NonSupportato = -2
    }

  

    public class Utils
    {
        static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static String SerializeObject<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(xmlTextWriter, pObject, ns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }
    }

}