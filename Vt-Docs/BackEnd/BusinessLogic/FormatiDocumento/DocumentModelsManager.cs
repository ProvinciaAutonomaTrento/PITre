using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.IO;
using DocsPaVO.Validations;
using DocsPaDB;
using DocsPaVO.FormatiDocumento;
using log4net;

namespace BusinessLogic.FormatiDocumento
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentModelsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentModelsManager));
        /// <summary>
        /// 
        /// </summary>
        private const string DEFAULT_MODEL_FILE_NAME = "model";

        /// <summary>
        /// 
        /// </summary>
        private DocumentModelsManager()
        { }

        #region Public methods

        /// <summary>
        /// Reperimento di tutti i tipi di file che hanno un modello predefinito impostato
        /// e che sono ammessi per l'amministrazione richiesta 
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <returns></returns>
        public static string[] GetFileTypes(int idAdmin)
        {
            return GetFileTypes(GetCodiceAmministrazione(idAdmin));
        }

        /// <summary>
        /// Reperimento di tutti i tipi di file che hanno un modello predefinito impostato
        /// e che sono ammessi per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public static string[] GetFileTypes(string admin)
        {
            List<string> fileTypes = new List<string>();

            // Verifica se la gestione formati documento è abilitata o meno
            if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
            {
                // Reperimento dei tipi file ammessi per cui è presente un modello predefinito
                AdminSupportedFile[] supportedFilesWithModels = GetAdminSupportedFileModels(admin);

                // Caricamento formati ammessi che hanno un modello predefinito impostato
                FillAdminSupportedFilesWithModels(admin, fileTypes, supportedFilesWithModels);
            }
            else
            {
                // Se la gestione non è abilitata, vengono reperiti i modelli predefiniti
                // indipendentemente dall'amministrazione 
                FillFilesWithModels(fileTypes);
            }

            return fileTypes.ToArray();
        }

        /// <summary>
        /// Reperimento modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static byte[] GetModelFile(int idAdmin, string fileType)
        {
            return GetModelFile(GetCodiceAmministrazione(idAdmin), fileType);
        }

        /// <summary>
        /// Reperimento modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static byte[] GetModelFile(string admin, string fileType)
        {
            string documentModelPath = string.Empty;

            // Reperimento percorso modello documento

            // Verifica se la gestione formati documento è abilitata o meno
            if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
                documentModelPath = GetDocumentModelPath(admin, fileType);
            else
                documentModelPath = GetDocumentModelPath(fileType);
            
            byte[] content = GetFileContent(documentModelPath);

            if (content == null) 
            {
                // Se il modello per l'amminitrazione non è presente,
                // viene reperito il modello corrispondente predefinito valido
                // per tutte le amministrazioni
                documentModelPath = GetDocumentModelPath(fileType);
                content = GetFileContent(documentModelPath);
            }

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="admin"></param>
        /// <param name="fileType"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        internal static ValidationResultInfo SetFileTypeDocumentModel(DBProvider provider, string admin, string fileType, byte[] fileContent)
        {
            ValidationResultInfo result = new ValidationResultInfo();

            // Reperimento percorso modello documento
            string documentModelPath = GetDocumentModelPath(admin, fileType);

            FileInfo fileInfo = new FileInfo(documentModelPath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            using (Stream stream = new FileStream(documentModelPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                stream.Write(fileContent, 0, fileContent.Length);

            result.Value = UpdateFileModel(provider, admin, fileType, true, result.BrokenRules);
            
            result.Value = (result.BrokenRules.Count == 0);
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="fileType"></param>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static ValidationResultInfo SetFileTypeDocumentModel(int idAdmin, string fileType, byte[] fileContent)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            string admin = GetCodiceAmministrazione(idAdmin);

            return SetFileTypeDocumentModel(null, admin, fileType, fileContent);
        }

        /// <summary>
        /// Impostazione modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileExtension"></param>
        /// <param name="fileContent"></param>
        public static ValidationResultInfo SetFileTypeDocumentModel(string admin, string fileType, byte[] fileContent)
        {
            return SetFileTypeDocumentModel(null, admin, fileType, fileContent);
        }

        /// <summary>
        /// Rimozione modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="admin"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static ValidationResultInfo RemoveFileTypeDocumentModel(DBProvider provider, string admin, string fileType)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            ValidationResultInfo result = new ValidationResultInfo();

            // Reperimento percorso modello documento
            string documentModelPath = GetDocumentModelPath(admin, fileType);

            if (File.Exists(documentModelPath))
                // Rimozione del file modello e della cartella
                File.Delete(documentModelPath);

            result.Value = UpdateFileModel(provider, admin, fileType, false, result.BrokenRules);

            result.Value = (result.BrokenRules.Count == 0);

            return result;
        }

        /// <summary>
        /// Rimozione modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileType"></param>
        /// <param name="fileContent"></param>
        public static ValidationResultInfo RemoveFileTypeDocumentModel(int idAdmin, string fileType)
        {
            string admin = GetCodiceAmministrazione(idAdmin);
            
            return RemoveFileTypeDocumentModel(null, admin, fileType);
        }

        /// <summary>
        /// Rimozione modello documento per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileType"></param>
        /// <param name="fileContent"></param>
        public static ValidationResultInfo RemoveFileTypeDocumentModel(string admin, string fileType)
        {
            return RemoveFileTypeDocumentModel(null, admin, fileType);
        }

        #endregion

        #region Private methods
        
        /// <summary>
        /// Reperimento codice amministrazione
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <returns></returns>
        private static string GetCodiceAmministrazione(int idAdmin)
        {
            string codiceAmministrazione = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SUPPORTED_FILE_ADMIN");

            queryDef.setParam("idAmministrazione", idAdmin.ToString());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider provider = new DBProvider())
                provider.ExecuteScalar(out codiceAmministrazione, commandText);

            return codiceAmministrazione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileTypes"></param>
        private static void FillFilesWithModels(List<string> fileTypes)
        {
            // Reperimento percorso modelli documenti indipendentemente dall'amministrazione
            string documentsModelRoot = GetDocumentsModelRoot();
            DirectoryInfo directoryInfo = new DirectoryInfo(documentsModelRoot);
             if (directoryInfo.Exists)
                 foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                     fileTypes.Add(fileInfo.Extension.Replace(".", ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileTypes"></param>
        private static void FillAdminSupportedFilesWithModels(string admin, List<string> fileTypes, AdminSupportedFile[] supportedFilesWithModels)
        {
            if (supportedFilesWithModels.Length > 0)
            {
                // Reperimento percorso modelli documenti indipendentemente dall'amministrazione
                string documentsModelRoot = GetDocumentsModelRoot();

                // Reperimento percorso modelli documenti per l'amministrazione
                string adminDocumentsModelRoot = GetDocumentsModelRoot(admin);

                DirectoryInfo directoryInfo = new DirectoryInfo(adminDocumentsModelRoot);

                if (directoryInfo.Exists)
                {
                    // Reperimento modelli predefiniti per tutti i tipi di file ammessi
                    foreach (AdminSupportedFile supportedFileType in supportedFilesWithModels)
                    {
                        FileInfo[] fileInfoList = directoryInfo.GetFiles(string.Concat(DEFAULT_MODEL_FILE_NAME, ".", supportedFileType.FileExtension));

                        if (supportedFileType.ContainsModel && fileInfoList.Length > 0)
                        {
                            FileInfo fileInfo = fileInfoList[0];

                            if (!fileTypes.Contains(supportedFileType.FileExtension))
                                fileTypes.Add(supportedFileType.FileExtension);
                        }
                        else
                        {
                            // Se nessun modello impostato dall'utente è presente per il formato di file,
                            // viene reperito il modello predefinito (se presente)
                            string defaultModelPath = string.Concat(documentsModelRoot, @"\", DEFAULT_MODEL_FILE_NAME, ".", supportedFileType.FileExtension);
                            if (File.Exists(defaultModelPath))
                                if (!fileTypes.Contains(supportedFileType.FileExtension))
                                    fileTypes.Add(supportedFileType.FileExtension);
                        }
                    }
                }
            }

            if (fileTypes.Count == 0)
            {
                // Vengono reperiti i modelli di file predefiniti
                // indipendentemente dall'amministrazione
                FillFilesWithModels(fileTypes);
            }
        }

        /// <summary>
        /// Reperimento tipi documento ammessi per l'amministrazione
        /// per cui è presente un modello predefinito
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <returns></returns>
        private static AdminSupportedFile[] GetAdminSupportedFileModels(int idAdmin)
        {
            List<AdminSupportedFile> list = new List<AdminSupportedFile>();
            
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_ADMIN_SUPPORTED_FILE_MODELS");

            queryDef.setParam("idAmministrazione", idAdmin.ToString());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider provider = new DBProvider())
            {
                using (IDataReader reader = provider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        AdminSupportedFile supportedFile = new AdminSupportedFile();
                        
                        supportedFile.FileExtension = reader.GetValue(reader.GetOrdinal("FILE_EXTENSION")).ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("CONTAINS_FILE_MODEL")))
                            supportedFile.ContainsModel = (reader.GetInt32(reader.GetOrdinal("CONTAINS_FILE_MODEL")) > 0);
                        
                        string documentType = reader.GetValue(reader.GetOrdinal("DOCUMENT_TYPE")).ToString();
                        supportedFile.DocumentType = (DocumentTypeEnum) Enum.Parse(typeof(DocumentTypeEnum), documentType, true);
                        list.Add(supportedFile);
                    }
                }
            }
            
            return list.ToArray();
        }

        /// <summary>
        /// Reperimento tipi documento ammessi per l'amministrazione
        /// per cui è presente un modello predefinito
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        private static AdminSupportedFile[] GetAdminSupportedFileModels(string admin)
        {
            List<AdminSupportedFile> list = new List<AdminSupportedFile>();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_ADMIN_BY_CODE_SUPPORTED_FILE_MODELS");

            queryDef.setParam("codiceAmministrazione", admin);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider provider = new DBProvider())
            {
                using (IDataReader reader = provider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        AdminSupportedFile supportedFile = new AdminSupportedFile();

                        supportedFile.FileExtension = reader.GetValue(reader.GetOrdinal("FILE_EXTENSION")).ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("CONTAINS_FILE_MODEL")))
                            supportedFile.ContainsModel = (reader.GetInt32(reader.GetOrdinal("CONTAINS_FILE_MODEL")) > 0);
                        
                        list.Add(supportedFile);
                    }
                }
            }

            return list.ToArray();
        }
            
        /// <summary>
        /// Reperimento contenuto file
        /// </summary>
        /// <param name="modelFilePath"></param>
        /// <returns></returns>
        private static byte[] GetFileContent(string modelFilePath)
        {
            byte[] content = null;

            FileInfo fileInfo = new FileInfo(modelFilePath);

            if (fileInfo.Exists)
            {
                using (Stream stream = new FileStream(modelFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    content = new byte[stream.Length];
                    stream.Read(content, 0, content.Length);
                }
            }

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileType"></param>
        /// <param name="containsFileModel"></param>
        /// <param name="brokenRules"></param>
        /// <returns></returns>
        private static bool UpdateFileModel(DBProvider provider, string admin, string fileType, bool containsFileModel, ArrayList brokenRules)
        {
            bool retValue = false;

            // Reperimento tipi documento per l'amministrazione
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_SET_CONTAINS_FILE_MODEL_2");
            
            if (containsFileModel)
                queryDef.setParam("containsFileModel", "1");
            else
                queryDef.setParam("containsFileModel", "0");

            queryDef.setParam("codiceAmministrazione", admin);
            queryDef.setParam("fileType", fileType);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            bool disposeProvider = (provider == null);

            try
            {
                if (disposeProvider)
                {
                    provider = new DBProvider();
                    provider.BeginTransaction();
                }

                int rowsAffected;
                provider.ExecuteNonQuery(commandText, out rowsAffected);
                retValue = (rowsAffected == 1);
                if (!retValue)
                    brokenRules.Add(new BrokenRule("UPDATE_FILE_MODEL_ERROR", "Errore nell'aggiornamento del modello predefinito", BrokenRule.BrokenRuleLevelEnum.Error));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (disposeProvider)
                {
                    if (retValue)
                        provider.CommitTransaction();
                    else
                        provider.RollbackTransaction();
                    
                    provider.Dispose();
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento percorso radice in cui risiedono i modelli
        /// </summary>
        /// <returns></returns>
        private static string GetDocumentsModelRoot()
        {
            string modelPath = AppDomain.CurrentDomain.BaseDirectory;

            return modelPath.Replace("/", @"\") + @"documentModels";
        }
        
        /// <summary>
        /// Reperimento percorso radice in cui risiedono i modelli per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        private static string GetDocumentsModelRoot(string admin)
        {
            return GetDocumentsModelRoot() + @"\" + admin;
        }

        /// <summary>
        /// Reperimento percorso del modello di documento per il file richiesto
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private static string GetDocumentModelPath(string fileType)
        {
            return GetDocumentsModelRoot() + @"\" + DEFAULT_MODEL_FILE_NAME + "." + fileType;
        }

        /// <summary>
        /// Reperimento percorso del modello di documento per il file richiesto per l'amministrazione richiesta
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private static string GetDocumentModelPath(string admin, string fileType)
        {
            return GetDocumentsModelRoot(admin) + @"\" + DEFAULT_MODEL_FILE_NAME + "." + fileType;
        }

        /// <summary>
        /// Tipo di file supportato dall'amministrazione
        /// </summary>
        private struct AdminSupportedFile
        {
            public AdminSupportedFile(string fileExtension, bool containsModel, DocumentTypeEnum documentType)
            {
                this.FileExtension = fileExtension;
                this.ContainsModel = containsModel;
                this.DocumentType = documentType;
            }

            public string FileExtension;
            public bool ContainsModel;
            public DocumentTypeEnum DocumentType;
        }

        /// <summary>
        /// Verifica se la funzionalità di gestione formati file è abilitata o meno
        /// </summary>
        private static void CheckServiceEnabled()
        {
            if (!Configurations.SupportedFileTypesEnabled)
                throw new ApplicationException("Gestione formati documento non abilitata");
        }

        #endregion
    }
}
