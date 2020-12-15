using System;
using System.Collections.Generic;
using System.Web;
using log4net;
using System.ServiceModel.Channels;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

namespace ClrVerificationService
{
    public class HSMConnector
    {
        private static ILog logger = LogManager.GetLogger(typeof(HSMConnector));

        public static FirmaRemota.FirmaRemotaPortTypeClient createClient(string endPoindAddress)
        {
            logger.Debug("INIZIO");
            FirmaRemota.FirmaRemotaPortTypeClient channel = null;
            try
            {
                var binding = new CustomBinding();
                binding.Name = "FirmaRemotaPortTypeEndpoint2BindingMIO";

                XmlDictionaryReaderQuotas readquota = new XmlDictionaryReaderQuotas
                {
                    MaxStringContentLength = 100000000,
                    MaxArrayLength = 100000000,
                    MaxBytesPerRead = 52428800
                };
                MtomMessageEncodingBindingElement mtomEconding = new MtomMessageEncodingBindingElement
                {
                    MaxReadPoolSize = 640,
                    MaxWritePoolSize = 160,
                    MessageVersion = MessageVersion.Soap12,
                    MaxBufferSize = 2147483647,
                };

                mtomEconding.ReaderQuotas.MaxArrayLength = 100000000;
                mtomEconding.ReaderQuotas.MaxBytesPerRead = 52428800;
                mtomEconding.ReaderQuotas.MaxStringContentLength = 52428800;

                HttpsTransportBindingElement httpsTransposport = new HttpsTransportBindingElement
                {
                    ManualAddressing = false,
                    MaxBufferPoolSize = 52428800,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647,
                    AllowCookies = false,
                    AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                    BypassProxyOnLocal = false,
                    HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard,
                    KeepAliveEnabled = true,
                    ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                    Realm = "",
                    TransferMode = System.ServiceModel.TransferMode.Streamed,
                    UnsafeConnectionNtlmAuthentication = false,
                    UseDefaultWebProxy = false,
                    RequireClientCertificate = true
                };

                binding.Elements.Add(mtomEconding);
                binding.Elements.Add(httpsTransposport);

                EndpointAddress epAddre = new EndpointAddress(endPoindAddress);
                ContractDescription cd = new ContractDescription("FirmaRemota.FirmaRemotaPortType");

                channel = new FirmaRemota.FirmaRemotaPortTypeClient(binding, epAddre);
                string certFindData = ConfigurationManager.AppSettings["HSM_CERTNAME"];
                channel.ClientCredentials.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, certFindData);
                channel.Endpoint.Behaviors.Add(new MaxFaultSizeBehavior(2147483647));
            }
            catch (Exception e)
            {
                logger.Error("createClient error :" + e.Message + e.StackTrace);
                throw e;
            }
            return channel;
        }

