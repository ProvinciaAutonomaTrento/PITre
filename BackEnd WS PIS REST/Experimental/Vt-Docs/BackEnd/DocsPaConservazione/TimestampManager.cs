using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;
using log4net;

namespace DocsPaConservazione
{
    /// <summary>
    /// Questa classe si occupa di generare il file XML contenente il documento firmato e la marca temporale,
    /// inoltre espone un metodo di verifica della marca temporale relativa al documento conservato.
    /// </summary>
    public class TimestampManager
    {
        private ILog logger = LogManager.GetLogger(typeof(TimestampManager));
        /// <summary>
        /// Restituisce la codifica esadecimale del file firmato passato come parametro
        /// </summary>
        /// <param name="SignedXml"></param>
        /// <returns></returns>
        public string getSignedXmlHex(byte[] SignedXml)
        {
            string signedXmlHex = string.Empty;
            try
            {
                //signedXmlHex = byteArrayToHexa(SignedXml); //inefficente
                signedXmlHex = BitConverter.ToString(SignedXml).Replace("-", string.Empty);
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella conversione Esadecimale: " + ex.Message);
            }
            return signedXmlHex;
        }

        /// <summary>
        /// Crea il file Xml finale comprensivo del file p7m e della marca (file tsr) codificati in base64
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="signedXml"></param>
        /// <param name="base64TSR"></param>
        /// <param name="idCons"></param>
        /// <returns></returns>
        public bool timeSignXml(DocsPaVO.utente.InfoUtente infoUtente, byte[] signedXml, string base64TSR, 
                    string idCons, string progressivoMarca)
        {
            bool result = false;
            System.IO.FileStream fs = null;
            try
            {
                //Creo il file binario .TSR
                if (!string.IsNullOrEmpty(base64TSR))
                {
                    byte[] base64 = Convert.FromBase64String(base64TSR);

                    string fileTsr = PathManager.GetPathFileChiusuraTSR(infoUtente, idCons, progressivoMarca);

                    fs = System.IO.File.Create(fileTsr);
                    fs.Write(base64, 0, base64.Length);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                string err = "Il file completo di marca temporale non è pronto per il download" + ex.Message;
                logger.Debug(err);
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }
            return result;
        }
        /*
         //Codice inefficiente
        /// <summary>
        /// Converte un file in una stringa Esadicimale
        /// </summary>
        /// <param name="byte8In"></param>
        /// <returns></returns>
        private static string byteArrayToHexa(byte[] byte8In)
        {
            string retval = "";
            foreach (byte b in byte8In)
            {
                retval += String.Format("{0:X2}", b);
            }
            return retval;
        }
        */
        /// <summary>
        /// Aggiorna sul DB (nelle relative tabelle) i dati della marca
        /// </summary>
        /// <param name="marca"></param>
        /// <param name="dataMarca"></param>
        /// <param name="dataScadenzaMarca"></param>
        /// <param name="SystemID">id dell'istanza di conservazione</param>
        /// <returns></returns>
        public bool updateTimeStamp(string marca, string dataMarca, string dataScadenzaMarca, string SystemID, string progressivoMarca)
        {
            bool result = false;
            try
            {
                //parsing del timeStamp
                int anno = System.Convert.ToInt32(dataMarca.Substring(0, 4));
                int mese = System.Convert.ToInt32(dataMarca.Substring(4, 2));
                int giorno = System.Convert.ToInt32(dataMarca.Substring(6, 2));
                int ora = System.Convert.ToInt32(dataMarca.Substring(8, 2));
                int min = System.Convert.ToInt32(dataMarca.Substring(10, 2));
                int sec = System.Convert.ToInt32(dataMarca.Substring(12, 2));
                DateTime StartDate = new DateTime(anno, mese, giorno, ora, min, sec);
                dataMarca = StartDate.ToString("MM/dd/yyyy");

                //parsing della data di scadenza
                DateTime outDate = new DateTime();
                DateTime EndDate = new DateTime();
                CultureInfo culture = new CultureInfo("en-US", true);
                if (DateTime.TryParse(dataScadenzaMarca, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out outDate))
                {
                    EndDate = DateTime.Parse(dataScadenzaMarca, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                }
                else
                {
                    CultureInfo culture2 = new CultureInfo("it-IT");
                    dataScadenzaMarca = dataScadenzaMarca.Replace(".", ":");
                    EndDate = Convert.ToDateTime(dataScadenzaMarca, culture2);
                }
                dataScadenzaMarca = EndDate.ToString("MM/dd/yyyy");

                string upInfoCons = " SET VAR_MARCA_TEMPORALE='" + marca + "' WHERE SYSTEM_ID='" + SystemID + "'";
                //string upInfoSupp = " SET DATA_APPO_MARCA=TO_DATE('" + dataMarca + "','MM/DD/YYYY'), DATA_SCADENZA_MARCA=TO_DATE('" + dataScadenzaMarca + "','MM/DD/YYYY'), VAR_MARCA_TEMPORALE='" + marca + "' WHERE ID_CONSERVAZIONE='" + SystemID + "'";
                DocsPaConsManager cm = new DocsPaConsManager();
                //result = cm.UpdateInfoSupporto(upInfoSupp);
                result = cm.UpdateDatiTimeStampConservazione(SystemID, marca, dataMarca, dataScadenzaMarca, progressivoMarca);
                if (result && cm.UpdateInfoConservazione(upInfoCons))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception exDate)
            {
            }
            return result;
        }
    }
}
