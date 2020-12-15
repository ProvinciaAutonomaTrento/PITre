using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;
using DocsPaVO.documento;
using log4net;
using System.Configuration;
using System.Collections;

namespace ClrVerificationService
{

    public enum EsitoVerificaStatus
    {
        Valid = 0,         //OK
        NotTimeValid = 1,  //Scaduto
        Revoked = 4,       //Revocato
        CtlNotTimeValid = 131072, //Data non corretta
        ErroreGenerico = -1,
        SHA1NonSupportato = -2
    }

    [Serializable]
    public class EsitoVerifica
    {
        public EsitoVerificaStatus status;
        public string message;
        public string errorCode;
        public DateTime? dataRevocaCertificato;
        public string SubjectDN;
        public string SubjectCN;
        public string[] additionalData;
        public DocsPaVO.documento.VerifySignatureResult VerifySignatureResult;
        public byte[] content;
    }

    public class Verifica
    {
        private static ILog logger = LogManager.GetLogger(typeof(Verifica));

        private byte[] readFile(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }

        public Verifica()
        {

        }
        

        public static FirmaDigitale.FirmaDigitalePortTypeClient createClient(string endPoindAddress)
        {
            logger.Debug("INIZIO");
            FirmaDigitale.FirmaDigitalePortTypeClient channel = null;
            try
            {
                #region WCFRuntimeSettings
                var binding = new CustomBinding();
                binding.Name = "FirmaDigitalePortTypeEndpoint2BindingMIO";

                XmlDictionaryReaderQuotas readquota = new XmlDictionaryReaderQuotas
                {
                    MaxStringContentLength = 100000000,
                    MaxArrayLength = 100000000,
                    MaxBytesPerRead = 524288 
                };
                MtomMessageEncodingBindingElement mtomEconding = new MtomMessageEncodingBindingElement
                {
                    MaxReadPoolSize = 640,
                    MaxWritePoolSize = 160,
                    MessageVersion = MessageVersion.Soap12,
                    MaxBufferSize = 2147483647, 
                };

                mtomEconding.ReaderQuotas.MaxArrayLength = 100000000;
                mtomEconding.ReaderQuotas.MaxBytesPerRead = 524288;
                mtomEconding.ReaderQuotas.MaxStringContentLength = 5242880;

                HttpsTransportBindingElement httpsTransposport = new HttpsTransportBindingElement
                {
                    ManualAddressing = false,
                    MaxBufferPoolSize = 524288,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647,
                    AllowCookies = false,
                    AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                    BypassProxyOnLocal = false,
                    HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard,
                    KeepAliveEnabled = true,
                    ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                    Realm = "",
                    TransferMode = System.ServiceModel.TransferMode.Buffered,
                    UnsafeConnectionNtlmAuthentication = false,
                    UseDefaultWebProxy = false,
                    RequireClientCertificate = true 
                };
                binding.Elements.Add(mtomEconding);
                binding.Elements.Add(httpsTransposport);

                EndpointAddress epAddre = new EndpointAddress(endPoindAddress);
                ContractDescription cd = new ContractDescription("FirmaDigitale.FirmaDigitalePortType");

                channel = new FirmaDigitale.FirmaDigitalePortTypeClient(binding, epAddre);
                string certFindData = ConfigurationManager.AppSettings["CERTNAME"];
                channel.ClientCredentials.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, certFindData);
                channel.Endpoint.Behaviors.Add(new MaxFaultSizeBehavior(2147483647));

            }
            catch (Exception e)
            {

                logger.Error("createClient error :" + e.Message + e.StackTrace);
            }
            return channel;
        }
                #endregion


        static string zeniDateConverter(DateTime? dataVerifica)
        {
            DateTime data = DateTime.MinValue;
            if (dataVerifica.HasValue)
                data = dataVerifica.Value;

            string retval = String.Format("{0}{1}{2}{3}{4}{5}",
                data.Year.ToString().Remove(0, 2).PadLeft(2, '0'),
                data.Month.ToString().PadLeft(2, '0'),
                data.Day.ToString().PadLeft(2, '0'),
                data.Hour.ToString().PadLeft(2, '0'),
                data.Minute.ToString().PadLeft(2, '0'),
                data.Second.ToString().PadLeft(2, '0'));
            return retval;
        }

        public CertificateInfo convertFromWarningCertificatoType(FirmaDigitale.WarningCertificatoType certificateType)
        {
            CertificateInfo certifcateInfo = new CertificateInfo();
            certifcateInfo.IssuerName = certificateType.issuer;
            certifcateInfo.SerialNumber = certificateType.serialNumber;
            certifcateInfo.ValidFromDate = certificateType.dataInizioValidita;
            certifcateInfo.ValidToDate = certificateType.dataFineValidita;
            certifcateInfo.SubjectName = certificateType.subject;
            certifcateInfo.SignatureAlgorithm = certificateType.dettaglioCertificato;
            if (certificateType.dataRevocaSpecified)
            {
                certifcateInfo.RevocationDate = certificateType.dataRevoca;
                certifcateInfo.RevocationStatusDescription = "Revocato";

            }
            else if (certificateType.dataSospensioneSpecified)
            {
                certifcateInfo.RevocationDate = certificateType.dataSospensione;
                certifcateInfo.RevocationStatusDescription = "Sospeso";
            }
            return certifcateInfo;
        }


