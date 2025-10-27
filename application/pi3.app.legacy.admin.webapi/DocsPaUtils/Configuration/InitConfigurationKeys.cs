// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using DocsPaVO.Settings;
using Serilog;
using System;
using System.Collections;

namespace DocsPaUtils.Configuration;

public class InitConfigurationKeys
{

    private static ILogger logger = Log.ForContext(typeof(InitConfigurationKeys));
    /// <summary>
    /// Hashtable che memorizza le singole istanze delle chiavi di configurazione
    /// per tutte le amministrazioni
    /// </summary>
    private static Hashtable _instances = null;
    private static readonly object _instancesLock = new object();

    /// <summary>
    /// Costruttore privato poichè¨ la classe implementa il design pattern singleton
    /// </summary>
    private InitConfigurationKeys()
    {
    }

    /// <summary>
    /// Crea, se non esiste, l'istanza della classe e inizializza gli attributi 
    /// </summary>
    /// <returns></returns>
    public static ConfigRepository getInstance(string idAmm)
    {
        string instanceName = HeaderValue.Instance.ConnectionName.Value;

        if (_instances == null)
            _instances = Hashtable.Synchronized(new Hashtable());

        lock (_instancesLock)
        {
            if (!_instances.ContainsKey(instanceName))
            {
                _instances.Add(instanceName, new Hashtable());
            }
            if (!(_instances[instanceName] as Hashtable).ContainsKey(idAmm))
            {
                ConfigRepository instance = new ConfigRepository();
                ArrayList ListaChiaviConfig = ChiaviConfigManager.GetChiaviConfig(idAmm);
                instance.ListaChiavi = ListaChiaviConfig;
                try
                {
                    (_instances[instanceName] as Hashtable).Add(idAmm, instance);
                }
                catch(ArgumentException ex)
                {
                    //
                }
                catch (Exception ex)
                {
                    //
                }
            }
        }
        return (ConfigRepository)(_instances[instanceName] as Hashtable)[idAmm];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idAmm">Se "0" viene cercata una chiave globale, </param>
    /// <param name="key">nome della chiave da reperire</param>
    /// <returns></returns>
    public static string GetValue(string idAmm, string key)
    {
        string kValue = null;
        ConfigRepository config = getInstance(idAmm);

        //if (key.Equals("BE_TEMP_PATH"))
        //    return "C:\\Lavoro\\Logs";
        if (config != null && config.ContainsKey(key))
            return config[key].ToString();
        else
            return kValue;
    }

    /// <summary>
    /// 
    /// </summary>
    public static void clearAll()
    {
        if (_instances != null)
            _instances.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void remove(string idAmm)
    {

        string instanceName = HeaderValue.Instance.ConnectionName.Value;
        if (_instances != null &&  (_instances[instanceName] as Hashtable).ContainsKey(idAmm))
        {
            (_instances[instanceName] as Hashtable).Remove(idAmm);
        }
    }

    /// <summary>
    /// Resetta le chiavi di configurazione da db
    /// </summary>
    /// <returns></returns>
    public static void resetKeys()
    {
        _instances = null;
        _instances = Hashtable.Synchronized(new Hashtable());
    }

}
