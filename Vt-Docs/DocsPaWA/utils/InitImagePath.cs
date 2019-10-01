using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using log4net;

namespace DocsPAWA.utils
{
    /// <summary>
    /// 
    /// </summary>
    public class InitImagePath
    {
        private static ILog logger = LogManager.GetLogger(typeof(InitImagePath));
        /// <summary>
        /// Hashtable che memorizza le singole istanze per la gestione
        /// grafica per tutte le amministrazione
        /// </summary>
        private static Hashtable _instances = null;
        // <string, InitImagePath>

        /// <summary>
        /// ID dell'amministrazione
        /// </summary>
        protected string _idAmm = string.Empty;

        private Hashtable pathHashTable = null;


        private string PATH;
        private static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();


        /// <summary>
        /// Costruttore privato poichè la classe implementa il design pattern singleton
        /// </summary>
        private InitImagePath()
        {

        }

        /// <summary>
        /// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
        /// utilizzando il metodo initializePath
        /// </summary>
        /// <returns></returns>
        public static InitImagePath getInstance(string idAmm)
        {
            if (_instances == null)
                _instances = Hashtable.Synchronized(new Hashtable());

            lock (_instances)
            {
                if (!_instances.ContainsKey(idAmm))
                {
                    InitImagePath instance = new InitImagePath();
                    instance.initializeHash(idAmm);

                    _instances.Add(idAmm, instance);
                }
            }

            return (InitImagePath)_instances[idAmm];

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
        public void clear()
        {
            if (_instances != null && _instances.ContainsKey(this._idAmm))
            {
                this.pathHashTable.Clear();

                _instances.Remove(this._idAmm);
            }
        }

        /// <summary>
        /// Legge dal file Web.Config le informazioni relative alla posizione del file XML 
        /// dei messaggi. Crea l’Hash Table e lo carica
        /// </summary>
        /// <param name="xmlPathName"></param>
        private void initializeHash(string idAmm)
        {
            this._idAmm = idAmm;

            //creazione hashTable
            pathHashTable = Hashtable.Synchronized(new Hashtable());
            this.PATH = docsPaWS.getpath("Scrittura");
            if (this.PATH == null || this.PATH == "")
                throw new Exception("Attenzione, chiave configurazione FrontEnd non valorizzata.");

            this.PATH = Utils.getHttpFullPath() + this.PATH.Substring(1);

            try
            {
                //lettura delle key 
                ArrayList keyArray = initializaArray();

                if (idAmm != null && keyArray != null && keyArray.Count != 0)
                {
                    //inizializzazione hashtable                               
                    foreach (string key in keyArray)
                    {
                        switch (key)
                        {
                           case "LOGO":
                                if (fileExist("logoente_" + idAmm + ".gif", "FrontEnd"))
                                    pathHashTable.Add(key, this.PATH + "/logoente_" + idAmm + ".gif");
                                else
                                    if (fileExist("logoente_" + idAmm + ".jpg", "FrontEnd"))
                                        pathHashTable.Add(key, this.PATH + "/logoente_" + idAmm + ".jpg");
                                    else
                                        pathHashTable.Add(key, "~/images/testata/320/gestdoc.jpg");
                                break;
                            case "BKG_LOGO":
                                if (fileExist("backgroundlogoente_" + idAmm + ".gif", "FrontEnd"))
                                    pathHashTable.Add(key, this.PATH + "/backgroundlogoente_" + idAmm + ".gif");
                                else
                                    if (fileExist("backgroundlogoente_" + idAmm + ".jpg", "FrontEnd"))
                                        pathHashTable.Add(key, this.PATH + "/backgroundlogoente_" + idAmm + ".jpg");
                                    else
                                        pathHashTable.Add(key, this.PATH + "/sf2.jpg");
                                break;
                            case "BKG_TESTO":
                                if (fileExist("backgroundlogo_" + idAmm + ".gif", "FrontEnd"))
                                    pathHashTable.Add(key, this.PATH + "/backgroundlogo_" + idAmm + ".gif");
                                else
                                    if (fileExist("backgroundlogo_" + idAmm + ".jpg", "FrontEnd"))
                                        pathHashTable.Add(key, this.PATH + "/backgroundlogo_" + idAmm + ".jpg");
                                    else
                                        pathHashTable.Add(key, this.PATH + "/sf1.jpg");
                                break;
                            case "CSS":
                                string result = docsPaWS.getCssAmministrazione(idAmm);
                                if(!string.IsNullOrEmpty(result))
                                {
                                    pathHashTable.Add(key, result);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel caricamento della hashtable: " + e.Message);
            }
        }

        /// <summary>
        /// Carica l'array per le immagini
        /// </summary>
        private ArrayList initializaArray()
        {
            // in futuro, queste informazioni potrebbero essere prese da un file xml
            ArrayList keyArray = new ArrayList();
            keyArray.Add("DOCUMENTI");
            keyArray.Add("DOCUMENTI_ATTIVO");
            keyArray.Add("RICERCA");
            keyArray.Add("RICERCA_ATTIVO");
            keyArray.Add("GESTIONE");
            keyArray.Add("GESTIONE_ATTIVO");
            keyArray.Add("OPZIONI");
            keyArray.Add("AIUTO");
            keyArray.Add("ESCI");
            keyArray.Add("LOGO");
            keyArray.Add("BKG_LOGO");
            keyArray.Add("BKG_TESTO");
            keyArray.Add("CSS");

            return keyArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
        }

        /// <summary>
        /// Legge nell'hash table il valore del messaggio in base al codice
        /// </summary>
        /// <param name="messageDef"></param>
        /// <returns>msg la stringa contenente il valore del messaggio</returns>
        public string getPath(String fileimg)
        {
            if (pathHashTable != null && pathHashTable.Count != 0)
            {
                fileimg = fileimg.ToUpper();
                return (String)pathHashTable[fileimg];
            }
            else
            {
                return null;
            }
        }
    }
}