        public bool richiediOTP(string aliasCertificato, string dominioCertificato, FirmaRemota.FirmaRemotaPortTypeClient client)
        {
            if (string.IsNullOrEmpty(dominioCertificato))
                dominioCertificato = ConfigurationManager.AppSettings["HSMCERTDOMAIN"];
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            try
            {
                string retval = client.RichiestaOTP(aliasCertificato, dominioCertificato);
                if (retval.ToUpperInvariant().Equals("OK"))
                    return true;
                else
                    return false;
            }
            catch (FaultException<FirmaRemota.WSFault> f)
            {
                string errMsg = componiErroreWSFault(f.Detail);
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
            catch (Exception e)
            {
                string errMsg = componiErrore(e, "VisualizzaCertificatoHSM");
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
        }



        private string GetCertificateListAsJsonFormat(ClrVerificationService.FirmaRemota.CertificatoType ct)
        {
            string jsonCertificateList = "";

            string DettaglioB64 = Convert.ToBase64String ( System.Text.ASCIIEncoding.Default.GetBytes ( ct.dettaglioCertificato));

            jsonCertificateList = "[";

            jsonCertificateList = jsonCertificateList + "{";
            jsonCertificateList = jsonCertificateList + "\"Archived\": \"" + "" + "\", ";
            jsonCertificateList = jsonCertificateList + "\"DigestAlgorithm\": \"" + "" + "\", ";
            jsonCertificateList = jsonCertificateList + "\"IssuerName\": \"" + ct.issuer.Replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"SerialNumber\": \"" + ct.serialNumber.Replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"SubjectName\": \"" + ct.subject.Replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"ThumbPrint\": \"" + DettaglioB64.Replace("\"", "\\\"") + "\", ";
            jsonCertificateList = jsonCertificateList + "\"ValidFromDate\": \"" + ct.dataInizioValidita + "\", ";
            jsonCertificateList = jsonCertificateList + "\"ValidToDate\": \"" + ct.dataFineValidita + "\", ";
            jsonCertificateList = jsonCertificateList + "\"Version\": \"" + "" + "\"";
            jsonCertificateList = jsonCertificateList + "} ";
            

            jsonCertificateList = jsonCertificateList + "]";

            return jsonCertificateList;
        }

        public static string componiErroreWSFault(FirmaRemota.WSFault fault)
        {
            string retval = string.Empty;
            if (fault !=null)
                if (fault.error != null)
                {
                    retval = String.Format ("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", fault.error.errorCode, fault.error.userMessage,  fault.error.type.ToString());
                }
            return retval;
        }

        public static string componiErrore(Exception e, string metodo)
        {
            return  String.Format("EXCPETION #METODO {0} #MESSAGGIO {1} #STACK {2}#",metodo, e.Message, e.StackTrace);
        }

        public string VisualizzaCertificatoHSM(string aliasCertificato, string dominioCertificato, FirmaRemota.FirmaRemotaPortTypeClient client)
        {
            if (string.IsNullOrEmpty (dominioCertificato))
                dominioCertificato = ConfigurationManager.AppSettings["HSMCERTDOMAIN"];
                

            DateTime dataVerifica = DateTime.Now;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            try
            {

                ClrVerificationService.FirmaRemota.CertificatoType ct = client.VisualizzaCertificato(aliasCertificato, dominioCertificato, out dataVerifica);
                return GetCertificateListAsJsonFormat(ct);
            }
            catch (FaultException<FirmaRemota.WSFault> f)
            {
                string errMsg = componiErroreWSFault(f.Detail);
                logger.Error(errMsg);
                throw new Exception (errMsg);
            }
            catch (Exception e)
            {
                string errMsg = componiErrore(e, "VisualizzaCertificatoHSM");
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
            
        }
        
        public byte[] FirmaFilePADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, FirmaRemota.FirmaRemotaPortTypeClient client)
        {
            DateTime dataVerifica = DateTime.Now;
            if (string.IsNullOrEmpty(dominioCertificato))
                dominioCertificato = ConfigurationManager.AppSettings["HSMCERTDOMAIN"];
            
            sbyte marca = 0;
            if (marcaTemporale)
                marca = 1;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            try
            {
                ClrVerificationService.FirmaRemota.fileFirmato retval = client.FirmaRemotaPDF(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marca);
                return retval.Value;
            }
            catch (FaultException<FirmaRemota.WSFault> f)
            {
                string errMsg = componiErroreWSFault(f.Detail);
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
            catch (Exception e)
            {
                string errMsg = componiErrore(e, "FirmaFilePADES");
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
        }


        public byte[] ControFirmaFileCADES(byte[] fileDafirmare, string aliasCertificatoDaControfirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, FirmaRemota.FirmaRemotaPortTypeClient client)
        {
            DateTime dataVerifica = DateTime.Now;
            if (string.IsNullOrEmpty(dominioCertificato))
                dominioCertificato = ConfigurationManager.AppSettings["HSMCERTDOMAIN"];

            sbyte marca = 0;
            sbyte firmaParallela = 0;

            if (marcaTemporale)
                marca = 1;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            List<byte[]> fileArr = new List<byte[]>();
            fileArr.Add(fileDafirmare);
            try
            {
                ClrVerificationService.FirmaRemota.DocumentiControFirmatiType retval = client.ControFirma(fileArr.ToArray(), aliasCertificatoDaControfirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marca);
                return retval.docControFirmato[0];
            }            
            catch (FaultException<FirmaRemota.WSFault> f)
            {
                string errMsg = componiErroreWSFault(f.Detail);
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
            catch (Exception e)
            {
                string errMsg = componiErrore(e, "ControFirmaFileCADES");
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
        }

        public byte[] FirmaFileCADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, bool cofirma, FirmaRemota.FirmaRemotaPortTypeClient client)
        {
            DateTime dataVerifica = DateTime.Now;
            if (string.IsNullOrEmpty(dominioCertificato))
                dominioCertificato = ConfigurationManager.AppSettings["HSMCERTDOMAIN"];

            sbyte marca = 0;
            sbyte firmaParallela = 0;

            if (marcaTemporale)
                marca = 1;

            if (cofirma)
                firmaParallela= 1;


            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            try
            {
                ClrVerificationService.FirmaRemota.fileFirmato retval = client.FirmaRemotaP7M(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marca, firmaParallela);
                return retval.Value;
            }
            catch (FaultException<FirmaRemota.WSFault> f)
            {
                string errMsg = componiErroreWSFault(f.Detail);
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
            catch (Exception e)
            {
                string errMsg = componiErrore(e, "FirmaFileCADES");
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }
        }

        public static string Session_PutFileToSign(string SessionToken,byte[] FileDafirmare,string FileName)
        {
            SessionToken= SessionToken.ToUpper();
            //string cacheDir = Path.Combine ( AppDomain.CurrentDomain.BaseDirectory , "MultiSignWorkDir");
            string cacheDir = ConfigurationManager.AppSettings["CACHEDIR"] + "MultiSignWorkDir";
            string sessionDir = Path.Combine (cacheDir,SessionToken);
            if (Directory.Exists(sessionDir ))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText (manifestFile);
                    Manifest.ManifestFile mft =  Manifest.ManifestFile.Deserialize(manifestXML);
                    
                    SHA256 mySHA256 = SHA256Managed.Create();
                    string sha256Hash = BitConverter.ToString(mySHA256.ComputeHash(FileDafirmare)).Replace("-", "");

                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        if (FileInformation.hash.ToUpper () == sha256Hash.ToUpper())
                        {
                            //file esiste nel manifest uscire.
                            return null;
                        }
                    }
                    try
                    {
                        string SignFileName = Guid.NewGuid().ToString() + FileName;
                        mft.FileInformation.Add(new Manifest.MainfestFileInformation { hash = sha256Hash.ToUpper(), OriginalFullName = SignFileName });
                        File.WriteAllBytes(Path.Combine(sessionDir, SignFileName), FileDafirmare);
                        File.WriteAllText(manifestFile, mft.Serialize());
                        return sha256Hash;
                    }
                    catch (Exception e)
                    {
                        //errori scrivendo il file o il manifest.. uscire con null
                        //return null;
                        logger.ErrorFormat("Errore in PutFileToSign  {0} stk {1}", e.Message, e.StackTrace);
                        throw e;
                    }
                }
            }
            return null;
        }


        public static string Session_GetSessions()
        {
            string retval ="";
            //string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MultiSignWorkDir");
            string cacheDir = ConfigurationManager.AppSettings["CACHEDIR"] + "MultiSignWorkDir";
            string[] dirs = Directory.GetDirectories(cacheDir);
            foreach (string dir in dirs)
                retval +=dir+":";
            return retval;
        }

        public static string Session_GetManifest(string SessionToken)
        {
            SessionToken = SessionToken.ToUpper();
            //string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MultiSignWorkDir");
            string cacheDir = ConfigurationManager.AppSettings["CACHEDIR"] + "MultiSignWorkDir";
            string sessionDir = Path.Combine(cacheDir, SessionToken);
            if (Directory.Exists(sessionDir))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText(manifestFile);
                    return manifestXML;
                }
            }
            return null;
        }

        public static bool Session_RemoteSign(string SessionToken, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, FirmaRemota.FirmaRemotaPortTypeClient client)
        {
            List< byte[]> filesRead = new List< byte[]>();
            SessionToken= SessionToken.ToUpper();

            //string cacheDir = Path.Combine ( AppDomain.CurrentDomain.BaseDirectory , "MultiSignWorkDir");
            string cacheDir = ConfigurationManager.AppSettings["CACHEDIR"] + "MultiSignWorkDir";
            string sessionDir = Path.Combine (cacheDir,SessionToken);
            if (Directory.Exists(sessionDir))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText(manifestFile);
                    Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);
                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        byte[] content =File.ReadAllBytes(Path.Combine(sessionDir, FileInformation.OriginalFullName));

                        filesRead.Add( content);
                    }

