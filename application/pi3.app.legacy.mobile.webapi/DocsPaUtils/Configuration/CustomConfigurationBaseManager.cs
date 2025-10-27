// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

namespace DocsPaUtils.Configuration;

/// <summary>
/// Questa classe gestisce l'acquisizione dei dati di configurazione relativi
/// ad una 'section' di tipo 'System.Configuration.NameValueSectionHandler'
/// </summary>
public class CustomConfigurationBaseManager
{
    private static ILogger logger = Log.ForContext(typeof(CustomConfigurationBaseManager));
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
        if (DocsPaVO.Settings.AppSettings.Instance.ENABLE_CONTATORE_TIT != null &&
            DocsPaVO.Settings.AppSettings.Instance.ENABLE_CONTATORE_TIT.ToUpper() != "")
            return DocsPaVO.Settings.AppSettings.Instance.ENABLE_CONTATORE_TIT;
        else
            return "";
    }

    public static string isEnableProtocolloTitolario()
    {
        if (DocsPaVO.Settings.AppSettings.Instance.ENABLE_CONTATORE_TIT != null &&
            DocsPaVO.Settings.AppSettings.Instance.ENABLE_CONTATORE_TIT.ToUpper() != "" &&
            DocsPaVO.Settings.AppSettings.Instance.ENABLE_PROTOCOLLO_TIT != null &&
            DocsPaVO.Settings.AppSettings.Instance.ENABLE_PROTOCOLLO_TIT.ToUpper() != "")
            return DocsPaVO.Settings.AppSettings.Instance.ENABLE_PROTOCOLLO_TIT;
        else
            return "";
    }

    public static bool isEnableRiferimentiMittente()
    {
        if (DocsPaVO.Settings.AppSettings.Instance.ENABLE_RIFERIMENTI_MITTENTE != null &&
            DocsPaVO.Settings.AppSettings.Instance.ENABLE_RIFERIMENTI_MITTENTE.ToString() == "1")
            return true;
        else
            return false;
    }
}
