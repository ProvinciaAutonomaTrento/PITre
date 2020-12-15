using System;
using System.Web.Services;
using log4net;

namespace InlineConverterEngine
{
    /// <summary>
    /// Questo web service fornisce un punto di accesso web al motore del convertitore
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VTDocs/ConverterEngine")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class ConvertEngineWS : System.Web.Services.WebService
    {
        // Riferimento al motore di conversione
        private static ConverterEngine.ConverterEngine _engine = null;
        private static ILog logger = LogManager.GetLogger(typeof(ConvertEngineWS));
 
        #region Funzioni pubbliche

        /// <summary>
        /// Metodo web per il reset del motore di conversione
        /// </summary>
        [WebMethod(Description="Metodo web per il reset del motore di conversione")]
        public void ResetEngine()
        {
            try
            {
                // Se l'engine non è instanziato viene instanziato
                // altrimenti ne viene richiesto il reset
                if (_engine == null)
                    this.InitializeEngine();
                else
                    _engine.Reset();
            }
            catch (Exception e)
            {
                logger.Error(string.Format("Errore {0} \r\n {1}", e.Message, e.StackTrace));
                if (e.InnerException != null)
                    logger.Error(string.Format("Errore interno {0} \r\n {1}", e.InnerException.Message, e.InnerException.StackTrace));
               
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

        }

        /// <summary>
        /// Metodo web per la conversione di un file
        /// </summary>
        /// <param name="filePath">Nome del file da convertire comprensivo di path</param>
        /// <param name="outFilePath">Nome da assegnare al file convertito</param>
        /// <param name="recognizeText">True se è richiescto anche il riconoscimento testo (OCR)</param>
        /// <returns>True se la conversione è andata a buon fine</returns>
        [WebMethod(Description="Metodo web per la conversione di un file")]
        public bool Convert(String filePath, String outFilePath, bool recognizeText)
        { 
            try
            {
            
                // Se l'engine non è instanziato, viene instanziato
                if (_engine == null)
                    this.InitializeEngine();

                // Conversione del file
                return _engine.Convert(filePath, outFilePath, recognizeText);

            }
            catch (Exception e)
            {
                logger.Error(string.Format("Errore {0} \r\n {1}", e.Message, e.StackTrace));
                if (e.InnerException != null)
                    logger.Error(string.Format("Errore interno {0} \r\n {1}", e.InnerException.Message, e.InnerException.StackTrace));
                
                throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(e);
            }

        }

        #endregion

        #region Funzioni private

        /// <summary>
        /// Funzione per l'inizializzazione del motore di conversione
        /// </summary>
        private void InitializeEngine()
        {
            _engine = new ConverterEngine.ConverterEngine();
        }

        #endregion

    }
}
