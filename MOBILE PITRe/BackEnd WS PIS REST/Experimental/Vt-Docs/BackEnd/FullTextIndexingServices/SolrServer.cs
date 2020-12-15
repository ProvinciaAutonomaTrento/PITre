using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Net;


namespace DocsPaDocumentale.FullTextSearch
{
    /// <summary>
    /// Servizio per la ricerca fulltext mediante
    /// il servizio "MicrosoftIndexService"
    /// </summary>
    public class SolrServer
    {
        #region Gestione configurazioni


        /// <summary>
        /// path del log
        /// </summary>
        private const string PATH_LOG = "DEBUG_PATH";
        private const string FULLTEXT_ENGINE_URL = "FULLTEXT_ENGINE_URL";

        /// <summary>
        /// Reperimento valore configurazione
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        protected string GetConfig(string configName)
        {
            string configValue = ConfigurationManager.AppSettings[configName];

            if (configValue == null)
                configValue = string.Empty;

            return configValue;
        }

        #endregion

        #region Gestione ricerca FullText
        /// <summary>
        /// Ricerca FullText mediante i servizi di indicizzazione microsoft
        /// </summary>
        /// <param name="textToSearch"></param>
        /// <param name="indexCatalogName"></param>
        /// <param name="maxRowCount"></param>
        /// <returns></returns>
        public virtual FullTextResultInfo[] FullTextSearch(string textToSearch, string indexCatalogName, int maxRowCount)
        {
            FullTextResultInfo[] retValue =null ;

            //this.WriteLog(false,string.Format("Text: {0} - CatalogName: {1} - MaxRows: {2}",textToSearch,indexCatalogName,maxRowCount.ToString()));

            // Validazione parametri in ingresso
            if (textToSearch != null && textToSearch != string.Empty &&
                indexCatalogName != null && indexCatalogName != string.Empty &&
                maxRowCount > 0)
            {

                string configValue = this.GetConfig(FULLTEXT_ENGINE_URL);

                try
                {
                    DateTime initDate = DateTime.Now;


                    Uri u = new Uri ( new Uri(configValue),indexCatalogName + "/select");
                    

                    //var builder = new UriBuilder("http://vtmi-vtdocsdb01.valueteam.com:8393/solr/collection/select");
                    var builder = new UriBuilder(u);
                    

                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["q"] = string.Format("content:\"{0}\"", textToSearch);
                    query["fl"] = "id,score";
                    query["df"] = "*";
                    query["wt"] = "xml";
                    query["indent"] = "true";
                    query["rows"] = maxRowCount.ToString();
                    builder.Query = query.ToString();
                    string url = builder.ToString();

                    WebClient wc = new WebClient();
                    //string dataa =wc.DownloadString("http://vtmi-vtdocsdb01.valueteam.com:8393/solr/collection/select?q=content%3A%22vanno+al+Galoppo%22&fl=id%2C+score&df=*&wt=xml&indent=true");
                    string dataa = wc.DownloadString(url);

                    retValue = parseXML(dataa);

                    DateTime endDate = DateTime.Now;
                    TimeSpan time = endDate.Subtract(initDate);

                    //this.WriteLog(false,string.Format("File contenenti la parola {0}: {1} - Tempo mms: {2}",retValue.Count.ToString(),textToSearch,time.TotalMilliseconds.ToString()));
                }
                catch (Exception ex)
                {
                    this.WriteLog(true, ex.Message);
                }
            }

            return retValue;
        }

        public static FullTextResultInfo[] parseXML(string xmlString)
        {
           
            SolrData sd = new SolrData();
            SolrData.response r = sd.deserialize(xmlString);
            ArrayList retValue = new ArrayList();

            if (r.result.docs != null)
            {
                foreach (SolrData.doc ri in r.result.docs)
                {
                    FullTextResultInfo docItem = new FullTextResultInfo();
                    foreach (SolrData.docValue li in ri.str)
                    {
                        if (li.name == "id")
                        {
                            docItem.DocTitle = li.Value;
                            docItem.Name = li.Value;
                            docItem.FileName = li.Value;
                        }
                    }

                    foreach (SolrData.docValue li in ri.floats)
                    {
                        if (li.name == "score")
                            docItem.Rank = li.Value;
                    }
                    retValue.Add(docItem);
                }
            }
            return (FullTextResultInfo[])retValue.ToArray(typeof(FullTextResultInfo));
        }


        /// <summary>
        /// Reperimento path log
        /// </summary>
        /// <returns></returns>
        protected virtual string logPath
        {
            get
            {
                string retValue = PATH_LOG;

                string configValue = this.GetConfig(PATH_LOG);

                if (configValue != null && configValue != string.Empty)
                {
                    try
                    {
                        retValue = configValue;
                    }
                    catch
                    {
                    }
                }

                return retValue;
            }
        }


        /// <summary>
        /// Scrittura su file di log
        /// </summary>
        /// <param name="errorLog"></param>
        /// <param name="logText"></param>
        protected void WriteLog(bool errorLog, string logText)
        {
            FileStream writer = null;
            StreamWriter streamWriter = null;

            try
            {
                string logpath = this.GetLogFilePath();
                if (logpath != string.Empty)
                {
                    writer = File.OpenWrite(logpath);

                    streamWriter = new StreamWriter(writer);
                    streamWriter.AutoFlush = true;

                    if (errorLog)
                        logText = string.Format("{0} - Errore: {1}", DateTime.Now.ToString(), logText);
                    else
                        logText = string.Format("{0} - {1}", DateTime.Now.ToString(), logText);

                    streamWriter.WriteLine(logText);
                }
                else throw new Exception("Non è stato possibile creare o scrivere sul log path");
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("DocsPa - FullTextIndexingServices - Errore : ", ex.Message);

            }
            finally
            {
                if (streamWriter != null)
                    streamWriter.Close();
            }
        }

        /// <summary>
        /// Reperimento percorso file di log
        /// </summary>
        /// <returns></returns>
        private string GetLogFilePath()
        {
            string folderPath = string.Empty;
            try
            {


                folderPath = this.logPath + @"\FullTextIndexingServices\";
                if (string.IsNullOrEmpty(folderPath))
                    return "";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = string.Format("log_{0}_{1}_{2}.txt", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());

                return folderPath + fileName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("DocsPa - FullTextIndexingServices - Errore :", "folderPath  " + folderPath + " non esiste o non si possiedono i diritti di scrittura." + ex.Message);
                return string.Empty;
            }

        }

        #endregion
    }
}