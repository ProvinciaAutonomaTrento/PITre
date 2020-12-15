using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using log4net;

namespace BusinessLogic.Documenti.DigitalSignature
{
	public class RemoteSignature
	{
        private static ILog logger = LogManager.GetLogger(typeof(RemoteSignature));
        public enum SignType
        {
            /// <remarks/>
            CADES,
            /// <remarks/>
            PADES,
        }

        bool _timeStamp = false;
        bool _coSign = false;
        string  _aliasCertificatoSingolo, _dominioCertificatoSingolo,_certificatePin;
        SignType _type;
        public RemoteSignature(string AliasCertificato, string DominioCertificato, string CertificatePin, SignType SignType,bool TimeStamp ,bool Cosign)
        {
            _aliasCertificatoSingolo = AliasCertificato;
            _dominioCertificatoSingolo = DominioCertificato;
            _type = SignType;
            _coSign = Cosign;
            _certificatePin = CertificatePin;
            _timeStamp = TimeStamp;
        }

        public class MultiSign 
        {
            string _SessionToken, _aliasCertificato, _dominioCertificato;
            bool _cofirma, _timestamp;

            public bool Cofirma
            {
                get { return _cofirma; }
            }

            public string SessionToken
            {
                get { return _SessionToken; }
            }

            externalSign.HSMService hsmSvc = null;
            SignType tipoFirma;

            public SignType TipoFirma
            {
                get { return tipoFirma; }
            }

            List<String> fileListToSign = new List<string>();

            public MultiSign(bool cosign, bool timestamp, SignType type)
            {
                hsmSvc = new externalSign.HSMService();
                hsmSvc.Url = Config.HSMServiceUrl();
                hsmSvc.Timeout = 100000;
                tipoFirma = type;
                _cofirma = cosign;
                _SessionToken = hsmSvc.Session_OpenMultiSign(cosign, timestamp, (externalSign.SignType)type);
            }
                        
            public MultiSign(string AliasCertificato, string DominioCertificato,  string SessionToken, bool cofirma)
            {
                hsmSvc = new externalSign.HSMService();
                hsmSvc.Url = Config.HSMServiceUrl();
                hsmSvc.Timeout = 100000;
                _aliasCertificato = AliasCertificato;
                _dominioCertificato = DominioCertificato;
                _SessionToken = SessionToken;
                _cofirma = cofirma;
                string manifest = GetSessionManifest();
                SetSessionVariablesFromManifest(manifest);
            }

            void SetSessionVariablesFromManifest(string manifest)
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(manifest);
                _cofirma = Boolean.Parse(xd.SelectSingleNode("/ManifestFile/cosign").InnerText);
                _timestamp = Boolean.Parse(xd.SelectSingleNode("/ManifestFile/timestamp").InnerText);
                tipoFirma = (SignType)Enum.Parse(typeof(SignType), xd.SelectSingleNode("/ManifestFile/SignatureType").InnerText, true);

            }

            public string Put (DocsPaVO.documento.FileDocumento filedoc, string versionId)
            {
                //2019-12-18: per nomi file troppo lunghi la firma massiva andava in errore
                //string name = System.IO.Path.GetFileNameWithoutExtension(filedoc.name);
                string fileHash =hsmSvc.Session_PutFileToSign(_SessionToken, filedoc.content, versionId);
                if (!String.IsNullOrEmpty (fileHash))
                {
                    fileListToSign.Add(fileHash);
                    return fileHash;
                } else 
                {
                    return null;
                }
            }

            public bool Sign( string pinCertificato, string otpFirma)
            {
                return hsmSvc.Session_RemoteSign(_SessionToken, _aliasCertificato, _dominioCertificato, pinCertificato, otpFirma);
            }

            public byte[] Get(string fileHandle)
            {
                try
                {
                    return hsmSvc.Session_GetSignedFile(_SessionToken, fileHandle);
                }
                catch 
                {
                    return null;
                }
            }

            public string GetSessionManifest()
            {
                return hsmSvc.Session_GetManifest(_SessionToken);
            }

            public bool CloseSession ()
            {
               return hsmSvc.Session_CloseMultiSign(_SessionToken);
            }

            public bool ActiveSession()
            {
                string sessions = hsmSvc.Session_GetSessions();
                if (sessions.ToLower().Contains(_SessionToken.ToLower()))
                    return true;

                return false;
            }


            public static void KillSession (string sessiontoken)
            {
                externalSign.HSMService hsmSvcPrivate = new externalSign.HSMService();
                hsmSvcPrivate.Url = Config.HSMServiceUrl();
                hsmSvcPrivate.Timeout = 100000;
                hsmSvcPrivate.Session_CloseMultiSign(sessiontoken);
            }


        }


        #region HSM signature

        public byte[] Sign(byte[] documento, string otpFirma)
        {
            if (_type == SignType.CADES)
                return FirmaFileCADES(documento, _aliasCertificatoSingolo, _dominioCertificatoSingolo, _certificatePin, otpFirma, _timeStamp, _coSign);
            else
               return FirmaFilePADES(documento, _aliasCertificatoSingolo, _dominioCertificatoSingolo, _certificatePin, otpFirma, _timeStamp);
        }

        public bool RichiediOTP()
        {
            return RichiediOTP(_aliasCertificatoSingolo, _dominioCertificatoSingolo);
        }

        public static bool RichiediOTP(string aliasCertificato, string dominioCertificato)
        {
                ////////////////////////////////////
            logger.Debug("Inizio");
                externalSign.HSMService hsm = new externalSign.HSMService();
                hsm.Url = Config.HSMServiceUrl();
                hsm.Timeout = 100000;
                logger.DebugFormat("Chiamo RichiediOTP su {0} ",hsm.Url);
                return hsm.RichiediOTP(aliasCertificato, dominioCertificato);
        
        }

        public string GetHSMCertificateList()
        {
            return GetHSMCertificateList(_aliasCertificatoSingolo, _dominioCertificatoSingolo);
        }

        public static string GetHSMCertificateList(string aliasCertificato, string dominioCertificato)
        {
            externalSign.HSMService hsm = new externalSign.HSMService();
            hsm.Url = Config.HSMServiceUrl();
            hsm.Timeout = 100000;
            string certJson = hsm.GetCertificatoHSM(aliasCertificato, dominioCertificato);
            return certJson;
        }

        public byte[] FirmaFilePADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale)
        {
            externalSign.HSMService hsm = new externalSign.HSMService();
            hsm.Url = Config.HSMServiceUrl();
            hsm.Timeout = 100000;
            return hsm.FirmaFilePADES(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale);
        }

        public byte[] ControFirmaFileCADES(byte[] fileDafirmare, string aliasCertificatoDaControfirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale)
        {
            externalSign.HSMService hsm = new externalSign.HSMService();
            hsm.Url = Config.HSMServiceUrl();
            hsm.Timeout = 100000;
            return hsm.ControFirmaFileCADES(fileDafirmare,aliasCertificatoDaControfirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale);
        }

        public byte[] FirmaFileCADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, bool cofirma)
        {
            externalSign.HSMService hsm = new externalSign.HSMService();
            hsm.Url = Config.HSMServiceUrl();
            hsm.Timeout = 100000;
            return hsm.FirmaFileCADES(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale,cofirma);
        }

        #endregion

        class Config
        {
            public static string HSMServiceUrl()
            {
                try
                {
                    return ConfigurationManager.AppSettings["HSMServiceUrl"];
                }
                catch
                {
                    return null;
                }
            }
        }
	}
}
