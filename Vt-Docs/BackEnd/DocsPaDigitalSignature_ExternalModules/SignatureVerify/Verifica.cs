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

namespace SignatureVerify
{
    
    public enum EsitoVerificaStatus
    {
        Valid=0,         //OK
        NotTimeValid=1,  //Scaduto
        Revoked=4,       //Revocato
        SHA1NonSupportato=16,
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
        public DocsPaVO.documento.VerifySignatureResult VerifySignatureResult;
        public byte[] content;
    }
   
    public class Verifica
    {
        private  byte[] readFile(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }

        public Verifica()
        {

        }

        private static FirmaDigitale.FirmaDigitalePortTypeClient createClient(string endPoindAddress)
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
                TransferMode = System.ServiceModel.TransferMode.Streamed,
                UnsafeConnectionNtlmAuthentication = false,
                UseDefaultWebProxy = false,
                RequireClientCertificate = true
            };
        
            binding.Elements.Add(mtomEconding);
            binding.Elements.Add(httpsTransposport);

            EndpointAddress epAddre = new EndpointAddress(endPoindAddress);
            ContractDescription cd = new ContractDescription("FirmaDigitale.FirmaDigitalePortType");

            FirmaDigitale.FirmaDigitalePortTypeClient channel = new FirmaDigitale.FirmaDigitalePortTypeClient(binding, epAddre);
            string certFindData = "client.firma.s.pitre";
            channel.ClientCredentials.ClientCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, certFindData);
            channel.Endpoint.Behaviors.Add(new MaxFaultSizeBehavior(2147483647));
            return channel;
        }
        #endregion

        public string VerificaFile(string fileName, string endPoint, Object[] args)
        {
            return Utils.SerializeObject<EsitoVerifica>(VerificaByteEV(readFile(fileName), endPoint, args));
        }
        public string VerificaByte(byte[] fileContents, string endPoint, Object[] args)
        {
             return Utils.SerializeObject<EsitoVerifica>(VerificaByteEV(fileContents, endPoint, args));
        }

        static string zeniDateConverter(DateTime? dataVerifica)
        {
            DateTime data= DateTime.MinValue;
            if  (dataVerifica.HasValue )
                data= dataVerifica.Value ;

            string retval = String.Format("{0}{1}{2}{3}{4}{5}",
                data.Year.ToString().Remove(0, 2).PadLeft(2, '0'),
                data.Month.ToString().PadLeft(2, '0'),
                data.Day.ToString().PadLeft(2, '0'),
                data.Hour.ToString().PadLeft(2, '0'),
                data.Minute.ToString().PadLeft(2, '0'),
                data.Second.ToString().PadLeft(2, '0'));
            return retval;
        }

        public EsitoVerifica VerificaByteEV(byte[] fileContents, string endPoint, Object[] args)
        {
            EsitoVerifica ev = new EsitoVerifica();
 
            string dataVerificaString = string.Empty;
            DateTime?  dataverificaDT;
            if (args.Length > 0)
            {
                dataverificaDT = args[0] as DateTime?;
                if (dataverificaDT == null)
                {
                    dataVerificaString = args[0] as string;
                    if (dataVerificaString == null)
                    {
                        ev.status =  EsitoVerificaStatus.ErroreGenerico;
                        return ev; 
                    }
                }
                else
                {
                    dataVerificaString = zeniDateConverter(dataverificaDT);
                }
            }

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;
            FirmaDigitale.FirmaDigitalePortTypeClient client = createClient(endPoint);

            FirmaDigitale.DocumentoType doc = new FirmaDigitale.DocumentoType();
            FirmaDigitale.DettaglioFirmaDigitaleType ret = null;
            try
            {
                sbyte? arg1 = 0;
                sbyte? arg2 = 0;
                sbyte? arg3 = 1;
                ret = client.VerificaFirma(fileContents, arg1, arg2, dataVerificaString, arg3, out doc);
                ev.status = EsitoVerificaStatus.Valid;

                ev.content = doc.fileOriginale;
                if (ret !=null)
                    ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, ret); ;
                return ev; 
            }
            catch (FaultException<FirmaDigitale.FaultType> f)
            {
                ev.status = EsitoVerificaStatus.ErroreGenerico;
                ev.message = f.Detail.userMessage;
                ev.errorCode = f.Detail.errorCode;
                return ev; 
                
            }

            catch (FaultException<FirmaDigitale.WarningResponseType> w)
            {
                List <string> lstW = new List<string> ();
                foreach (FirmaDigitale.WarningType s in w.Detail.WarinigFault)
                {
                    lstW.Add( Utils.SerializeObject<FirmaDigitale.WarningType>(s));
                  
                    ev.message = s.errorMsg;
                    ev.errorCode = s.errorCode;
                    ev.SubjectDN = s.SubjectDN;
                    ev.SubjectCN = s.SubjectCN;

                    ev.status = EsitoVerificaStatus.ErroreGenerico ;

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
                ev.content = w.Detail.Documento.fileOriginale;
                ev.VerifySignatureResult = ConvertToVerifySignatureResult(ev.status, d); ;
                ev.additionalData = lstW.ToArray();
                return ev;

            }
        }

        private static string convertOidtoString(string oidVal)
        {
            switch (oidVal)
            {
                case "1.2.840.113549.1.1.5":
                    return "Sha1RSA";
                case "1.2.840.113549.1.1.4":
                    return "Md5RSA";
                case "1.2.840.10040.4.3":
                    return "Sha1DSA";
                case "1.3.14.3.2.29":
                    return "Sha1RSA";
                case "1.3.14.3.2.15":
                    return "ShaRSA";
                case "1.3.14.3.2.3":
                    return "Md5RSA";
                case "1.2.840.113549.1.1.2":
                    return "Md2RSA";
                case "1.2.840.113549.1.1.3":
                    return "Md4RSA";
                case "1.3.14.3.2.2":
                    return "Md4RSA";
                case "1.3.14.3.2.4":
                    return "Md4RSA";
                case "1.3.14.7.2.3.1":
                    return "Md2RSA";
                case "1.3.14.3.2.13":
                    return "Sha1DSA";
                case "1.3.14.3.2.27":
                    return "DsaSHA1";
                case "2.16.840.1.101.2.1.1.19":
                    return "MosaicUpdatedSig";
                case "1.3.14.3.2.26":
                    return "Sha1";
                case "1.2.840.113549.2.5":
                    return "Md5";
                case "2.16.840.1.101.3.4.2.1":
                    return "Sha256";
                case "2.16.840.1.101.3.4.2.2":
                    return "Sha384";
                case "2.16.840.1.101.3.4.2.3":
                    return "Sha512";
                case "1.2.840.113549.1.1.11":
                    return "Sha256RSA";
                case "1.2.840.113549.1.1.12":
                    return "Sha384RSA";
                case "1.2.840.113549.1.1.13":
                    return "Sha512RSA";
                case "1.2.840.113549.1.1.10":
                    return "RSASSA-PSS";
                case "1.2.840.10045.4.1":
                    return "Sha1ECDSA";
                case "1.2.840.10045.4.3.2":
                    return "Sha256ECDSA";
                case "1.2.840.10045.4.3.3":
                    return "Sha384ECDSA";
                case "1.2.840.10045.4.3.4":
                    return "Sha512ECDSA";
                case "1.2.840.10045.4.3":
                    return "SpecifiedECDSA";
            }
            return oidVal;
        }

        private static VerifySignatureResult ConvertToVerifySignatureResult(EsitoVerificaStatus status, FirmaDigitale.DettaglioFirmaDigitaleType d)
        {
            VerifySignatureResult vsr = new VerifySignatureResult();
            List<DocsPaVO.documento.SignerInfo> siLst = new List<DocsPaVO.documento.SignerInfo>();
            if (d.datiFirmatari != null)
            {
                foreach (FirmaDigitale.FirmatarioType ft in d.datiFirmatari)
                {
                    DocsPaVO.documento.SignerInfo si = new DocsPaVO.documento.SignerInfo();
                    si.CertificateInfo = new DocsPaVO.documento.CertificateInfo
                    {
                        ValidFromDate = ft.firmatario.dataInizioValiditaCert,
                        ValidToDate = ft.firmatario.dataFineValiditaCert,
                        RevocationDate = ft.firmatario.dataRevocaCertificato,
                        RevocationStatus = (int)status,
                        RevocationStatusDescription = status.ToString(),
                        IssuerName = "CN="+ft.firmatario.cnCertAuthority,
                        SignatureAlgorithm= "N.D."
                    };

                    si.SubjectInfo = new DocsPaVO.documento.SubjectInfo
                    {
                        CodiceFiscale = ft.firmatario.codiceFiscale,
                        CommonName = ft.firmatario.commonName,
                        CertId = ft.firmatario.distinguishName,
                        Organizzazione = ft.firmatario.organizzazione,
                        SerialNumber = ft.firmatario.serialNumber,
                    };
                    si.SignatureAlgorithm = convertOidtoString(ft.firmatario.digestAlgorithm);
                    siLst.Add(si);
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
            vsr.StatusCode = (int)status;
            vsr.StatusDescription = status.ToString();
            vsr.CRLOnlineCheck = true;
            return vsr;
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
