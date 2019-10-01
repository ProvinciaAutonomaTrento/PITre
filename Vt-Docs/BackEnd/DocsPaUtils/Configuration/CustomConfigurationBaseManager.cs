using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using log4net;

namespace DocsPaUtils.Configuration
{
	/// <summary>
	/// Questa classe gestisce l'acquisizione dei dati di configurazione relativi 
	/// ad una 'section' di tipo 'System.Configuration.NameValueSectionHandler'
	/// </summary>
	public class CustomConfigurationBaseManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(CustomConfigurationBaseManager));
		/// <summary>
		/// Ritorna la lista di chiavi
		/// </summary>
		/// <param name="sectionName">Nome della 'section' nel file di configurazione</param>
		/// <returns>Lista delle chiavi o 'null' in caso di errore</returns>
		protected static ArrayList GetConfigKeys(string sectionName)
		{
			ArrayList result = new ArrayList();

			try
			{
				NameValueCollection nameValueCollection = (NameValueCollection)ConfigurationManager.GetSection(sectionName);
			
				foreach(string key in nameValueCollection.Keys)
				{
					result.Add(key);
				}
			}
			catch(Exception exception)
			{
				logger.Error(exception.ToString());
				result = null;
			}

			return result;
		}

		/// <summary>Ritorna il valore associato ad una chiave</summary>
		/// <param name="sectionName">Nome della 'section' nel file di configurazione</param>
		/// <param name="key">Chiave di ricerca</param>
		/// <returns>Valore della chiave o 'null' in caso di errore</returns>
		protected static string GetConfigValue(string sectionName, string key)
		{
			string result = null;

			try
			{
				NameValueCollection nameValueCollection = (NameValueCollection)ConfigurationManager.GetSection(sectionName);
				result = nameValueCollection[key];
			}
			catch(Exception exception)
			{
				logger.Error(exception.ToString());
				result = null;
			}

			return result;
		}

        public static string isEnableContatoreTitolario()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_CONTATORE_TIT"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["ENABLE_CONTATORE_TIT"].ToUpper() != "")
                return System.Configuration.ConfigurationManager.AppSettings["ENABLE_CONTATORE_TIT"];
            else
                return "";
        }

        public static string isEnableProtocolloTitolario()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_CONTATORE_TIT"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["ENABLE_CONTATORE_TIT"].ToUpper() != "" &&
                System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROTOCOLLO_TIT"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROTOCOLLO_TIT"].ToUpper() != "")
                return System.Configuration.ConfigurationManager.AppSettings["ENABLE_PROTOCOLLO_TIT"];
            else
                return "";
        }

        public static bool isEnableRiferimentiMittente()
        {
            if (System.Configuration.ConfigurationManager.AppSettings["ENABLE_RIFERIMENTI_MITTENTE"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["ENABLE_RIFERIMENTI_MITTENTE"].ToString() == "1")
                return true;
            else
                return false;
        }
	}
}