        public CertificateInfo convertFromCertificatoType(FirmaDigitale.CertificatoType certificateType)
        {
            CertificateInfo certifcateInfo = new CertificateInfo();
            certifcateInfo.IssuerName = certificateType.issuer;
            certifcateInfo.SerialNumber = certificateType.serialNumber;
            certifcateInfo.ValidFromDate = certificateType.dataInizioValidita;
            certifcateInfo.ValidToDate = certificateType.dataFineValidita;
            certifcateInfo.SubjectName = certificateType.subject;
            certifcateInfo.SignatureAlgorithm = certificateType.dettaglioCertificato;
            return certifcateInfo;
        }

        public EsitoVerifica VerificaCertificato(byte[] certificateDER, byte[] certificateCAPEM, FirmaDigitale.FirmaDigitalePortTypeClient client)
        {
            logger.Debug("INIZIO");
            List<string> addiData = new List<string>();
            EsitoVerifica ev = new EsitoVerifica();
           // FirmaDigitale.DettaglioFirmaDigitaleType ret = null;
            CertificateInfo ciInfo = new CertificateInfo();
            sbyte? controlloCRLCert =0;
            sbyte? controlloCRLCa =1;
            ciInfo.X509Certificate = certificateDER;
            ciInfo.ThumbPrint = BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(certificateDER)).Replace("-", "");

            controlloCRLCert = null;
            int statusInt=-1;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
            DateTime dataverifica;
            try
            {
                logger.DebugFormat("len {0}", certificateDER.Length);
                FirmaDigitale.CertificatoType certOut = client.VerificaCertificato(certificateDER, certificateCAPEM, controlloCRLCert, controlloCRLCa, out dataverifica);
                logger.Debug("verificaOK");
                ciInfo = convertFromCertificatoType(certOut);
                statusInt = 0;
            }
                
            catch (FaultException<FirmaDigitale.WarningCertificatoType > w)
            {
                string status = w.Detail.status;
                string errMsg = w.Detail.errorMsg;
                logger.Debug(status);
                logger.Debug(errMsg);
                addiData.Add(status);
                addiData.Add(errMsg);
                ciInfo = convertFromWarningCertificatoType(w.Detail);
                Int32.TryParse(w.Detail.status, out statusInt);
                ciInfo.RevocationStatus = statusInt;

                ev.errorCode = w.Detail.status.ToString();
                if (errMsg.ToLower().Contains("revoc"))
                    ev.status = EsitoVerificaStatus.Revoked;
                else
                    ev.status = EsitoVerificaStatus.NotTimeValid;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("errore {0} {1}", e.Message, e.StackTrace);
                ev.message = e.Message;
            }
            
           // ciInfo.ThumbPrint = BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(certificateDER)).Replace("-", "");
            ev.additionalData = addiData.ToArray();

            //quarda che devo fare per restituire il certificateinfo
            List<SignerInfo> retSI = new List<SignerInfo>();
            List<PKCS7Document> p7doc = new List<PKCS7Document>();
            retSI.Add(new SignerInfo { CertificateInfo = ciInfo });
            p7doc.Add(new PKCS7Document { SignersInfo = retSI.ToArray()});
            ev.VerifySignatureResult= new VerifySignatureResult { StatusCode = statusInt, PKCS7Documents = p7doc.ToArray()};
            
