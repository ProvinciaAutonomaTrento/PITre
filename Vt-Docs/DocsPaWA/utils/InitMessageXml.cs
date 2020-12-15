using System;
using System.Collections;
using System.Xml;
using System.Configuration;
using log4net;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for InitMessageXml.
    /// </summary>
    public class InitMessageXml
    {
        private static ILog logger = LogManager.GetLogger(typeof(InitMessageXml));
        private static Hashtable messageHashTable = null;
        private static InitMessageXml itsInstance = null;

        /// <summary>
		/// Costruttore privato poichè la classe implementa il design pattern singleton
		/// </summary>
        private InitMessageXml()
		{}

        /// <summary>
        /// Crea, se non esiste, l’istanza della classe e inizializza gli attributi 
        /// utilizzando il metodo initializeMessage
        /// </summary>
        /// <returns></returns>
        public static InitMessageXml getInstance()
        {
            if (itsInstance == null || (itsInstance != null && messageHashTable.Count == 0))
            {
                itsInstance = new InitMessageXml();
                itsInstance.initializeMessage(getKey("MessageFilePath"));
            }

            return itsInstance;
        }

        /// <summary>
        /// Legge dal file Web.Config le informazioni relative alla posizione del file XML 
        /// dei messaggi. Crea l’Hash Table e lo carica
        /// </summary>
        /// <param name="xmlPathName"></param>
        public void initializeMessage(String xmlPathName)
        {
            //creazione hashTable
            messageHashTable = Hashtable.Synchronized(new Hashtable());
            try
            {
                //lettura file xml
                System.Xml.XmlDocument xmlDocument = new XmlDocument();
                if (xmlPathName == "")
                {
                    logger.Error("xmlPathName errato !");
                    return;
                }
                logger.Debug("Loading XML file: " + xmlPathName);
                xmlDocument.Load(xmlPathName);
                //caricamento Hash Table
                XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("messaggio");
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    XmlNodeList xmlNodeList2 = xmlNode.ChildNodes;
                    messageHashTable.Add(xmlNodeList2.Item(0).InnerText.ToUpper(), xmlNodeList2.Item(1).InnerText);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel caricamento della hashtable: " + e.Message);
            }
        }

        /// <summary>
        /// Legge nell'hash table il valore del messaggio in base al codice
        /// </summary>
        /// <param name="messageDef"></param>
        /// <returns>msg la stringa contenente il valore del messaggio</returns>
        public string getMessage(String messageDef)
        {
            messageDef = messageDef.ToUpper();
            string msg = (String)messageHashTable[messageDef];
            return msg;
        }

        public static string getKey(string keyName)
        {
            string retValue = null;
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings[keyName] != null)
                {
                    retValue = AppDomain.CurrentDomain.BaseDirectory + System.Configuration.ConfigurationManager.AppSettings[keyName];
                }
            }
            catch (System.Exception exception)
            {
                logger.Error("Errore nella lettura dei dati dalla chiave di configurazione '" + keyName + ": " + exception.Message);
            }
            return retValue;
        }

    }
}
