using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DocsPaVO.areaConservazione;
using DocsPaVO.Conservazione.PARER.BigFiles;
using log4net;

namespace BusinessLogic.Conservazione.PARER
{
    public class BigFilesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(BigFilesManager));

        public static string InvioOggetto(string idAmm, ChiaveVersamento key, string itemType, out string errorCode, out string errorMessage)
        {
            logger.Debug("BEGIN");
            string esito = string.Empty;
            errorCode = string.Empty;
            errorMessage = string.Empty;
            string url = getConfigKey("0", "BE_VERSAMENTO_BF_URL_INVIO") + "InvioOggettoAsincrono";

            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;

            string requestString = string.Empty;
            string responseString = string.Empty;

            try
            {
                string nmVersatore = getConfigKey("0", "BE_VERSAMENTO_USER").ToUpper();
                string soapUsername = getConfigKey("0", "BE_VERSAMENTO_BF_USERNAME");
                string soapPassword = getConfigKey("0", "BE_VERSAMENTO_BF_PASSWORD");

                XmlDocument soapEnvelope = CreateSoapEnvelope();
                soapEnvelope.SelectSingleNode("//*[name()='wsse:Username']").InnerText = soapUsername;
                soapEnvelope.SelectSingleNode("//*[name()='wsse:Password']").InnerText = soapPassword;
                soapEnvelope.SelectSingleNode("//*[name()='wsu:Created']").InnerText = DateTime.Now.ToString();

                string invioRequest = invioOggettoGetRequest(nmVersatore, itemType, string.Format("{0}-{1}-{2}", key.tipoRegistro, key.anno, key.numero));
                soapEnvelope.SelectSingleNode("//*[name()='soapenv:Body']").InnerXml = invioRequest;

                logger.Debug("SOAP REQUEST: " + soapEnvelope.InnerXml);

                webRequest = CreateWebRequest(url);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelope, webRequest);

                logger.Debug("HTTP REQUEST: " + webRequest.ToString());
                logger.Debug("Invio richiesta...");

                webResponse = (HttpWebResponse)webRequest.GetResponse();

                logger.Debug("Risposta ricevuta");

                using(StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
                logger.Debug("Risposta dal server: " + responseString);

                XmlDocument soapResponse = new XmlDocument();
                soapResponse.LoadXml(responseString);

                esito = soapResponse.SelectSingleNode("//*[name()='cdEsito']").InnerText;
                logger.Debug("Esito: " + esito);

                if(esito.ToUpper() != "OK")
                {
                    errorCode = soapResponse.SelectSingleNode("//*[name()='cdErr']").InnerText;
                    errorMessage = soapResponse.SelectSingleNode("//*[name()='dsErr']").InnerText;
                    logger.Debug("Errore: " + errorCode + " - " + errorMessage);
                }
            }
            catch(WebException wEx)
            {
                logger.Debug("Errore nella risposta del servizio InvioOggettoPreIngest");
                webResponse = (HttpWebResponse)wEx.Response;
                if(webResponse != null)
                {
                    logger.DebugFormat("Risposta dal server: {0} - {1}", webResponse.StatusCode, webResponse.StatusDescription);
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responseString = sr.ReadToEnd();
                    }
                    logger.Debug(responseString);
                }
                
                esito = "FAULT";
            }
            catch(Exception ex)
            {
                logger.Debug("Errore InvioOggetto - ", ex);
                esito = "FAULT";
            }

            logger.Debug("END");
            return esito;
        }

        public static string NotificaTrasferimento(ChiaveVersamento key, string itemType, string fileName, string fileHash)
        {
            logger.Debug("BEGIN");
            string esito = string.Empty;

            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;

            string requestString = string.Empty;
            string responseString = string.Empty;

            string errorCode = string.Empty;
            string errorMessage = string.Empty;

            try
            {
                string url = getConfigKey("0", "BE_VERSAMENTO_BF_URL_INVIO") + "NotificaTrasferimento";

                string nmVersatore = getConfigKey("0", "BE_VERSAMENTO_USER").ToUpper();
                string soapUsername = getConfigKey("0", "BE_VERSAMENTO_BF_USERNAME");
                string soapPassword = getConfigKey("0", "BE_VERSAMENTO_BF_PASSWORD");

                XmlDocument soapEnvelope = CreateSoapEnvelope();
                soapEnvelope.SelectSingleNode("//*[name()='wsse:Username']").InnerText = soapUsername;
                soapEnvelope.SelectSingleNode("//*[name()='wsse:Password']").InnerText = soapPassword;

                string notificaRequest = notificaTrasferimentoGetRequest(nmVersatore, itemType, string.Format("{0}-{1}-{2}", key.tipoRegistro, key.anno, key.numero), fileName, fileHash);
                soapEnvelope.SelectSingleNode("//*[name()='soapenv:Body']").InnerXml = notificaRequest;

                logger.Debug("SOAP REQUEST: " + soapEnvelope.InnerXml);

                webRequest = CreateWebRequest(url);

                InsertSoapEnvelopeIntoWebRequest(soapEnvelope, webRequest);

                logger.Debug("Invio richiesta...");
                webResponse = (HttpWebResponse)webRequest.GetResponse();

                logger.Debug("Analisi risposta...");
                using(StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
                logger.Debug(responseString);

                XmlDocument soapResponse = new XmlDocument();
                soapResponse.LoadXml(responseString);

                esito = soapResponse.SelectSingleNode("//*[name()='cdEsito']").InnerText;
                logger.Debug("Esito: " + esito);

                if (esito.ToUpper() != "OK")
                {
                    errorCode = soapResponse.SelectSingleNode("//*[name()='cdErr']").InnerText;
                    errorMessage = soapResponse.SelectSingleNode("//*[name()='dsErr']").InnerText;
                    logger.Debug("Errore: " + errorCode + " - " + errorMessage);
                }

            }
            catch(WebException wEx)
            {
                logger.Debug("Errore nella risposta del servizio NotificaAvvenutoTrasferimentoFile");
                webResponse = (HttpWebResponse)wEx.Response;
                if(webResponse != null)
                {
                    logger.DebugFormat("Risposta dal server: {0} - {1}", webResponse.StatusCode, webResponse.StatusDescription);
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responseString = sr.ReadToEnd();
                    }
                    logger.Debug(responseString);
                }
                esito = "FAULT";
            }
            catch(Exception ex)
            {
                logger.Debug("Errore NotificaTrasferimento - ", ex);
                esito = "FAULT";
            }

            logger.Debug("END");
            return esito;
        }

        private static XmlDocument CreateSoapEnvelope()
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
           
            string soapHeader = @"<soapenv:Header><wsse:Security soapenv:mustUnderstand=""1"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">";
            soapHeader += @"<wsse:UsernameToken wsu:Id=""UsernameToken - {0}""><wsse:Username></wsse:Username>";
            soapHeader += @"<wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText""></wsse:Password>";
            soapHeader += @"<wsse:Nonce EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">{0}</wsse:Nonce><wsu:Created></wsu:Created></wsse:UsernameToken>";
            soapHeader += @"</wsse:Security></soapenv:Header>";

            string nonce = GetNonce();
            soapHeader = soapHeader.Replace("{0}", nonce);


            string soapBody = @"<soapenv:Body></soapenv:Body>";

            //string soapEnvelope = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/1999/XMLSchema"">" + soapHeader + soapBody + @"</soapenv:Envelope>";
            string soapEnvelope = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ws=""http://ws.sacerasi.eng.it/"">" + soapHeader + soapBody + @"</soapenv:Envelope>";

            soapEnvelopeDocument.LoadXml(soapEnvelope);
            //soapEnvelopeDocument.LoadXml(@"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/1999/XMLSchema""><SOAP-ENV:Header></SOAP-ENV:Header><SOAP-ENV:Body></SOAP-ENV:Body></SOAP-ENV:Envelope>");
            return soapEnvelopeDocument;
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            //webRequest.Accept = "text/xml";
            //webRequest.Connection = "Keep-Alive";
            //webRequest.UserAgent = "Apache-HttpClient/4.1.1 (java 1.5)";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private static string GetNonce()
        {
            //Allocate a buffer
            var ByteArray = new byte[20];
            //Generate a cryptographically random set of bytes
            RandomNumberGenerator rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(ByteArray);
            //Base64 encode and then return
            return Convert.ToBase64String(ByteArray);
        }

        private static string invioOggettoGetRequest(string user, string itemType, string key)
        {
            string request = string.Empty;

            try
            {
                InvioOggettoAsincrono invioRequest = new InvioOggettoAsincrono();
                invioRequest.Ambiente = getConfigKey("0", "BE_VERSAMENTO_AMBIENTE");
                invioRequest.Versatore = user;
                invioRequest.Chiave = key;
                invioRequest.Tipo = "UDGrandiDimensioni";
                invioRequest.Cifrato = false;
                invioRequest.ForzaWarning = false;
                invioRequest.ForzaAccettazione = true;

                XmlSerializer serializer = new XmlSerializer(typeof(InvioOggettoAsincrono));

                using(var sww = new StringWriter())
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = Encoding.UTF8;
                    settings.CloseOutput = true;
                    settings.OmitXmlDeclaration = true;

                    using(XmlWriter writer = XmlWriter.Create(sww, settings))
                    {
                        serializer.Serialize(writer, invioRequest, ns);
                        request = sww.ToString();
                    }
                }
            }
            catch(Exception)
            {
                request = string.Empty;
            }

            return request.Replace("invioOggettoAsincrono", "ws:invioOggettoAsincrono");
        }

        private static string notificaTrasferimentoGetRequest(string user, string itemType, string key, string fileName, string fileHash)
        {
            string request = string.Empty;

            try
            {
                NotificaAvvenutoTrasferimento notificaRequest = new NotificaAvvenutoTrasferimento();
                notificaRequest.Ambiente = getConfigKey("0", "BE_VERSAMENTO_AMBIENTE");
                notificaRequest.Versatore = user;
                notificaRequest.Chiave = key;

                FileDepositato file = new FileDepositato();
                file.Hash = fileHash;
                file.NomeFile = fileName;
                file.TipoFile = "UDGrandiDimensioni";

                notificaRequest.ListaFiles = new List<FileDepositato>() { file };

                XmlSerializer serializer = new XmlSerializer(typeof(NotificaAvvenutoTrasferimento));
                using (var sww = new StringWriter())
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = Encoding.UTF8;
                    settings.CloseOutput = true;
                    settings.OmitXmlDeclaration = true;

                    using(XmlWriter writer = XmlWriter.Create(sww, settings))
                    {
                        serializer.Serialize(writer, notificaRequest, ns);
                        request = sww.ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                request = string.Empty;
            }

            return request.Replace("notificaAvvenutoTrasferimentoFile", "ws:notificaAvvenutoTrasferimentoFile");
        }

        private static string getConfigKey(string idAmm, string key)
        {
            string retVal = string.Empty;

            // se è definita una chiave globale restituisco questa...
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", key)))
            {
                retVal = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", key);
            }
            else
            // ...altrimenti verifico l'esistenza di una chiave definita per l'amministrazione
            {
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, key)))
                {
                    retVal = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, key);
                }
                else
                {
                    retVal = string.Empty;
                }
            }

            return retVal;

        }
    }
}