                    sbyte marca = 0;
                    sbyte firmaParallela = 0;

                    if (mft.timestamp)
                        marca = 1;

                    if (mft.cosign)
                        firmaParallela = 1;

                    ClrVerificationService.FirmaRemota.FileFirmatiType retval=null;
                    try
                    {
                        if (client != null)
                        {
                            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
                            if (mft.SignatureType == Manifest.SignType.CADES)
                            {
                                retval = client.FirmaRemotaMultiplaP7M(filesRead.ToArray(), aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marca, firmaParallela);
                            }
                            else
                            {
                                retval = client.FirmaRemotaMultiplaPDF(filesRead.ToArray(), aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marca);
                            }
                        }
                        else
                        {
                            retval = new ClrVerificationService.FirmaRemota.FileFirmatiType();
                            //dummy
                            retval.documentoFirmato = filesRead.ToArray();
                        }

                    }
                    catch (FaultException<FirmaRemota.WSFault> f)
                    {
                        string errMsg = componiErroreWSFault(f.Detail);
                        logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }
                    catch (Exception e)
                    {
                        logger.ErrorFormat("Errore chimando la firma multipla {0} {1}", e.Message, e.StackTrace);
                        return false;
                    }
                    int index = 0;
                    foreach (byte[] filebyteArray in retval.documentoFirmato )
                    {
                        filesRead[index++] = filebyteArray;
                    }

