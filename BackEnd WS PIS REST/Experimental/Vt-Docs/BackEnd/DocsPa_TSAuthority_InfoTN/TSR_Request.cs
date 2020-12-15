using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DocsPa_I_TSAuthority;
using DocsPaVO.areaConservazione;
using log4net;

namespace DocsPa_TSAuthority_InfoTN_V2HASH
{
    /// <summary>
    /// Namespace per la generazione del timestamp tramite hash usando i nuovi serivi di IT
    /// </summary>
    public class TSR_Request : I_TSR_Request
    {
        private static ILog logger = LogManager.GetLogger(typeof(TSR_Request));

        #region I_TSR_Request Members
        /// <summary>
        /// Generazione timestamp con il nuovo metodo di IT tramite hash (risparmia traffico rete)
        /// </summary>
        /// <param name="TimeStampQuery">Struttura contente le informazioni per la generazione della marca</param>
        /// <returns>Struttura contente le informazioni della marca generata</returns>
        public OutputResponseMarca getTimeStamp(InputMarca TimeStampQuery)
        {
            logger.Debug("Chiamato DocsPa_TSAuthority_InfoTN_V2HASH");
            string urlTSA = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["URL_TSA"]))
            {
                urlTSA = ConfigurationManager.AppSettings["URL_TSA"].ToString();

            }
            DocsPa_TSAuthority_InfoTN.MarcaWCF m = new DocsPa_TSAuthority_InfoTN.MarcaWCF();

            InputMarca inputMarca = new DocsPaVO.areaConservazione.InputMarca();
            inputMarca.applicazione = TimeStampQuery.applicazione;
            inputMarca.file_p7m = TimeStampQuery.file_p7m;
            inputMarca.riferimento = TimeStampQuery.riferimento;

            DocsPaVO.areaConservazione.OutputResponseMarca outVal = m.getMarcaByHash(inputMarca, urlTSA);
            return outVal;
        }
        #endregion
    }
}

namespace DocsPa_TSAuthority_InfoTN_V2
{
    /// <summary>
    /// Namespace per la generazione del timestamp usando i nuovi serivi di IT
    /// </summary>
    public class TSR_Request : I_TSR_Request
    {
        private static ILog logger = LogManager.GetLogger(typeof(TSR_Request));
        #region I_TSR_Request Members
        /// <summary>
        /// Generazione timestamp con il nuovo metodo di IT
        /// </summary>
        /// <param name="TimeStampQuery">Struttura contente le informazioni per la generazione della marca</param>
        /// <returns>Struttura contente le informazioni della marca generata</returns>
        public OutputResponseMarca getTimeStamp(InputMarca TimeStampQuery)
        {
            logger.Debug("Chiamato DocsPa_TSAuthority_InfoTN_V2");
            string urlTSA = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["URL_TSA"]))
            {
                urlTSA = ConfigurationManager.AppSettings["URL_TSA"].ToString();
               
            }
            DocsPa_TSAuthority_InfoTN.MarcaWCF m = new DocsPa_TSAuthority_InfoTN.MarcaWCF();

            InputMarca inputMarca = new DocsPaVO.areaConservazione.InputMarca();
            inputMarca.applicazione = TimeStampQuery.applicazione;
            inputMarca.file_p7m = TimeStampQuery.file_p7m;
            inputMarca.riferimento = TimeStampQuery.riferimento;

            DocsPaVO.areaConservazione.OutputResponseMarca outVal = m.getMarcaByFile(inputMarca, urlTSA);
            return outVal;
        }
        #endregion
    }
}


namespace DocsPa_TSAuthority_InfoTN
{
    /// <summary>
    /// Namespace per la generazione del timestamp usando i vecchi serivi di IT
    /// </summary>
    public class TSR_Request: I_TSR_Request
    {
        #region I_TSR_Request Members
        private static ILog logger = LogManager.GetLogger(typeof(TSR_Request));
        /// <summary>
        /// Generazione timestamp con il vecchio metodo di IT
        /// </summary>
        /// <param name="TimeStampQuery">Struttura contente le informazioni per la generazione della marca</param>
        /// <returns>Struttura contente le informazioni della marca generata</returns>
        public OutputResponseMarca getTimeStamp(InputMarca TimeStampQuery)
        {
            logger.Debug("Chiamato DocsPa_TSAuthority_InfoTN");
            OutputResponseMarca resultMarca = new OutputResponseMarca();
            marcatura.InputMarca inputMarca = new DocsPa_TSAuthority_InfoTN.marcatura.InputMarca();
            marcatura.OutputResponseMarca outMarca = new DocsPa_TSAuthority_InfoTN.marcatura.OutputResponseMarca();
            marcatura.marcatura Marca = new DocsPa_TSAuthority_InfoTN.marcatura.marcatura();

            string urlTSA = string.Empty;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["URL_TSA"]))
            {
                urlTSA = ConfigurationManager.AppSettings["URL_TSA"].ToString();
                Marca.Url = urlTSA;
            }

            inputMarca.applicazione = TimeStampQuery.applicazione;
            inputMarca.file_p7m = TimeStampQuery.file_p7m;
            inputMarca.riferimento = TimeStampQuery.riferimento;
            outMarca = Marca.getTSR(inputMarca);
            //mapping del risultato sull'oggetto di DocsPa
            resultMarca.descrizioneErrore = outMarca.descrizioneErrore;
            resultMarca.docm = outMarca.docm;
            resultMarca.dsm = outMarca.dsm;
            resultMarca.esito = outMarca.esito;
            resultMarca.fhash = outMarca.fhash;
            resultMarca.marca = outMarca.marca;
            resultMarca.sernum = outMarca.sernum;
            resultMarca.TSA = new TSARFC2253();
            resultMarca.TSA.TSARFC2253Name = outMarca.TSA;

            return resultMarca;
        }

        #endregion
    }
}
