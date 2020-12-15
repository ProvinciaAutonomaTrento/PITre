using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using DocsPaVO.Caching;
using System.Threading;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Security.Cryptography;
//using Microsoft.Web.Services3;
//using Microsoft.Web.Services3.Design;
//using Microsoft.Web.Services3.Diagnostics.Configuration;
using log4net;



namespace BusinessLogic.Documenti
{
    #region classe statica CacheFileManager
    /// <summary>
    /// BusinessClass per la gestione dei file in cache
    /// </summary>
    public sealed class CacheFileManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(CacheFileManager));
        #region variabili locali

        //public static HttpServerUtility server = HttpContext.Current.Server;
        private static CacheFileManager istance = null;
        private static CacheConfig configurazione = null;
        private static Dictionary<string, CacheConfig> _intance = null;

        //private static long MaxRequestLength = 0;
        //private static string UploadPath = string.Empty;

        #endregion
        #region campi parametrizzati sul web config
        public static string docRoot
        {
            get
            {
                string docRoot = string.Empty;
                const string DOC_ROOT = "DOC_ROOT";

                if (ConfigurationSettings.AppSettings[DOC_ROOT] != null &&
                    ConfigurationSettings.AppSettings[DOC_ROOT].ToString() != string.Empty)
                    docRoot = ConfigurationSettings.AppSettings[DOC_ROOT].ToString();

                return docRoot;
            }
        }

        #endregion
        #region costruttore
        public CacheFileManager()
        { }
        #endregion
        #region putfile
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileDocument"></param>
        public static bool PutFile(DocsPaVO.utente.InfoUtente infoUtente,
                            DocsPaVO.documento.FileRequest fileRequest,
                            DocsPaVO.documento.FileDocumento fileDoc,
                            string estensione,
                            out string errorMessage)
        {

            configurazione = CacheFileManager.GetInstance(infoUtente.idAmministrazione);
            long MaxRequestLength = 0;

            if (configurazione != null)
                MaxRequestLength = (long)configurazione.massima_dimensione_file;

            logger.Debug("inizio il putfile del cache");

            bool retValue = true;
            errorMessage = string.Empty;

           
            if (!Directory.Exists(configurazione.doc_root_server_locale))
                Directory.CreateDirectory(configurazione.doc_root_server_locale);


            double folderSize = GetFolderSize(configurazione.doc_root_server_locale);//CacheFileManager.docRoot);
            //verifico la dimensione della memoria cache se è infinita cioè -1
            if (configurazione.massima_dimensione_caching == -1)
            {
                if (!PutFileETDOC(fileRequest, fileDoc, estensione, infoUtente))
                {
                    logger.Debug("errore nel putfile etdoc");
                    errorMessage = "Non è stato possibile acquisire il documento. <BR><BR>Ripetere l'operazione di acquisizione.";
                    retValue = false;
                }
            }
            else
            {
                //verifico la dimensione del della cache
                if ((folderSize + fileDoc.content.Length) <= configurazione.massima_dimensione_caching)
                {
                    logger.Debug("eseguo il putfileetdoc");
                    //eseguoi il put file
                    if (!PutFileETDOC(fileRequest, fileDoc, estensione, infoUtente))
                    {
                        logger.Debug("errore nel putfile etdoc");
                        errorMessage = "Non è stato possibile acquisire il documento. <BR><BR>Ripetere l'operazione di acquisizione.";
                        retValue = false;
                    }
                }
                else //cerco di liberare lo spazio sufficiente per scrivere in cache
                {
                    //verifico se si puo cancellare un numero n di file per liberare lo spazio necessario per salvare il file
                    InfoFileCaching[] cachingList = GetDocumentiInCache("1",infoUtente.idAmministrazione);
                    if (cachingList.Length > 0)
                    {
                        double filesize = fileDoc.content.Length;
                        for (int i = 0; i < cachingList.Length && filesize > 0; i++)
                        {
                            if (DequeueCacheFile(cachingList[i], filesize))
                                filesize = (filesize - (double)cachingList[i].file_size);
                        }
                        if (filesize <= 0)
                        {
                            if (!PutFileETDOC(fileRequest, fileDoc, estensione, infoUtente))
                            {
                                errorMessage = "Non è stato possibile acquisire il documento. <BR><BR>Ripetere l'operazione di acquisizione.";
                                retValue = false;
                            }

                        }
                        else
                        {
                            errorMessage = "Memoria cache Piena.<br><br> Non è stato possibile rimuovere nessun file dalla memoria cache";
                            retValue = false;
                        }
                    }
                    else
                    {
                        //MESSAGGIO DI ERRORE
                        errorMessage = "Memoria cache piena. <br><br> Non è stato possibile rimuovere nessun file dalla memoria cache";
                        retValue = false;
                    }
                }
            }
            return retValue;
        }


        #endregion
        #region operazioni sul db
        /// <summary>
        /// Rimozione del file dalla cache
        /// </summary>
        /// <param name="info"></param>
        /// <param name="newFileSize">
        /// Indica la dimensione del file reale inserito in cache
        /// </param>
        public static bool DequeueCacheFile(InfoFileCaching info, double newFileSize)
        {
            bool result = false;
            FileInfo file = new FileInfo(info.CacheFilePath);
            if (file.Exists)
            {
                if (info.file_size <= newFileSize)
                {
                    File.Delete(info.CacheFilePath);
                    DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                    // Rimozione dei metadati descrittivi del file in cache nel database
                    query.deleteCache(info);
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public static void deleteInfoFileInCache(DocsPaVO.Caching.InfoFileCaching info)
        {
            if (info != null)
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                query.deleteCache(info);
            }
        }

/// <summary>
/// 
/// </summary>
/// <param name="docnumber"></param>
/// <returns></returns>
        public static string GET_ID_AMM_BY_DOC_NUMER(string docnumber)
        {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                return query.GET_ID_AMM_BY_DOC_NUMER(docnumber);
        }

        /// <summary>
        /// funzione che recupera i dati dalle tabelle della cache
        /// </summary>
        /// <returns></returns>
        public static InfoFileCaching[] GetDocumentiInCache(string aggiornato, string idAmministrazione)
        {
            InfoFileCaching[] infoFileCaching = null;
            
            if (!string.IsNullOrEmpty(aggiornato) && !string.IsNullOrEmpty(idAmministrazione))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                infoFileCaching = query.ricercaDocumemtoInCache(aggiornato, idAmministrazione);
            }

            if (infoFileCaching == null)
                infoFileCaching = new InfoFileCaching[0];
            return infoFileCaching;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="physicalPath"></param>
        /// <returns></returns>
        public static double GetFolderSize(string physicalPath)
        {
            double dblDirSize = 0;
            DirectoryInfo objDirInfo = new DirectoryInfo(physicalPath);
            Array arrChildFiles = objDirInfo.GetFiles();
            Array arrSubFolders = objDirInfo.GetDirectories();
            foreach (FileInfo objChildFile in arrChildFiles)
            {
                dblDirSize += objChildFile.Length;
            }
            foreach (DirectoryInfo objChildFolder in arrSubFolders)
            {
                dblDirSize += GetFolderSize(objChildFolder.FullName);
            }
            return dblDirSize;
        }

        public static bool updateCaching(DocsPaVO.Caching.InfoFileCaching info)
        {
            bool retval = false;
            if (info != null)
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                retval = query.updateCache(info);
            }
            return retval;
        }

        public static bool inserimentoCaching(DocsPaVO.Caching.InfoFileCaching info)
        {
            bool retval =false;
            if (info != null)
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                retval = query.inserimetoCache(info);
            }
            return retval;
        }

        public static bool insertCacheConStream(DocsPaVO.Caching.InfoFileCaching fileInfo, byte[] stream)
        {
            bool retval = false;
            if (fileInfo != null && stream.Length > 0)
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                retval = query.inserimetoCache(fileInfo, stream);
            }
            return retval;
        }

        public static bool inserimentoCachingPerFile(string docNumber,
                                                string versionId,
                                                string estensione,
                                                string filePath,
                                                string idAmministrazione,
                                                byte[] stream)
        {
        
            bool retval = false;

            if (!string.IsNullOrEmpty(docNumber) &&
                !string.IsNullOrEmpty(versionId) &&
                !string.IsNullOrEmpty(estensione) &&
                !string.IsNullOrEmpty(filePath) &&
                !string.IsNullOrEmpty(idAmministrazione) &&
                stream.Length > 0)
            {
                InfoFileCaching info = new InfoFileCaching();
                info.Aggiornato = 1;
                info.DocNumber = Convert.ToInt32(docNumber);
                info.CacheFilePath = filePath;
                info.idAmministrazione = idAmministrazione;
                info.file_size = stream.Length;
                info.var_impronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(stream);
                info.Version_id = Convert.ToInt32(versionId);
                info.ext = estensione;
                info.last_access = System.DateTime.Now.ToString();
                //aggiorno il db del caching

                retval = inserimentoCaching(info);
            }

            return retval;
        }

        public static InfoFileCaching getDettaglioFileCache(string docnumber, string versionId, string idAmministrazione)
        {
            InfoFileCaching info = null;
            if (!string.IsNullOrEmpty(docnumber) &&
                !string.IsNullOrEmpty(versionId) &&
                !string.IsNullOrEmpty(idAmministrazione))
            {

                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                info = query.getFileCache(docnumber, versionId, idAmministrazione);

            }
            return info;
        }

        

        public static bool aggiornamentoComponents(DocsPaVO.Caching.InfoFileCaching info)
        {
            bool retval = false;
            if(info != null)
            {
            DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
            retval = query.updateComponentCache(info);
            }
            return retval;
        }

        public static bool isFileInCache(string docnumber)
        {

            bool retval = false;
            DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
            retval = query.verificaEsistenzaFileInCache(docnumber);
            return retval;

        }



        public static bool verificaEsistenzaFileInCache(DocsPaVO.documento.FileRequest fileRequest, string idAmministrazione)
        {

            bool retval = false;
            if (fileRequest != null &&
                !string.IsNullOrEmpty(fileRequest.docNumber) &&
                !string.IsNullOrEmpty(fileRequest.versionId) &&
                !string.IsNullOrEmpty(idAmministrazione))
            {
                InfoFileCaching info = new InfoFileCaching();
                info.DocNumber = int.Parse(fileRequest.docNumber);
                info.Version_id = int.Parse(fileRequest.versionId);
                info.idAmministrazione = idAmministrazione;

                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                retval = query.verificaEsistenzaFileInCache(info);
            }

            return retval;

        }
        
        public static bool setConfigurazioneCache(DocsPaVO.Caching.CacheConfig info)
        {
            bool retval = false;
            if (info != null)
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                retval = query.inserisciConfigCache(info);
            }

            return retval;
        }

        public static bool updateConfigurazioneCache(DocsPaVO.Caching.CacheConfig info)
        {
            bool retval = false;
            if (info != null)
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                retval = query.updateConfigCache(info);
                istance.resetInstance(info.idAmministrazione);
            }
            return retval;
        }

        public static string ricercaPathComponents(string docnumber, string version_id)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(docnumber) &&
                !string.IsNullOrEmpty(version_id))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                path = query.recuperaPathComponents(docnumber, version_id);
            }
            return path;
        }

        public static string ricercaPathCaching(string docNumber, string versionId, string idAmministrazione)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(docNumber) &&
                !string.IsNullOrEmpty(versionId) &&
                !string.IsNullOrEmpty(idAmministrazione))
            {
                InfoFileCaching info = new InfoFileCaching();
                info.DocNumber = int.Parse(docNumber);
                info.Version_id = int.Parse(versionId);
                info.idAmministrazione = idAmministrazione;
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                path = query.recuperaPathCache(info);
            }

            return path;
        }

        public static void deleteConfigurazioneCache(string idAmministrazione)
        {
            if (!string.IsNullOrEmpty(idAmministrazione))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                query.deleteConfigCache(idAmministrazione);
            }
        }


        public static CacheConfig getConfigurazioneCache(string idAmministrazione)
        {
            CacheConfig config = null;
            if (!string.IsNullOrEmpty(idAmministrazione) || 
                (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["USE_CACHE"]) &&
                bool.Parse(ConfigurationManager.AppSettings["USE_CACHE"]))
                )
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                    config = query.getConfigurazioneCache(idAmministrazione);
                    if (_intance != null && config!= null)
                        _intance.Add(idAmministrazione, config);
                }
                catch (Exception e)
                {
                    logger.Debug("non esistono configurazioni per il modulo di cache");

                }
            }
            
            return config;

        }

        public static DocsPaVO.Caching.InfoFileCaching getFileDaCache(string docnumber, string versionId, string idAmministrazione)
        {
            InfoFileCaching info = null;
            
            if (!string.IsNullOrEmpty(docnumber) &&
                !string.IsNullOrEmpty(versionId) &&
                !string.IsNullOrEmpty(idAmministrazione))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                info = query.getFileCache(docnumber, versionId, idAmministrazione);
            }

            return info;
        }

        public static DocsPaVO.Caching.InfoFileCaching getFileDaCache(string docnumber)
        {
            InfoFileCaching info = null;

            if (!string.IsNullOrEmpty(docnumber))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                info = query.getFileCache(docnumber);
            }

            return info;
        }

        public static DocsPaVO.documento.FileRequest FileInCache(DocsPaVO.documento.FileRequest fileRequest, string idAmministrazione)
        {
            DocsPaVO.documento.FileRequest fr = null;
            if (fileRequest != null && !string.IsNullOrEmpty(idAmministrazione))
            {
                string docnumber = "0";
                string versionId = "0";

                if (!string.IsNullOrEmpty(fileRequest.docNumber))
                    docnumber = fileRequest.docNumber;

                if (!string.IsNullOrEmpty(fileRequest.versionId))
                    versionId = fileRequest.versionId;

                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                DocsPaVO.Caching.InfoFileCaching info = query.getFileCache(docnumber, versionId, idAmministrazione);

                if (info != null
                    && !string.IsNullOrEmpty(info.CacheFilePath))
                {
                    if (fileRequest.fileName != info.CacheFilePath)
                    {
                        fileRequest.fileName = info.CacheFilePath;
                        fileRequest.fileSize = info.file_size.ToString();
                    }
                    if(string.IsNullOrEmpty(fileRequest.path))
                    {
                        FileInfo fi = new FileInfo(info.CacheFilePath);
                        fileRequest.path = fi.FullName.Substring(0, ((fi.FullName.Length - fi.Name.Length) - 1));
                        fr = fileRequest;
                    }
                    
                }
            }
            return fr;
        }



        public static DocsPaVO.Caching.InfoFileCaching getFileDaComponents(string docnumber, string versionId, string idAmministrazione)
        {
            InfoFileCaching info = null;
            if (!string.IsNullOrEmpty(docnumber) &&
                !string.IsNullOrEmpty(versionId) &&
                !string.IsNullOrEmpty(idAmministrazione))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                info = query.getFileComponents(docnumber, versionId, idAmministrazione);
            }
            return info;

        }
        public static bool isActiveCaching(string idAmministrazione)
        {
            bool retval = false;
            if (!string.IsNullOrEmpty(idAmministrazione))
            {
                try
                {
                    DocsPaVO.Caching.CacheConfig configurazione = CacheFileManager.GetInstance(idAmministrazione);

                    if (configurazione != null && configurazione.caching)
                        retval = true;
                }
                catch
                {
                }
            }
            return retval;
                 }

        #endregion
        #region getfile



        public static bool copia(byte[] stream, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retval = false;
            try
            {
                string[] estenzioneFile = Path.GetExtension(fileRequest.fileName).Split('.');
                // Reperimento cartella documentale in cui inserire il file
                string relativeDocPath = GetDocPathAdvanced(fileRequest, infoUtente);

                if (string.IsNullOrEmpty(relativeDocPath))
                    throw new ApplicationException("Impossibile costruire il percorso relativo del file nel documentale");

                // Creazione nome file
                string fileName = string.Format("{0}.{1}", fileRequest.versionId, estenzioneFile[1]);
                // Creazione path completo del file
                string filePath = string.Format(@"{0}\{1}\{2}",
                                    System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"],
                                    relativeDocPath,
                                    fileName);

                using (FileStream fs1 = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs1.Write(stream, 0, stream.Length);
                    fs1.Close();
                }
                return inserimentoCachingPerFile(fileRequest.docNumber, fileRequest.versionId, estenzioneFile[1], filePath, infoUtente.idAmministrazione, stream);
            }
            catch (Exception e)
            {
                logger.Debug("errore durante la copia del file: " + e.Message);
            }
            finally { }
            return retval;
        }

       

        public static bool copyFileInCache(byte[] stream, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = false;
            long MaxRequestLength = 0;
            try
            {
                //configurazione = CacheFileManager.getConfigurazioneCache(infoUtente.idAmministrazione);//server.MachineName);
                configurazione = CacheFileManager.GetInstance(infoUtente.idAmministrazione);
                if (configurazione != null)
                    MaxRequestLength = (long)configurazione.massima_dimensione_file;

                logger.Debug("inizio copia in locale");

                

                if (!Directory.Exists(configurazione.doc_root_server_locale))
                    Directory.CreateDirectory(configurazione.doc_root_server_locale);

                double folderSize = GetFolderSize(configurazione.doc_root_server_locale);
           
                if (configurazione.massima_dimensione_caching == -1)
                {
                    retValue = copia(stream, fileRequest, infoUtente);
                }
                else
                {
                    //verifico la dimensione della memoria cache
                    if ((folderSize + stream.Length) <= configurazione.massima_dimensione_caching)
                    {
                        retValue = copia(stream, fileRequest, infoUtente);
                    }
                    else //cerco di liberare lo spazio sufficiente per scrivere in cache
                    {
                        //verifico se si puo cancellare un numero n di file per liberare lo spazio necessario per salvare il file
                        InfoFileCaching[] cachingList = GetDocumentiInCache("1",infoUtente.idAmministrazione);
                        if (cachingList.Length > 0)
                        {
                            double filesize = stream.Length;
                            for (int i = 0; i < cachingList.Length && filesize > 0; i++)
                            {
                                if (DequeueCacheFile(cachingList[i], filesize))
                                    filesize = (filesize - (double)cachingList[i].file_size);
                            }
                            if (filesize == 0)
                            {
                                retValue = copia(stream, fileRequest, infoUtente);
                            }
                            else
                            {
                                retValue = false;
                                throw new Exception("Memoria cache piena. Non è stato possibile rimuovere nessun file dalla memoria cache");
                                
                            }
                        }
                        else
                        {
                            retValue = false;
                            //MESSAGGIO DI ERRORE
                            throw new Exception("Memoria cache piena. <br><br> Non è stato possibile rimuovere nessun file dalla memoria cache");
                          
                        }
                    }
                }
                return retValue;
            }
            catch (Exception e)
            {
                logger.Debug("errore nelal copia del file da remoto: " + e.Message);
            }
            return retValue;
        }

        public static bool copyFileInCacheConLimite(byte[] stream, DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = false;
            long MaxRequestLength = 0;
            double MaxDimCache = 0;
            double folderSize = 0;
            try
            {
                //configurazione = CacheFileManager.getConfigurazioneCache(infoUtente.idAmministrazione);//server.MachineName);
                configurazione = CacheFileManager.GetInstance(infoUtente.idAmministrazione);
                
                if (configurazione != null)
                {
                    folderSize = GetFolderSize(configurazione.doc_root_server_locale);
                    //verifico se è maggiore del limite infinito
                    if (configurazione.massima_dimensione_file > -1)
                        MaxRequestLength = (long)configurazione.massima_dimensione_file;
                    else
                        MaxRequestLength = stream.LongLength;
                    //verifico se è maggiore del limite infinito
                    if (configurazione.massima_dimensione_caching > -1)
                        MaxDimCache = configurazione.massima_dimensione_caching;
                    else
                        MaxDimCache = (folderSize + stream.Length);
                }
                logger.Debug("inizio copia in locale");

 

                if (!Directory.Exists(configurazione.doc_root_server_locale))
                    Directory.CreateDirectory(configurazione.doc_root_server_locale);

                
                //verifico la dimensione del file
                if (stream.Length <= MaxRequestLength)
                {
                    //verifico la dimensione della memoria cache
                    if ((folderSize + stream.Length) <= MaxDimCache)
                    {
                        string[] estenzioneFile = Path.GetExtension(fileRequest.fileName).Split('.');
                        // Reperimento cartella documentale in cui inserire il file
                        string relativeDocPath = GetDocPathAdvanced(fileRequest, infoUtente);

                        if (string.IsNullOrEmpty(relativeDocPath))
                            throw new ApplicationException("Impossibile costruire il percorso relativo del file nel documentale");

                        // Creazione nome file
                        string fileName = string.Format("{0}.{1}", fileRequest.versionId, estenzioneFile[1]);
                        // Creazione path completo del file
                        string filePath = string.Format(@"{0}\{1}\{2}",
                            configurazione.doc_root_server_locale,                
                            //System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"],
                                            relativeDocPath,
                                            fileName);
                        if (!Directory.Exists(configurazione.doc_root_server_locale + "\\" + relativeDocPath))
                            Directory.CreateDirectory(configurazione.doc_root_server_locale + "\\" + relativeDocPath);

                        using (FileStream fs1 = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            fs1.Write(stream, 0, stream.Length);
                            fs1.Close();
                        }

                        if (isFileInCache(fileRequest.docNumber))
                            retValue = aggiornaCachePerFile(fileRequest.docNumber, fileRequest.versionId, estenzioneFile[1], filePath, infoUtente.idAmministrazione, stream, 1);
                        else
                            retValue = inserimentoCachingPerFile(fileRequest.docNumber, fileRequest.versionId, estenzioneFile[1], filePath, infoUtente.idAmministrazione, stream);

                    }
                    else //cerco di liberare lo spazio sufficiente per scrivere in cache
                    {
                        //verifico se si puo cancellare un numero n di file per liberare lo spazio necessario per salvare il file
                        InfoFileCaching[] cachingList = GetDocumentiInCache("1",infoUtente.idAmministrazione);
                        if (cachingList.Length > 0)
                        {
                            double filesize = stream.Length;
                            for (int i = 0; i < cachingList.Length && filesize > 0; i++)
                            {
                                if (DequeueCacheFile(cachingList[i], filesize))
                                    filesize = (filesize - (double)cachingList[i].file_size);
                            }
                            if (filesize == 0)
                            {
                                string[] estenzioneFile = Path.GetExtension(fileRequest.fileName).Split('.');
                                // Reperimento cartella documentale in cui inserire il file
                                string relativeDocPath = GetDocPathAdvanced(fileRequest, infoUtente);

                                if (string.IsNullOrEmpty(relativeDocPath))
                                    throw new ApplicationException("Impossibile costruire il percorso relativo del file nel documentale");

                                // Creazione nome file
                                string fileName = string.Format("{0}.{1}", fileRequest.versionId, estenzioneFile[1]);
                                // Creazione path completo del file
                                string filePath = string.Format(@"{0}\{1}\{2}",
                                    configurazione.doc_root_server_locale,                
                                    //System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"],
                                                    relativeDocPath,
                                                    fileName);

                                using (FileStream fs1 = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    fs1.Write(stream, 0, stream.Length);
                                    fs1.Close();
                                }

                                if(isFileInCache(fileRequest.docNumber))
                                    retValue = aggiornaCachePerFile(fileRequest.docNumber, fileRequest.versionId, estenzioneFile[1], filePath, infoUtente.idAmministrazione, stream,1);
                                else
                                    retValue = inserimentoCachingPerFile(fileRequest.docNumber, fileRequest.versionId, estenzioneFile[1], filePath, infoUtente.idAmministrazione, stream);
                            }
                            else
                            {
                                retValue = false;
                                throw new Exception("Memoria cache piena. Non è stato possibile rimuovere nessun file dalla memoria cache");
                            }
                        }
                        else
                        {
                            retValue = false;
                            //MESSAGGIO DI ERRORE
                            throw new Exception("Memoria cache piena. <br><br> Non è stato possibile rimuovere nessun file dalla memoria cache");
                        }
                    }
                }
                else
                {
                    retValue = false;
                    throw new Exception("Il file da inserire in memoria cache <br><br>ha una dimensione superiore alla massima dimesione del file in cache");
                }
                return retValue;
            }
            catch (Exception e)
            {
                logger.Debug("errore nella copia del file da remoto: " + e.Message);
                throw new Exception(e.Message);
            }
            return retValue;
        }


        public static byte[] getFile(string docNumber, string versionId, string idAmministrazione)
        {
            byte[] stream = null;
            try
            {
                if (!string.IsNullOrEmpty(docNumber) &&
                    !string.IsNullOrEmpty(versionId) &&
                    !string.IsNullOrEmpty(idAmministrazione))
                {
                    string path = ricercaPathCaching(docNumber, versionId, idAmministrazione);
                    if (!string.IsNullOrEmpty(path))
                    {
                        FileInfo file = new FileInfo(path);
                        stream = new byte[file.Length];
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            fs.Read(stream, 0, stream.Length);
                            fs.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore nella lettuta del file per l'interoiperabilità: " + e.Message);
            }

            return stream;
        }

        public static bool getFile(ref DocsPaVO.utente.InfoUtente infoUtente,
                                    ref DocsPaVO.documento.FileRequest fileRequest,
                                    DocsPaVO.documento.FileDocumento fileDoc,
                                    out string messageError)
        {
            bool result = true;//prevedo successo 
            configurazione = CacheFileManager.GetInstance(infoUtente.idAmministrazione);
            long MaxRequestLength = 0;

            if (configurazione != null)
                MaxRequestLength = (long)configurazione.massima_dimensione_file;


            if (!Directory.Exists(CacheFileManager.docRoot))
                Directory.CreateDirectory(CacheFileManager.docRoot);

            if (!Directory.Exists(configurazione.doc_root_server_locale))
                Directory.CreateDirectory(configurazione.doc_root_server_locale);

            messageError = string.Empty;
            //esiste il file in cache
            if (verificaEsistenzaFileInCache(fileRequest,infoUtente.idAmministrazione))
            {
                if (!GetFileETDOC(ref fileDoc, ref fileRequest,infoUtente))
                {
                    logger.Debug("Errore nella gestione del File (getFile)");
                    messageError = "Errore nella gestione del File (getFile)";
                    result = false;
                    throw new Exception("Errore nella gestione del File (getFile)");
                }
            }
            else//non esisite il file in cache
            {
                double folderSize = GetFolderSize(CacheFileManager.docRoot);
                //verifico la dimensione del file
                //if (double.Parse(fileRequest.fileSize) <= configurazione.massima_dimensione_file)
                //{
                //verifico la dimensione della memoria cache
                if ((folderSize + double.Parse(fileRequest.fileSize)) <= configurazione.massima_dimensione_caching)
                {
                    //eseguoi l'upload del file file
                    logger.Debug("copio il file: " + fileRequest.fileName);
                    string[] estenzione = Path.GetExtension(fileRequest.fileName).Split('.');
                    FileInfo file = new FileInfo(fileRequest.fileName);
                    fileDoc.content = new byte[file.Length];
                    using (FileStream fs = new FileStream(fileRequest.fileName, FileMode.Open, FileAccess.Read))
                    {
                        fs.Read(fileDoc.content, 0, fileDoc.content.Length);
                        fs.Close();
                    }

                    if (result)
                    {
                        if (!PutFileETDOC(fileRequest, fileDoc, estenzione[1], infoUtente))
                            result = false;
                    }
                    if (result)
                    {//aggiorno la tabella di cache
                        string filepath = string.Empty;
                        if (fileRequest.docServerLoc != string.Empty)
                            filepath = fileRequest.docServerLoc + fileRequest.path + "\\" + fileRequest.fileName;
                        else
                            filepath = fileRequest.fileName;
                        result = aggiornaCachePerFile(fileRequest.docNumber, fileRequest.versionId, estenzione[1], filepath, infoUtente.idAmministrazione, fileDoc.content,1);
                        fileRequest.fileName = filepath;
                    }
                    if (result)
                        //chiamata ricorsiva  
                        result = CacheFileManager.getFile(ref infoUtente, ref fileRequest, fileDoc, out messageError);
                }
                else //cerco di liberare lo spazio sufficiente per scrivere in cache
                {
                    //verifico se si puo cancellare un numero n di file per liberare lo spazio necessario per salvare il file
                    InfoFileCaching[] cachingList = GetDocumentiInCache("1",infoUtente.idAmministrazione);
                    if (cachingList.Length > 0)
                    {
                        double filesize = double.Parse(fileRequest.fileSize);
                        for (int i = 0; i < cachingList.Length && filesize > 0; i++)
                        {
                            //cancellazione del file
                            if (DequeueCacheFile(cachingList[i], filesize))
                                filesize = (filesize - (double)cachingList[i].file_size);
                        }
                        if (filesize == 0)
                        {
                            string[] estenzione = Path.GetExtension(fileRequest.fileName).Split('.');
                            FileInfo file = new FileInfo(fileRequest.fileName);
                            fileDoc.content = new byte[file.Length];
                            using (FileStream fs = new FileStream(fileRequest.fileName, FileMode.Open, FileAccess.Read))
                            {
                                fs.Read(fileDoc.content, 0, fileDoc.content.Length);
                                fs.Close();
                            }
                            if (result)
                            {
                                if (!PutFileETDOC(fileRequest, fileDoc, estenzione[1], infoUtente))
                                {
                                    result = false;
                                }
                            }
                            if (result)
                            {//aggiorno la tabella di cache

                                string filepath = string.Empty;
                                if (fileRequest.docServerLoc != string.Empty)
                                    filepath = fileRequest.docServerLoc + fileRequest.path + "\\" + fileRequest.fileName;
                                else
                                    filepath = fileRequest.fileName;
                                result = aggiornaCachePerFile(fileRequest.docNumber, fileRequest.versionId, estenzione[1], filepath, infoUtente.idAmministrazione, fileDoc.content,1);
                                fileRequest.fileName = filepath;
                            }
                            if (result)
                                //chiamata ricorsiva
                                result = CacheFileManager.getFile(ref infoUtente, ref fileRequest, fileDoc, out messageError);
                        }
                        else
                        {
                            //visualizzo solo il file
                            logger.Debug("Memoria cache piena. Non è stato possibile rimuovere nessun file dalla memoria cache");
                            messageError = "Memoria cache piena. Non è stato possibile rimuovere nessun file dalla memoria cache";
                            if (!GetFileETDOC(ref fileDoc, ref fileRequest,infoUtente))
                            {
                                logger.Debug("Errore nella gestione del File (getFile)");
                                messageError = "Errore nella gestione del File (getFile)";
                                result = false;
                                throw new Exception("Errore nella gestione del File (getFile)");
                            }
                        }
                    }
                    else
                    {
                        //visualizzo solo il file
                        logger.Debug("Memoria piena.<br><br> Non è stato possibile rimuovere nessun file dalla memoria cache");
                        if (!GetFileETDOC(ref fileDoc, ref fileRequest,infoUtente))
                        {
                            logger.Debug("Errore nella gestione del File (getFile)");
                            messageError = "Errore nella gestione del File (getFile)";
                            result = false;
                            throw new Exception("Errore nella gestione del File (getFile)");
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        public static string GetDocPathAdvanced(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            string result = null;

            try
            {
                if (fileRequest != null)
                {
                    string filePath = "";

                    //legge il record da DPA_CORR_GLOBALI in JOIN con DPA_AMMINISTRA
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                    System.Data.DataSet corrispondente;
                    if (documentale.DOC_GetCorrByIdPeople(objSicurezza.idPeople, out corrispondente))
                    {
                        //logger.Debug ("Errore nella lettura del corrispondente relativo al documento");
                        //throw new Exception();
                    }
                    //legge l'amministrazione
                    string amministrazione = corrispondente.Tables[0].Rows[0]["VAR_CODICE_AMM"].ToString();

                    //legge l'id della uo di appartenenza del gruppo
                    string id = documentale.DOC_GetIdUoBySystemId(objSicurezza.idGruppo);
                    if (id == null)
                    {
                        logger.Debug("Errore nella lettura del gruppo relativo al documento");
                        throw new Exception();
                    }
                    //recupera il nome della UO
                    string codiceUO = documentale.DOC_GetUoById(id);
                    //legge la tabella profile
                    System.Data.DataSet documento;
                    if (!documentale.DOC_GetDocByDocNumber(fileRequest.docNumber, out documento))
                    {
                        logger.Debug("Errore nella lettura del documento: " + fileRequest.docNumber);
                        throw new Exception();
                    }
                    //legge l'anno di creazione del documento
                    string anno = System.DateTime.Parse(documento.Tables[0].Rows[0]["CREATION_DATE"].ToString()).Year.ToString();
                    //verifica se il documento è protocollato
                    string tipoProtocollo;
                    tipoProtocollo = documento.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString().ToUpper();
                    //se A -> protocollo in arrivo; se P -> protocollo in partenza ; tutto il resto -> non protocollato
                    string registro = "";
                    string arrivoPartenza = "";
                    if (tipoProtocollo == "A" || tipoProtocollo == "P" || tipoProtocollo == "I")
                    {
                        //crea il path nel caso di documento protocollato -> AMMINISTRAZIONE + REGISTRO + ANNO + [COD_UO] + [ARRIVO|PARTENZA]

                        //legge il registro della protocollazione
                        registro = documentale.DOC_GetRegistroById(documento.Tables[0].Rows[0]["ID_REGISTRO"].ToString());
                        if (registro == null)
                        {
                            logger.Debug("Errore nella lettura del registro");
                            registro = "";
                        }

                        if (tipoProtocollo == "A")
                        {
                            arrivoPartenza = "Arrivo";
                        }
                        if (tipoProtocollo == "P")
                        {
                            arrivoPartenza = "Partenza";
                        }
                        if (tipoProtocollo == "I")
                        {
                            arrivoPartenza = "Interno";
                        }
                    }

                    filePath = System.Configuration.ConfigurationManager.AppSettings["DOC_PATH"];
                    if (filePath == null) filePath = "";

                    filePath = filePath.Replace("AMMINISTRAZIONE", amministrazione);
                    filePath = filePath.Replace("REGISTRO", registro);
                    filePath = filePath.Replace("ANNO", anno);
                    filePath = filePath.Replace("ARRIVO_PARTENZA", arrivoPartenza);
                    filePath = filePath.Replace("UFFICIO", codiceUO);
                    filePath = filePath.Replace("UTENTE", objSicurezza.userId);

                    filePath = filePath.Replace(@"\\", @"\");
                    if (filePath.EndsWith(@"\"))
                    {
                        filePath = filePath.Remove(filePath.Length - 1, 1);
                    }

                    //verifica se la directory esiste
                    string appo = @System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\" + filePath;
                    DirectoryInfo docFullPath = new DirectoryInfo(appo);


                    if (!docFullPath.Exists)
                    {
                        //crea la directory
                        docFullPath.Create();
                    }

                    //restituisce la directory
                    result = filePath;

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore creazione path documentale per documento: " + fileRequest.docNumber, e);
                result = null;
            }
            return result;
        }


        public static bool aggiornaCachePerFile(string docNumber,
                                                string versionId,
                                                string estensione,
                                                string filePath,
                                                string idAmministrazione,
                                                byte[] stream,
                                                int aggiornamento)
        {
             bool retval = false;
            if (!string.IsNullOrEmpty(docNumber) &&
                !string.IsNullOrEmpty(versionId) &&
                !string.IsNullOrEmpty(estensione) &&
                !string.IsNullOrEmpty(filePath) &&
                !string.IsNullOrEmpty(idAmministrazione) &&
                stream.Length > 0)
            {
            InfoFileCaching info = new InfoFileCaching();
            info.Aggiornato = aggiornamento;
            info.DocNumber = Convert.ToInt32(docNumber);
            info.CacheFilePath = filePath;
            info.idAmministrazione = idAmministrazione;
            info.file_size = stream.Length;
            info.var_impronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(stream);
            info.Version_id = Convert.ToInt32(versionId);
            info.ext = estensione;
            info.last_access = System.DateTime.Now.ToString();
            //aggiorno il db del caching
            retval = updateCaching(info);
            }
            return retval;
        }

        public static DocsPaVO.Caching.InfoFileCaching massimaVersioneDelDocumento(string docNumber)
        {
            InfoFileCaching info = null;
            if (!string.IsNullOrEmpty(docNumber))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                info = query.massimaVersioneDelDocumento(docNumber);
            }

            return info;
        }


        public static bool aggiornaOrarioDiAccessoAlFile(string docNumber,
                                                string versionId,
                                                string estensione,
                                                string filePath,
                                                string idAmministrazione,
                                                byte[] stream,
                                                int aggiornamento)
        {
           
            bool retval = false;
            if (!string.IsNullOrEmpty(docNumber) &&
                !string.IsNullOrEmpty(versionId) &&
                !string.IsNullOrEmpty(estensione) &&
                !string.IsNullOrEmpty(filePath) &&
                !string.IsNullOrEmpty(idAmministrazione) &&
                stream.Length > 0)
            {

                InfoFileCaching info = getDettaglioFileCache(docNumber, versionId, idAmministrazione);
                if (info == null)
                {
                    info = new InfoFileCaching();
                    info.Aggiornato = aggiornamento;
                    info.DocNumber = Convert.ToInt32(docNumber);
                    info.CacheFilePath = filePath;
                    info.idAmministrazione = idAmministrazione;
                    info.file_size = stream.Length;
                    info.var_impronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(stream);
                    info.Version_id = Convert.ToInt32(versionId);
                    info.ext = estensione;
                }
                info.last_access = System.DateTime.Now.ToString();
                //aggiorno il db del caching
                retval = updateCaching(info);
            }
            return retval;
        }


        public static DocsPaVO.Caching.InfoFileCaching[] ricercaDocumemtoInCacheDaTrasferire(string statoAggiornamento, string idAmministrazione)
        {

            InfoFileCaching[] info = null;
            if (!string.IsNullOrEmpty(statoAggiornamento) &&
                !string.IsNullOrEmpty(idAmministrazione))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                info = query.ricercaDocumemtoInCacheDatrasferire(statoAggiornamento, idAmministrazione);
            }

            if (info == null)
                info = new InfoFileCaching[0];

            return info;
        }

        
        public static DocsPaVO.utente.InfoUtente ricercaInfoUtente(string userid)
        {
            DocsPaVO.utente.InfoUtente info = null;
            if (!string.IsNullOrEmpty(userid))
            {
                DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                info = query.ricercaInfoutente(userid);
            }

            return info;
        }

        #region put & get fileEtdoc
        public static bool PutFileETDOC(DocsPaVO.documento.FileRequest fileRequest,
                                 DocsPaVO.documento.FileDocumento fileDocumento,
            string estensione,
            DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retValue = false;
            string isFirmato = "0";

            try
            {
                // Reperimento cartella documentale in cui inserire il file
                string relativeDocPath = GetDocPathAdvanced(fileRequest, infoUtente);

                if (string.IsNullOrEmpty(relativeDocPath))
                    throw new ApplicationException("Impossibile costruire il percorso relativo del file nel documentale");

                //if (!Directory.Exists(configurazione.doc_root_server_locale+@"\"+relativeDocPath))
                //    Directory.CreateDirectory(configurazione.doc_root_server_locale + @"\" + relativeDocPath);

                if (!Directory.Exists(configurazione.doc_root_server_locale+@"\"+relativeDocPath))
                    Directory.CreateDirectory(configurazione.doc_root_server_locale + @"\" + relativeDocPath);

                // Creazione nome file
                string fileName = string.Format("{0}.{1}", fileRequest.versionId, estensione);
                // Creazione path completo del file
                string filePath = string.Format(@"{0}\{1}\{2}",
                                    configurazione.doc_root_server_locale,//System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"],
                                    relativeDocPath,
                                    fileName);

                // Calcolo impronta sul contenuto del file
                string printThumb = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileDocumento.content);


                //anche se è un p7m voglio che l'estensione sia quella del file acquisito


                if (!estensione.Equals("P7M"))
                {
                    while (estensione.EndsWith("P7M"))
                    {
                        estensione = estensione.Remove(estensione.LastIndexOf("."));
                        isFirmato = "1";
                    }
                }
                else
                {
                    isFirmato = "1";
                }

                if (estensione.Contains("."))
                    estensione = estensione.Remove(0, estensione.LastIndexOf(".") + 1);

                if (estensione.Length > 7)
                    estensione = estensione.Substring(0, 7);

                if (string.IsNullOrEmpty(ricercaPathCaching(fileRequest.docNumber, fileRequest.versionId, infoUtente.idAmministrazione)))
                {
                    InfoFileCaching info = new InfoFileCaching();
                    info.Aggiornato = 0;
                    info.DocNumber = Convert.ToInt32(fileRequest.docNumber);
                    info.CacheFilePath = filePath;
                    info.idAmministrazione = infoUtente.idAmministrazione;
                    info.Version_id = Convert.ToInt32(fileRequest.versionId);
                    info.file_size = fileDocumento.content.Length;
                    info.var_impronta = printThumb;
                    info.ext = estensione;
                    info.last_access = System.DateTime.Now.ToString();
                    //aggiorno il db del caching
                    retValue = inserimentoCaching(info);
                }
                else
                    retValue = true;

                if (retValue)
                {
                    // Scrittura file, solamente se l'operazione di aggiornamento sui dati è andata a buon fine
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                        fileStream.Write(fileDocumento.content, 0, fileDocumento.content.Length);
                    // Aggiornamento oggetto FileRequest
                    fileRequest.docServerLoc = configurazione.doc_root_server_locale; //System.Configuration.ConfigurationSettings.AppSettings["DOC_ROOT"];
                    fileRequest.path = @"\" + relativeDocPath;
                    fileRequest.fileName = fileName;
                    fileRequest.fileSize = fileDocumento.content.Length.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore durante la scrittura del documento: {0}", ex.Message));

                retValue = false;
            }

            return retValue;
        }


        public static bool GetFileETDOC(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true; // Presume successo
            string filePath = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Caching cache = new DocsPaDB.Query_DocsPAWS.Caching();

                InfoFileCaching info = new InfoFileCaching();
                info.DocNumber = int.Parse(fileRequest.docNumber);
                info.Version_id = int.Parse(fileRequest.versionId);
                info.idAmministrazione = infoUtente.idAmministrazione;

                /* nuovo */
                filePath = cache.recuperaPathCache(info);

                fileDocumento.name = Path.GetFileName(filePath);
                
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                int fileLength = (int)fileStream.Length;
                fileDocumento.content = new byte[fileLength];
                fileStream.Read(fileDocumento.content, 0, fileLength);
                fileStream.Close();
                fileDocumento.estensioneFile = Path.GetExtension(filePath);
                fileDocumento.estensioneFile = fileDocumento.estensioneFile.Substring(1, 3);
                fileDocumento.length = fileLength;
            }
            catch (System.UnauthorizedAccessException)
            {
                logger.Debug(" Errore durante la lettura del file " + fileDocumento.name + ". Non si possiedono i diritti di lettura sul file .");

                result = false;
                throw new Exception("Errore durante la lettura del file" + fileDocumento.name + ".");

            }
            catch (FileNotFoundException)
            {
                logger.Debug("Errore durante la lettura del file" + fileDocumento.name + ". Verificare se si possiedono i diritti di lettura sul FS e/o se il path del file esiste.");

                result = false;
                throw new Exception("Errore durante la lettura del file" + fileDocumento.name + ".");
            }
            catch (System.IO.IOException)
            {
                logger.Debug("Errore durante la lettura del file" + fileDocumento.name + ". Verificare se si possiedono i diritti di lettura sul FS e/o se il path del file esiste.");

                result = false;
                throw new Exception("Errore durante la lettura del file" + fileDocumento.name + ".");
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la creazione di un documento.", exception);

                result = false;
                throw exception;

            }
            if (result)
                result = aggiornaOrarioDiAccessoAlFile(fileRequest.docNumber, fileRequest.versionId, fileDocumento.estensioneFile, filePath, infoUtente.idAmministrazione, fileDocumento.content, 1);//aggiornaCachePerFile;
            return result;
        }
        #endregion



        public static CacheConfig GetInstance(string idAmm)
        {
            if (CacheFileManager.istance == null)
            {
                CacheFileManager.istance = new CacheFileManager();
                CacheFileManager.istance.initializeInstance(idAmm);
            }
            if (CacheFileManager.istance != null && !_intance.ContainsKey(idAmm))
            {
                lock (_intance)
                {
                    CacheFileManager.istance.initializeInstance(idAmm);
                }
            }

            CacheConfig config = null;

            CacheFileManager._intance.TryGetValue(idAmm, out config);
            
            return config;
        }




        //Inizializza il singleton
        public void initializeInstance( string idAmm)
        {
            if (_intance == null)
            {
                // Creazione oggetto dictionary contenente i dati delle etichette per tutte le amministrazioni
                _intance = new Dictionary<string, CacheConfig>();
                CacheFileManager.getConfigurazioneCache(idAmm);
            }

            if (!_intance.ContainsKey(idAmm))
            {
                lock (_intance)
                {
                    CacheFileManager.getConfigurazioneCache(idAmm);
                }
            }
        }


        public void resetInstance(string idAmm)
        {
            
            if (_intance.ContainsKey(idAmm))
            {
                lock (_intance)
                {
                    _intance.Remove(idAmm);
                }
            }
        }


        public static byte[] streamFileDallaCache(string path)
        {
            byte[] stream = null;
            
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string pathfile = path;
                    if (!string.IsNullOrEmpty(pathfile))
                    {
                        FileInfo file = new FileInfo(pathfile);
                        stream = new byte[file.Length];
                        using (FileStream fs = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
                        {
                            fs.Read(stream, 0, stream.Length);
                            fs.Close();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore nel metodo streemFiledalservergenerale: " + e.Message);
            }
            return stream;
        }


        public static bool cancellaFileTrasferitoInCache(string pathDestinazione)
        {
            bool retval = false;
            try
            {
                if (!string.IsNullOrEmpty(pathDestinazione))
                {
                    File.Delete(pathDestinazione);
                    retval = true;
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore nella cancellazione del file trasferito in cache cache: " + e.Message);
            }
            return retval;
        }


        public static bool writeOnServer(byte[] stream, string pathFile)
        {
            bool retval = false;

            try
            {
                if(stream.Length >0 && !string.IsNullOrEmpty(pathFile) )
                using (FileStream fs1 = new FileStream(pathFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs1.Write(stream, 0, stream.Length);
                    fs1.Close();
                }
                retval = true;
            }
            catch (Exception e)
            {
                logger.Debug("writeOnServer - errore: " + e.Message);
                retval =  false;
            }

            return retval;
        }


        public static byte[] streamFileDalServerGenerale(string docnumber, string versionId)
        {
            byte[] stream = null;
            try
            {
                if (!string.IsNullOrEmpty(docnumber) && !string.IsNullOrEmpty(versionId))
                {
                    DocsPaDB.Query_DocsPAWS.Caching query = new DocsPaDB.Query_DocsPAWS.Caching();
                    string pathfile = query.recuperaPathComponents(docnumber, versionId);
                    if (!string.IsNullOrEmpty(pathfile))
                    {
                        FileInfo file = new FileInfo(pathfile);
                        stream = new byte[file.Length];
                        using (FileStream fs = new FileStream(pathfile, FileMode.Open, FileAccess.Read))
                        {
                            fs.Read(stream, 0, stream.Length);
                            fs.Close();
                        }

                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug("errore nel metodo streemFiledalservergenerale: " + e.Message);
            }
            return stream;
        }

    }//fine classe
    #endregion
}//namespace
