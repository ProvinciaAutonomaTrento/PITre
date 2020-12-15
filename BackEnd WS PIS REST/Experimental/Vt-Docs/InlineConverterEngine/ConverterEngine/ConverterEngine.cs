using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using DPA.Common;
using InlineConverterEngine.EngineException;
using log4net;

namespace InlineConverterEngine.ConverterEngine
{
    /// <summary>
    /// Questa classe implementa il motore per la conversione inline.<br />
    /// Per funzionare correttamente è necessario che nel web config siano
    /// presenti le seguenti chiavi:
    /// - PDF_CONVERTIBLE_FILE_TYPES valorizzata con tutte le estensioni accettate per la conversione. Le varie estensioni devono essere separate da un carattere di pipe (|)
    /// - PDF_CONVERTER_TYPE valorizzata con il nome dell'assembly cui delegare la convesione
    /// <example>
    /// - PDF_CONVERTIBLE_FILE_TYPES = TIFF|TIF|JPG|JPEG|BMP|PNG
    /// - PDF_CONVERTER_TYPE
    /// </example>
    /// </summary>
    public sealed class ConverterEngine
    {
        private static ILog logger = LogManager.GetLogger(typeof(ConverterEngine));
      
        #region Variabili globali

        // Estensioni accettate per la conversione
        private string _convertibleFileTypes = String.Empty;

        // Nome dell'assembly da utilizzare per la conversione
        private string _converterType = String.Empty;

        // Nome del Mutex da usare
        private string _mutexId = String.Empty;

        // Mutex utilizzato per regolare le richieste di conversione
        private static Mutex _mutex = null;

        // Istanza del convertitore da utilizzare per la conversione
        private static IPdfConverter _pdfConverter = null;

        // Array delle estensioni accettate dai convertitori
        private static string[] _fileExtensions;

        #endregion

        #region Funzioni pubbliche

        /// <summary>
        /// Metodo costruttore per l'inizializzazione del motore di conversione
        /// </summary>
        public ConverterEngine()
        {
            // Inizializzazione del motore
            this.Initialize();

        }

        /// <summary>
        /// Questa funzione consente di reinizializzare il motore
        /// </summary>
        public void Reset()
        {
            // Reset del mutex
            _mutex.Close();
            _mutex = null;

            // Inizializzazione del motore
            this.Initialize();

        }

        /// <summary>
        /// Funzione per la conversione di un file
        /// </summary>
        /// <param name="filePath">Nome del file da convertire comprensivo di path</param>
        /// <param name="outFilePath">Nome da assegnare al file convertito</param>
        /// <param name="recognizeText">True se è richiescto anche il riconoscimento testo (OCR)</param>
        /// <returns>True se la conversione è andata a buon fine</returns>
        public bool Convert(String filePath, String outFilePath, bool recognizeText)
        {
            // Valore da restituire
            bool toReturn = false;

            // Se il file non può essere convertito, viene lanciata un'eccezione
            if (!this.CanConvertFile(filePath))
                throw new NotConvertibleFileException();

            // Se il convertitore non è inizializzato, viene lanciata un'eccezione
            // In teoria questa eccezione non dovrebbe essere mai lanciata. E' stata
            // inserita per eventuali sviluppi / modifiche successive
            if (_pdfConverter == null)
                throw new ConverterNotInitializedException();

            // Conversione del file
            toReturn = _pdfConverter.Convert(filePath, outFilePath, recognizeText);

            // Restituzione dell'esito della conversione
            return toReturn;

        }

        #endregion


        #region Funzione di supporto

        /// <summary>
        /// Funzione per l'inizializzazione del motore di conversione
        /// </summary>
        private void Initialize()
        {
            // Inizializzazione del mutex e del convertitore solo se il
            // non è stato già inizializzato.
            // Inizialmente il thread non deve possedere il mutex
            if (_mutex == null)
            {
                // Lettura del valori salvati nella configurazione
                this.ReadConfiguration();
                this._mutexId = ConfigurationManager.AppSettings["INSTANCE_ID"];


                if (_mutexId.Equals("$RANDOMIZE$"))
                    _mutexId = Guid.NewGuid().ToString();

                logger.DebugFormat("MutexID: CONVERTER_ENGINE_MUTEX{0}", _mutexId);
                _mutex = new Mutex(false, String.Format("CONVERTER_ENGINE_MUTEX{0}", _mutexId));
                _pdfConverter = this.GetConverter();

                // Interpretazione dei tipi di file accettati per la conversione
                this.InitializeFileTypeCollection();
            }
        }

        /// <summary>
        /// Funzione per l'instanziazione della classe responsabile della conversione
        /// </summary>
        /// <returns>Riferimento all'istanza del convetitore</returns>
        private IPdfConverter GetConverter()
        {
            // Convertitore da restituire
            IPdfConverter toReturn = null;

            // Se non è valorizzato il nome dell'assembly da utilizzare per la conversione,
            // viene lanciata un'eccezione appropriata

            ControlloConverterType();

            // Tramite reflection si prova ad istanziare la classe del convertitore
            try
            {
                // Recupero del tipo di convertitore a partire dal nome dell'assembly
                Type type = Type.GetType(_converterType, true, true);
                logger.Debug(String.Format("tipo convertitore: {0} versione {1}", type.Assembly.FullName, type.Assembly.ImageRuntimeVersion));

                // Creazione dell'istanza
                toReturn = (IPdfConverter)Activator.CreateInstance(type);

            }
            catch (Exception ex)
            {
                // Rilancio dell'eccezione ai livelli superiori
                logger.Error (String.Format ("Errore: {0} \r\n {1}",ex.Message,ex.StackTrace ));
                if (ex.InnerException !=null)
                    logger.Error(string.Format("Errore interno {0} \r\n {1}", ex.InnerException.Message, ex.InnerException.StackTrace));

                throw new EngineInitializationException(ex);
            }

            // Restituzione dell'istanza del convertitore
            return toReturn;

        }

        private void ControlloConverterType()
        {
            if (String.IsNullOrEmpty(_converterType))
                throw new ConverterTypeNotValidException();
        }

        /// <summary>
        /// Funzione per l'inizializzazione della collezione delle
        /// estensioni dei file accettati dal convertitore
        /// </summary>
        private void InitializeFileTypeCollection()
        {
            // Inizializzazione dell'array con le estensioni dei 
            // file che è possibile convertire
            _fileExtensions = this._convertibleFileTypes.Split('|');

        }

        /// <summary>
        /// Funzione per la lettura dei valori di configurazione.
        /// </summary>
        private void ReadConfiguration()
        {
            // Lettura del nome dell'assembly da utilizzare per la conversione
            this._converterType = ConfigurationManager.AppSettings["PDF_CONVERTER_TYPE"];

            // Lettura delle estensioni accettate per la conversione
            this._convertibleFileTypes = ConfigurationManager.AppSettings["PDF_CONVERTIBLE_FILE_TYPES"];

        }

        /// <summary>
        /// Questa funzione si occupa di verificare se un dato file può essere convertito
        /// </summary>
        /// <param name="filePath">Nome file da controllare</param>
        /// <returns>True se il file è convertibile</returns>
        private bool CanConvertFile(String filePath)
        {

            // Reperimento dell'estensione del file
            String fileExtension = Path.GetExtension(filePath).Remove(0, 1);

            // Controllo della presenza dell'estensione all'interno delle estensioni
            return _fileExtensions.Where(e => e.ToUpper().Equals(fileExtension.ToUpper())).FirstOrDefault() != null;

        }

        #endregion

    }
}
