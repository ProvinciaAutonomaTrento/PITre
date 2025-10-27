// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.LibroFirma;
using Serilog;

namespace BusinessLogic.Documenti
{
    public class AllegatiManager
    {
        private static ILogger logger = Log.ForContext(typeof(AllegatiManager));

        public static System.Collections.ArrayList getAllegati(string docNumber, string filterAllegatiPec, string simplifiedInteroperabilityId = "")
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            return doc.GetAllegati(docNumber, filterAllegatiPec, simplifiedInteroperabilityId);
        }

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

                    if (statoCongelato && allegato.TypeAttachment != 2 && allegato.TypeAttachment != 3 && allegato.TypeAttachment != 6)
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
                        //Se l'allegato non è una ricevuta controllo se il doc principale è in libro firma; se il doc principale è in libro firma non è possibile allegare
                        if (allegato.TypeAttachment != 2 && allegato.TypeAttachment != 3 && allegato.TypeAttachment != 6 && LibroFirmaManager.IsModificaBloccataPerDocumentoPrincipaleInLF(allegato.docNumber, infoUtente.idAmministrazione))
                            throw new Exception("Non è possibile creare l'allegato poichè il documento principale è in libro firma");

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

        public static DocsPaVO.documento.Allegato aggiungiAllegato(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.Allegato allegato)
        {
            return aggiungiAllegato(infoUtente, allegato, true);
        }

        public static bool setFlagAllegati_PEC_IS_EXT(string versionId, string docNumber, string flagChar)
        {
            DocsPaDB.Query_DocsPAWS.Documenti obj = new DocsPaDB.Query_DocsPAWS.Documenti();
            return obj.SetFlagAllegati_PEC_IS_EXT(versionId, docNumber, flagChar);
        }

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
        /// Verifica se è abilitata la gestione della profilazione allegati
        /// </summary>
        /// <returns></returns>
        public static bool isEnabledProfilazioneAllegati()
        {
            return DocsPaDB.Query_DocsPAWS.Documenti.isEnabledProfilazioneAllegati;
        }

    }
}