                    index = 0;
                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        byte[] content = filesRead[index++];
                        string newName= "signed_"+FileInformation.OriginalFullName;
                        File.WriteAllBytes (Path.Combine(sessionDir, newName),content);
                        FileInformation.SignedFullName = newName;
                    }
                    File.WriteAllText(manifestFile, mft.Serialize());

                    return true;
                }
            }
            return false;
        }


        public static byte[] Session_GetSignedFile(string SessionToken, string hashFileDaFirmare)
        {
            SessionToken= SessionToken.ToUpper();
            //string cacheDir = Path.Combine ( AppDomain.CurrentDomain.BaseDirectory , "MultiSignWorkDir");
            string cacheDir = ConfigurationManager.AppSettings["CACHEDIR"] + "MultiSignWorkDir";
            string sessionDir = Path.Combine (cacheDir,SessionToken);
            if (Directory.Exists(sessionDir))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText(manifestFile);
                    Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);
                   
                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        if (FileInformation.hash.ToUpper() == hashFileDaFirmare.ToUpper())
                        {
                            //file esiste nel manifest leggere e uscire.
                            //per test, poi commentare, se no torna solo e sempre quello inviato (echo)
                            //return File.ReadAllBytes(Path.Combine(sessionDir, FileInformation.OriginalFullName));
                            try
                            {
                                return File.ReadAllBytes(Path.Combine(sessionDir, FileInformation.SignedFullName));
                            }
                            catch
                            {
                                logger.ErrorFormat("Il file {0} | {1} non è leggibile", sessionDir, FileInformation.SignedFullName);
                                return null;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static string OpenMultiSignSession(bool cosign, bool timestamp,int Type)
        {
            //string cacheDir = AppDomain.CurrentDomain.BaseDirectory + "MultiSignWorkDir";
            string cacheDir = ConfigurationManager.AppSettings["CACHEDIR"] + "MultiSignWorkDir";
            string sessionToken = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            try
            {
                DirectoryInfo di = System.IO.Directory.CreateDirectory(String.Format("{0}\\{1}", cacheDir, sessionToken));
                Manifest.SignType st =(Manifest.SignType ) Type;
                Manifest.ManifestFile m = new Manifest.ManifestFile { SignatureType = st, timestamp = timestamp, Token = sessionToken, cosign = cosign };
                File.WriteAllText (Path.Combine (di.FullName ,"Manifest.xml"), m.Serialize());
                return sessionToken;
            } catch (Exception e)
            {
                logger.ErrorFormat("Errore in OpenMultiSignSession  {0} stk {1}", e.Message, e.StackTrace);
                throw e;
                //non creata per svariati motivi
                //return null;
            }
        }

        public static bool Session_CloseMultiSign(string SessionToken)
        {
            //string cacheDir = AppDomain.CurrentDomain.BaseDirectory + "MultiSignWorkDir";
            string cacheDir = ConfigurationManager.AppSettings["CACHEDIR"] + "MultiSignWorkDir";

            string dir = String.Format("{0}\\{1}", cacheDir, SessionToken);
            if (System.IO.Directory.Exists(dir))
            {
                try
                {
                    System.IO.Directory.Delete(dir, true);
                    return true;
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Errore in CloseMultiSign {0} stk {1}", e.Message, e.StackTrace);
                    throw e;
                    //return false;
                }
            }
            return false;
        }

    }
}