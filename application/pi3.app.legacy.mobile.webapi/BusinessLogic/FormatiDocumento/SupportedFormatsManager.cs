// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDB;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.Validations;
using Serilog;
using System.Collections;
using System.Data;

namespace BusinessLogic.FormatiDocumento
{
    public class SupportedFormatsManager
    {
        private static ILogger logger = Log.ForContext(typeof(SupportedFormatsManager));

        private static void CheckServiceEnabled()
        {
            if (!Configurations.SupportedFileTypesEnabled)
                throw new ApplicationException("Gestione formati documento non abilitata");
        }

        private static int GetInt32Value(IDataReader reader, string fieldName)
        {
            int retValue = 0;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = reader.GetInt32(reader.GetOrdinal(fieldName));

            return retValue;
        }

        private static string GetStringValue(IDataReader reader, string fieldName)
        {
            string retValue = string.Empty;

            if (!reader.IsDBNull(reader.GetOrdinal(fieldName)))
                retValue = reader.GetValue(reader.GetOrdinal(fieldName)).ToString();

            return retValue;
        }

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


    }
}
