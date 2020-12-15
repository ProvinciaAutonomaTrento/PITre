using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;

namespace DocsPaConservazione
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class LogConservazione
    {
        /// <summary>
        /// 
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Utente
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DataAzione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CodiceAzione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Azione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Esito
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idIstanza"></param>
        /// <param name="dataIniziale"></param>
        /// <param name="dataFinale"></param>
        /// <param name="utente"></param>
        /// <param name="azione"></param>
        /// <param name="esito"></param>
        /// <returns></returns>
        public static LogConservazione[] GetLogs(DocsPaVO.utente.InfoUtente infoUtente,
                                              string idIstanza,
                                              string dataIniziale,
                                              string dataFinale,
                                              string utente,
                                              string azione, 
                                              string esito)
        {
            List<LogConservazione> logs = new List<LogConservazione>();

            BusinessLogic.UserLog.UserLog log = new BusinessLogic.UserLog.UserLog();

            //GM 27-8-2013
            //log non archiviati (DPA_LOG) = 1
            //log archviati (DPA_LOG_STORICO) = 2
            //tutti i log (union all delle due) = 3
            int queryTable = 3;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format( "SELECT A.VAR_CODICE_AMM FROM DPA_AMMINISTRA A INNER JOIN PEOPLE P ON A.SYSTEM_ID = P.ID_AMM WHERE P.SYSTEM_ID = {0}", infoUtente.idPeople);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    //GM 31-7-2013
                    //modifica per filtri ricerca su azione ed esito
                    string xmlData = log.GetXmlLogFiltrato(
                                    dataIniziale,
                                    dataFinale,
                                    utente,
                                    idIstanza,
                                    "CONSERVAZIONE",
                                    azione,
                                    field,
                                    esito,
                                    string.Empty,
                                    queryTable); //GM 27-8-2013 la ricerca viene effettuata su tutti i log

                    if (!string.IsNullOrEmpty(xmlData))
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(xmlData);

                        XmlNodeList nodes = document.DocumentElement.ChildNodes;

                        foreach (XmlNode n in nodes)
                        {
                            LogConservazione logInfo = new LogConservazione();

                            foreach (XmlNode child in n.ChildNodes)
                            {
                                if (child.Name == "ID_OGGETTO")
                                {
                                    logInfo.IdIstanza = child.InnerText;
                                }
                                else if (child.Name == "USERID_OPERATORE")
                                {
                                    logInfo.Utente = child.InnerText;
                                }
                                else if (child.Name == "DTA_AZIONE")
                                {
                                    DateTime dataAzione;
                                    if (DateTime.TryParse(child.InnerText, out dataAzione))
                                        logInfo.DataAzione = dataAzione.ToString("dd/MM/yyyy HH:mm:ss");
                                    else
                                        logInfo.DataAzione = child.InnerText;
                                }
                                else if (child.Name == "VAR_COD_AZIONE")
                                {
                                    logInfo.CodiceAzione = child.InnerText;
                                }
                                else if (child.Name == "VAR_DESC_AZIONE")
                                {
                                    logInfo.Azione = child.InnerText;
                                }
                                else if (child.Name == "VAR_DESC_OGGETTO")
                                {
                                    logInfo.Descrizione = child.InnerText;
                                }
                                else if (child.Name == "CHA_ESITO")
                                {
                                    logInfo.Esito = (child.InnerText == "1");
                                }
                            }

                            logs.Add(logInfo);
                        }
                    }
                }
            }

            return logs.ToArray();
        }

    }
}