            return ev;
        }
        public CertificateInfo GetCertificateInfoFromEv(EsitoVerifica ev)
        {
            //grande assunto
            try
            {
                return ev.VerifySignatureResult.PKCS7Documents[0].SignersInfo[0].CertificateInfo;
            }
            catch
            {
                return new CertificateInfo ();
            }
        }

        public EsitoVerifica VerificaByteEV(byte[] fileContents, byte[] fileOriginale, DateTime? dataverificaDT, FirmaDigitale.FirmaDigitalePortTypeClient client)
        {
            logger.Debug("INIZIO");

            string verbosesgb = ConfigurationManager.AppSettings["VERBOSEDEBUG"];
            bool verboseDebug = false;
            Boolean.TryParse(verbosesgb, out verboseDebug);

            
            EsitoVerifica ev = new EsitoVerifica();
            FirmaDigitale.DettaglioFirmaDigitaleType ret = null;

            string dataVerificaString = string.Empty;
            

            try
            {
                if (dataverificaDT != null)
                {
                    dataVerificaString = zeniDateConverter(dataverificaDT);
                }
                else
                {

                    dataVerificaString = null;
                }
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;


            }
            catch (Exception ex)
            {
                string inner = string.Empty;
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                if (ex.InnerException != null)
                {
                    inner = ex.InnerException.Message;
                }
                ev.message = string.Format("{0},{1}   INNER {2} ", ex.Message, ex.StackTrace, inner);
                return ev;
            }


            try
            {

                sbyte? firmaSHA1WithRSA = 0;
                sbyte? marcaSHA1WithRSA = 0;
                sbyte? controlloFirmeAnnidate = 1;
                logger.Debug("Data Verifica: " + dataVerificaString + " (" + dataverificaDT.ToString() + ")");

                if (fileOriginale != null)
                    ret = client.VerificaFirmaConOriginale(fileContents, fileOriginale, firmaSHA1WithRSA, marcaSHA1WithRSA, dataVerificaString, controlloFirmeAnnidate);
                else
                {
                    FirmaDigitale.DocumentoType doc = new FirmaDigitale.DocumentoType();
                    ret = client.VerificaFirma(fileContents, firmaSHA1WithRSA, marcaSHA1WithRSA, dataVerificaString, controlloFirmeAnnidate, out doc);

                    if (doc != null)
                        ev.content = doc.fileOriginale;
                }
                ev.status = EsitoVerificaStatus.Valid;

              

                if (ret != null)
                    ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, ret); ;

                if (verboseDebug)
                {
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                }
                logger.Debug("Firma OK");

                return ev;
            }

            catch (FaultException<FirmaDigitale.FaultType> f)
            {
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.message = f.Detail.userMessage;
                ev.errorCode = f.Detail.errorCode;
                logger.Error("errore :" + ev.message);
                if (logger.IsInfoEnabled)
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                return ev;

            }

            catch (FaultException<FirmaDigitale.WarningResponseType> w)
            {
                List<string> lstW = new List<string>();
                foreach (FirmaDigitale.WarningType s in w.Detail.WarningFault)
                {
                    lstW.Add(Utils.SerializeObject<FirmaDigitale.WarningType>(s));

                    ev.message = s.errorMsg;
                    ev.errorCode = s.errorCode;
                    ev.SubjectDN = s.SubjectDN;
                    ev.SubjectCN = s.SubjectCN;

                    ev.status = EsitoVerificaStatus.ErroreGenerico;

                    if (s.errorCode == "1426")
                    {
                        ev.status = EsitoVerificaStatus.CtlNotTimeValid;
                    }
                    if (s.errorCode == "1468")
                    {
                        ev.status = EsitoVerificaStatus.SHA1NonSupportato;
                    }
                    if (s.errorCode == "1407")  // check
                    {
                        ev.status = EsitoVerificaStatus.NotTimeValid;
                    }
                    if (s.errorCode == "1408")  // check
                    {
                        ev.status = EsitoVerificaStatus.Revoked;
                        foreach (FirmaDigitale.FirmatarioType ft in w.Detail.DettaglioFirmaDigitale.datiFirmatari)
                        {
                            if (ft.firmatario.dataRevocaCertificato != DateTime.MinValue)
                                ev.dataRevocaCertificato = ft.firmatario.dataRevocaCertificato;
                        }
                    }
                }

                //TEST!!!!
                FirmaDigitale.DettaglioFirmaDigitaleType d = w.Detail.DettaglioFirmaDigitale;
                if (w.Detail.Documento !=null)
                    ev.content = w.Detail.Documento.fileOriginale;

                ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, d); ;
                ev.additionalData = lstW.ToArray();

                logger.Debug("Firma verificata con warning");
                if (logger.IsInfoEnabled)
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                return ev;

            }

            catch (Exception ex)
            {
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.message = string.Format("{0},{1}", ex.Message, ex.StackTrace);
                logger.Error(ev.message);
                if (logger.IsInfoEnabled)
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                return ev;

            }
        }

        public EsitoVerifica VerificaByteEVCompleta(byte[] fileContents, DateTime? dataverificaDT, FirmaDigitale.FirmaDigitalePortTypeClient client)
        {
            logger.Debug("INIZIO");
            logger.Debug("VERIFICA FIRMA COMPLETA");
            string verbosesgb = ConfigurationManager.AppSettings["VERBOSEDEBUG"];
            bool verboseDebug = false;
            Boolean.TryParse(verbosesgb, out verboseDebug);


            EsitoVerifica ev = new EsitoVerifica();
            FirmaDigitale.DettaglioFirmaDigitaleType ret = null;

            string dataVerificaString = string.Empty;


            try
            {
                if (dataverificaDT != null)
                {
                    dataVerificaString = zeniDateConverter(dataverificaDT);
                }
                else
                {

                    dataVerificaString = null;
                }
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;


            }
            catch (Exception ex)
            {
                string inner = string.Empty;
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                if (ex.InnerException != null)
                {
                    inner = ex.InnerException.Message;
                }
                ev.message = string.Format("{0},{1}   INNER {2} ", ex.Message, ex.StackTrace, inner);
                return ev;
            }


            try
            {
                sbyte? controlloFirmeAnnidate = 1;
                logger.Debug("Data Verifica: " + dataVerificaString + " (" + dataverificaDT.ToString() + ")");

                FirmaDigitale.DocumentoType doc = new FirmaDigitale.DocumentoType();
                ret = client.VerificaFirmaCompleta(fileContents, dataVerificaString, controlloFirmeAnnidate, out doc);
                
                ev.status = EsitoVerificaStatus.Valid;

                if (ret != null)
                    ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, ret); ;

                if (verboseDebug)
                {
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                }
                logger.Debug("Firma OK");

                return ev;
            }

            catch (FaultException<FirmaDigitale.FaultType> f)
            {
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.message = f.Detail.userMessage;
                ev.errorCode = f.Detail.errorCode;
                logger.Error("errore in verifica firma completa:" + ev.message);
                if (logger.IsInfoEnabled)
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                return ev;

            }

            catch (FaultException<FirmaDigitale.WarningResponseType> w)
            {
                List<string> lstW = new List<string>();
                foreach (FirmaDigitale.WarningType s in w.Detail.WarningFault)
                {
                    lstW.Add(Utils.SerializeObject<FirmaDigitale.WarningType>(s));

                    ev.message = s.errorMsg;
                    ev.errorCode = s.errorCode;
                    ev.SubjectDN = s.SubjectDN;
                    ev.SubjectCN = s.SubjectCN;

                    ev.status = EsitoVerificaStatus.ErroreGenerico;

                    if (s.errorCode == "1426")
                    {
                        ev.status = EsitoVerificaStatus.CtlNotTimeValid;
                    }
                    if (s.errorCode == "1468")
                    {
                        ev.status = EsitoVerificaStatus.SHA1NonSupportato;
                    }
                    if (s.errorCode == "1407")  // check
                    {
                        ev.status = EsitoVerificaStatus.NotTimeValid;
                    }
                    if (s.errorCode == "1408")  // check
                    {
                        ev.status = EsitoVerificaStatus.Revoked;
                        foreach (FirmaDigitale.FirmatarioType ft in w.Detail.DettaglioFirmaDigitale.datiFirmatari)
                        {
                            if (ft.firmatario.dataRevocaCertificato != DateTime.MinValue)
                                ev.dataRevocaCertificato = ft.firmatario.dataRevocaCertificato;
                        }
                    }
                }

                //TEST!!!!
                FirmaDigitale.DettaglioFirmaDigitaleType d = w.Detail.DettaglioFirmaDigitale;
                if (w.Detail.Documento != null)
                    ev.content = w.Detail.Documento.fileOriginale;

                ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, d); ;
                ev.additionalData = lstW.ToArray();

                logger.Debug("Firma verificata con warning");
                if (logger.IsInfoEnabled)
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                return ev;

            }

            catch (Exception ex)
            {
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.message = string.Format("{0},{1}", ex.Message, ex.StackTrace);
                logger.Error(ev.message);
                if (logger.IsInfoEnabled)
                    logger.Info(Utils.SerializeObject<EsitoVerifica>(ev));
                return ev;

            }
        }


        public static  DateTime convertSimpleDateTime(String DatetimeString)
        {
            DateTime retval = DateTime.MinValue;
            if (String.IsNullOrEmpty ( DatetimeString))
                return retval;
            try
            {
                if (DatetimeString.Length ==13)
                    retval =DateTime.ParseExact(DatetimeString, "yyMMddHHmmssZ", null);
                else
                    retval = DateTime.ParseExact(DatetimeString, "yyyyMMddHHmmssZ", null);

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore convertendo {0} {1} {2}", DatetimeString, e.Message, e.StackTrace);
            }
            return retval;

        }

        private static string convertOidToString(string oid)
        {

             if (string.IsNullOrEmpty(oid))
                return oid;

            switch (oid.Trim())
            {
                case "1.2.840.113549.1.1.5":
                    return "sha1RSA";
                case "1.2.840.113549.1.1.4":
                    return "md5RSA";
                case "1.2.840.10040.4.3":
                    return "sha1DSA";
                case "1.3.14.3.2.29":
                    return "sha1RSA";
                case "1.3.14.3.2.15":
                    return "shaRSA";
                case "1.3.14.3.2.3":
                    return "md5RSA";
                case "1.2.840.113549.1.1.2":
                    return "md2RSA";
                case "1.2.840.113549.1.1.3":
                    return "md4RSA";
                case "1.3.14.3.2.2":
                    return "md4RSA";
                case "1.3.14.3.2.4":
                    return "md4RSA";
                case "1.3.14.7.2.3.1":
                    return "md2RSA";
                case "1.3.14.3.2.13":
                    return "sha1DSA";
                case "1.3.14.3.2.27":
                    return "dsaSHA1";
                case "2.16.840.1.101.2.1.1.19":
                    return "mosaicUpdatedSig";
                case "1.3.14.3.2.26":
                    return "sha1";
                case "1.2.840.113549.2.5":
                    return "md5";
                case "2.16.840.1.101.3.4.2.1":
                    return "sha256";
                case "2.16.840.1.101.3.4.2.2":
                    return "sha384";
                case "2.16.840.1.101.3.4.2.3":
                    return "sha512";
                case "1.2.840.113549.1.1.11":
                    return "sha256RSA";
                case "1.2.840.113549.1.1.12":
                    return "sha384RSA";
                case "1.2.840.113549.1.1.13":
                    return "sha512RSA";
                case "1.2.840.113549.1.1.10":
                    return "RSASSA-PSS";
                case "1.2.840.10045.4.1":
                    return "sha1ECDSA";
                case "1.2.840.10045.4.3.2":
                    return "sha256ECDSA";
                case "1.2.840.10045.4.3.3":
                    return "sha384ECDSA";
                case "1.2.840.10045.4.3.4":
                    return "sha512ECDSA";
                case "1.2.840.10045.4.3":
                    return "specifiedECDSA";
            }
            return oid;
        }

     
        private static DocsPaVO.documento.SignerInfo convertToSignerInfo(DeSign.signer signer)
        {
            //DeSign.signer signer = signerObj as DeSign.signer;
            DocsPaVO.documento.SignerInfo si = new SignerInfo();

            int ErrCode;
            Int32.TryParse(signer.errorCode, out ErrCode);
            byte[] cert = null;
            string thumbPrint = string.Empty;
            //generazione thumbprint
            if (!String.IsNullOrEmpty(signer.certificate))
            {
                cert = Convert.FromBase64String(signer.certificate.Replace("-----BEGIN CERTIFICATE-----", string.Empty).Replace("-----END CERTIFICATE-----", string.Empty));
                thumbPrint = BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(cert)).Replace("-", "");
            }
            string RevocationStatusDescription = signer.errorMessage;
            if (String.IsNullOrEmpty(RevocationStatusDescription))
                RevocationStatusDescription = "Valido";

            si.CertificateInfo = new CertificateInfo
            {
                SerialNumber = signer.serial,
                ValidFromDate = convertSimpleDateTime(signer.certNotBefore),
                ValidToDate = convertSimpleDateTime(signer.certNotAfter),
                IssuerName = "CN="+signer.issuer.CN,
                SubjectName = signer.subject.CN,
                RevocationStatus = ErrCode,
                RevocationStatusDescription = RevocationStatusDescription,
                RevocationDate = convertSimpleDateTime(signer.crlRevocationDate),
                SignatureAlgorithm ="N.D.",
                ThumbPrint = thumbPrint,
                X509Certificate = cert
            };

            if (si.CertificateInfo.RevocationDate != DateTime.MinValue)
                si.CertificateInfo.RevocationStatus = 4; //Revocato

            string codFisc = signer.subject.SER;
            if (!string.IsNullOrEmpty(codFisc) && codFisc.Contains(":"))
                codFisc=codFisc.Split(':')[1];

            si.SubjectInfo = new SubjectInfo
            {
                Organizzazione = signer.subject.O,
                CommonName = signer.subject.CN,
                CodiceFiscale = codFisc,
                Nome = signer.subject.GIVEN,
                Cognome = signer.subject.SUR,
                Country = signer.subject.C,
                SerialNumber = signer.serial,
                CertId = signer.subject.DNQUALIF
            };
            si.SignatureAlgorithm = convertOidToString(signer.digestAlgorithm);
            /*
            logger.Debug("INSERISCO LE INFORMAZIONI SULLO STATO DELLA FIRMA TEST");
            si.errorCode = signer.errorCode;
            si.errorMessage = signer.errorMessage;
            si.status = signer.status;
            */
            if ((signer.signatureTimeStamp != null) && (signer.signatureTimeStamp.timeStampSerial != null) && (signer.signatureTimeStamp.timeStampDate != null))
            {
                List<TSInfo> tsList = new List<TSInfo>();
                TSInfo tsi = new TSInfo
                {
                    dataInizioValiditaCert = convertSimpleDateTime(signer.signatureTimeStamp.certNotBefore),
                    dataFineValiditaCert = convertSimpleDateTime(signer.signatureTimeStamp.certNotAfter),
                    TSdateTime = convertSimpleDateTime(signer.signatureTimeStamp.timeStampDate),
                    TSimprint = signer.signatureTimeStamp.timeStampImprint,
                    TSANameIssuer = signer.signatureTimeStamp.issuer.CN,
                    TSANameSubject = signer.signatureTimeStamp.subject.CN,
                    TSserialNumber = signer.signatureTimeStamp.timeStampSerial,
                    TSType = TsType.PKCS
                };
                tsList.Add(tsi);
                si.SignatureTimeStampInfo = tsList.ToArray();
            }

            //controfirmatari
            List<SignerInfo> csiLst = new List<SignerInfo>();
            foreach (DeSign.countersigner countersigner in signer.countersigner)
            {
                DocsPaVO.documento.SignerInfo csi = new SignerInfo();
                csi.isCountersigner = true;

                if (!String.IsNullOrEmpty(countersigner.certificate))
                {
                    cert = Convert.FromBase64String(countersigner.certificate.Replace("-----BEGIN CERTIFICATE-----", string.Empty).Replace("-----END CERTIFICATE-----", string.Empty));
                    thumbPrint = BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(cert)).Replace("-", "");
                }
                string csiRevocationStatusDescription = countersigner.errorMessage;
                if (String.IsNullOrEmpty(RevocationStatusDescription))
                    RevocationStatusDescription = "Valido";

                csi.CertificateInfo = new CertificateInfo
                {
                    SerialNumber = countersigner.serial,
                    ValidFromDate = convertSimpleDateTime(countersigner.certNotBefore),
                    ValidToDate = convertSimpleDateTime(countersigner.certNotAfter),
                    IssuerName = "CN=" + countersigner.issuer.CN,
                    SubjectName = countersigner.subject.CN,
                    RevocationStatus = ErrCode,
                    RevocationStatusDescription = RevocationStatusDescription,
                    RevocationDate = convertSimpleDateTime(countersigner.crlRevocationDate),
                    SignatureAlgorithm = "N.D.",
                    ThumbPrint = thumbPrint,
                    X509Certificate= cert
                };

                if (csi.CertificateInfo.RevocationDate != DateTime.MinValue)
                    csi.CertificateInfo.RevocationStatus = 4; //Revocato


                codFisc = countersigner.subject.SER;
                if (codFisc.Contains(":"))
                    codFisc = codFisc.Split(':')[1];

                csi.SubjectInfo = new SubjectInfo
                {
                    Organizzazione = countersigner.subject.O,
                    CommonName = countersigner.subject.CN,
                    CodiceFiscale = codFisc,
                    Nome = countersigner.subject.GIVEN,
                    Cognome = countersigner.subject.SUR,
                    Country = countersigner.subject.C,
                    SerialNumber = countersigner.serial,
                    CertId = countersigner.subject.DNQUALIF
                };
                csi.SignatureAlgorithm = convertOidToString(countersigner.digestAlgorithm);
                if ((countersigner.signatureTimeStamp != null) && (countersigner.signatureTimeStamp.timeStampSerial != null) && (countersigner.signatureTimeStamp.timeStampDate != null))
                {
                    List<TSInfo> ctsList = new List<TSInfo>();
                    TSInfo tsi = new TSInfo
                    {
                        dataInizioValiditaCert = convertSimpleDateTime(countersigner.signatureTimeStamp.certNotBefore),
                        dataFineValiditaCert = convertSimpleDateTime(countersigner.signatureTimeStamp.certNotAfter),
                        TSdateTime = convertSimpleDateTime(countersigner.signatureTimeStamp.timeStampDate),
                        TSimprint = countersigner.signatureTimeStamp.timeStampImprint,
                        TSANameIssuer = countersigner.signatureTimeStamp.issuer.CN,
                        TSANameSubject = countersigner.signatureTimeStamp.subject.CN,
                        TSserialNumber = countersigner.signatureTimeStamp.timeStampSerial,
                        TSType = TsType.PKCS
                    };
                    ctsList.Add(tsi);
                    csi.SignatureTimeStampInfo = ctsList.ToArray();
                }
                csiLst.Add(csi);
            }
            if (csiLst.Count > 0)
                si.counterSignatures = csiLst.ToArray();

            return si;
        }


        private static VerifySignatureResult ConvertToVerifySignatureResultUsingDatiGeneraliVerifica(EsitoVerificaStatus status, FirmaDigitale.DettaglioFirmaDigitaleType d)
        {
            DeSign.deSign design = null;
            string verbosesgb = ConfigurationManager.AppSettings["VERBOSEDEBUG"];
            bool verboseDebug = false;
            Boolean.TryParse(verbosesgb, out verboseDebug);

             try
            {
                if (!String.IsNullOrEmpty(d.datiGeneraliVerifica))
                {

                    if (verboseDebug)
                        logger.InfoFormat("Risposta da infocert {0}", d.datiGeneraliVerifica);

                    design = DeSign.deSign.Deserialize(d.datiGeneraliVerifica);
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore deserializzando i dati da infocert {0} {1}", e.Message, e.StackTrace);
                return null;
            }

            VerifySignatureResult vsr = new VerifySignatureResult();


            List<DocsPaVO.documento.SignerInfo> siLst = new List<SignerInfo>();
            List<DocsPaVO.documento.PKCS7Document> p7docsLst = new List<PKCS7Document>();
            string digestAlgo = string.Empty;
            bool badSignature = false;
            foreach (object item in design.signedData)
            {
                //trattasi di signedData
                DeSign.signedData sd = item as DeSign.signedData;
                foreach (DeSign.signer signer in sd.signer)
                {

                    DocsPaVO.documento.SignerInfo si = convertToSignerInfo(signer);

                    if (si.CertificateInfo.RevocationStatus != 0)
                    {

                        vsr.StatusCode = si.CertificateInfo.RevocationStatus;
                        vsr.StatusDescription = si.CertificateInfo.RevocationStatusDescription;
                    }
                    digestAlgo = convertOidToString(signer.digestAlgorithm);
                    
                    if (signer.status == "KO")
                        if (!badSignature)
                            badSignature = true;

                    siLst.Add(si);
                }

            }

            List<TSInfo> tsList = new List<TSInfo>();
            foreach (object item in design.timeStamp)
            {
                //trattasi di signedData
                DeSign.timeStamp ts = item as DeSign.timeStamp;
                TSInfo tsi = new TSInfo
                {
                    dataInizioValiditaCert = convertSimpleDateTime(ts.certNotBefore),
                    dataFineValiditaCert = convertSimpleDateTime(ts.certNotAfter),
                    TSdateTime = convertSimpleDateTime(ts.timeStampDate),
                    TSimprint = ts.timeStampImprint,
                    TSANameIssuer = ts.issuer.CN,
                    TSANameSubject = ts.subject.CN,
                    TSserialNumber = ts.timeStampSerial,
                    TSType = TsType.PKCS
                };
                tsList.Add(tsi);
            }
            if (tsList.Count>0)
                vsr.DocumentTimeStampInfo = tsList.ToArray();

            p7docsLst.Add(new PKCS7Document
            {
                SignersInfo = siLst.ToArray(),
                DocumentFileName = null,
                Level = 0
            });
            vsr.PKCS7Documents = p7docsLst.ToArray();
            vsr.CRLOnlineCheck = true;
            
            if ((badSignature) &&(vsr.StatusCode ==0))
                vsr.StatusCode = -1;

            return vsr;
        }


        private static VerifySignatureResult ConvertToVerifySignatureResultUsingInternalStructures(EsitoVerificaStatus status, FirmaDigitale.DettaglioFirmaDigitaleType d)
        {
            VerifySignatureResult vsr = new VerifySignatureResult();
            List<DocsPaVO.documento.SignerInfo> siLst = new List<DocsPaVO.documento.SignerInfo>();

            string verbosesgb = ConfigurationManager.AppSettings["VERBOSEDEBUG"];
            bool verboseDebug = false;
            Boolean.TryParse(verbosesgb, out verboseDebug);


            if (verboseDebug)
            {
                logger.Debug(d.datiGeneraliVerifica);
            }
            

            if (d.fileMarcatoSpecified)
            {
                if (d.dataVerificaFirmaSpecified)
                    logger.InfoFormat ("data verifica firma  {0}", d.dataVerificaFirma);

                if (d.fileMarcato)
                {
                    logger.Debug("Marcato");
                    TSInfo ts = new TSInfo();
                    if (d.marcaDetached != null)
                    {
                        logger.Debug("marcaDetached !=null");
                        //Gestire la marca
                        //logger.DebugFormat("TSANameIssuer [{0}]", d.marcaDetached.TSANameIssuer);
                        //logger.DebugFormat("TSANameSubject [{0}]", d.marcaDetached.TSANameSubject);
                        //logger.DebugFormat("TSimprint [{0}]", d.marcaDetached.TSimprint);
                        //logger.DebugFormat("TSserialNumber [{0}]", d.marcaDetached.TSserialNumber);
                        

                        if (!String.IsNullOrEmpty(d.marcaDetached.TSANameIssuer))
                            ts.TSANameIssuer = d.marcaDetached.TSANameIssuer;

                        if (!String.IsNullOrEmpty(d.marcaDetached.TSANameSubject))
                            ts.TSANameSubject = d.marcaDetached.TSANameSubject;

                        if (!String.IsNullOrEmpty(d.marcaDetached.TSimprint))
                            ts.TSimprint = d.marcaDetached.TSimprint;

                        if (!String.IsNullOrEmpty(d.marcaDetached.TSserialNumber))
                            ts.TSserialNumber = d.marcaDetached.TSserialNumber;


                        if (d.marcaDetached.TSdateTimeSpecified)
                            ts.TSdateTime = d.marcaDetached.TSdateTime;


                        if (d.marcaDetached.dataFineValiditaCertSpecified)
                            ts.dataFineValiditaCert = d.marcaDetached.dataFineValiditaCert;


                        if (d.marcaDetached.dataInizioValiditaCertSpecified)
                            ts.dataInizioValiditaCert = d.marcaDetached.dataInizioValiditaCert;
                    }
                    else
                    {

                        //default nel caso la try sotto desse errore
                        ts.TSANameIssuer = "Marca non Detached, dati non disponibili";

                        //se questo dato ce l'ho lo valido
                        if (d.dataVerificaFirmaSpecified)
                            ts.TSdateTime = d.dataVerificaFirma;
  
                    }
                }
                else
                {
                }
            }
            
            if (d.datiFirmatari != null)
            {
                foreach (FirmaDigitale.FirmatarioType ft in d.datiFirmatari)
                {
                    //firmatari
                    DocsPaVO.documento.SignerInfo si = ExtractSignerInfo(status, ft.firmatario, ft.marcaFirma);
                    
                    //controfirmatari
                    if (ft.controfirmatario != null)
                    {
                        List<DocsPaVO.documento.SignerInfo> csiLst = new List<DocsPaVO.documento.SignerInfo>();
                        foreach (FirmaDigitale.FirmatarioTypeControfirmatario cft in ft.controfirmatario)
                        {
                            DocsPaVO.documento.SignerInfo csi =ExtractSignerInfo(status, cft.firma,cft.marca);
                            csi.isCountersigner = true;
                            csiLst.Add(csi);
                        }
                        if (csiLst.Count>0)
                            si.counterSignatures= csiLst.ToArray();    
                    }

                    siLst.Add (si);
                }
            }

            List<DocsPaVO.documento.PKCS7Document> p7docsLst = new List<DocsPaVO.documento.PKCS7Document>();
            DocsPaVO.documento.PKCS7Document p7doc = new DocsPaVO.documento.PKCS7Document
            {
                SignersInfo = siLst.ToArray(),
                DocumentFileName = null,
                Level = 0
            };
            p7docsLst.Add(p7doc);
            vsr.PKCS7Documents = p7docsLst.ToArray();
            vsr.CRLOnlineCheck = true;
            return vsr;
        }

        private static DocsPaVO.documento.SignerInfo ExtractSignerInfo(EsitoVerificaStatus status, FirmaDigitale.DatiFirmaType firmatario, FirmaDigitale.MarcaType marcaFirma)
        {
            DocsPaVO.documento.SignerInfo si = new DocsPaVO.documento.SignerInfo();
            if (firmatario != null)
            {
                // firmatari 
                si.CertificateInfo = new DocsPaVO.documento.CertificateInfo
                {
                    ValidFromDate = firmatario.dataInizioValiditaCert,
                    ValidToDate = firmatario.dataFineValiditaCert,
                    RevocationStatusDescription = status.ToString(),
                    RevocationStatus = (int)status,
                    IssuerName = "CN=" + firmatario.cnCertAuthority,
                    SerialNumber= firmatario.serialNumber,
                    SubjectName= firmatario.commonName,
                    SignatureAlgorithm = "N.D.",
                    ThumbPrint ="N.D"
                };
                if (firmatario.dataRevocaCertificato != DateTime.MinValue)
                {
                    si.CertificateInfo.RevocationStatus = 4; //Revocato
                    si.CertificateInfo.RevocationDate = firmatario.dataRevocaCertificato;
                }
                si.SubjectInfo = new DocsPaVO.documento.SubjectInfo
                {
                    CodiceFiscale = firmatario.codiceFiscale,
                    CommonName = firmatario.commonName,
                    CertId = firmatario.distinguishName,
                    Organizzazione = firmatario.organizzazione,
                    SerialNumber = firmatario.serialNumber,
                    Nome = firmatario.nome,
                    Cognome = firmatario.cognome,
                    Country= firmatario.nazione
                };
                si.SignatureAlgorithm = convertOidToString(firmatario.digestAlgorithm);
            }

            if (marcaFirma != null)
            {
                List<TSInfo> tsList = new List<TSInfo>();
                TSInfo tsi = new TSInfo
                {
                    dataInizioValiditaCert = marcaFirma.dataInizioValiditaCert,
                    dataFineValiditaCert = marcaFirma.dataFineValiditaCert,
                    TSdateTime = marcaFirma.TSdateTime,
                    TSimprint = marcaFirma.TSimprint,
                    TSANameIssuer = marcaFirma.TSANameIssuer,
                    TSANameSubject = marcaFirma.TSANameSubject,
                    TSserialNumber = marcaFirma.TSserialNumber,
                    TSType = TsType.PKCS
                };
                tsList.Add(tsi);
                si.SignatureTimeStampInfo = tsList.ToArray();
            }
            return si;
        }




        private static VerifySignatureResult ConvertToVerifySignatureResult(EsitoVerificaStatus status, FirmaDigitale.DettaglioFirmaDigitaleType d)
        {
            string usedgv = ConfigurationManager.AppSettings["USEDATIGENERALIVERIFICA"];
            bool UseDatiGeneraliVerifica = false;
            Boolean.TryParse(usedgv, out UseDatiGeneraliVerifica);

            if (!UseDatiGeneraliVerifica)
                return ConvertToVerifySignatureResultUsingInternalStructures(status, d);
            else 
                return ConvertToVerifySignatureResultUsingDatiGeneraliVerifica(status, d);
        }
    }


    #region MaxFaultSizeBehavior
    public class MaxFaultSizeBehavior : IEndpointBehavior
    {
        private int _size;

        public MaxFaultSizeBehavior(int size)
        {
            _size = size;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MaxFaultSize = _size;
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
    #endregion

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
