using System;
using System.Collections;
using System.Xml;
using System.Configuration;
using log4net;

namespace DocsPaUtils.LogsManagement 
{
	/// <summary>
	/// </summary>
	public class ImportExportLogger
	{
        private static ILog logger = LogManager.GetLogger(typeof(ImportExportLogger));
		private static ImportExportLogger itsInstance = null;
		private static XmlDocument xmlDocument = null;
		private static string fileName = null;
		private static int counter = 0;

		/// <summary>
		/// Costruttore privato poichè la classe implementa il design pattern singleton
		/// </summary>
		private ImportExportLogger()
		{}

		/// <summary>
		/// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
		/// utilizzando il metodo initializeQuery
		/// </summary>
		/// <returns></returns>
		public static ImportExportLogger getInstance(string logType, int totalItems) 
		{
			if (itsInstance == null) 
			{
				itsInstance = new ImportExportLogger();
				ResetLog(logType, totalItems);
			}

			return itsInstance;
		}

		/// <summary>
		/// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
		/// utilizzando il metodo initializeQuery
		/// </summary>
		/// <returns></returns>
		public static ImportExportLogger getInstance() 
		{
			if (itsInstance == null) 
			{
				itsInstance = new ImportExportLogger();
				ResetLog("", 0);
			}

			return itsInstance;
		}

		/// <summary>
		/// Crea dal file Web.Config le informazioni relative alla posizione del file XML 
		/// delle query. 
		/// </summary>
		/// <param name="xmlPathName"></param>
		private static void Initialize(string xmlPathName, string logType, int total)
		{
			fileName = xmlPathName;
			counter = 0;

			try
			{				
				//lettura file xml
				xmlDocument = new XmlDocument();
			
				System.Diagnostics.Debug.WriteLine("Creating XML file: " + fileName);
				xmlDocument.LoadXml("<log type='" + logType + "' counter='0' totalItems='" + total + "' message='' />");
				xmlDocument.Save(fileName);
			}
			catch(Exception e)
			{
				logger.Error("Errore durante la creazione del file di log.", e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void IncreaseCounter()
		{
			counter++;
			xmlDocument.DocumentElement.Attributes["counter"].InnerText = counter.ToString();
			SaveXmlFile();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void SetMessage(string message)
		{
			xmlDocument.DocumentElement.Attributes["message"].InnerText = message;
			SaveXmlFile();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void SetMessage(int counter, int totalItems, string message)
		{
			xmlDocument.DocumentElement.Attributes["counter"].InnerText    = counter.ToString();
			xmlDocument.DocumentElement.Attributes["totalItems"].InnerText = totalItems.ToString();
			xmlDocument.DocumentElement.Attributes["message"].InnerText	   = message;
			SaveXmlFile();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="counter"></param>
		/// <returns></returns>
		public void UpdateCounter(int counter)
		{
			xmlDocument.DocumentElement.Attributes["counter"].InnerText = counter.ToString();
			SaveXmlFile();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="counter"></param>
		/// <returns></returns>
		public void AddLog(string message)
		{
			XmlElement logEntry = xmlDocument.CreateElement("messageEntry");
			logEntry.InnerText = message;
			xmlDocument.DocumentElement.AppendChild(logEntry);
			SaveXmlFile();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="counter"></param>
		/// <returns></returns>
		public void AddException(string message, Exception exception)
		{
			XmlElement exceptionEntry = xmlDocument.CreateElement("exceptionEntry");
			XmlElement messageElement = xmlDocument.CreateElement("exceptionEntry");
			XmlElement exceptionElement = xmlDocument.CreateElement("exceptionEntry");
			messageElement.InnerText = message;
			exceptionElement.InnerText = exception.ToString();
			exceptionEntry.AppendChild(messageElement);
			exceptionEntry.AppendChild(exceptionElement);
			xmlDocument.DocumentElement.AppendChild(exceptionEntry);
			SaveXmlFile();
		}

		/// <summary>
		/// 
		/// </summary>
		public static void ResetLog(string logType, int totalItems)
		{
			Initialize(System.Configuration.ConfigurationManager.AppSettings["importExportLogPath"], logType, totalItems);
		}

		/// <summary>
		/// 
		/// </summary>
		private void SaveXmlFile()
		{
			try
			{
				xmlDocument.Save(fileName);
				System.Diagnostics.Debug.WriteLine("Aggiornato il file di log per import/export.");
			}
			catch(Exception)
			{
				System.Diagnostics.Debug.WriteLine("Il file di log per import/export e' in uso da un altro thread.\nIl salvataggio non e' stato effettuato.");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="counter"></param>
		/// <param name="total"></param>
		/// <returns></returns>
		public static bool ReadImportState(out int counter, out int total, out string message)
		{
			bool result = true; // Presume successo
			counter = 0;
			total = 0;
			message = null;

			try
			{
				string logFile = System.Configuration.ConfigurationManager.AppSettings["importExportLogPath"];
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(logFile);

				counter = Int32.Parse(xmlDocument.DocumentElement.Attributes["counter"].InnerText);
				total   = Int32.Parse(xmlDocument.DocumentElement.Attributes["totalItems"].InnerText);
				message = xmlDocument.DocumentElement.Attributes["message"].InnerText;
			}
			catch(Exception exception)
			{
				System.Diagnostics.Debug.WriteLine("Errore durante la lettura del log di import/export.");
				System.Diagnostics.Debug.WriteLine(exception.ToString());
				result = false;
			}

			return result;
		}
	}
}
