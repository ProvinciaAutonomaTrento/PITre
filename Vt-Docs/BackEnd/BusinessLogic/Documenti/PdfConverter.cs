using System;
using System.IO;
using System.Threading;
using DPA.Common;
using log4net;
using InlineConverterEngine.Proxy;

namespace BusinessLogic.Documenti
{
	/// <summary>
	/// Classe necessaria per accedere alle funzioni di
	/// conversione in pdf dei documenti server-side
	/// </summary>
	public sealed class PdfConverter
	{
        private static ILog logger = LogManager.GetLogger(typeof(PdfConverter));
        /// <summary>
        /// Oggetto Mutex per gestire la concorrenza nella conversione in pdf
        /// </summary>
        private static Mutex _mutex = null;

		// Istanza convertitore in pdf corrente
		private static IPdfConverter _converter=null;

		private static bool _convertInlineActive=false;
		private static string _converterType=string.Empty;
		private static string _convertibleFileTypes=string.Empty;

        // Indirizzo del motore di conversione
        private static String _converterEngineUrl = String.Empty;

		static PdfConverter()
		{
			// Caricamento configurazioni conversione in pdf dal file web.config
			LoadConfigurations();
		}

		#region Public methods

        /// <summary>
        /// Funzione per la conversione del file con l'engine
        /// </summary>
        /// <param name="filePath">File da convertire</param>
        /// <param name="outPdfFilePath">Nome da assegnare al file convertito</param>
        /// <param name="recognizeText">True se è richiesto il riconoscimento OCR</param>
        /// <returns>True se la conversione è andata a buon fine</returns>
        private static bool ConvertWithEngine(String filePath, String outPdfFilePath, bool recognizeText)
        {
            // Scrittura del log di inizio conversione
            logger.Debug(String.Format(
                    "INIT - BusinessLogic.Documenti.PdfConverter.ConvertWithEngine(filePath: '{0}', outPdfFilePath: '{1}', recognizeText: '{2}')",
                                filePath, outPdfFilePath, recognizeText));

            // Valore da restituire
            bool toReturn = false;

            // Proxy cui inoltrare le chiamate verso il convertitore
            ConvertEngineWS converter = new ConvertEngineWS();

            // Impostazione dell'url del convertitore
            converter.Url = _converterEngineUrl;

            try
            {
                // Conversione del file
                toReturn = converter.Convert(
                    filePath,
                    outPdfFilePath,
                    recognizeText);
            }
            catch (Exception e)
            {
                // Recupero dell'eccezione originale e sua scrittura nel log
                ApplicationException originalException = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);

                logger.Debug("Eccezione durante la conversione con il motore esterno.", originalException);

            }

            // Scrittura del log di fine conversione
            logger.Debug(String.Format(
                    "END - BusinessLogic.Documenti.PdfConverter.ConvertWithEngine(filePath: '{0}', outPdfFilePath: '{1}', recognizeText: '{2}')",
                                filePath, outPdfFilePath, recognizeText));

            // Restituzione del risultato di conversione
            return toReturn;
        }

