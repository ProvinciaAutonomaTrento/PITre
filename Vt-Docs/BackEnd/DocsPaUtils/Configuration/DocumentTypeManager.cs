using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

namespace DocsPaUtils.Configuration
{
	/// <summary>
	/// Questa classe gestisce l'acquisizione dei dati di configurazione relativi 
	/// alle tipologie di documento gestite da DocsPa
	/// </summary>
	public class DocumentTypeManager : CustomConfigurationBaseManager
	{
		private const string sectionName = "documentType";

		/// <summary>
		/// Ritorna la lista di chiavi
		/// </summary>
		/// <returns>Lista delle chiavi o 'null' in caso di errore</returns>
		public static ArrayList GetKeys()
		{
			return GetConfigKeys(sectionName);
		}

		/// <summary>Ritorna il valore associato ad una chiave</summary>
		/// <param name="key">Chiave di ricerca</param>
		/// <returns>Valore della chiave o 'null' in caso di errore</returns>
		public static string GetValue(string key)
		{
			return GetConfigValue(sectionName, key);
		}
	}
}
