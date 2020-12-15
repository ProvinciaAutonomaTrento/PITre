using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.Validations;
using DocsPaDB;
using log4net;

namespace BusinessLogic.FormatiDocumento
{
    /// <summary>
    /// Classe per la gestione dei tipi di file supportati da docspa
    /// </summary>
    public class SupportedFormatsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SupportedFormatsManager));
        /// <summary>
        /// 
        /// </summary>
        private SupportedFormatsManager()
        { }

        #region Public methods
        
        /// <summary>
        /// Rimozione di tutti i formati di file per l'amministrazione richiesta
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static ValidationResultInfo ClearFileTypes(int idAmministrazione)
        {
              // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            ValidationResultInfo resultInfo = new ValidationResultInfo();

            try
            {
                // 1. reperimento modelli documento per l'amministrazione
                SupportedFileType[] fileTypes = GetFileTypes(idAmministrazione);

                using (DBProvider provider = new DBProvider())
                {
                    foreach (SupportedFileType fileType in fileTypes)
                    {
                        // 2. rimozione di eventuali modelli documento per l'amministrazione
                        DocumentModelsManager.RemoveFileTypeDocumentModel(fileType.IdAmministrazione, fileType.FileExtension);

                        // 3. rimozione del formato file
                        RemoveFileType(fileType, provider, resultInfo.BrokenRules);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nella rimozione dei formati documento: {0}", ex.Message);
                logger.Debug(errorMessage);

                resultInfo.BrokenRules.Add(new BrokenRule("ERROR_CLEAR_TYPES", errorMessage));
            }

            resultInfo.Value = (resultInfo.BrokenRules.Count == 0);

            return resultInfo;
        }

        /// <summary>
        /// Inizializzazione formati di file predefiniti per l'amministrazione richiesta
        /// </summary>
        /// <param name="idAmministrazione"></param>
        public static ValidationResultInfo InitializeDefaultFileTypes(int idAmministrazione)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            ValidationResultInfo resultInfo = new ValidationResultInfo();

            try
            {
                // 1. reperimento modelli documento per l'amministrazione
                SupportedFileType[] fileTypes = GetFileTypes(idAmministrazione);

                // 2. rimozione di eventuali modelli documento per l'amministrazione
                foreach (SupportedFileType fileType in fileTypes)
                    DocumentModelsManager.RemoveFileTypeDocumentModel(fileType.IdAmministrazione, fileType.FileExtension);

                // 3. reperimento formati predefiniti
                SupportedFileType[] defaultFileTypes = GetDefaultFileTypes();

                using (DBProvider provider = new DBProvider())
                {
                    provider.BeginTransaction();

                    foreach (SupportedFileType fileType in defaultFileTypes)
                    {
                        // Impostazione dei dati dell'amministrazione
                        fileType.IdAmministrazione = idAmministrazione;
                        fileType.CodiceAmministrazione = GetCodiceAmministrazione(idAmministrazione, provider);

                        // 4. inserimento dei formati predefiniti
                        InsertFileType(fileType, provider, resultInfo.BrokenRules);
                    }
                    if (resultInfo.BrokenRules.Count == 0)
                        provider.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Errore nell'inizializzazione dei formati predefiniti: {0}", ex.Message);
                logger.Debug(errorMessage);

                resultInfo.BrokenRules.Add(new BrokenRule("ERROR_INITIALIZE_DEFAULT_TYPES", errorMessage));
            }

            resultInfo.Value = (resultInfo.BrokenRules.Count == 0);

            return resultInfo;
        }

        /// <summary>
        /// Reperimento dei tipi di file supportati da un'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static SupportedFileType[] GetFileTypes(int idAmministrazione)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            // Reperimento tipi documento predefiniti per tutte le amministrazioni
            List<SupportedFileType> list = new List<SupportedFileType>();

            // Reperimento tipi documento per l'amministrazione
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SUPPORTED_FILE_TYPES");
            queryDef.setParam("idAmministrazione", idAmministrazione.ToString());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(GetSupportedFileType(idAmministrazione, reader));
                }
            }

            return list.ToArray();
        }
        /// <summary>
        /// Reperimento dei tipi di file predefiniti
        /// </summary>
        /// <returns></returns>
        public static SupportedFileType[] GetDefaultFileTypes()
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            // Reperimento tipi documento predefiniti per tutte le amministrazioni
            List<SupportedFileType> list = new List<SupportedFileType>();

            // Reperimento tipi documento per l'amministrazione
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DEFAULT_SUPPORTED_FILE_TYPES");

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(GetSupportedFileType(0, reader));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento di un tipo file supportato da un'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static SupportedFileType GetFileType(int idAmministrazione, string fileExtension)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            SupportedFileType supportedFileType = null;

            // Reperimento tipi documento per l'amministrazione
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SUPPORTED_FILE_TYPE");
            queryDef.setParam("idAmministrazione", idAmministrazione.ToString());
            queryDef.setParam("fileExtension", fileExtension);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                        supportedFileType = GetSupportedFileType(idAmministrazione, reader);
                }
            }

            return supportedFileType;
        }

        /// <summary>
        /// Salvataggio di un oggetto "SupportedFileType"
        /// </summary>
        /// <param name="fileType"></param>
        public static ValidationResultInfo SaveFileType(ref SupportedFileType fileType)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();
            
            ValidationResultInfo resultInfo = new ValidationResultInfo();

            bool commit = false;

            using (DBProvider provider = new DBProvider())
            {
                string codiceAmministrazione = fileType.CodiceAmministrazione;

                provider.BeginTransaction();

                if (fileType.SystemId != 0)
                {
                    // Modifica di un tipo di file
                    commit = UpdateFileType(fileType, provider, resultInfo.BrokenRules);
                }
                else
                {
                    // Inserimento nuovo tipo di file
                    commit = InsertFileType(fileType, provider, resultInfo.BrokenRules);
                }

                // bool removed = false;

                //if (commit)
                //{
                //    bool adminContainsFile = AdminContainsFileType(fileType, provider);

                //    // Verifica se il file è già associato all'amministrazione
                //    if (fileType.FileTypeUsed && !adminContainsFile)
                //    {
                //        // Associazione tipo di file all'amministrazione
                //        commit = AddAdminFileType(fileType, provider, resultInfo.BrokenRules);
                //    }
                //    else if (!fileType.FileTypeUsed && adminContainsFile)
                //    {
                //        // Rimozione associazione tipo di file dall'amministrazione
                //        commit = RemoveAdminFileType(fileType, provider, resultInfo.BrokenRules);
                //        removed = commit;
                //    }
                //}

                if (commit)
                {
                    ValidationResultInfo tmpResult = null;

                    //if (!fileType.FileTypeUsed)
                    //{
                    //    // Rimozione del modello documento
                    //    tmpResult = DocumentModelsManager.RemoveFileTypeDocumentModel(provider, codiceAmministrazione, fileType.FileExtension);
                    //    if (tmpResult.BrokenRules.Count > 0)
                    //        resultInfo.BrokenRules.AddRange(tmpResult.BrokenRules);
                    //}
                    //else if (fileType.ModelFileContent != null)

                    if (fileType.ModelFileContent != null)
                    {
                        if (fileType.MaxFileSize > 0 &&
                            (fileType.MaxFileSize * 1024) < fileType.ModelFileContent.Length)
                        {
                            resultInfo.BrokenRules.Add(new BrokenRule("INVALID_MODEL_FILE_SIZE", "La dimensione del file modello predefinito è maggiore rispetto a quella prevista per il formato", BrokenRule.BrokenRuleLevelEnum.Error));
                        }
                        else
                        {
                            // Upload del documento
                            tmpResult = DocumentModelsManager.SetFileTypeDocumentModel(provider, codiceAmministrazione, fileType.FileExtension, fileType.ModelFileContent);
                            if (tmpResult.BrokenRules.Count > 0)
                                resultInfo.BrokenRules.AddRange(tmpResult.BrokenRules);
                        }
                    }

                    commit = (resultInfo.BrokenRules.Count == 0);
                }

                if (commit)
                {
                //    if (removed)
                //    {
                //        // Rimozione del modello documento
                //        DocumentModelsManager.RemoveFileTypeDocumentModel(provider, codiceAmministrazione, fileType.FileExtension);
                //    }
                //    else if (fileType.ModelFileContent != null)
                //    {
                //        if (fileType.MaxFileSize > 0 &&
                //            (fileType.MaxFileSize * 1024) < fileType.ModelFileContent.Length)
                //        {
                //            resultInfo.BrokenRules.Add(new BrokenRule("INVALID_MODEL_FILE_SIZE", "La dimensione del file modello predefinito è maggiore rispetto a quella prevista per il formato", BrokenRule.BrokenRuleLevelEnum.Error));
                //        }
                //        else
                //        {
                //            // Upload del documento
                //            DocumentModelsManager.SetFileTypeDocumentModel(provider, codiceAmministrazione, fileType.FileExtension, fileType.ModelFileContent);
                //        }
                //    }

                    provider.CommitTransaction();
                }
                else
                {
                    provider.RollbackTransaction();
                }
            }

            resultInfo.Value = (resultInfo.BrokenRules.Count == 0);

            return resultInfo;
        }

        /// <summary>
        /// Rimozione di un oggetto "SupportedFileType"
        /// </summary>
        /// <param name="fileType"></param>
        public static ValidationResultInfo RemoveFileType(ref SupportedFileType fileType)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            ValidationResultInfo resultInfo = new ValidationResultInfo();

            bool commit = false;

            using (DBProvider provider = new DBProvider())
            {
                provider.BeginTransaction();

                if (ContainsFileType(fileType, provider))
                {
                    // Rimozione del modello documento
                    ValidationResultInfo removeResultInfo = DocumentModelsManager.RemoveFileTypeDocumentModel(provider, fileType.CodiceAmministrazione, fileType.FileExtension);
                    commit = removeResultInfo.Value;
                    if (!commit)
                        resultInfo.BrokenRules.AddRange(removeResultInfo.BrokenRules);
                    removeResultInfo = null;
                }
                else
                {
                    // Continua con la rimozione
                    commit = true;
                }

                if (commit)
                {
                    // Rimozione del tipo di file, solamente se non più utilizzato da alcuna amministrazione
                    commit = RemoveFileType(fileType, provider, resultInfo.BrokenRules);
                }

                if (commit)
                    provider.CommitTransaction();
                else
                    provider.RollbackTransaction();
            }

            return resultInfo;
        }

        /// <summary>
        /// Reperimento dei tipi di file supportati da un'amministrazione
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static SupportedFileType[] GetFileTypesPreservation(int idAmministrazione)
        {
            // Verifica se la funzionalità di gestione formati file è abilitata o meno
            CheckServiceEnabled();

            // Reperimento tipi documento predefiniti per tutte le amministrazioni
            List<SupportedFileType> list = new List<SupportedFileType>();

            // Reperimento tipi documento per l'amministrazione
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SUPPORTED_FILE_TYPES_PRESERVATION");
            queryDef.setParam("idAmministrazione", idAmministrazione.ToString());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                        list.Add(GetSupportedFileType(idAmministrazione, reader));
                }
            }

            return list.ToArray();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Verifica se la funzionalità di gestione formati file è abilitata o meno
        /// </summary>
        private static void CheckServiceEnabled()
        {
            if (!Configurations.SupportedFileTypesEnabled)
                throw new ApplicationException("Gestione formati documento non abilitata");
        }

        /// <summary>
        /// Verifica se un filetype è già esistente
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        private static bool ContainsFileType(SupportedFileType fileType, DBProvider provider)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CONTAINS_FILE_TYPE");
            
            queryDef.setParam("fileExtension", fileType.FileExtension);
            queryDef.setParam("idAmministrazione", fileType.IdAmministrazione.ToString());
            
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string field;
            if (provider.ExecuteScalar(out field, commandText))
                retValue = (Convert.ToInt32(field) > 0);

            return retValue;
        }

        /// <summary>
        /// Rimozione tipo di file
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="provider"></param>
        /// <param name="brokenRules"></param>
        /// <returns></returns>
        private static bool RemoveFileType(SupportedFileType fileType, DBProvider provider, ArrayList brokenRules)
        {
            bool retValue = false;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_REMOVE_SUPPORTED_FILE_TYPES");

            queryDef.setParam("systemId", fileType.SystemId.ToString());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            int rowsAffected;
            if (provider.ExecuteNonQuery(commandText, out rowsAffected))
                retValue = (rowsAffected > 0);

            if (retValue)
            {
                fileType.SystemId = 0;
                fileType.IdAmministrazione = 0;
                fileType.CodiceAmministrazione = string.Empty;
            }

            return retValue;
        }

        /// <summary>
        /// Aggiornamento tipo di file
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="provider"></param>
        /// <param name="brokenRules"></param>
        /// <returns></returns>
        private static bool UpdateFileType(SupportedFileType fileType, DBProvider provider, ArrayList brokenRules)
        {
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_UPDATE_SUPPORTED_FILE_TYPES");

                queryDef.setParam("description", fileType.Description);
                
                if (fileType.FileTypeUsed)
                    queryDef.setParam("fileTypeUsed", "1");
                else
                    queryDef.setParam("fileTypeUsed", "0");

                if (fileType.FileTypeSignature)
                    queryDef.setParam("fileTypeSignature", "1");
                else
                    queryDef.setParam("fileTypeSignature", "0");

                if (fileType.FileTypePreservation)
                    queryDef.setParam("fileTypePreservation", "1");
                else
                    queryDef.setParam("fileTypePreservation", "0");

                if (fileType.FileTypeValidation)
                    queryDef.setParam("fileTypeValidation", "1");
                else
                    queryDef.setParam("fileTypeValidation", "0");


                //queryDef.setParam("mimeType", fileType.MimeType);
                queryDef.setParam("fileExtensione", fileType.FileExtension);
                queryDef.setParam("maxFileSize", fileType.MaxFileSize.ToString());
                queryDef.setParam("maxFileSizeAlertMode", ((int) fileType.MaxFileSizeAlertMode).ToString());

                if (fileType.ContainsFileModel)
                    queryDef.setParam("containsFileModel", "1");
                else
                    queryDef.setParam("containsFileModel", "0");

                queryDef.setParam("documentType", ((int) fileType.DocumentType).ToString());
                queryDef.setParam("systemId", fileType.SystemId.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                int rowsAffected;
                if (provider.ExecuteNonQuery(commandText, out rowsAffected))
                    retValue = (rowsAffected > 0);

                //if (retValue)
                //{
                //    if (AdminContainsFileType(fileType, provider))
                //    {
                //        // Aggiornamento valore del flag "CONTAINS_FILE_MODEL"
                //        retValue = SetContainsFileModelFlag(fileType, provider, brokenRules);
                //    }
                //}
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Tipo file {0}: errore nella modifica");
                logger.Debug(errorMessage, ex);

                brokenRules.Add(new BrokenRule("UPDATE_ERROR", errorMessage, BrokenRule.BrokenRuleLevelEnum.Error));
            }

            return retValue;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="containsFileModel"></param>
        ///// <param name="fileType"></param>
        ///// <param name="provider"></param>
        ///// <param name="brokenRules"></param>
        ///// <returns></returns>
        //private static bool SetContainsFileModelFlag(SupportedFileType fileType, DBProvider provider, ArrayList brokenRules)
        //{
        //    bool retValue = false;

        //    // Reperimento tipi documento per l'amministrazione
        //    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_SET_CONTAINS_FILE_MODEL");
            
        //    if (fileType.ContainsFileModel)
        //        queryDef.setParam("containsFileModel", "1");
        //    else
        //        queryDef.setParam("containsFileModel", "0");
            
        //    queryDef.setParam("idSupportedFileType", fileType.SystemId.ToString());

        //    string commandText = queryDef.getSQL();
        //    logger.Debug(commandText);
            
        //    int rowsAffected;
        //    if (provider.ExecuteNonQuery(commandText, out rowsAffected))
        //    {
        //        retValue = (rowsAffected == 1);
        //        if (!retValue)
        //            brokenRules.Add(new BrokenRule("SET_CONTAINS_FILE_MODEL_ERROR", "Errore nell'impostazione del modello predefinito", BrokenRule.BrokenRuleLevelEnum.Error));
        //    }

        //    return retValue;
        //}

        /// <summary>
        /// Inserimento nuovo tipo di file
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="provider"></param>
        /// <param name="brokenRules"></param>
        private static bool InsertFileType(SupportedFileType fileType, DBProvider provider, ArrayList brokenRules)
        {
            bool retValue = false;
            
            try
            {
                // Verifica se il tipo di file è già presente
                if (ContainsFileType(fileType, provider))
                {
                    string errorMessage = string.Format("Tipo file {0} già esistente", fileType.FileExtension);
                    logger.Debug(errorMessage);

                    brokenRules.Add(new BrokenRule("FILE_TYPE_EXIST", errorMessage, BrokenRule.BrokenRuleLevelEnum.Error));
                    retValue = false;
                }
                else
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_INSERT_SUPPORTED_FILE_TYPES");

                    queryDef.setParam("colSystemId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryDef.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));

                    if (fileType.IdAmministrazione > 0)
                        queryDef.setParam("idAmministrazione", fileType.IdAmministrazione.ToString());
                    else
                        queryDef.setParam("idAmministrazione", "Null");

                    if (fileType.FileTypeUsed)
                        queryDef.setParam("fileTypeUsed", "1");
                    else
                        queryDef.setParam("fileTypeUsed", "0");

                    if (fileType.FileTypeSignature)
                        queryDef.setParam("fileTypeSignature", "1");
                    else
                        queryDef.setParam("fileTypeSignature", "0");

                    if (fileType.FileTypePreservation)
                        queryDef.setParam("fileTypePreservation", "1");
                    else
                        queryDef.setParam("fileTypePreservation", "0");

                    if (fileType.FileTypeValidation)
                        queryDef.setParam("fileTypeValidation", "1");
                    else
                        queryDef.setParam("fileTypeValidation", "0");

                    queryDef.setParam("description", fileType.Description);
                    //queryDef.setParam("mimeType", fileType.MimeType);
                    queryDef.setParam("fileExtensione", fileType.FileExtension);
                    queryDef.setParam("maxFileSize", fileType.MaxFileSize.ToString());
                    queryDef.setParam("maxFileSizeAlertMode", ((int)fileType.MaxFileSizeAlertMode).ToString());
                    
                    if (fileType.ContainsFileModel)
                        queryDef.setParam("containsFileModel", "1");
                    else
                        queryDef.setParam("containsFileModel", "0");

                    queryDef.setParam("documentType", ((int)fileType.DocumentType).ToString());

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    int rowsAffected;
                    if (provider.ExecuteNonQuery(commandText, out rowsAffected))
                        retValue = (rowsAffected > 0);

                    if (retValue)
                    {
                        commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                        logger.Debug(commandText);

                        string newId;

                        if (provider.ExecuteScalar(out newId, commandText))
                        {
                            fileType.SystemId = Convert.ToInt32(newId);
                            retValue = true;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Tipo file {0}:  errore nell'inserimento", fileType.FileExtension);
                logger.Debug(errorMessage, ex);

                brokenRules.Add(new BrokenRule("INSERT_ERROR", errorMessage, BrokenRule.BrokenRuleLevelEnum.Error));
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento oggetto "SupportedFileType" dal datareader
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static SupportedFileType GetSupportedFileType(int idAmministrazione, IDataReader reader)
        {
            DataTable tableSchema = reader.GetSchemaTable();

            SupportedFileType fileType = new SupportedFileType();
            
            fileType.SystemId = GetInt32Value(reader, "SYSTEM_ID");
            fileType.IdAmministrazione = GetInt32Value(reader, "ID_AMMINISTRAZIONE");
            fileType.CodiceAmministrazione = GetStringValue(reader, "CODICE_AMMINISTRAZIONE");
            fileType.Description = GetStringValue(reader, "DESCRIPTION");
            //fileType.MimeType = GetStringValue(reader, "MIME_TYPE");
            fileType.FileExtension = GetStringValue(reader, "FILE_EXTENSION");
            fileType.MaxFileSize = GetInt32Value(reader, "MAX_FILE_SIZE");
            fileType.MaxFileSizeAlertMode = (MaxFileSizeAlertModeEnum)GetInt32Value(reader, "MAX_FILE_SIZE_ALERT_MODE");
            fileType.ContainsFileModel = (GetInt32Value(reader, "CONTAINS_FILE_MODEL") > 0);

            // Verifica se il file è utilizzato dall'amministrazione
            fileType.FileTypeUsed = (GetInt32Value(reader, "FILE_TYPE_USED") > 0);
            fileType.DocumentType = (DocumentTypeEnum)GetInt32Value(reader, "DOCUMENT_TYPE");

            fileType.FileTypePreservation = (GetInt32Value(reader, "FILE_TYPE_PRESERVATION") > 0);
            fileType.FileTypeSignature = (GetInt32Value(reader, "FILE_TYPE_SIGNATURE") > 0);

            fileType.FileTypeValidation = (GetInt32Value(reader, "FILE_TYPE_VALIDATION") > 0);

            if (reader.GetSchemaTable().Select("ColumnName='CHA_CONVERTIBLE'").Length > 0)
                fileType.FileTypeConvertible = string.IsNullOrEmpty(reader["CHA_CONVERTIBLE"].ToString()) || !reader["CHA_CONVERTIBLE"].ToString().Equals("1") ? false : true;

            return fileType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static string GetStringValue(IDataReader reader, string fieldName)
        {
            string retValue = string.Empty;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = reader.GetValue(reader.GetOrdinal(fieldName)).ToString();

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static bool GetBooleanValue(IDataReader reader, string fieldName)
        {
            bool retValue = false;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = (reader.GetInt32(reader.GetOrdinal(fieldName)) > 0);

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static int GetInt32Value(IDataReader reader, string fieldName)
        {
            int retValue = 0;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = reader.GetInt32(reader.GetOrdinal(fieldName));

            return retValue;
        }

        /// <summary>
        /// Reperimento codice amministrazione
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        private static string GetCodiceAmministrazione(int idAdmin, DBProvider provider)
        {
            string codiceAmministrazione = string.Empty;

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SUPPORTED_FILE_ADMIN");

            queryDef.setParam("idAmministrazione", idAdmin.ToString());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            provider.ExecuteScalar(out codiceAmministrazione, commandText);

            return codiceAmministrazione;
        }

        #endregion
    }
}