		/// <summary>
		/// Conversione in PDF mediante il convertitore correntemente impostato
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="outPdfFilePath"></param>
		/// <param name="recognizeText"></param>
		/// <returns></returns>
		public static bool Convert(string filePath, string outPdfFilePath, bool recognizeText)
		{
            logger.Debug(string.Format("INIT - BusinessLogic.Documenti.PdfConverter.Convert(filePath: '{0}', outPdfFilePath: '{1}', recognizeText: '{2}')", 
                                filePath, outPdfFilePath, recognizeText));

            bool retValue = false;

            try
            {
                if (CurrentMutex.WaitOne())
                {
                    // Se la conversione è attiva e se il file può essere convertito
                    if (CanConvertFile(filePath))
                    {
                        IPdfConverter converter = GetConverter();

                        if (converter != null)
                            retValue = converter.Convert(filePath, outPdfFilePath, recognizeText);
                        else
                            throw new ApplicationException("Convertitore PDF inline non istanziato, impossibile effettuare la conversione del documento");
                    }
                    else
                        logger.Debug("Conversione PDF inline non supportata per il formato del documento");
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new ApplicationException("Errore nella conversione PDF inline del documento", ex);
            }
            finally
            {
                CurrentMutex.ReleaseMutex();
                
                if (CurrentMutex != null)
                {
                    CurrentMutex.Close();
                    _mutex = null;
                }

                logger.Debug("END - BusinessLogic.Documenti.PdfConverter.Convert");
            }

			return retValue;
		}

		/// <summary>
		/// Conversione in PDF mediante il convertitore correntemente impostato
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="outPdfFilePath"></param>
		/// <returns></returns>
		public static bool Convert(string filePath,string outPdfFilePath)
		{
            // Risultato della conversione
            bool converted;

            // Se è presente la chiave di configurazione "INLINE_CONVERTER_URL", la
            // richiesta di conversione viene inoltrata al motore di conversione
            if (!String.IsNullOrEmpty(_converterEngineUrl))
                converted = ConvertWithEngine(filePath, outPdfFilePath, false);
            else
			    converted = Convert(filePath, outPdfFilePath, false);

            // Restituzione dell'esito della conversione
            return converted;
		}

		/// <summary>
		/// Verifica se il convertitore corrente supporta ocr
		/// </summary>
		public static bool OcrSupported()
		{
			bool retValue=false;

			if (ConvertInlineActive())
			{
				IPdfConverter converter=GetConverter();
				retValue=converter.OcrSupported;
			}

			return retValue;
		}

		/// <summary>
		/// Verifica se la conversione in PDF inline è attiva o meno
		/// </summary>
		/// <returns></returns>
		public static bool ConvertInlineActive()
		{
			return _convertInlineActive;
		}

		/// <summary>
		/// Verifica se, in base ai formati predefiniti impostati nel file web.config,
		/// il file richiesto può essere convertito in formato pdf
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static bool CanConvertFile(string fileName)
		{
			bool retValue=false;

			if (ConvertInlineActive())
			{
				string[] convertibleTypes=GetConvertibleFileTypes();
		
				if (convertibleTypes.Length>0)
				{
					FileInfo fileInfo=new FileInfo(fileName);
					string fileExt=fileInfo.Extension.ToLower().Replace(".","");
                    //if pdf no convert 
                    if (fileExt != null && fileExt.Equals("pdf"))
                        return retValue = false;

					foreach (string type in convertibleTypes)
					{
						retValue=(type.Equals("*") || type.ToLower().Equals(fileExt));
						if (retValue)
							break;
					}
				}
			}

			return retValue;
		}

		/// <summary>
		/// Reperimento di tutte le tipolgie predefinite di file 
		/// per cui è possibile effettuare la conversione in pdf
		/// </summary>
		/// <returns></returns>
		public static string[] GetConvertibleFileTypes()
		{
			string[] retValue=null;

			if (ConvertInlineActive() && _convertibleFileTypes!=null && _convertibleFileTypes!=string.Empty)
				retValue=_convertibleFileTypes.Split('|');

			if (retValue==null)
				retValue=new string[1] { "*" };

			return retValue;
		}

		#endregion

		#region Private methods

        /// <summary>
        /// Reperimento oggetto mutex necessario per la
        /// gestione della concorrenza nella conversione in pdf 
        /// </summary>
        /// <returns></returns>
        private static Mutex CurrentMutex
        {
            get
            {
                if (_mutex == null)
                {
                    // Creazione oggetto Mutex con un nome qualificato
                    _mutex = new Mutex(false, "PDF_CONVERTER_MUTEX");
                }

                return _mutex;
            }
        }

		/// <summary>
		/// Caricamento configurazioni di conversione in pdf inline dal file web.config
		/// </summary>
		private static void LoadConfigurations()
		{
			try
			{
				_convertInlineActive=System.Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["PDF_CONVERT_INLINE_ACTIVE"]);
			}
			catch
			{}

			_converterType=System.Configuration.ConfigurationManager.AppSettings["PDF_CONVERTER_TYPE"];
			_convertibleFileTypes=System.Configuration.ConfigurationManager.AppSettings["PDF_CONVERTIBLE_FILE_TYPES"];

            // Lettura dell'url del motore di conversione esterno
            try
            {
                _converterEngineUrl = System.Configuration.ConfigurationManager.AppSettings["INLINE_CONVERTER_URL"];
            }
            catch (Exception) { }
		}

		/// <summary>
		/// Creazione istanza oggetto convertitore in pdf
		/// </summary>
		/// <returns></returns>
		private static IPdfConverter GetConverter()
		{
			if (_converter==null && !string.IsNullOrEmpty(_converterType))
			{
				try
				{
					Type type=Type.GetType(_converterType, true, true);
				
					_converter=(IPdfConverter) Activator.CreateInstance(type);
				}
				catch(Exception ex)
				{
					logger.Debug(string.Format("PdfConverter.CreateConverter: errore nella creazione dell'oggetto convertitore '{0}'", _converterType), ex);
				}
			}

			return _converter;
		}

		#endregion
	}
}
