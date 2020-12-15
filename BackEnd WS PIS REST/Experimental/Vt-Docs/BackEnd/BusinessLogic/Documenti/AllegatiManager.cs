using System;
using System.Configuration;
using log4net;
 
namespace BusinessLogic.Documenti
{
	/// <summary>
	/// </summary>
	public class AllegatiManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(AllegatiManager));
        /// <summary>
        /// Verifica se è abilitata la gestione della profilazione allegati
        /// </summary>
        /// <returns></returns>
        public static bool isEnabledProfilazioneAllegati()
        {
            return DocsPaDB.Query_DocsPAWS.Documenti.isEnabledProfilazioneAllegati;
        }

        /// <summary>
        /// Verifica se un allegato è profilato o meno
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public static bool isAllegatoProfilato(DocsPaVO.documento.Allegato allegato)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();
            
            return documentiDb.isAllegatoProfilato(allegato.docNumber);
        }

        /// <summary>
        /// Reperimento dell'id del documento principale
        /// </summary>
        /// <param name="allegato"></param>
        /// <returns></returns>
        public static string getIdDocumentoPrincipale(DocsPaVO.documento.Allegato allegato)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();

            return documentiDb.GetIdDocumentoPrincipale(allegato.docNumber);
        }

		/// <summary>
        /// Reperimento degli allegati di un documento
		/// </summary>
		/// <param name="infoDoc"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
        public static System.Collections.ArrayList getAllegati(string docNumber, string filterAllegatiPec, string simplifiedInteroperabilityId = "")
		{
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

			return doc.GetAllegati(docNumber, filterAllegatiPec, simplifiedInteroperabilityId);
		}

        /// <summary>
        /// Prende gli id degli allegati collegati ad un documento principale.
        /// Fatta per l'ottimizzazione del tab allegati.
        /// </summary>
        /// <param name="idDocPrincipale"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getAllegatiSuperSimple(string idDocPrincipale)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetAllegatiSuperSimple(idDocPrincipale);
        }
        /// <summary>
        /// return 1 se l'allegato è di tipo pec
        /// </summary>
        /// <param name="version_id"></param>
        /// <returns></returns>
        public static string getIsAllegatoPEC(string version_id)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            //return doc.GeIsAllegatoPEC(version_id);
            // Utilizzo CHA_ALLEGATI_ESTERNO in VERSIONS
            string tipoAll = doc.GetTipologiaAllegato(version_id);
            if (tipoAll == "P") return "1";
            else return "0";
        }

        /// <summary>
        /// return 1 se l'allegato è di tipo IS
        /// </summary>
        /// <param name="version_id"></param>
        /// <returns></returns>
        public static string getIsAllegatoIS(string version_id)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            //return doc.GeIsAllegatoIS(version_id);
            // Utilizzo CHA_ALLEGATI_ESTERNO in VERSIONS
            string tipoAll = doc.GetTipologiaAllegato(version_id);
            if (tipoAll == "I") return "1";
            else return "0";
        }


        /// <summary>
        /// Inserimento di un nuovo allegato per un documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="allegato"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.Allegato aggiungiAllegato(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.Allegato allegato, bool statoCongelato)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                if (allegato.repositoryContext != null)
                {
                    // Inserimento di un allegato nel repositorycontext
                    allegato.version = "1"; // Prima versione dell'allegato
                    allegato.versionId = allegato.position.ToString(); // Viene assegnato un nuovo id temporaneo (la sequenza di inserimento)
                    allegato.versionLabel = DocsPaDB.Query_DocsPAWS.Documenti.FormatCodiceAllegato(allegato.position); // Viene creato, in base alla sequenza di inserimento, il codice progressivo

                    // L'allegato viene impostato come non acquisito
                    allegato.subVersion = "!";
                    allegato.fileName = string.Empty;
                    allegato.fileSize = "0";
                }
                else
                {

                    if (statoCongelato)
                    {
                        // Controllo su stato congelato, solo se non si sta creando l'allegato nel repository context
                        DocumentConsolidation.CanExecuteAction(infoUtente,
                                        allegato.docNumber,
                                        DocumentConsolidation.ConsolidationActionsDeniedEnum.AddAttatchments, true);
                    }
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

                    string oldApp = null;
                    string putfile = "";

                    try
                    {
                        if (allegato.applicazione != null)
                        {
                            if (allegato.applicazione.systemId == null)
                                allegato.applicazione = BusinessLogic.Documenti.FileManager.getApplicazione(allegato.applicazione.estensione);

                            logger.Debug("Update della tabella profile");

                            string param = "DOCNUMBER=" + allegato.docNumber;
                            doc.GetApplication(out oldApp, allegato.docNumber, allegato.applicazione.systemId, param);
                        }

                        if (!string.IsNullOrEmpty(allegato.versionLabel))
                            // L'allegato è già stato creato (ha già la versionLabel), 
                            // si sta usando il metodo per fare un'acquisizione sull'allegato
                            putfile = "Y";

                        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                        if (!documentManager.AddAttachment(allegato, putfile))
                            throw new Exception("Errore nell'inserimento dell'allegato nel documentale");
                        else
                            transactionContext.Complete();
                    }
                    catch (Exception e)
                    {
                        string message = string.Format("Errore nell'inserimento dell'allegato: {0}", e.Message);
                        logger.Debug(message);
                        throw new Exception(message);
                    }
                }
            }

            return allegato;
        }

		/// <summary>
        /// Inserimento di un nuovo allegato per un documento
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="allegato"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.Allegato aggiungiAllegato(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.Allegato allegato)
		{
            return aggiungiAllegato (infoUtente,allegato ,true);
		}

        /// <summary>
        /// Inserimento di un nuovo allegato per un documento(aggiunge un allegato associato ad una notifica PEC)
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="allegato"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.Allegato aggiungiAllegatoPEC(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.Allegato allegato)
		{
            return aggiungiAllegato(infoUtente, allegato, false);
		}

		/// <summary>
        /// Modifica dei dati di un allegato
		/// </summary>
		/// <param name="allegato"></param>
		/// <param name="debug"></param>
		public static void modificaAllegato(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.Allegato allegato)
		{
            if (allegato.repositoryContext != null)
            {
                // Modifica allegato gestito mediante il repository context
            }
            else
            {
                // Controllo su stato congelato
                DocumentConsolidation.CanExecuteAction(infoUtente,
                                    allegato.docNumber,
                                    DocumentConsolidation.ConsolidationActionsDeniedEnum.ModifyAttatchments, true);

                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                    documentManager.ModifyAttatchment(allegato);

                    transactionContext.Complete();
                }
            }
		}

		/// <summary>
        /// Rimozione di un allegato
		/// </summary>
        /// <param name="allegato"></param>
        /// <param name="infoUtente"></param>
		public static bool rimuoviAllegato(DocsPaVO.documento.Allegato allegato, DocsPaVO.utente.InfoUtente infoUtente)
		{
            bool retValue = false;

            if (allegato.repositoryContext != null)
            {
                // Rimozione dell'allegato nell'ambito del  repository context
                SessionRepositoryFileManager fm = BusinessLogic.Documenti.SessionRepositoryFileManager.GetFileManager(allegato.repositoryContext);

                // Verifica esistenza file acquisito nel repository context
                if (fm.ExistFile(allegato))
                    fm.RemoveFile(allegato);

                retValue = true;
            }
            else
            {
                // Controllo su stato congelato
                DocumentConsolidation.CanExecuteAction(infoUtente,
                            allegato.docNumber,
                            DocumentConsolidation.ConsolidationActionsDeniedEnum.RemoveAttatchments, true);

                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                    retValue = documentManager.RemoveAttatchment(allegato);

                    if (retValue)
                        transactionContext.Complete();
                }

            }

            return retValue;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="allegato"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public static bool scambiaAllegatoDocumento(DocsPaVO.utente.InfoUtente infoUtente,
                    DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            // Controllo su stato congelato
            DocumentConsolidation.CanExecuteAction(infoUtente, documento.docNumber, DocumentConsolidation.ConsolidationActionsDeniedEnum.ModifyVersions, true);

            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                retValue = documentManager.ScambiaAllegatoDocumento(allegato, documento);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="docnumber"></param>
        /// <param name="versionLabel"></param>
        /// <returns></returns>
		private static string getVersionId(string docnumber, string versionLabel)
		{
		    DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti(); 
			return obj.GetVersionId(docnumber, versionLabel);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static bool setFlagAllegatiEsterni(string versionId, string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
            return obj.SetFlagAllegatiEsterni(versionId, docNumber);
        }

        /// <summary>
        /// MEtodo per il set del flag CHA_ALLEGATI_ESTERNO in VERSIONS
        /// </summary>
        /// <param name="versionId"></param>
        /// <param name="docNumber"></param>
        /// <param name="flagChar"></param>
        /// <returns></returns>
        public static bool setFlagAllegati_PEC_IS_EXT(string versionId, string docNumber, string flagChar)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
            return obj.SetFlagAllegati_PEC_IS_EXT(versionId, docNumber,flagChar);
        }

        /// <summary>
        /// return 1 se l'allegato è di tipo pec
        /// </summary>
        /// <param name="version_id"></param>
        /// <returns></returns>
        public static string getIsAllegatoEsterno(string version_id)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetIsAllegatoEsterno(version_id);
        }
	}
}
