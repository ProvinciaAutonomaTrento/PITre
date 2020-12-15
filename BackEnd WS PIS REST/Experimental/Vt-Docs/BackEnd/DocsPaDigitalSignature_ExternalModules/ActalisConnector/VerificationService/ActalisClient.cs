using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using log4net;
using ActalisConnector.VerificaRemota;
using DocsPaVO.documento;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.ServiceModel;

namespace ActalisConnector
{
    public class CrlVerificationService
    {
        bool certExpired = false;
        bool revoked = false;
        public enum EsitoVerificaStatus
        {
            Valid = 0,         //OK
            NotTimeValid = 1,  //Scaduto
            Revoked = 4,       //Revocato
            Tampered = 8,
            SHA1NonSupportato = 16,
            ErroreGenerico = -1
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
            public VerifySignatureResult VerifySignatureResult;
            public byte[] content;
        } 


        private static ILog logger = LogManager.GetLogger(typeof(CrlVerificationService));

        static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            // Ignore errors
            return true;
        }

        public static VerificationServiceClient createClient(string endPoindAddress)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
            //string svcUrl = ConfigurationManager.AppSettings["SERVICE_URL"];
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress remoteAddress = new EndpointAddress(endPoindAddress);
            VerificationServiceClient client = new VerificationServiceClient(binding, remoteAddress);
            return client;
        }

        public EsitoVerifica VerificaCertificato(byte[] certificateDER, byte[] certificateCAPEM, VerificaRemota.VerificationServiceClient client)
        {
            EsitoVerifica ev = new EsitoVerifica();
            CertificateInfo ciInfo = new CertificateInfo();
            int statusInt=-1;
            try
            {
                ReturnCertificateValidation certVal =  client.VerifyCertificate(Convert.ToBase64String(certificateDER), DateTime.Now);
                ciInfo = toCertificateInfo(certVal.certificate);
                statusInt =0;
                ev.status = EsitoVerificaStatus.Valid;
                if (certVal.certificate.certRevocation.certRevoked)
                {
                    statusInt=-1;
                    ev.status = EsitoVerificaStatus.Revoked;
                    ev.errorCode = certVal.certificate.certRevocation.revocationReason;
                    revoked = true;
                }

                if (!certVal.certificate.certTimeValid)
                {
                    ev.status = EsitoVerificaStatus.NotTimeValid;
                    certExpired = true;
                }

            } catch (Exception e)
            {
                logger.ErrorFormat("errore {0} {1}", e.Message, e.StackTrace);
                ev.message = e.Message;
                ev.status = EsitoVerificaStatus.ErroreGenerico;
            }

            //quarda che devo fare per restituire il certificateinfo
            List<SignerInfo> retSI = new List<SignerInfo>();
            List<PKCS7Document> p7doc = new List<PKCS7Document>();
            retSI.Add(new SignerInfo { CertificateInfo = ciInfo });
            p7doc.Add(new PKCS7Document { SignersInfo = retSI.ToArray() });
            ev.VerifySignatureResult = new VerifySignatureResult { StatusCode = statusInt, PKCS7Documents = p7doc.ToArray() };
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
                return new CertificateInfo();
            }
        }

        public string verifica(byte[] fileContents, DateTime? dataverificaDT, bool ancheFile, VerificaRemota.VerificationServiceClient client)
        {
            bool dateSpecified = dataverificaDT.HasValue;
            DateTime dt = DateTime.Now;

            if (dateSpecified)
                dt = dataverificaDT.Value;

            ActalisConnector.Utils.SignFormat signFormat;
            bool fileSigned = Utils.IsFileSigned(fileContents, out signFormat);
            Return ret = new Return ();
            byte[] originalFile = null;
            //string error = null;
            EsitoVerifica retval = new EsitoVerifica();
            try
            {
                switch (signFormat)
                {
                    case ActalisConnector.Utils.SignFormat.CAdES:
                        ret = client.VerifyP7M(fileContents, dt);
                        if (ancheFile)
                            originalFile = ret.originalFile;

                        retval = getResult(ret, originalFile);
                        break;
                    case ActalisConnector.Utils.SignFormat.PAdES:
                        ret = client.VerifyPDF(fileContents, dt);
                        if (ancheFile)
                            originalFile = fileContents;

                        retval = getResult(ret, originalFile);
                        break;
                    case ActalisConnector.Utils.SignFormat.XAdES:
                        ret = client.VerifyXML(fileContents, dt);
                        if (ancheFile)
                            originalFile = ret.originalFile;

                        retval = getResult(ret, originalFile);
                        break;
                    default:
                        retval.message = "Formato file non riconsciuto";
                        retval.status = EsitoVerificaStatus.ErroreGenerico;
                        break;
                }


            } catch (Exception ex)
            {
                logger.ErrorFormat("errore {0} {1}", ex.Message, ex.StackTrace);
                retval.message = ex.Message;
                retval.status = EsitoVerificaStatus.ErroreGenerico;
            }

            return Utils.SerializeObject<EsitoVerifica>(retval);
        }

        private EsitoVerifica getResult(Return ret, byte[] originalFile)
        {
            VerifySignatureResult vsr = ConvertToVerifySignatureResult(ret);
            
            EsitoVerifica retval = new EsitoVerifica { VerifySignatureResult = vsr, content = originalFile };
            
            retval.status = EsitoVerificaStatus.Valid;
            
            if (revoked)
                retval.status = EsitoVerificaStatus.Revoked;
            else if (certExpired)
                retval.status = EsitoVerificaStatus.NotTimeValid;


            return retval;
        }


        private static TSInfo ConvertFromActalisTS(signerTimeStamp sTs)
        {
            if (sTs == null)
                return null;
            TSInfo tsi = new TSInfo
            {
                TSdateTime = sTs.tsGenTime,
                TSimprint = sTs.tsDigestMessageImprint,
                TSANameIssuer = sTs.tsTsaName,
                TSserialNumber = sTs.tsSerialNumber,
                TSType = TsType.PKCS
            };
            return tsi;
        }

        private   List<SignerInfo> ConvertFromActalisSigner(signer[] actalisSigners)
        {
            List<SignerInfo> retval = new List<SignerInfo>();
            foreach (signer signer in actalisSigners)
            {
                SignerInfo si = toDPASignerInfo(signer);
                si.isCountersigner = false;
                retval.Add(si); 
            }
            return retval;
        }

        private  SignerInfo toDPASignerInfo(signer ActalisSigner)
        {
            CertificateInfo ci = toCertificateInfo(ActalisSigner.certificate);
            TSInfo[] tsi = { ConvertFromActalisTS(ActalisSigner.timeStamp) };
            SignerInfo si = new SignerInfo();
            try
            {
                si.CertificateInfo = ci;
                si.SignatureAlgorithm = ActalisSigner.signatureInfo.sigAlgorithm;
                si.SigningTime = ActalisSigner.signatureInfo.sigTime;
                si.SignatureTimeStampInfo = tsi;


                //gestione dei controfirmatari
                if (ActalisSigner.counterSignature != null)
                {
                    List<SignerInfo> DPACounterSignerList = new List<SignerInfo>();
                    foreach (signer CounterSigner in ActalisSigner.counterSignature)
                    {
                        //Ricorsione
                        SignerInfo csi = toDPASignerInfo(CounterSigner);
                        csi.isCountersigner = true;
                        DPACounterSignerList.Add(csi);
                    }
                    si.counterSignatures = DPACounterSignerList.ToArray();
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore in toDPASignerInfo {0} stk {1}", ex.Message, ex.StackTrace);
            }
            return si;
        }

        private CertificateInfo toCertificateInfo(certificate actalisCert)
        {
            // Costruzione delle informazioni sul certificato.
            CertificateInfo ci = new CertificateInfo();
            try
            {
                //controllare sta cosa qui sotto
                ci.X509Certificate = Convert.FromBase64String(actalisCert.certCert);
                ci.ThumbPrint = actalisCert.certFinger256;
                ci.IssuerName = actalisCert.certIssuer.issuerName;
                ci.SubjectName = actalisCert.certName;
                ci.RevocationStatus = (actalisCert.certRevocation.certRevoked == false) ? 0 : 1;
                ci.SerialNumber = actalisCert.certSerialNo;
                ci.ValidFromDate = actalisCert.certValFrom;
                ci.ValidToDate = actalisCert.certValUntil;
                ci.SignatureAlgorithm = "N.A.";

                if (actalisCert.certRevocation.certRevoked)
                {
                    ci.RevocationStatusDescription = actalisCert.certRevocation.revocationReason;
                    ci.RevocationDate = actalisCert.certRevocation.revocationDate;
                    revoked = true;
                }
                if (!actalisCert.certTimeValid)
                    certExpired = true;


            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore in toCertificateInfo {0} stk {1}", ex.Message, ex.StackTrace);
            }
            return ci;
        }

        private  VerifySignatureResult ConvertToVerifySignatureResult(Return actalisReturn)
        {
            VerifySignatureResult vsr = new VerifySignatureResult();
            try
            {
                List<DocsPaVO.documento.SignerInfo> siLst = null;
                SignerInfo[] signersInfo = null;
                if (actalisReturn.signers != null)
                {
                    siLst = ConvertFromActalisSigner(actalisReturn.signers);
                    signersInfo = siLst.ToArray();
                }
                List<DocsPaVO.documento.PKCS7Document> p7docsLst = new List<DocsPaVO.documento.PKCS7Document>();
                DocsPaVO.documento.PKCS7Document p7doc = new DocsPaVO.documento.PKCS7Document
                {
                    SignersInfo = signersInfo,
                    DocumentFileName = null,
                    Level = 0
                };
                p7docsLst.Add(p7doc);
                vsr.PKCS7Documents = p7docsLst.ToArray();


                /// ATTNEZIONE DA GESTIRE
                /// actalisReturn.error quando tutto va bene torna OK, mentre non dovrebbe tornare nulla
                /// volendo possiamo mettere che se torna OK, la stringa in actalisReturn.error viene eliminata.
                /// estratto dalla documentazione VOL Pagina 4
                /// Error  1..1  Stringa   Eventuale codice di errore. Vuoto se non si è verificato nessun errore nella validazione. 
                /// decommentare codice sotto per risolvere il bug
                /*
                if (actalisReturn.error.Equals ("OK"))
                    actalisReturn.error="";
                */
                vsr.StatusCode = String.IsNullOrEmpty(actalisReturn.error) ? 0 : 1;
                vsr.StatusDescription = actalisReturn.error;
                vsr.CRLOnlineCheck = true;
            }
            catch (Exception ex)
            {
                string err = string.Format ("Errore in toCertificateInfo {0} stk {1}", ex.Message, ex.StackTrace);
                logger.ErrorFormat(err);
                vsr.StatusDescription = err;
                vsr.StatusCode = -1;
            }
            return vsr;
        }


        /*
        private VerificationResult GetResult(Return result, byte[] originalFile)
        {
            List<Signer> signers = new List<Signer>();

            // Recupero delle informazioni su firme e controfirme.
            signers = this.GetCounterSignarures(result.signers);
            
            // Inizializzazione e restituzione del risultato.
            VerificationResult verificationResult = new VerificationResult(originalFile, signers);

            return verificationResult;
        }

        private List<Signer> GetCounterSignarures(signer[] signers)
        {
            // Lista delle informazioni da restituire.
            List<Signer> retVal = new List<Signer>();

            // Recupero delle informazioni sulle firme apposte al documento.
            foreach (var signer in signers)
            {
                // Costruzione delle informazioni sul certificato.
                CertificateInfo certificateInfo = new CertificateInfo()
                {
                    Content = signer.certificate.certCert,
                    Hash = signer.certificate.certFinger256,
                    IssuerName = signer.certificate.certIssuer.issuerName,
                    IssuerDistinguishName = signer.certificate.certIssuer.issuerDistinguishName,
                    Usage = signer.certificate.certKeyUsage,
                    Name = signer.certificate.certName,
                    Policies = signer.certificate.certPolicy.Select(p => new Policy() { Description = p.certPolText }).ToList(),
                    PublicKey = signer.certificate.certPublicKey.certPublicKey1,
                    Revoked = signer.certificate.certRevocation.certRevoked,
                    RevocationDate = signer.certificate.certRevocation.revocationDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    SerialNumber = signer.certificate.certSerialNo,
                    Valid = signer.certificate.certValid,
                    ReleaseDate = signer.certificate.certValFrom.ToString("dd/MM/yyyy HH:mm:ss"),
                    ExpirationDate = signer.certificate.certValUntil.ToString("dd/MM/yyyy HH:mm:ss"),
                    Version = signer.certificate.certVersion
                };

                // Costruzione delle informazioni sulla firma.
                Signer tempSigner = new Signer()
                {
                    Certificate = certificateInfo,
                    Corrupted = signer.signatureInfo.sigCorrupted,
                    Date = signer.signatureInfo.sigTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    Delib45Warnings = signer.signatureInfo.sigDelib45Valid.warnDelib45 && signer.signatureInfo.sigDelib45Valid.warnDelib45 ? signer.signatureInfo.sigDelib45Valid.warnDelib45Cause : String.Empty,
                    SignatureAlgorithm = signer.signatureInfo.sigAlgorithm
                };

                // Recupero ricorsivo delle eventuali controfirme della firma.
                if (signer.counterSignature != null)
                {
                    tempSigner.CounterSignatures = this.GetCounterSignarures(signer.counterSignature);
                }

                // Aggiunta delle informazioni sulla firma, alla lista delle firme.
                retVal.Add(tempSigner);

            }

            // Restituzione della lista di firme.
            return retVal;

        }
        */

    }
}