// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDB;
using DocsPaVO.Validations;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.FormatiDocumento
{
    public sealed class DocumentModelsManager
    {
        private static ILogger logger = Log.ForContext(typeof(DocumentModelsManager));

        private const string DEFAULT_MODEL_FILE_NAME = "model";

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
        /// Reperimento percorso del modello di documento per il file richiesto
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private static string GetDocumentModelPath(string fileType)
        {
            return GetDocumentsModelRoot() + @"\" + DEFAULT_MODEL_FILE_NAME + "." + fileType;
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
                    var read =stream.Read(content, 0, content.Length);
                }
            }

            return content;
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
        /// Verifica se la funzionalità di gestione formati file è abilitata o meno
        /// </summary>
        private static void CheckServiceEnabled()
        {
            if (!Configurations.SupportedFileTypesEnabled)
                throw new ApplicationException("Gestione formati documento non abilitata");
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

    }
}
