using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using log4net;
using System.Security.Cryptography;
using System.Configuration;
using ArubaSignServicePortBindingQSServiceNS;

namespace ArubaConnector
{
    public class HSMConnector_asspbQSS:I_HSMConnector
    {
        private static ILog logger = LogManager.GetLogger(typeof(HSMConnector_asspbQSS));
 
        public object createClient(string endPoindAddress)
        {
            ArubaSignServicePortBindingQSService ss = new ArubaSignServicePortBindingQSService();
            ss.Url = endPoindAddress;
            ss.Timeout = System.Threading.Timeout.Infinite;
            return ss;
        }


        public bool richiediOTP(string aliasCertificato, string dominioCertificato, object client)
        {
              throw new NotImplementedException();
        }

        public string VisualizzaCertificatoHSM(string aliasCertificato, string dominioCertificato, object client)
        {
            throw new NotImplementedException();
        }

        /*
        private string GetCertificateListAsJsonFormat(ClrVerificationService.FirmaRemota.CertificatoType ct)
        {
            string jsonCertificateList = "";

            string DettaglioB64 = Convert.ToBase64String(System.Text.ASCIIEncoding.Default.GetBytes(ct.dettaglioCertificato));

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

        public string VisualizzaCertificatoHSM(string aliasCertificato, string dominioCertificato, ArubaSignServicePortBindingQSService client)
        {
            if (string.IsNullOrEmpty(dominioCertificato))
                dominioCertificato = ConfigurationManager.AppSettings["HSMCERTDOMAIN"];


            DateTime dataVerifica = DateTime.Now;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            try
            {

                ClrVerificationService.FirmaRemota.CertificatoType ct = client.VisualizzaCertificato(aliasCertificato, dominioCertificato, out dataVerifica);
                return GetCertificateListAsJsonFormat(ct);
            }
            catch (Exception e)
            {
                string errMsg = componiErrore(e, "VisualizzaCertificatoHSM");
                logger.Error(errMsg);
                throw new Exception(errMsg);
            }

        }

        */
        /*
        public static string componiErroreWSFault()
        {
            string retval = string.Empty;
            if (fault != null)
                if (fault.error != null)
                {
                    retval = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", fault.error.errorCode, fault.error.userMessage, fault.error.type.ToString());
                }
            return retval;
        }
        */

        private static string componiErrore(Exception e, string metodo)
        {
            return String.Format("EXCPETION #METODO {0} #MESSAGGIO {1} #STACK {2}#", metodo, e.Message, e.StackTrace);
        }

        public byte[] FirmaFilePADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, object client)
        {

            auth authData = new auth
            {
                otpPwd = otpFirma,
                user = aliasCertificato,
                userPWD = pinCertificato,
                typeOtpAuth = dominioCertificato,
                typeHSM = "COSIGN" // come specificato a pagina 9 del manuale ARSS Developer Guide v1.0
            };

            ArubaSignServicePortBindingQSService wsclient = client as ArubaSignServicePortBindingQSService;
            if (wsclient == null)
                return null;

            signRequestV2 sr = new signRequestV2 { binaryinput = fileDafirmare, transport = typeTransport.BYNARYNET, transportSpecified = true,requiredmark = marcaTemporale, identity = authData, certID = "AS0", profile = null };
            pdfSignApparence pdfsignApp = new pdfSignApparence { page = 1 };
            signReturnV2 retval = wsclient.pdfsignatureV2(sr, pdfsignApp);


            if (retval.status.Equals("OK"))
                return retval.binaryoutput;
            else
            {

                string errorCode = retval.return_code;
                string errorMessage = retval.description;

                string faultMSG = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", errorCode, errorMessage, "ERROR");
                logger.ErrorFormat("Errore in FirmaFilePADES Codice {0} Messaggio {1}", errorCode, errorMessage);
                throw new Exception(faultMSG);

                /*
                    "0001" -> Errore generico nel processo di firma
                    "0002" -> Parametri non corretti per il tipo di trasporto indicato
                    "0003" -> Errore in fase di verifica delle credenziali
                    "0004" -> Errore nel PIN (maggiori dettagli su SignReturnV2.description)
                    "0005" -> Tipo di trasporto non valido
                    "0006" -> Tipo di trasporto non autorizzato
                 */

            }
        }


        public byte[] ControFirmaFileCADES(byte[] fileDafirmare, string aliasCertificatoDaControfirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, object client)
        {
            throw new NotImplementedException();
        }

