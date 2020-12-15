using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DocsPaVO.documento;
using log4net;

namespace DocsPaConsRequest
{
    public class DocsPaConsRequest
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaConsRequest));

        public static string sendRequest(NameValueCollection formFields, Dictionary<string, FileDocumento> fileFields, string uri)
        {

            string retVal = string.Empty;

            // Richiede .NET Framework 4.5
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;

            HttpWebRequest webrequest;
            webrequest = (HttpWebRequest)WebRequest.Create(uri);

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            webrequest.Method = "POST";
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.KeepAlive = false;
            webrequest.Timeout = System.Threading.Timeout.Infinite;

            string formFieldTemplate = "Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}\r\n";

            StringBuilder sbHeader = new StringBuilder();

            // parametri da inserire nell'header
            if (formFields != null)
            {
                foreach (string key in formFields.AllKeys)
                {
                    string[] values = formFields.GetValues(key);
                    if (values != null)
                    {
                        foreach (string value in values)
                        {
                            sbHeader.AppendFormat("--{0}\r\n", boundary);
                            sbHeader.AppendFormat(formFieldTemplate, key, value);
                        }
                    }
                }
            }

            byte[] header = Encoding.UTF8.GetBytes(sbHeader.ToString());
            byte[] footer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            //byte[] footer = Encoding.ASCII.GetBytes("--\r\n");

            MemoryStream ms = new MemoryStream();
            ms.Write(header, 0, header.Length);


            //string fileFieldTemplate = "Content-Disposition: form-data; name=\"{0}\";\r\n\r\nfilename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";
            string fileFieldTemplate = "Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\";Content-Type: {2}\r\n\r\n";

            // file da inviare
            if (fileFields != null)
            {
                ms.Write(boundaryBytes, 0, boundaryBytes.Length);
                //bool firstItem = true;

                // Individuo l'ultimo elemento
                string lastId = fileFields.Last().Key;

                foreach (KeyValuePair<string, FileDocumento> kvp in fileFields)
                {

                    string CRLF = "\r\n";
                    byte[] CRLFbytes = Encoding.UTF8.GetBytes(CRLF);
                    FileDocumento fd = kvp.Value;
                    if (fd != null && fd.content != null)
                    {
                        StringBuilder sbFile = new StringBuilder();
                        sbFile.AppendFormat("--{0}\r\n", boundary);
                        sbFile.AppendFormat(fileFieldTemplate, kvp.Key, fd.fullName, "application/octet-stream");
                        //string fileHead = string.Format(fileFieldTemplate, kvp.Key, fd.fullName, "application/octet-stream");

                        byte[] headerBytes = Encoding.UTF8.GetBytes(sbFile.ToString());
                        ms.Write(headerBytes, 0, headerBytes.Length);
                        ms.Write(fd.content, 0, fd.content.Length);

                        // aggiungo il CRLF solo se non sto inserendo l'ultimo elemento
                        if (lastId != kvp.Key)
                            ms.Write(CRLFbytes, 0, CRLFbytes.Length);

                        //ms.Write(boundaryBytes, 0, boundaryBytes.Length);
                    }
                    else
                    {
                        logger.Debug("CONTENT NULL per ID=" + kvp.Key);
                    }
                }
            }

            //ms.Write(boundaryBytes, 0, boundaryBytes.Length);
            ms.Write(footer, 0, footer.Length);
            long contentLength = ms.Length;

            logger.Debug("Creazione stream richiesta....");
            webrequest.ContentLength = contentLength;
            using (Stream rs = webrequest.GetRequestStream())
            {
                ms.Position = 0;
                byte[] buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                rs.Write(buffer, 0, buffer.Length);
            }

            logger.Debug("Esecuzione chiamata...");

            try
            {
                using (var webResponse = (HttpWebResponse)webrequest.GetResponse())
                {

                    logger.DebugFormat("Risposta status code del servizio REST: {0}", webResponse.StatusCode);

                    // gestione timeout richiesta
                    if (webResponse.StatusCode.Equals(HttpStatusCode.RequestTimeout))
                    {
                        retVal = "TIMEOUT";
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            retVal = reader.ReadToEnd();
                        }
                    }

                    logger.Debug("Risposta servizio: " + retVal);
                }
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("timed out"))
                {
                    logger.Debug("Timeout nell'operazione");
                    retVal = "TIMEOUT";
                }
                else
                {
                    switch (ex.Status)
                    {
                        case WebExceptionStatus.Timeout:
                            // Gestione per timeout richieste HTTP
                            logger.Debug("Timeout nell'operazione");
                            retVal = "TIMEOUT";
                            break;

                        case WebExceptionStatus.ReceiveFailure:
                        case WebExceptionStatus.KeepAliveFailure:
                            // Gestione per timeout richieste HTTPS.
                            // Se la richiesta è in timeout l'eccezione è gestita dalla classe interna SSL e lo stato viene modificato in ReceiveFailure o KeepAliveFailure.
                            // Per determinare se l'errore sia davvero un timeout è necessario analizzare il messaggio di errore della proprietà InnerException.
                            if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message != null)
                            {
                                string message = ex.InnerException.InnerException.Message;
                                string msg = "Impossibile stabilire la connessione. Risposta non corretta della parte connessa dopo l'intervallo di tempo oppure mancata risposta dall'host collegato";
                                string msgEn = "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond";

                                if (message.Equals(msg, StringComparison.CurrentCultureIgnoreCase) || message.Equals(msgEn, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    // TIMEOUT
                                    logger.Debug("Timeout nell'operazione");
                                    retVal = "TIMEOUT";
                                }
                                else
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                throw ex;
                            }
                            break;
                        default:
                            throw ex;
                    }
                }
            }

            return retVal;
        }
    }
}
