using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using log4net;

namespace NttDataWA.Utils
{
    public class ConfigRepository : Hashtable
    {
        DocsPaWR.ChiaveConfigurazione[] listaChiavi = null;

        public DocsPaWR.ChiaveConfigurazione[] ListaChiavi
        {
            get
            {
                try
                {
                    return listaChiavi;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }

            set
            {
                try
                {
                    listaChiavi = value;
                    for (int i = 0; listaChiavi != null && i < listaChiavi.Length; i++)
                        Add(listaChiavi[i].Codice, listaChiavi[i]);
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }
    }

    public class InitConfigurationKeys
    {
        private static ILog logger = LogManager.GetLogger(typeof(InitConfigurationKeys));
        /// <summary>
        /// Hashtable che memorizza le singole istanze
        /// </summary>
        private static Hashtable _instances = null;

        /// <summary>
        /// ID dell'amministrazione
        /// </summary>
        protected string _idAmm = string.Empty;

        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();


        /// <summary>
        /// Costruttore privato poichè la classe implementa il design pattern singleton
        /// </summary>
        private InitConfigurationKeys()
        {
        }

        /// <summary>
        /// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
        /// utilizzando il metodo initializePath
        /// </summary>
        /// <returns></returns>
        public static ConfigRepository getInstance(string idAmm)
        {
            try
            {
                if (_instances == null)
                    _instances = Hashtable.Synchronized(new Hashtable());

                lock (_instances)
                {
                    if (!_instances.ContainsKey(idAmm))
                    {
                        ConfigRepository instance = new ConfigRepository();
                        instance = getListaChiavi(idAmm);
                        _instances.Add(idAmm, instance);
                    }
                }

                return (ConfigRepository)_instances[idAmm];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// ripulisce l'hashtable più esterna
        /// </summary>
        public static void clearAll()
        {
            try
            {
                if (_instances != null)
                    _instances.Clear();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// ripulisce l'hashtable legata all'amministrazione specificata
        /// </summary>
        public static void remove(string idAmm)
        {
            try
            {
                if (_instances != null && _instances.ContainsKey(idAmm))
                {
                    _instances.Remove(idAmm);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="xmlPathName"></param>
        private static ConfigRepository getListaChiavi(string idAmm)
        {

            //creazione hashTable
            ConfigRepository hashListaChiavi = new ConfigRepository();

            try
            {
                //lettura delle chiavi 
                hashListaChiavi.ListaChiavi = docsPaWS.getListaChiaviConfig(idAmm);
            }
            
            catch (System.Exception ex)
            {
                logger.Error("Errore nel caricamento della hashtable: " + ex.Message.ToString());

                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return hashListaChiavi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm">Se "0" viene cercata una chiave globale, </param>
        /// <param name="key">nome della chiave da reperire</param>
        /// <returns></returns>
        public static string GetValue(string idAmm, string key)
        {
            try
            {
                string kValue = string.Empty;
                ConfigRepository config = (ConfigRepository)getInstance(idAmm);

                if (config != null && config.ContainsKey(key))
                    return ((DocsPaWR.ChiaveConfigurazione)config[key]).Valore;
                else
                    return kValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}
