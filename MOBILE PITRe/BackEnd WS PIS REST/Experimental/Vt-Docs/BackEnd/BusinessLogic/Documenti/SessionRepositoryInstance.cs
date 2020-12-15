using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Configuration;
using log4net;

namespace BusinessLogic.Documenti
{
    /// <summary>
    /// Classe per la gestione della sincronizzazione dei file
    /// dal repository temporaneo al sistema documentale
    /// </summary>
    public class SessionRepositorySyncronizer
    {
        private static ILog logger = LogManager.GetLogger(typeof(SessionRepositorySyncronizer));
        /// <summary>
        /// Spostamento dei file dal repository di sessione temporaneo
        /// al repository documentale in cui sono stati memorizzati i metadati del documento
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <remarks>
        /// L'operazione può essere utilizzata solamente se i metadati del documento
        /// sono stati salvati e se i file eventualmente acquisiti ancora risiedono nel repository temporaneo
        /// </remarks>
        public static void Syncronize(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            if (!string.IsNullOrEmpty(schedaDocumento.systemId) && 
                schedaDocumento.repositoryContext != null)
            {
                SessionRepositoryFileManager fileManager = SessionRepositoryFileManager.GetFileManager(schedaDocumento.repositoryContext);

                DocsPaVO.documento.FileRequest[] versions = (DocsPaVO.documento.FileRequest[])schedaDocumento.documenti.ToArray(typeof(DocsPaVO.documento.FileRequest));
                if (versions.Length > 0)
                    schedaDocumento.documenti = new ArrayList(CopyToRepository(fileManager, versions));
                
                versions = (DocsPaVO.documento.FileRequest[])schedaDocumento.allegati.ToArray(typeof(DocsPaVO.documento.Allegato));
                if (versions.Length > 0)
                    schedaDocumento.allegati = new ArrayList(CopyToRepository(fileManager, versions));

                // Imposta il repository come scaduto
                fileManager.Delete();

                schedaDocumento.repositoryContext = null;
            }
            else
                throw new ApplicationException("Nessun repository di sessione definito per il documento");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileManager"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileRequest CopyToRepository(SessionRepositoryFileManager fileManager, DocsPaVO.documento.FileRequest version)
        {
            DocsPaVO.documento.FileRequest result = null;

            version.repositoryContext = null;

            if (fileManager.ExistFile(version))
            {
                // Reperimento del file dal repository di sessione
                DocsPaVO.documento.FileDocumento document = fileManager.GetFile(version);

                // Inserimento del file nel repository documentale
                result = BusinessLogic.Documenti.FileManager.putFile(version, document, fileManager.Context.Owner);
            }
            else
                result = version;

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileManager"></param>
        /// <param name="versions"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.FileRequest[] CopyToRepository(SessionRepositoryFileManager fileManager, DocsPaVO.documento.FileRequest[] versions)
        {
            List<DocsPaVO.documento.FileRequest> newVersions = new List<DocsPaVO.documento.FileRequest>();
            string tmpVersionLabel;
            foreach (DocsPaVO.documento.FileRequest version in versions)
            {
                tmpVersionLabel = version.versionLabel;

                version.repositoryContext = null;

                if (fileManager.ExistFile(version))
                {
                    // Reperimento del file dal repository di sessione
                    DocsPaVO.documento.FileDocumento document = fileManager.GetFile(version);

                    // Inserimento del file nel repository documentale
                    DocsPaVO.documento.FileRequest result = BusinessLogic.Documenti.FileManager.putFile(version, document, fileManager.Context.Owner);

                    if (!string.IsNullOrEmpty(tmpVersionLabel) && result != null)
                        result.versionLabel = tmpVersionLabel;

                    newVersions.Add(result);

                    // Rimozione del file dal repository di sessione
                    version.repositoryContext = null;
                }
                else
                    newVersions.Add(version);
            }

            return newVersions.ToArray();
        }
    }

    /// <summary>
    /// Classe per la gestione della Garbage Collection dei
    /// per la rimozione dei repository di sessione file non più validi
    /// </summary>
    public class SessionRepositoryCG
    {
        private static ILog logger = LogManager.GetLogger(typeof(SessionRepositoryCG));
        /// <summary>
        /// 
        /// </summary>
        private static Timer _timer = null;

        /// <summary>
        /// Inizio del processo di garbage collection dei repository temporanei
        /// </summary>
        /// <param name="period"></param>
        /// <param name="expireTime"></param>
        public static void BeginCollect(TimeSpan period, TimeSpan expireTime)
        {
            _timer = new Timer(new TimerCallback(OnTimerCallback), expireTime,
                TimeSpan.FromSeconds(0), period);
        }

        /// <summary>
        /// Processo di sospensione della garbage collection
        /// </summary>
        public static void SuspendCollect()
        {
            _timer = null;
        }

        /// <summary>
        /// Handler dell'evento timercallback in cui vengono rimossi 
        /// i repository di sessione scaduti
        /// </summary>
        /// <param name="state"></param>
        private static void OnTimerCallback(object state)
        {
            //DocsPaUtils.LogsManagement.logger.Debug("START SessionRepositoryCG.TimerCallback");

            try
            {
                string path = SessionRepositoryFileManager.RepositoryRootPath;

                if (Directory.Exists(path))
                {
                    foreach (string repositoryPath in Directory.GetDirectories(path))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(repositoryPath);
                        DateTime creationDate = dirInfo.CreationTime;
                        
                        // Confronto della data corrente con la data di creazione della cartella
                        // in cui risiede il repository temporaneo. Se la differenza supera i minuti
                        // espressi, la cartella repository viene rimossa
                        if ((DateTime.Now - creationDate) > ((TimeSpan)state))
                        {
                            logger.Debug(string.Format("Rimozione del repository temporaneo creato il '{0}' con id '{1}'", creationDate.ToString(), repositoryPath));

                            try
                            {
                                Directory.Delete(repositoryPath, true);
                            }
                            catch (Exception ex)
                            {
                                //DocsPaUtils.LogsManagement.logger.Debug(string.Format("Errore nella rimozione del repository temporaneo creato il '{0}' con id '{1}'", creationDate.ToString(), repositoryPath), ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //DocsPaUtils.LogsManagement.logger.Debug("Errore in SessionRepositoryCG.TimerCallback", ex);
            }

            //DocsPaUtils.LogsManagement.logger.Debug("END SessionRepositoryCG.TimerCallback");
        }
    }

    /// <summary>
    /// Rappresenta un documento e tutti i file acquisiti nel repository di sessione
    /// </summary>
    [Serializable()]
    public class SessionRepositoryFileManager
    {
        #region Public members

        /// <summary>
        /// 
        /// </summary>
        static SessionRepositoryFileManager()
        {
            // Al primo utilizzo dell'istanza, inizializzazione del processo di 
            // garbage collection. Il check viene effettuato ogni ora, 
            // i repository hanno una scadenza di un'ora
            SessionRepositoryCG.BeginCollect(TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Se true, la gestione dei session repository è disabilitata
        /// </summary>
        public static bool IsSessionRepositoryDisabled(DocsPaVO.utente.InfoUtente infoUtente)
        {
            string valoreChiaveDB = string.Empty;
            DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(infoUtente.idAmministrazione);
            if (chiaviAmm != null && chiaviAmm.ContainsKey("BE_SESSION_REPOSITORY_DISABLED"))
                //valoreChiaveDB = ((DocsPaVO.amministrazione.ChiaveConfigurazione)chiaviAmm["BE_SESSION_REPOSITORY_DISABLED"]).Valore.ToString();
                valoreChiaveDB = chiaviAmm["BE_SESSION_REPOSITORY_DISABLED"].ToString();
            else
                valoreChiaveDB = "false";

            //const string KEY = "SESSION_REPOSITORY_DISABLED";

            bool retValue;
            //Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings[KEY], out retValue);
            Boolean.TryParse(valoreChiaveDB, out retValue);
            return retValue;
        }

        /// <summary>
        /// Creazione di un nuovo repository di sessione per l'utente che ne fa richiesta
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="forceCreation">
        /// Se true, forza la creazione del repository indipendentemente dal fatto che i repository di sessione siano disabilitati o meno
        /// </param>
        /// <returns></returns>
        public static DocsPaVO.documento.SessionRepositoryContext NewRepository(DocsPaVO.utente.InfoUtente infoUtente, bool forceCreation)
        {
            if (!forceCreation && IsSessionRepositoryDisabled(infoUtente))
                // Gestione dei repository di sessione non abilitata
                return null;
            else
            {
                DocsPaVO.documento.SessionRepositoryContext context = new DocsPaVO.documento.SessionRepositoryContext(infoUtente);

                string path = GetContextRepositoryPath(context);

                // Creazione directory del repository
                Directory.CreateDirectory(path);

                // Serializzazione owner 
                SessionRepositorySerializerHelper.SerializeObject(Path.Combine(path, "owner.dat"), infoUtente);

                return context;
            }
        }

        /// <summary>
        /// Creazione di un nuovo repository di sessione per l'utente che ne fa richiesta
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SessionRepositoryContext NewRepository(DocsPaVO.utente.InfoUtente infoUtente)
        {
            return NewRepository(infoUtente, false);
        }

        /// <summary>
        /// Creazione oggetto filemanager per la gestione dei file nel repository di sessione per l'utente corrente
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SessionRepositoryFileManager GetFileManager(DocsPaVO.documento.SessionRepositoryContext context)
        {
            //if (SessionRepositoryDisabled)
            //    // Gestione dei repository di sessione non abilitata
            //    return null;
            //else

            if (context != null)
                return new SessionRepositoryFileManager(context);
            else
                return null;
        }

        /// <summary>
        /// Reperimento della root path di tutti i repository di sessione
        /// </summary>
        public static string RepositoryRootPath
        {
            get
            {
                //if (SessionRepositoryDisabled)
                //    // Gestione dei repository di sessione non abilitata
                //    return null;
                //else
                //{
                    // Reperimento della directory base del repository documentale
                    string docRoot = ConfigurationManager.AppSettings["DOC_ROOT"];

                    if (string.IsNullOrEmpty(docRoot))
                        throw new ApplicationException("Base Directory del repository documentale non definita nella chiave di configurazione DOC_ROOT");

                    return Path.Combine(docRoot, "SessionRepository");
                //}
            }
        }

        /// <summary>
        /// Reperimento del path del repository di sessione per l'utente corrente
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetContextRepositoryPath(DocsPaVO.documento.SessionRepositoryContext context)
        {
            //if (SessionRepositoryDisabled)
            //    // Gestione dei repository di sessione non abilitata
            //    return null;
            //else

            if (context != null)
                return Path.Combine(SessionRepositoryFileManager.RepositoryRootPath, context.Token);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath">
        /// Path del repository
        /// </param>
        /// <param name="context"></param>
        private SessionRepositoryFileManager(DocsPaVO.documento.SessionRepositoryContext context)
        {
            this.Context = context;

            // Verifica che il repository di sessione non sia scaduto
            this.CheckForRepositoryExpired();

            // Reperimento dell'owner persistito  e confronto con l'owner corrente
            // per verificare utilizzi non autorizzati
            DocsPaVO.utente.InfoUtente owner = (DocsPaVO.utente.InfoUtente)SessionRepositorySerializerHelper.Deserialize(Path.Combine(GetContextRepositoryPath(this.Context), "owner.dat"));

            if (!owner.dst.Equals(this.Context.Owner.dst))
                throw new ApplicationException(string.Format("Utente {0} non autorizzato all'utilizzo del repository", this.Context.Owner.userId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public void SetFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento file)
        {
            this.CheckForRepositoryExpired();

            SessionRepositorySerializerHelper.SerializeObject(this.GetFilePath(fileRequest), file);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public DocsPaVO.documento.FileDocumento GetFile(DocsPaVO.documento.FileRequest fileRequest)
        {
            this.CheckForRepositoryExpired();

            this.CheckForFileExist(fileRequest);

            return (DocsPaVO.documento.FileDocumento)
                SessionRepositorySerializerHelper.Deserialize(this.GetFilePath(fileRequest));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        public void RemoveFile(DocsPaVO.documento.FileRequest fileRequest)
        {
            this.CheckForRepositoryExpired();
            
            this.CheckForFileExist(fileRequest);

            File.Delete(this.GetFilePath(fileRequest));
        }

        /// <summary>
        /// Rimozione di tutti i file associati al documento
        /// </summary>
        public void RemoveAllFiles()
        {
            this.CheckForRepositoryExpired();

            foreach (FileInfo file in new DirectoryInfo(SessionRepositoryFileManager.GetContextRepositoryPath(this.Context)).GetFiles())
                file.Delete();
        }

        /// <summary>
        /// Verifica dell'esistenza del file
        /// </summary>
        /// <param name="id"></param>
        public bool ExistFile(DocsPaVO.documento.FileRequest fileRequest)
        {
            this.CheckForRepositoryExpired();

            return File.Exists(this.GetFilePath(fileRequest));
        }

        /// <summary>
        /// Contesto del repository
        /// </summary>
        public DocsPaVO.documento.SessionRepositoryContext Context
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Delete()
        {
            Directory.Delete(SessionRepositoryFileManager.GetContextRepositoryPath(this.Context), true);
        }

        #endregion

        #region Protected members

        /// <summary>
        /// 
        /// </summary>
        protected void CheckForRepositoryExpired()
        {
            if (!Directory.Exists(SessionRepositoryFileManager.GetContextRepositoryPath(this.Context)))
                throw new ApplicationException(string.Format("Il repository '{0}' risulta scaduto e non può essere più utilizzato", this.Context.Token));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        protected void CheckForFileExist(DocsPaVO.documento.FileRequest fileRequest)
        {
            if (!this.ExistFile(fileRequest))
                throw new ApplicationException(string.Format("File '{0}' inesistente nel repository", this.GetFilePath(fileRequest)));
        }

        /// <summary>
        /// Reperimento del path della versione
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        protected virtual string GetFilePath(DocsPaVO.documento.FileRequest fileRequest)
        {
            if (fileRequest is DocsPaVO.documento.Allegato)
                return this.GetAttatchmentFilePath((DocsPaVO.documento.Allegato) fileRequest);
            else
                return Path.Combine(GetContextRepositoryPath(this.Context), fileRequest.version);
        }

        /// <summary>
        /// Reperimento del path 
        /// </summary>
        /// <param name="attatchment"></param>
        /// <returns></returns>
        protected string GetAttatchmentFilePath(DocsPaVO.documento.Allegato attatchment)
        {
            string path=this.GetOrCreateAttatchmentFolder(attatchment);
            return Path.Combine(path, attatchment.version);
        }

        /// <summary>
        /// Reperimento cartella nel repository per la gestione degli allegati
        /// </summary>
        /// <param name="attatchment"></param>
        /// <returns></returns>
        protected string GetOrCreateAttatchmentFolder(DocsPaVO.documento.Allegato attatchment)
        {
            return Directory.CreateDirectory(Path.Combine(GetContextRepositoryPath(this.Context), attatchment.versionLabel)).FullName;
        }

        #endregion

        /// <summary>
        /// Helper class per la serializzazione / deserializzazione degli oggetti nel repository
        /// </summary>
        private sealed class SessionRepositorySerializerHelper
        {
            public SessionRepositorySerializerHelper()
            { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="obj"></param>
            public static void SerializeObject(string filePath, object obj)
            {
                using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, obj);
                    stream.Flush();
                    stream.Close();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public static object Deserialize(string filePath)
            {
                object obj = null;

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    obj = formatter.Deserialize(stream);
                    stream.Flush();
                    stream.Close();
                }

                return obj;
            }
        }
    }
}
