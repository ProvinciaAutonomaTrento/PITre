using System;
using System.Collections;
using System.Xml;
using System.Configuration;
using log4net;

namespace DocsPaUtils
{
	/// <summary>
	/// </summary>
	public class InitQuery
	{
        private static ILog logger = LogManager.GetLogger(typeof(InitQuery));
		private Hashtable queryHashTable = null;
		private static InitQuery itsInstance = null;
//		private String dateFormat; 
//		private String dateTimeFormat;

		/// <summary>
		/// Costruttore privato poichè la classe implementa il design pattern singleton
		/// </summary>
		private InitQuery()
		{}

		/// <summary>
		/// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
		/// utilizzando il metodo initializeQuery
		/// </summary>
		/// <returns></returns>
		public static InitQuery getInstance() 
		{
			if (itsInstance == null) 
			{
				itsInstance = new InitQuery();
				itsInstance.initializeQuery(getKey("QueryFilePath"));
			}

			return itsInstance;
		}
        public static void resetInstance()
        {
            itsInstance = null;
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

				//Viene chiamata la "switchQueryList" che identifica 
				//a seconda del tipo di DB, il queryList da usare
				xmlPathName = switchQueryList(xmlPathName);
				if(xmlPathName == "")
				{
					logger.Debug("xmlPathName errato !");
					return;				
				}

                logger.Debug("Loading XML file: " + xmlPathName);
				xmlDocument.Load(xmlPathName);								
				
//				//inizializzazione attributo "dateFormat"
//				XmlNodeList xmlNodeListDate = xmlDocument.GetElementsByTagName("dateFormat");				
//				dateFormat = xmlNodeListDate.Item(0).InnerText; 
//				
//				//inizializzazione attributo "dateTimeFormat"
//				XmlNodeList xmlNodeListDateTime = xmlDocument.GetElementsByTagName("dateTimeFormat");				
//				dateTimeFormat = xmlNodeListDateTime.Item(0).InnerText; 
				
				//caricamento Hash Table
				XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("query");
				
				foreach(XmlNode xmlNode in xmlNodeList)
				{				
					XmlNodeList xmlNodeList2 = xmlNode.ChildNodes;

                    if (queryHashTable.ContainsKey(xmlNodeList2.Item(0).InnerText.ToUpper()))
                        logger.Debug(xmlNodeList2.Item(0).InnerText.ToUpper());
                    else
                        queryHashTable.Add(xmlNodeList2.Item(0).InnerText.ToUpper(),xmlNodeList2.Item(1).InnerText);					
				}	
			}
			catch(Exception e)
			{
                logger.Error("Errore nel caricamento della hashtable", e);
			}
		}

		public string switchQueryList(string xmlPathName)
		{
			try
			{
				string[] xmlPathName_Split = xmlPathName.Split('/');
				xmlPathName = "";
				for(int i=0; i<xmlPathName_Split.Length-1; i++)
				{
					xmlPathName += xmlPathName_Split[i] + "/";
				}
					
				if( System.Configuration.ConfigurationManager.AppSettings["dbType"].ToUpper() == "ORACLE" )
				{
					xmlPathName += "queryList_Oracle.xml";
				}
				else
				{
					xmlPathName += "queryList_Sql.xml";				
				}
				return xmlPathName;		
			}
			catch(Exception ex)
			{
                logger.Error("Errore nello switch del queryList file ", ex);
				xmlPathName = "";
				return xmlPathName; 
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
                logger.Error("Errore nella lettura dei dati dalla chiave di configurazione '" + keyName, exception);
			}

			return retValue;
		}
	}
}
