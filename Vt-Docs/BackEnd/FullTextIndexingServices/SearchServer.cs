using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace DocsPaDocumentale.FullTextSearch
{
    /// <summary>
    /// Servizio per la ricerca fulltext mediante
    /// il servizio "MicrosoftIndexService"
    /// </summary>
    public class SearchServer
    {
        #region Gestione configurazioni

        /// <summary>
        /// path del log
        /// </summary>
        private const string PATH_LOG = "DEBUG_PATH";
     
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
            ArrayList retValue = new ArrayList();

            // Validazione parametri in ingresso
            if (textToSearch != null && textToSearch != string.Empty &&
                indexCatalogName != null && indexCatalogName != string.Empty &&
                maxRowCount > 0)
            {
                DataSet dataSet = new DataSet();
                string connectionString = "Provider=Search.CollatorDSO;Extended Properties='Application=Windows';";

                try
                {
                    DateTime initDate = DateTime.Now;

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();

                        using (OleDbCommand command = new OleDbCommand(this.GetCommandText(textToSearch, maxRowCount, indexCatalogName), connection))
                        {
                            using (IDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {

                                    if (retValue.Count < maxRowCount)
                                        retValue.Add(this.GetFullTextResultInfo(reader));
                                    else
                                        break;
                                }
                            }
                        }
                    }

                    DateTime endDate = DateTime.Now;
                    TimeSpan time = endDate.Subtract(initDate);

                    //this.WriteLog(false,string.Format("File contenenti la parola {0}: {1} - Tempo mms: {2}",retValue.Count.ToString(),textToSearch,time.TotalMilliseconds.ToString()));
                }
                catch (Exception ex)
                {
                    this.WriteLog(true,ex.Message);
                }
            }

            return (FullTextResultInfo[])retValue.ToArray(typeof(FullTextResultInfo));
        }

        /// <summary>
        /// Reperimento query per ricerca FullText
        /// </summary>
        /// <param name="textToSearch"></param>
        /// <returns></returns>
        protected virtual string GetCommandText(string textToSearch, int maxRowCount, string indexCatalogName)
        {

            if (maxRowCount > 0)
            {
                return string.Format(
                                  "SELECT TOP {2} System.Search.Rank, System.ItemPathDisplay, System.Title, System.FileName,System.ItemDate FROM " +
                                  "SYSTEMINDEX WHERE FREETEXT ('*,{0}')  AND SCOPE = '{1}' order by System.Search.Rank DESC", textToSearch.Replace("'", "''"), indexCatalogName, maxRowCount.ToString());
            }
            else
            {
                return string.Format(
                                     "SELECT System.Search.Rank, System.ItemPathDisplay, System.Title, System.FileName,System.ItemDate FROM " +
                                     "SYSTEMINDEX WHERE FREETEXT ('*,{0}') AND SCOPE = '{1}' order by System.Search.Rank DESC", textToSearch.Replace("'", "''"), indexCatalogName);
            }
        }

        /// <summary>
        /// Creazione oggetto "FullTextResultInfo" contenente le informazioni
        /// su un file ricercato
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual FullTextResultInfo GetFullTextResultInfo(IDataReader reader)
        {
            FullTextResultInfo retValue = new FullTextResultInfo();

            retValue.FileName = reader.GetValue(reader.GetOrdinal("SYSTEM.FILENAME")).ToString();

            if (retValue.FileName != string.Empty)
            {
                int indexOf = retValue.FileName.IndexOf(".");

                if (indexOf > -1)
                    retValue.Name = retValue.FileName.Substring(0, indexOf);
                else
                    retValue.Name = retValue.FileName;
            }

            retValue.Rank = reader.GetValue(reader.GetOrdinal("SYSTEM.SEARCH.RANK")).ToString();
            retValue.VPath = reader.GetValue(reader.GetOrdinal("SYSTEM.ITEMPATHDISPLAY")).ToString();
            retValue.DocTitle = reader.GetValue(reader.GetOrdinal("SYSTEM.TITLE")).ToString();
            retValue.Characterization = "";
            retValue.Write = reader.GetValue(reader.GetOrdinal("SYSTEM.ITEMDATE")).ToString();

            return retValue;
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