// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;

namespace BusinessLogic.Documenti.DigitalSignature;

public class RemoteSignature
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(RemoteSignature));

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
}
