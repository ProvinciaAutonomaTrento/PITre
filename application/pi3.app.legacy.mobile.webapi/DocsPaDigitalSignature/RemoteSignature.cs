// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Xml;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.ValueObjects;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using System.IO;
using Pi3.Core.Services.Configuration;
using System.Security.Cryptography;
using DocsPaVO.documento;

namespace BusinessLogic.Documenti.DigitalSignature;

public class RemoteSignature
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(RemoteSignature));

    public enum SignType
    {
        /// <remarks/>
        CADES,
        /// <remarks/>
        PADES,
    }

    bool _timeStamp = false;
    bool _coSign = false;
    string _aliasCertificatoSingolo, _dominioCertificatoSingolo, _certificatePin;
    SignType _type;
    IFirmaRemota2Service _firmaRemotaService;


    public RemoteSignature(string AliasCertificato, string DominioCertificato, string CertificatePin, SignType SignType, bool TimeStamp, bool Cosign,
        IFirmaRemota2Service firmaRemotaService)
    {
        _aliasCertificatoSingolo = AliasCertificato;
        _dominioCertificatoSingolo = DominioCertificato;
        _type = SignType;
        _coSign = Cosign;
        _certificatePin = CertificatePin;
        _timeStamp = TimeStamp;
        _firmaRemotaService = firmaRemotaService;
    }

    public class MultiSign
    {
        string _SessionToken, _aliasCertificato, _dominioCertificato, _temporaryManifestPath, _temporaryDirectory;
        bool _cofirma, _timestamp;
        IFirmaRemota2Service _firmaRemotaService;
        IConfigurationService _configurationService;

        public bool Cofirma
        {
            get { return _cofirma; }
        }

        public string SessionToken
        {
            get { return _SessionToken; }
        }

#if false
        externalSign.HSMService hsmSvc = null;
#endif
        SignType tipoFirma;

        public SignType TipoFirma
        {
            get { return tipoFirma; }
        }

        List<String> fileListToSign = new List<string>();

        public MultiSign(bool cosign, bool timestamp, SignType type, IConfigurationService configurationService, string temporaryRootPath)
        {
#if false
            hsmSvc = new externalSign.HSMService();
            hsmSvc.Url = Config.HSMServiceUrl();
            hsmSvc.Timeout = 100000;
            tipoFirma = type;
            _cofirma = cosign;
            _SessionToken = hsmSvc.Session_OpenMultiSign(cosign, timestamp, (externalSign.SignType)type);
#endif
            tipoFirma = type;
            _cofirma = cosign;
            //_firmaRemotaService = firmaRemotaService;
            _configurationService = configurationService;
            _SessionToken = Session_OpenMultiSign(cosign, timestamp, type, temporaryRootPath);

        }


        private string Session_OpenMultiSign(bool cofirma, bool timestamp, SignType tipoFirma, string temporaryRootPath)
        {
            string sessionToken = string.Empty;
            try
            {

                int type = tipoFirma.Equals(RemoteSignature.SignType.CADES) ? 0 : 1;
                sessionToken = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

                var guid = new Guid(sessionToken);

                //var repositoryRootPath = (_configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true)).Result;
                if (string.IsNullOrEmpty(temporaryRootPath))
                    temporaryRootPath = (_configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true)).Result;
                //TO DO path temporaneo - da dove lo prendo?
                //var repositoryRootPath = string.Empty;

                var directory = Path.Combine(
                            temporaryRootPath,
                            sessionToken.ToUpper(),
                            "TemporaryUploads")
                    .PathAsUnixPath();

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                _temporaryDirectory = directory;

                Manifest.SignType st = (Manifest.SignType)type;
                Manifest.ManifestFile m = new Manifest.ManifestFile { SignatureType = st, timestamp = timestamp, Token = sessionToken, cosign = cofirma };

                var filePath = Path.Combine(directory, "Manifest.xml");

                logger.Debug($"Path > {filePath}");

                _temporaryManifestPath = filePath;

                XmlDocument docSave = new XmlDocument();
                docSave.LoadXml(m.ToXmlString());
                docSave.Save(filePath);

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return sessionToken;
        }

        public MultiSign(string AliasCertificato, string DominioCertificato, string SessionToken, bool cofirma, IFirmaRemota2Service firmaRemotaService, string temporaryRootPath)
        {
#if false
            hsmSvc = new externalSign.HSMService();
            hsmSvc.Url = Config.HSMServiceUrl();
            hsmSvc.Timeout = 100000;
#endif
            _aliasCertificato = AliasCertificato;
            _dominioCertificato = DominioCertificato;
            _SessionToken = SessionToken;
            _cofirma = cofirma;
            _firmaRemotaService = firmaRemotaService;

            string manifest = GetSessionManifest(temporaryRootPath);
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

        public string Put(DocsPaVO.documento.FileDocumento filedoc, string versionId, string fileName)
        {
            //2019-12-18: per nomi file troppo lunghi la firma massiva andava in errore
            //string name = System.IO.Path.GetFileNameWithoutExtension(filedoc.name);

#if false
            string fileHash = hsmSvc.Session_PutFileToSign(_SessionToken, filedoc.content, versionId);
#endif  
            string fileHash = Session_PutFileToSign(_SessionToken, filedoc.content, versionId, fileName); ;
            if (!String.IsNullOrEmpty(fileHash))
            {
                fileListToSign.Add(fileHash);
                return fileHash;
            }
            else
            {
                return null;
            }
        }

        private string Session_PutFileToSign(string sessionToken, byte[] content, string versionId, string frFileName)
        {
            string manifestFile = _temporaryManifestPath;
            string directory = _temporaryDirectory;

            if (File.Exists(manifestFile))
            {
                String manifestXML = File.ReadAllText(manifestFile);
                Manifest.ManifestFile m = Manifest.ManifestFile.Deserialize(manifestXML);

                SHA256 mySHA256 = SHA256.Create();
                string sha256Hash = BitConverter.ToString(content.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
                if (!m.FileInformation.Any(x => x.hash.ToUpper() == sha256Hash.ToUpper()))
                {
                    string fileName = versionId;
                    string signFileName = Guid.NewGuid().ToString() + fileName + Path.GetExtension(frFileName);
                    File.WriteAllBytes(Path.Combine(directory, signFileName), content);
                    m.FileInformation.Add(new Manifest.MainfestFileInformation { hash = sha256Hash.ToUpper(), OriginalFullName = signFileName });
                    File.WriteAllText(manifestFile, m.Serialize());
                    return sha256Hash;
                }
            }

            return null;
        }

        public async Task<bool> Sign(string pinCertificato, string otpFirma)
        {
#if false
            return hsmSvc.Session_RemoteSign(_SessionToken, _aliasCertificato, _dominioCertificato, pinCertificato, otpFirma);
#endif
            List<byte[]> filesRead = new List<byte[]>();

            var directory = _temporaryDirectory;

            logger.Debug($"HSM_SignMultiSignSession: INIZIO recupero file Manifest.xml {directory} Alias: {_aliasCertificato}");

            var manifestFile = _temporaryManifestPath; //Path.Combine(directory, "Manifest.xml");

            if (System.IO.File.Exists(manifestFile))
            {
                List<FileDaFirmare> fileDaFirmare = new List<FileDaFirmare>();
                String manifestXML = System.IO.File.ReadAllText(manifestFile);
                Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);
                foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                {
                    logger.Debug($"HSM_SignMultiSignSession: INIZIO recupero content file {FileInformation.OriginalFullName} Alias: {_aliasCertificato}");

                    byte[] content = System.IO.File.ReadAllBytes(Path.Combine(directory, FileInformation.OriginalFullName));

                    filesRead.Add(content);
                    fileDaFirmare.Add(new FileDaFirmare() { FileBase64 = content, FileName = FileInformation.OriginalFullName });

                    logger.Debug($"HSM_SignMultiSignSession: FINE recupero content file {FileInformation.OriginalFullName} Alias: {_aliasCertificato}");
                }
                sbyte firmaParallela = 0;

                if (mft.cosign)
                    firmaParallela = 1;

                if (mft.SignatureType == Manifest.SignType.CADES)
                {
                    var firmaCadesRequest = new Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.ValueObjects.FirmaCAdESRequest()
                    {
                        AliasCertificato = _aliasCertificato,
                        DominioCertificato = _dominioCertificato,
                        OtpFirma = otpFirma,
                        PinCertificato = pinCertificato,
                        MarcaTemporale = mft.timestamp,
                        FilesDaFirmare = fileDaFirmare,
                        unzipOutput = true
                    };

                    logger.Debug($"HSM_SignMultiSignSession: INIZIO chiamata a FirmaCAdESREST Alias: {_aliasCertificato}");

                    Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.ValueObjects.FirmaCAdESResponse firmaCadesResponse = await _firmaRemotaService.FirmaCAdESREST(firmaCadesRequest);

                    if (firmaCadesResponse != null && firmaCadesResponse.FileFirmato != null && firmaCadesResponse.FileFirmato.Count() > 0)
                    {
                        logger.Debug($"HSM_SignMultiSignSession: INIZIO scrittura file firmati Alias: {_aliasCertificato}");

                        foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                        {
                            logger.Debug($"HSM_SignMultiSignSession: INIZIO scrittura file firmato {FileInformation.OriginalFullName}");

                            byte[] content = (firmaCadesResponse.FileFirmato.Where(x => x.FileName.Contains(FileInformation.OriginalFullName)).FirstOrDefault()).FileBase64;
                            string newName = "signed_" + FileInformation.OriginalFullName;
                            System.IO.File.WriteAllBytes(Path.Combine(directory, newName), content);
                            FileInformation.SignedFullName = newName;

                            logger.Debug($"HSM_SignMultiSignSession: FINE scrittura file firmato {FileInformation.OriginalFullName} Alias: {_aliasCertificato}");
                        }

                        logger.Debug($"HSM_SignMultiSignSession: FINE scrittura file firmati Alias: {_aliasCertificato}");

                        File.WriteAllText(manifestFile, mft.Serialize());

                        return true;

                    }
                    else
                    {
                        logger.Debug($"HSM_SignMultiSignSession: file firmati non restituiti Alias: {_aliasCertificato}");
                    }
                }
                else
                {
                    var firmaPadesRequest = new Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.ValueObjects.FirmaPAdESRequest()
                    {
                        AliasCertificato = _aliasCertificato,
                        DominioCertificato = _dominioCertificato,
                        OtpFirma = otpFirma,
                        PinCertificato = pinCertificato,
                        MarcaTemporale = mft.timestamp,
                        FilesDaFirmare = fileDaFirmare,
                        unzipOutput = true
                    };

                    logger.Debug($"HSM_SignMultiSignSession: INIZIO chiamata a FirmaPAdESREST Alias: {_aliasCertificato}");

                    Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.ValueObjects.FirmaPAdESResponse firmaPadesResponse = await _firmaRemotaService.FirmaPAdESREST(firmaPadesRequest);

                    logger.Debug($"HSM_SignMultiSignSession: FINE chiamata a FirmaPAdESREST Alias: {_aliasCertificato}");

                    if (firmaPadesResponse != null && firmaPadesResponse.FileFirmato != null && firmaPadesResponse.FileFirmato.Count() > 0)
                    {
                        logger.Debug($"HSM_SignMultiSignSession: INIZIO scrittura file firmati Alias: {_aliasCertificato}");

                        foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                        {
                            logger.Debug($"HSM_SignMultiSignSession: INIZIO scrittura file firmato {FileInformation.OriginalFullName} Alias: {_aliasCertificato}");

                            byte[] content = (firmaPadesResponse.FileFirmato.Where(x => x.FileName.Equals(FileInformation.OriginalFullName)).FirstOrDefault()).FileBase64;
                            string newName = "signed_" + FileInformation.OriginalFullName;
                            System.IO.File.WriteAllBytes(Path.Combine(directory, newName), content);
                            FileInformation.SignedFullName = newName;

                            logger.Debug($"HSM_SignMultiSignSession: FINE scrittura file firmato {FileInformation.OriginalFullName} Alias: {_aliasCertificato}");
                        }

                        logger.Debug($"HSM_SignMultiSignSession: FINE scrittura file firmati Alias: {_aliasCertificato}");

                        File.WriteAllText(manifestFile, mft.Serialize());

                        return true;

                    }
                    else
                    {
                        logger.Debug($"HSM_SignMultiSignSession: file firmati non restituiti Alias: {_aliasCertificato}");
                    }
                }
            }
            return false;
        }

        public byte[] Get(string fileHandle)
        {
            try
            {
#if false
                return hsmSvc.Session_GetSignedFile(_SessionToken, fileHandle);
#endif

                string SessionToken = _SessionToken.ToUpper();                
                string sessionDir = _temporaryDirectory;
                if (Directory.Exists(sessionDir))
                {
                    string manifestFile = Path.Combine(sessionDir, "Manifest.xml");
                    if (File.Exists(manifestFile))
                    {
                        String manifestXML = File.ReadAllText(manifestFile);
                        Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);

                        foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                        {
                            if (FileInformation.hash.ToUpper() == fileHandle.ToUpper())
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
                                    logger.Error("Il file {0} | {1} non è leggibile", sessionDir, FileInformation.SignedFullName);
                                    return null;
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error($"Errore in Session_GetSignedFile: {ex.Message}");
                return null;
            }
        }

        public string GetSessionManifest(string temporaryRootPath)
        {
#if false
            return hsmSvc.Session_GetManifest(_SessionToken);
#endif
            string SessionToken = _SessionToken.ToUpper();
           
            if (string.IsNullOrEmpty(temporaryRootPath))
                temporaryRootPath = (_configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true)).Result;
            //TO DO path temporaneo - da dove lo prendo?
            //var repositoryRootPath = string.Empty;

            var sessionDir = Path.Combine(
                        temporaryRootPath,
                        SessionToken.ToUpper(),
                        "TemporaryUploads")
                .PathAsUnixPath();

            if (Directory.Exists(sessionDir))
            {
                _temporaryDirectory = sessionDir;
                string manifestFile = Path.Combine(sessionDir, "Manifest.xml"); 

                if (File.Exists(manifestFile))
                {
                    _temporaryManifestPath = manifestFile;
                    String manifestXML = File.ReadAllText(manifestFile);
                    return manifestXML;
                }
            }
            return null;
        }

        public bool CloseSession()
        {
#if false
            return hsmSvc.Session_CloseMultiSign(_SessionToken);
#endif
            return false;
        }

        public bool ActiveSession()
        {
#if false
            string sessions = hsmSvc.Session_GetSessions();
            if (sessions.ToLower().Contains(_SessionToken.ToLower()))
                return true;
#endif
            return false;
        }


        public static void KillSession(string sessiontoken)
        {
#if false
            externalSign.HSMService hsmSvcPrivate = new externalSign.HSMService();
            hsmSvcPrivate.Url = Config.HSMServiceUrl();
            hsmSvcPrivate.Timeout = 100000;
            hsmSvcPrivate.Session_CloseMultiSign(sessiontoken);
#endif
        }

    }

    #region Manifest
    public class Manifest
    {
        public enum SignType
        {
            CADES,
            PADES
        }

        public class MainfestFileInformation
        {
            public string OriginalFullName;
            public string hash;
            public string SignedFullName;
        }

        public class ManifestFile
        {
            private static System.Xml.Serialization.XmlSerializer serializer;
            public string Token;
            public SignType SignatureType;
            public List<MainfestFileInformation> FileInformation = new List<MainfestFileInformation>();
            public bool cosign;
            public bool timestamp;

            private static System.Xml.Serialization.XmlSerializer Serializer
            {
                get
                {
                    if ((serializer == null))
                    {
                        serializer = new System.Xml.Serialization.XmlSerializer(typeof(ManifestFile));
                    }
                    return serializer;
                }
            }

            public virtual string Serialize()
            {
                System.IO.StreamReader streamReader = null;
                System.IO.MemoryStream memoryStream = null;
                try
                {
                    memoryStream = new System.IO.MemoryStream();
                    Serializer.Serialize(memoryStream, this);
                    memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                    streamReader = new System.IO.StreamReader(memoryStream);
                    return streamReader.ReadToEnd();
                }
                finally
                {
                    if ((streamReader != null))
                    {
                        streamReader.Dispose();
                    }
                    if ((memoryStream != null))
                    {
                        memoryStream.Dispose();
                    }
                }
            }

            public static ManifestFile Deserialize(string xml)
            {
                System.IO.StringReader stringReader = null;
                try
                {
                    stringReader = new System.IO.StringReader(xml);
                    return ((ManifestFile)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
                }
                finally
                {
                    if ((stringReader != null))
                    {
                        stringReader.Dispose();
                    }
                }
            }
        }

        #endregion

    }
    public static byte[] Xmlsignature(string codiceAOOIPA, string codiceEnteIPA, byte[] fileDaFirmare, out string statusCode)
    {
        statusCode = string.Empty;
        // var restClient = RestService.For<IRestApi>("http://localhost/PiTreSigilloElettronico");
        //  string keyUrl = "http://t.pitre.tn.it/SigilloElettronico/api/SigilloelettronicoXml";// DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REST_URL_SIGILLO");
        //string keyUrl = "http://t.pitre.tn.it/SigilloElettronico/api";// DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REST_URL_SIGILLO");
        //var restClient = RestService.For<IRestApi>(keyUrl);

        //  return restClient.Xmlsignature(codiceAOOIPA, codiceEnteIPA, fileDaFirmare).Result;
        try
        {
#if false // chiamata a web service esterno
            SignXMLType signXml = new SignXMLType();

            signXml.codiceAOOIPA = codiceAOOIPA;
            signXml.codiceEnteIPA = codiceEnteIPA;
            signXml.fileDaFirmare = fileDaFirmare;

            string url = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REST_URL_SIGILLO");
            //  string url =  "http://localhost/PiTreSigilloElettronico/api"; // "http://t.pitre.tn.it/SigilloElettronico/api";//  DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REST_URL_SIGILLO");
            //  string apiUrl = url + "/SigilloelettronicoXml/Xmlsignature";
            //   string apiUrl = url + "/SigilloelettronicoXml/Xmlsignature";
            string apiUrl = url + "/SigilloelettronicoXml/signature";
            //var handler = new WebRequestHandler();

            //string certPath = AppDomain.CurrentDomain.BaseDirectory + @"crt\";

            //var certFile = Path.Combine(certPath, "client-t.pitre-to-restSign-servizi-tibco-test.tndigit.it.pfx");
            //handler.ClientCertificates.Add(new X509Certificate2(certFile, certPass));
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);

            // var settings = new JsonSerializerSettings { ContractResolver = SkipSpecifiedContractResolver.Instance, NullValueHandling = NullValueHandling.Ignore };
            //string json = JsonConvert.SerializeObject(signXml, settings);

            string json = JsonConvert.SerializeObject(signXml);

            logger.Debug("request to RestService  -> " + json);


            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("ContentType", "application/json");
            //  var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(user + ":" + pass);
            //  string val = System.Convert.ToBase64String(plainTextBytes);
            //  client.DefaultRequestHeaders.Add("Authorization", "Basic " + val);

            logger.Debug("prima di invio richiesta");

            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            logger.Debug("risultato richiesta catturato");
            statusCode = ((int)response.StatusCode).ToString();
            if (response.IsSuccessStatusCode)
            {
                //   byte[] res =  response.Content.ReadAsByteArrayAsync().Result;

                string responseBody = response.Content.ReadAsStringAsync().Result;

                logger.Debug("Body risposta -> " + responseBody);

                var jsonResult = JsonConvert.DeserializeObject(responseBody).ToString();
                // var jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(responseBody);
                logger.Debug("DeserializeObject in var jesonResut OK! ");
                // byte[] res = JsonConvert.DeserializeObject<byte[]>(jsonResult);

                SignResponseXMLType signResponseXmlType = JsonConvert.DeserializeObject<SignResponseXMLType>(jsonResult);

                logger.Debug("serializzazione in signResponseType fatta ");

                return signResponseXmlType.fileFirmato;
            }
            else
                logger.Debug("risposta di errore servizio con status " + response.StatusCode);

            return new byte[0];
#endif
        }
        catch (Exception e)
        {
            logger.Debug("eccezione!! " + e.Message);
            logger.Debug("Inner eccezione!! " + e.InnerException);
            logger.Debug(e.StackTrace);
            return new byte[0];
        }

        return null;
    }

    public static byte[] Pdfsignature(string codiceAOOIPA, string codiceEnteIPA, byte[] fileDaFirmare, int Page, int LeftX, int LeftY, int RightX, int RightY, string StampText, out string statusCode)
    {
        statusCode = string.Empty;
        // var restClient = RestService.For<IRestApi>("http://localhost/PiTreSigilloElettronico/api/SigilloElettronicoPdf");
        //    string keyUrl = "http://t.pitre.tn.it/SigilloElettronico/api/SigilloElettronicoPdf";// DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REST_URL_SIGILLO");
        //    var restClient = RestService.For<IRestApi>(keyUrl);

        //    return restClient.Pdfsignature(  codiceAOOIPA, codiceEnteIPA, fileDaFirmare,  Page, RightX, RightY, LeftX,  LeftY,   StampText).Result;
        try
        {
#if false // chiamata a web service esterno
            SignPDFType signpdf = new SignPDFType();

            signpdf.codiceAOOIPA = codiceAOOIPA;
            signpdf.codiceEnteIPA = codiceEnteIPA;
            signpdf.fileDaFirmare = fileDaFirmare;

            signpdf.Apparence = new ApparenceType();
            signpdf.Apparence.page = Page;
            signpdf.Apparence.leftx = LeftX;
            signpdf.Apparence.lefty = LeftY;
            signpdf.Apparence.rightx = RightX;
            signpdf.Apparence.righty = RightY;
            signpdf.Apparence.testo = StampText;
            signpdf.Apparence.bShowDateTime = false;
            signpdf.Apparence.bShowDateTimeSpecified = true;


            string url = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REST_URL_SIGILLO");
            //string url =  "http://localhost/PiTreSigilloElettronico/api"; // "http://t.pitre.tn.it/SigilloElettronico/api";//  DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REST_URL_SIGILLO");
            //  string apiUrl = url + "/SigilloelettronicoXml/Xmlsignature";
            //   string apiUrl = url + "/SigilloelettronicoXml/Xmlsignature";
            string apiUrl = url + "/SigilloelettronicoPdf/signature";
            //var handler = new WebRequestHandler();

            //string certPath = AppDomain.CurrentDomain.BaseDirectory + @"crt\";

            //var certFile = Path.Combine(certPath, "client-t.pitre-to-restSign-servizi-tibco-test.tndigit.it.pfx");
            //handler.ClientCertificates.Add(new X509Certificate2(certFile, certPass));
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);

            // var settings = new JsonSerializerSettings { ContractResolver = SkipSpecifiedContractResolver.Instance, NullValueHandling = NullValueHandling.Ignore };
            //string json = JsonConvert.SerializeObject(signXml, settings);

            string json = JsonConvert.SerializeObject(signpdf);

            logger.Debug("request to RestService  -> " + json);


            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("ContentType", "application/json");
            //  var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(user + ":" + pass);
            //  string val = System.Convert.ToBase64String(plainTextBytes);
            //  client.DefaultRequestHeaders.Add("Authorization", "Basic " + val);

            logger.Debug("prima di invio richiesta");

            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            logger.Debug("risultato richiesta catturato");
            statusCode = ((int)response.StatusCode).ToString();
            if (response.IsSuccessStatusCode)
            {
                //   byte[] res =  response.Content.ReadAsByteArrayAsync().Result;

                string responseBody = response.Content.ReadAsStringAsync().Result;

                logger.Debug("Body risposta -> " + responseBody);

                var jsonResult = JsonConvert.DeserializeObject(responseBody).ToString();
                // var jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(responseBody);
                logger.Debug("DeserializeObject in var jesonResut OK! ");
                // byte[] res = JsonConvert.DeserializeObject<byte[]>(jsonResult);

                SignResponsePDFType signResponsePdfType = JsonConvert.DeserializeObject<SignResponsePDFType>(jsonResult);

                logger.Debug("serializzazione in signResponseType fatta ");

                return signResponsePdfType.fileFirmato;
            }
            else
                logger.Debug("risposta di errore servizio con status " + response.StatusCode);

            return new byte[0];
#endif
        }
        catch (Exception e)
        {
            logger.Debug("eccezione!! " + e.Message);
            logger.Debug("Inner eccezione!! " + e.InnerException);
            logger.Debug(e.StackTrace);
            return new byte[0];
        }

        return null;
    }

    public static async Task<bool> RichiediOTP(string aliasCertificato, string dominioCertificato, IFirmaRemota2Service firmaRemotaService)
    {
#if false
            ////////////////////////////////////
            logger.Debug("Inizio");
            externalSign.HSMService hsm = new externalSign.HSMService();
            hsm.Url = Config.HSMServiceUrl();
            hsm.Timeout = 100000;
            logger.DebugFormat("Chiamo RichiediOTP su {0} ", hsm.Url);
            return hsm.RichiediOTP(aliasCertificato, dominioCertificato);
#endif
        var response = await firmaRemotaService.RichiestaOtpREST(new Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.ValueObjects.RichiestaOtpRequest()
        {
            AliasCertificato = aliasCertificato,
            DominioCertificato = dominioCertificato
        });

        logger.Debug("Richiesta OTP: response status -> " + response.Status);

        //TODO: capire quale è la condizione di successo
        return response.Status == "OK"; //???;

    }

    public async Task<byte[]> Sign(string fileName, byte[] documento, string otpFirma)
    {
        if (_type == SignType.CADES)
            return await FirmaFileCADES(fileName, documento, _aliasCertificatoSingolo, _dominioCertificatoSingolo, _certificatePin, otpFirma, _timeStamp, _coSign);
        else
            return await FirmaFilePADES(fileName, documento, _aliasCertificatoSingolo, _dominioCertificatoSingolo, _certificatePin, otpFirma, _timeStamp);
    }

    public async Task<byte[]> FirmaFilePADES(string fileName, byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale)
    {
#if false
        externalSign.HSMService hsm = new externalSign.HSMService();
        hsm.Url = Config.HSMServiceUrl();
        hsm.Timeout = 100000;
        return hsm.FirmaFilePADES(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale);
#endif
        var response = await _firmaRemotaService.FirmaPAdESREST(new FirmaPAdESRequest()
        {
            FilesDaFirmare = new List<FileDaFirmare>()
            {
                new FileDaFirmare()
                {
                    FileBase64 = fileDafirmare, // Contenuto binario del file da firmare digitalmente
                    FileName = fileName
                }
            },
            AliasCertificato = aliasCertificato,
            DominioCertificato = dominioCertificato,
            OtpFirma = otpFirma,
            MarcaTemporale = marcaTemporale,
            PinCertificato = pinCertificato
        });

        if (response != null! && response.FileFirmato.Any())
            return response.FileFirmato[0].FileBase64;
        else
            throw new ApplicationException("Nessun file firmato");
    }

    public async Task<byte[]> FirmaFileCADES(string fileName, byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, bool cofirma)
    {
#if false
        externalSign.HSMService hsm = new externalSign.HSMService();
        hsm.Url = Config.HSMServiceUrl();
        hsm.Timeout = 100000;
        return hsm.FirmaFileCADES(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale, cofirma);
#endif
        var response = await _firmaRemotaService.FirmaCAdESREST(new FirmaCAdESRequest()
        {
            FilesDaFirmare = new List<FileDaFirmare>()
            {
                new FileDaFirmare()
                {
                    FileBase64 =  fileDafirmare,
                    FileName = fileName
                }
            },
            AliasCertificato = aliasCertificato,
            DominioCertificato = dominioCertificato,
            OtpFirma = otpFirma,
            MarcaTemporale = marcaTemporale,
            PinCertificato = pinCertificato,
            FirmaParallela = cofirma
        });

        if (response != null! && response.FileFirmato.Any())
            return response.FileFirmato[0].FileBase64;
        else
            throw new ApplicationException("Nessun file firmato");
    }

    public static string GetHSMCertificateList(string aliasCertificato, string dominioCertificato)
    {
#if false
        externalSign.HSMService hsm = new externalSign.HSMService();
        hsm.Url = Config.HSMServiceUrl();
        hsm.Timeout = 100000;
        string certJson = hsm.GetCertificatoHSM(aliasCertificato, dominioCertificato);
        return certJson;
#endif
        return null;
    }

}
