using System;
using System.Collections;
using System.Xml;
using System.Configuration;
using log4net;

namespace DocsPaUtils
{
	/// <summary>
	/// </summary>
	public class InitImportExportQuery
	{
        private static ILog logger = LogManager.GetLogger(typeof(InitImportExportQuery));
		private Hashtable queryHashTable = null;
		private static InitImportExportQuery itsInstanceIE = null;

		/// <summary>
		/// Costruttore privato poichè la classe implementa il design pattern singleton
		/// </summary>
		private InitImportExportQuery()
		{}

		/// <summary>
		/// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
		/// utilizzando il metodo initializeQuery
		/// </summary>
		/// <returns></returns>
		public static InitImportExportQuery getInstance() 
		{
			if (itsInstanceIE == null) 
			{
				itsInstanceIE = new InitImportExportQuery();
				itsInstanceIE.initializeQuery(getKey("ImportExportQuery"));
			}

			return itsInstanceIE;
		}

		/// <summary>
		/// Legge dal file Web.Config le informazioni relative alla posizione del file XML 
		/// delle query. Crea l’Hash Table, lo carica e inizializza gli attributi 
		/// "dateFormat", "dateTimeFormat"
		/// </summary>
		/// <param name="xmlPathName"></param>
		public void initializeQuery(String xmlPathName)
		{
			//creazione hashTable
            queryHashTable = Hashtable.Synchronized(new Hashtable());

			try
			{				
				//lettura file xml
				System.Xml.XmlDocument xmlDocument = new XmlDocument();
				
				logger.Debug("Loading XML file: " + xmlPathName);
				xmlDocument.Load(xmlPathName);
						
				//caricamento Hash Table
				XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("query");
				
				foreach(XmlNode xmlNode in xmlNodeList)
				{				
					XmlNodeList xmlNodeList2 = xmlNode.ChildNodes;
					queryHashTable.Add(xmlNodeList2.Item(0).InnerText.ToUpper(),xmlNodeList2.Item(1).InnerText);					
				}	
			}
			catch(Exception e)
			{
				logger.Debug("Errore nel caricamento della hashtable", e);
			}
		}

		/// <summary>
		/// Istanzia un oggetto di tipo Query con i valori caricati dal file di configurazione xml
		/// </summary>
		/// <param name="queryDef"></param>
		/// <returns></returns>
		public Query getQuery(String queryDef)
		{
			queryDef = queryDef.ToUpper();
			Query q = new Query((String)queryHashTable[queryDef] /*, dateFormat, dateTimeFormat*/);
			return q;
		}

		/// <summary>
		/// Legge nel file di configurazione web.config il nome del file xml contenente le query
		/// </summary>
		/// <param name="keyName"></param>
		/// <returns></returns>
		public static string getKey(string keyName)
		{
			string retValue=null;

			try
			{
				if (System.Configuration.ConfigurationManager.AppSettings[keyName]!=null)
				{
					retValue= AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings[keyName];
				}
				else
				{
					// TODO: gestione errore "La chiave di configurazione '"+keyName+"' non esiste. "
				}
			}
			catch(System.Exception exception)
			{
				logger.Debug("Errore nella lettura dei dati dalla chiave di configurazione '" + keyName, exception);
			}

			return retValue;
		}
	}
}