        public byte[] FirmaFileCADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, bool cofirma, object client)
        {

            auth authData = new auth
            {
                otpPwd = otpFirma,
                user = aliasCertificato,
                userPWD = pinCertificato,
                typeOtpAuth = dominioCertificato,
                typeHSM = "COSIGN" // come specificato a pagina 9 del manuale ARSS Developer Guide v1.0
            };

            ArubaSignServicePortBindingQSService wsclient = client as ArubaSignServicePortBindingQSService;
            if (wsclient == null)
                return null;

            signRequestV2 sr = new signRequestV2 { binaryinput = fileDafirmare, transport = typeTransport.BYNARYNET,transportSpecified = true, requiredmark = marcaTemporale, identity = authData, certID = "AS0", profile = null };
            signReturnV2 retval = null;
            
            if (cofirma) //aggiunta firma parallela.
                retval = wsclient.addpkcs7sign(sr, false);
            else
                retval = wsclient.pkcs7signV2(sr, false);


            if (retval.status.Equals("OK"))
                return retval.binaryoutput;
            else
            {

                string errorCode = retval.return_code;
                string errorMessage = retval.description;

                string faultMSG = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", errorCode, errorMessage, "ERROR");
                logger.ErrorFormat("Errore in FirmaFilePADES Codice {0} Messaggio {1}", errorCode, errorMessage);
                throw new Exception(faultMSG);

                /*
                    "0001" -> Errore generico nel processo di firma
                    "0002" -> Parametri non corretti per il tipo di trasporto indicato
                    "0003" -> Errore in fase di verifica delle credenziali
                    "0004" -> Errore nel PIN (maggiori dettagli su SignReturnV2.description)
                    "0005" -> Tipo di trasporto non valido
                    "0006" -> Tipo di trasporto non autorizzato
                 */

            }
        }

        

     


        public  bool Session_RemoteSign(string SessionToken, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, object client)
        {
            List<byte[]> filesRead = new List<byte[]>();
            SessionToken = SessionToken.ToUpper();


            ArubaSignServicePortBindingQSService wsclient = client as ArubaSignServicePortBindingQSService;
            if (wsclient == null)
                return false;

            auth authData = new auth
            {
                otpPwd = otpFirma,
                user = aliasCertificato,
                userPWD = pinCertificato,
                typeOtpAuth = dominioCertificato,
                typeHSM = "COSIGN" // come specificato a pagina 9 del manuale ARSS Developer Guide v1.0
            };

            pdfSignApparence pdfsignApp = new pdfSignApparence { page = 1 };

            string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MultiSignWorkDir");
            string sessionDir = Path.Combine(cacheDir, SessionToken);
            if (Directory.Exists(sessionDir))
            {
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                if (File.Exists(manifestFile))
                {
                    String manifestXML = File.ReadAllText(manifestFile);
                    Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);


                    string sessionID = wsclient.opensession(authData);
                    if (sessionID == "KO-0001")
                    {
                        string errMsg = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", sessionID, "Errore generico", "ERROR");
                        logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }
                    if (sessionID == "KO-0003")
                    {
                        string errMsg = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", sessionID, "Errore in fase di verifica delle credenziali", "ERROR");
                        logger.Error(errMsg);
                        throw new Exception(errMsg );
                    }
                    if (sessionID == "KO-0004")
                    {
                        string errMsg = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", sessionID, "Errore nel PIN", "ERROR");
                        logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }

                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        byte[] content = File.ReadAllBytes(Path.Combine(sessionDir, FileInformation.OriginalFullName));
                        signReturnV2 retval = null;
                        signRequestV2 sr = new signRequestV2 { binaryinput = content, transport = typeTransport.BYNARYNET,transportSpecified = true, requiredmark = mft.timestamp, identity = authData, certID = "AS0", profile = null };
                        
                        if (mft.SignatureType == Manifest.SignType.CADES)
                        {
                            if (mft.cosign) //aggiunta firma parallela.
                                retval = wsclient.addpkcs7sign(sr, false);
                            else
                                retval = wsclient.pkcs7signV2(sr, false);

                            if (retval.status.Equals("OK"))
                            {
                                filesRead.Add(retval.binaryoutput);
                            }
                            else
                            {
                                string errorCode = retval.return_code;
                                string errorMessage = retval.description;

                                string faultMSG = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", errorCode, errorMessage, "ERROR");
                                logger.ErrorFormat("Errore in pkcs7signV2 o addpkcs7sign /  multi  Codice {0} Messaggio {1}", errorCode, errorMessage);
 
                                filesRead.Add(null);
                            } 
                        }
                        else
                        {
                            retval = wsclient.pdfsignatureV2(sr, pdfsignApp);
                            if (retval.status.Equals("OK"))
                            {
                                filesRead.Add(retval.binaryoutput);
                            }
                            else
                            {
                                string errorCode = retval.return_code;
                                string errorMessage = retval.description;

                                string faultMSG = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", errorCode, errorMessage, "ERROR");
                                logger.ErrorFormat("Errore in pdfsignatureV2_multi  Codice {0} Messaggio {1}", errorCode, errorMessage);
                                filesRead.Add(null);
                            }
                        }
                    }


                    int index = 0;
                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        byte[] content = filesRead[index++];
                        string newName = "signed_" + FileInformation.OriginalFullName;
                        File.WriteAllBytes(Path.Combine(sessionDir, newName), content);
                        FileInformation.SignedFullName = newName;
                    }
                    File.WriteAllText(manifestFile, mft.Serialize());

                    string closeRetval = wsclient.closesession(authData, sessionID);
                    if (closeRetval == "KO-0001")
                    {
                        string errMsg = String.Format("WSFAULT #CODE {0} #MESSAGE {1}  #TYPE {2}#", sessionID, "Errore generico", "ERROR");
                        logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }

                    return true;
                }
            }
            return false;
        }


        #region session common stuff

        public string Session_PutFileToSign(string SessionToken, byte[] FileDafirmare, string FileName)
        {
            return new SessionManager().Session_PutFileToSign(SessionToken,  FileDafirmare, FileName);
        }

        public string Session_GetSessions()
        {
            return new SessionManager().Session_GetSessions();
        }

        public  byte[] Session_GetSignedFile(string SessionToken, string hashFileDaFirmare)
        {
            return new SessionManager().Session_GetSignedFile(SessionToken, hashFileDaFirmare);
        }

        public  string OpenMultiSignSession(bool cosign, bool timestamp, int Type)
        {
            return new SessionManager().OpenMultiSignSession(cosign, timestamp, Type);
        }

        public  bool Session_CloseMultiSign(string SessionToken)
        {
            return new SessionManager().Session_CloseMultiSign(SessionToken);
        }


        public string Session_GetManifest(string SessionToken)
        {
            return new SessionManager().Session_GetManifest(SessionToken);
        }
        #endregion
    }
}