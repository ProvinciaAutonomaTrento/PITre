using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;

using System.Configuration;
using DocsPaVO.areaConservazione;
using DocsPaVO.utente;
using System.Text;
using System.IO;

//using Microsoft.Web.Services3;
//using Microsoft.Web.Services3.Design;
//using Microsoft.Web.Services3.Diagnostics.Configuration;
using log4net;


namespace DocsPaWS
{
    /// <summary>
    /// Summary description for DocsPaMarcaturaWS
    /// </summary>
    [WebService(Namespace = "http://localhost")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DocsPaMarcaturaWS : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaMarcaturaWS));
        protected static string path;
        public static string Path { get { return path; } }

        /// <summary>
        /// </summary>
        public DocsPaMarcaturaWS()
        {
            path = this.Server.MapPath("");
            //InitializeComponent();
        }

        /// <summary>
        /// Effettua una richiesta di marca temporale e verifica la validità della marca ottenuta 
        /// prima di restituirla come output.
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        [WebMethod]
        public OutputResponseMarca getTSR(InputMarca richiesta, InfoUtente utente)
        {
            OutputResponseMarca resultMarca = new OutputResponseMarca();

            //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco solo
            //il messaggio di errore dell'oggetto OutputResponseMarca
            if (utente == null)
            {
                logger.Debug("InfoUtente nullo o non valido.");
                resultMarca.esito = "KO";
                resultMarca.descrizioneErrore = "Utente nullo o non autorizzato!";
                return resultMarca;
            }

            try
            {
                //Scelta del tipo di implementazione per la richiesta della marca temporale
                string typeName = System.Configuration.ConfigurationManager.AppSettings["TYPE_TSA"];

                Type instanceType = Type.GetType(typeName, false);
                if (instanceType == null)
                    throw new ApplicationException(string.Format("Tipo non valido per la configurazione '{0}'", "TYPE_TSA"));

                DocsPa_I_TSAuthority.I_TSR_Request instance = (DocsPa_I_TSAuthority.I_TSR_Request)Activator.CreateInstance(instanceType);

                //ottengo una marca temporale in base alla specifica implementazione settata nel web.config
                resultMarca = instance.getTimeStamp(richiesta);

                //genero l'array di byte per il file p7m e TSR
                byte[] p7m = String_To_Bytes(richiesta.file_p7m);
                byte[] TSR = Convert.FromBase64String(resultMarca.marca);

                //verifico la marca e completo l'oggetto OutputResponseMarca
                resultMarca = VerificaMarca(p7m, TSR);
            }
            catch (Exception eMarca)
            {
                resultMarca.esito = "KO";
                resultMarca.descrizioneErrore = "richiesta della marca fallita: " + eMarca.Message;
                logger.Debug("richiesta della marca fallita: " + eMarca.Message);
            }

            return resultMarca;
        }

        /// <summary>
        /// Verifica la marca ottenuta in risposta ed estrae tutti i dati dalla marca completando
        /// i dati dell'oggetto OutputResponseMarca con quelli letti direttamente dalla marca!
        /// </summary>
        /// <param name="filep7m"></param>
        /// <param name="fileTSR"></param>
        /// <returns></returns>
        [WebMethod]
        public OutputResponseMarca VerificaMarca(byte[] filep7m, byte[] fileTSR)
        {
            OutputResponseMarca outputMarca = new OutputResponseMarca();

            BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp checkMarca = new BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp();
            outputMarca = checkMarca.Verify(filep7m, fileTSR);

            return outputMarca;
        }


        #region utility
        
        /// <summary>
        /// Convert hex string to byte_array
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        protected byte[] String_To_Bytes(string strInput)
        {
            // i variable used to hold position in string
            int i = 0;
            // x variable used to hold byte array element position
            int x = 0;
            // allocate byte array based on half of string length
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting
            //  it to decimal equivalent and store in byte array
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values
            return bytes;
        }

        #endregion

        #region metodi commentati
        ///// <summary>
        ///// Effettua una richiesta di marca temporale e verifica la validità della marca ottenuta 
        ///// prima di restituirla come output.
        ///// </summary>
        ///// <param name="richiesta"></param>
        ///// <param name="utente"></param>
        ///// <returns></returns>
        //[WebMethod]
        //public OutputResponseMarca getTSR(InputMarca richiesta, InfoUtente utente)
        //{
        //    OutputResponseMarca resultMarca = new OutputResponseMarca();

        //    //Se l'oggetto infoUtente non è valorizzato non eseguo alcuna operazione e restituisco solo
        //    //il messaggio di errore dell'oggetto OutputResponseMarca
        //    if (utente == null)
        //    {
        //        logger.Debug("InfoUtente nullo o non valido.");
        //        resultMarca.descrizioneErrore = "Utente nullo o non autorizzato!";
        //        return resultMarca;
        //    }

        //    string DocsPaTsa = string.Empty;
        //    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ENABLE_DOCSPA_TSA"].ToString()))
        //    {
        //        DocsPaTsa = ConfigurationManager.AppSettings["ENABLE_DOCSPA_TSA"].ToString();
        //    }
        //    //Nel caso in cui DocsPaTsa sia 1 la gestione della comunicazione con la TSA 
        //    //viene gestita direttamente internamente a DocsPa
        //    if (DocsPaTsa != "1")
        //    {
        //        marcatura.InputMarca inputMarca = new DocsPaWS.marcatura.InputMarca();
        //        marcatura.OutputResponseMarca outMarca = new DocsPaWS.marcatura.OutputResponseMarca();
        //        marcatura.marcatura Marca = new DocsPaWS.marcatura.marcatura();
        //        inputMarca.applicazione = richiesta.applicazione;
        //        inputMarca.file_p7m = richiesta.file_p7m;
        //        inputMarca.riferimento = richiesta.riferimento;
        //        outMarca = Marca.getTSR(inputMarca);
        //        //mapping del risultato sull'oggetto di DocsPa
        //        resultMarca.descrizioneErrore = outMarca.descrizioneErrore;
        //        resultMarca.docm = outMarca.docm;
        //        resultMarca.dsm = outMarca.dsm;
        //        resultMarca.esito = outMarca.esito;
        //        resultMarca.fhash = outMarca.fhash;
        //        resultMarca.marca = outMarca.marca;
        //        resultMarca.sernum = outMarca.sernum;
        //        resultMarca.TSA.TSARFC2253Name = outMarca.TSA;
        //    }
        //    else
        //    {
        //        resultMarca = TSR_Request(richiesta);
        //    }

        //    return resultMarca;
        //}

        ///// <summary>
        ///// Esegue la richiesta di timestamping verso una TSA e verifica la marca ottenuta in risposta
        ///// </summary>
        ///// <param name="richiesta"></param>
        ///// <returns></returns>
        //protected OutputResponseMarca TSR_Request(InputMarca richiesta)
        //{
        //    OutputResponseMarca outputMarca = new OutputResponseMarca();

        //    byte[] dati = String_To_Bytes(richiesta.file_p7m);
        //    SHA1 sha1 = SHA1CryptoServiceProvider.Create();
        //    byte[] hash = sha1.ComputeHash(dati);

        //    TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
        //    reqGen.SetCertReq(true);

        //    //Funzione randomica per il Nonce.
        //    //RandomNumberGenerator nRand = new RNGCryptoServiceProvider();
        //    long casuale = (long)nRandom.Next();
        //    TimeStampRequest tsReq = reqGen.Generate(TspAlgorithms.Sha1, hash, BigInteger.ValueOf(casuale));
        //    byte[] tsData = tsReq.GetEncoded();

        //    string urlTSA = string.Empty;
        //    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["URL_TSA"].ToString()))
        //    {
        //        urlTSA = ConfigurationManager.AppSettings["URL_TSA"].ToString();
        //        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlTSA);
        //        req.Method = "POST";
        //        req.ContentType = "application/timestamp-query";

        //        //Username e password per accedere alla Time Stamping Authority
        //        string pwd = string.Empty;
        //        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PASSWORD_UTENTE_TSA"].ToString()))
        //        {
        //            pwd = ConfigurationManager.AppSettings["PASSWORD_UTENTE_TSA"].ToString();
        //            req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(pwd)));
        //        }

        //        req.ContentLength = tsData.Length;

        //        Stream reqStream = req.GetRequestStream();
        //        reqStream.Write(tsData, 0, tsData.Length);
        //        reqStream.Close();

        //        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        //        if (res == null)
        //        {
        //            outputMarca.esito = "KO";
        //            outputMarca.descrizioneErrore = "Impossibile contattare la TSA o autorizzazione negata";
        //            return outputMarca;
        //        }
        //        else
        //        {
        //            Stream resStream = new BufferedStream(res.GetResponseStream());
        //            TimeStampResponse tsRes = new TimeStampResponse(resStream);
        //            resStream.Close();
        //            BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp checkMarca = new BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp();
        //            outputMarca = checkMarca.Verify(tsReq, tsRes);
        //        }
        //    }
        //    else
        //    {
        //        outputMarca.esito = "KO";
        //        outputMarca.descrizioneErrore = "Impossibile contattare la TSA o url configurata errata!";
        //        return outputMarca;
        //    }

        //    return outputMarca;
        //}
        #endregion
    }
}
