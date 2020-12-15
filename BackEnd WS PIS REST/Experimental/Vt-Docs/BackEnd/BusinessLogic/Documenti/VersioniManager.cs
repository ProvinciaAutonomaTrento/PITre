using System;
using DocsPaVO.PrjDocImport;
using DocsPaVO.documento;
using System.Collections;
using System.Collections.Generic;
using DocsPaVO.utente;
using log4net;

namespace BusinessLogic.Documenti
{
	/// <summary>
	/// </summary>
	public class VersioniManager	
	{
        private static ILog logger = LogManager.GetLogger(typeof(VersioniManager));
		/// <summary>
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="infoUtente"></param>
		/// <param name="daInviare"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.FileRequest addVersion(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente, bool daInviare) 
		{
            logger.Info("BEGIN");
            logger.Debug("addVersion");

            if (fileRequest.repositoryContext != null)
            {
                if (!daInviare)
                {
                    // Inserimento della versione nel repositorycontext (il documento ancora non è stato salvato)
                    int newVersion, newVersionLabel;
                    Int32.TryParse(fileRequest.version, out newVersion);
                    Int32.TryParse(fileRequest.versionLabel, out newVersionLabel);

                    fileRequest.subVersion = "!";
                    fileRequest.version = (newVersion + 1).ToString();
                    fileRequest.versionLabel = (newVersionLabel + 1).ToString();
                }
            }
            else
            {
                // Verifica stato di consolidamento del documento
                DocumentConsolidation.CanExecuteAction(infoUtente, fileRequest.docNumber, DocumentConsolidation.ConsolidationActionsDeniedEnum.AddVersions, true);

                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                if (!documentManager.AddVersion(fileRequest, daInviare))
                    fileRequest = null;
            }
            logger.Info("END");
            return fileRequest;
		}

        /// <summary>
        /// Rimozione di una versione del documento
		/// </summary>
		/// <param name="fileRequest"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
        public static bool removeVersion(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (fileRequest.repositoryContext == null)
            {
                // Verifica stato di consolidamento del documento
                DocumentConsolidation.CanExecuteAction(infoUtente, fileRequest.docNumber, DocumentConsolidation.ConsolidationActionsDeniedEnum.RemoveVersions, true);
            }

            logger.Debug("removeVersion");
            bool result = false;


            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                result = documentManager.RemoveVersion(fileRequest);
                if (result)
                    transactionContext.Complete();
            }
            return result;

        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="versionId"></param>
		/// <param name="daInviare"></param>
		private static void updateDaInviare(string versionId, bool daInviare) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.UpdateDaInviare(versionId, daInviare);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fr"></param>
		/// <param name="daInviare"></param>
		private static void updateVersion(DocsPaVO.documento.FileRequest fr, bool daInviare) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.UpdateVersionManager(fr, daInviare);
		}

        /// <summary>
        /// Setta a 1 cha_segnatura se la versione contiene un documento in formato pdf, con segnatura impressa
        /// </summary>
        /// <param name="objSicurezza"></param>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica l'esito dell'operazione di update
        /// </returns>
        public static bool ModifyVersionSegnatura(DocsPaVO.utente.InfoUtente objSicurezza, string versionId)
        {
            logger.Debug("updateSegnature");

            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
            return documentManager.ModifyVersionSegnatura(versionId);
        }

        /// <summary>
        /// Informa se la versione ha associato un file con impressa la segnatura
        /// </summary>
        /// <param name="objSicurezza"></param>
        /// <param name="versionId"></param>
        /// <returns>
        /// bool che indica se la versione ha associato un file con impressa segnatura o meno
        /// </returns>
        public static bool IsVersionWithSegnature(DocsPaVO.utente.InfoUtente objSicurezza, string versionId)
        {
            logger.Debug("IsVersionWithSegnature");

            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
            return documentManager.IsVersionWithSegnature(versionId);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appId"></param>
		/// <param name="docNumber"></param>
		private static void updateApplication(string appId, string docNumber) 
		{
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
			doc.UpdateApplication(appId, docNumber);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="docNumber"></param>
		/// <param name="objSicurezza"></param>
		/// <returns></returns>
		public static string getLatestVersionID(string docNumber, DocsPaVO.utente.InfoUtente objSicurezza) 
		{
			logger.Debug("getLatestVersionID");
			string versionId = null;

			DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
			versionId = documentManager.GetLatestVersionId(docNumber);

			return versionId;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fileReq"></param>
		public static void modificaVersione(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileReq)
		{
            try
            {
                if (fileReq.repositoryContext == null)
                {
                    // Verifica stato di consolidamento del documento
                    DocumentConsolidation.CanExecuteAction(infoUtente, fileReq.docNumber, DocumentConsolidation.ConsolidationActionsDeniedEnum.RemoveVersions, true);
                }

                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                    documentManager.ModifyVersion(fileReq);

                    transactionContext.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
		}

        public static ImportResult RemoveOldVersionsFromGrey(string idProfile, DocsPaVO.utente.InfoUtente infoUtente,RemoveVersionType type)
        {
            ImportResult res = new ImportResult();
            res.IdProfile=idProfile;
            res.Outcome=DocsPaVO.PrjDocImport.ImportResult.OutcomeEnumeration.KO;
            res.IdProfile = idProfile;
            try
            {
                SchedaDocumento sd = DocManager.getDettaglio(infoUtente, idProfile, null);
                res.DocNumber = sd.docNumber;
                VersioniRemoveHandler removeHandler = new VersioniRemoveHandler(sd, infoUtente, type);
                removeHandler.Execute();
                res = removeHandler.RemoveResult;
                return res;
            }
            catch (Exception e)
            {
                res.Message = "Errore generale durante la rimozione delle versioni";
                return res;
            }
        }

        
	}

    public enum RemoveVersionType
    {
        ALL_BUT_THE_LAST,ALL_BUT_LAST_TWO
    }

}
