using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

using DocsPa_I_TSAuthority;
using DocsPaVO.areaConservazione;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.Math;
using System.Security.Cryptography;
using System.Net;

namespace DocsPa_TSAuthority
{
    /// <summary>
    /// 
    /// </summary>
    public class TSR_Request: I_TSR_Request
    {
        //mettendolo static prosegue la sequenza randomica invece di ripeterla dall'inizio!!!
        protected static Random nRandom = new Random(100);

        #region I_TSR_Request Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TimeStampQuery"></param>
        /// <returns></returns>
        public OutputResponseMarca getTimeStamp(InputMarca TimeStampQuery)
        {
            OutputResponseMarca outputMarca = new OutputResponseMarca();


            byte[] dati = String_To_Bytes(TimeStampQuery.file_p7m);
            //SHA1 sha1 = SHA1CryptoServiceProvider.Create();
            //byte[] hash = sha1.ComputeHash(dati);

            SHA256Managed sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(dati);

            TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();
            reqGen.SetCertReq(true);


            //Funzione randomica per il Nonce.
            //RandomNumberGenerator nRand = new RNGCryptoServiceProvider();
            long casuale = (long)nRandom.Next();
            //TimeStampRequest tsReq = reqGen.Generate(TspAlgorithms.Sha1, hash, BigInteger.ValueOf(casuale));
            TimeStampRequest tsReq = reqGen.Generate(TspAlgorithms.Sha256, hash, BigInteger.ValueOf(casuale));
            byte[] tsData = tsReq.GetEncoded();

            string urlTSA = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["URL_TSA"]))
            {
                urlTSA = ConfigurationManager.AppSettings["URL_TSA"].ToString();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urlTSA);
                req.Method = "POST";
                req.ContentType = "application/timestamp-query";

                //Username e password per accedere alla Time Stamping Authority
                string pwd = string.Empty;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PASSWORD_UTENTE_TSA"]))
                {
                    pwd = ConfigurationManager.AppSettings["PASSWORD_UTENTE_TSA"].ToString();
                    req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(pwd)));
                }

                req.ContentLength = tsData.Length;

                Stream reqStream = req.GetRequestStream();
                reqStream.Write(tsData, 0, tsData.Length);
                reqStream.Close();

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res == null)
                {
                    outputMarca.esito = "KO";
                    outputMarca.descrizioneErrore = "Impossibile contattare la TSA o autorizzazione negata";
                    return outputMarca;
                }
                else
                {
                    Stream resStream = new BufferedStream(res.GetResponseStream());
                    TimeStampResponse tsRes = new TimeStampResponse(resStream);
                    resStream.Close();
                    BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp checkMarca = new BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp();
                    outputMarca = checkMarca.Verify(tsReq, tsRes);
                }
            }
            else
            {
                outputMarca.esito = "KO";
                outputMarca.descrizioneErrore = "Impossibile contattare la TSA o url configurata errata!";
                return outputMarca;
            }

            return outputMarca;
        }

        #endregion

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
    }
}
