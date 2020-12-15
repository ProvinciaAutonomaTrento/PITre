using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using System.Text;
using log4net;
using BusinessLogic.interoperabilita.Semplificata;
using BusinessLogic.Amministrazione;
using DocsPaVO.Interoperabilita.Semplificata;

namespace BusinessLogic.Documenti
{
    /// <summary>
    /// Summary description for DocManager.
    /// </summary>
    public class DocManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocManager));

        public string documentoGetTipoProto(string docNumber)
        {
            string result = null;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.getTipoProto(docNumber);
            return result;
        }
        /// <summary>
        /// verifica se un documento è inserito in cestino;
        /// return value "1" se in cestino; "0" se non in cestino; "" (vuoto) se errore
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string checkdocInCestino(string docNumber)
        {
            string rtn = string.Empty;
            using (DocsPaDB.DBProvider db = new DocsPaDB.DBProvider())
            {
                if (docNumber != null && !docNumber.Equals(""))
                {
                    string cmd = "select " + DocsPaDbManagement.Functions.Functions.getNVL("cha_in_cestino", "0") + " from profile where docnumber=" + docNumber;
                    using (System.Data.IDataReader dr = db.ExecuteReader(cmd))
                    {
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0))
                            {
                                rtn = dr.GetValue(0).ToString();
                            }

                        }

                    }
                }
            }
            return rtn;

        }

        public bool AcquisisciDirittiDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

            bool retVal = doc.AcquisisciDirittiDocumento(schedaDocumento, infoUtente);
            docManager.RefreshAclDocumento(schedaDocumento);

            return retVal;
        }

        public static string getAccessRightDocBySystemID(string idProfile, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            return doc.getAccessRightDocBySystemID(idProfile, infoUtente);
        }

        /// <summary>
        /// Reperimento scheda documento a partire dal solo "docNumber".
        /// NB: Viene ignorata la gestione della visibilità. 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento getDettaglioNoSecurity(DocsPaVO.utente.InfoUtente infoUtente, string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc = doc.GetDettaglioNoSecurity(infoUtente, docNumber, docNumber);

            if (schedaDoc == null)
            {
                throw new ApplicationException(string.Format("Documento con ID '{0}' non trovato", docNumber));
            }
            else
            {
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

                // Reperimento del token di autenticazione per il superutente del documentale
                string superUserToken = userManager.GetSuperUserAuthenticationToken();
                string oldToken = infoUtente.dst;
                infoUtente.dst = superUserToken;

                try
                {
                    // Reperimento informazioni se il documento è in stato checkout
                    schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // Ripristino token di autenticazione
                    infoUtente.dst = oldToken;
                }
            }

            try
            {
                DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
                string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                app.estensione = ext;
                docPrinc.applicazione = app;
                schedaDoc.documenti[0] = docPrinc;
            }
            catch (Exception) { }

            return schedaDoc;
        }

        //DA TESTARE - Andrea
        /// <summary>
        /// Reperimento scheda documento a partire dal "id_vecchio_doc".
        /// NB: Viene ignorata la gestione della visibilità. 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento getDettaglioNoSecurityByIDVecchioDoc(DocsPaVO.utente.InfoUtente infoUtente, string id_vecchio_doc)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc = doc.GetDettaglioNoSecurityByIDVecchioDoc(infoUtente, id_vecchio_doc, id_vecchio_doc);

            if (schedaDoc == null)
            {
                throw new ApplicationException(string.Format("Documento con ID '{0}' non trovato", id_vecchio_doc));
            }
            else
            {
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

                // Reperimento del token di autenticazione per il superutente del documentale
                string superUserToken = userManager.GetSuperUserAuthenticationToken();
                string oldToken = infoUtente.dst;
                infoUtente.dst = superUserToken;

                try
                {
                    // Reperimento informazioni se il documento è in stato checkout
                    schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // Ripristino token di autenticazione
                    infoUtente.dst = oldToken;
                }
            }

            try
            {
                DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
                string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                app.estensione = ext;
                docPrinc.applicazione = app;
                schedaDoc.documenti[0] = docPrinc;
            }
            catch (Exception) { }

            return schedaDoc;
        }

        #region Documenti privati-personale
        public static bool CambiaPersonalePrivato(string idProfile, string idGruppoTrasm)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.InsertSecurityPersonalePrivato(idProfile, idGruppoTrasm);
            return result;
        }
        #endregion

        #region Editing ACL
        public static bool ControllaDirittiUtente(string idProfile, string idPeople, string idGruppo)
        {
            bool result = true;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.ControllaDirittiUtente(idProfile, idPeople, idGruppo);
            return result;
        }

        public static int VerificaDeletedACL(string idObj, DocsPaVO.utente.InfoUtente infoUtente)
        {
            int result = -1;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.VerificaDeletedACL(idObj);
            return result;
        }

        public static int VerificaACL(string tipoObj, string idObj, string idPeople, string idGruppo, out string errorMessage)
        {
            int result = -1;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.VerificaACL(tipoObj, idObj, idPeople, idGruppo, out errorMessage);
            return result;
        }

        public static int VerificaACL(string tipoObj, string idObj, DocsPaVO.utente.InfoUtente infoUtente, out string errorMessage)
        {
            int result = -1;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.VerificaACL(tipoObj, idObj, infoUtente.idPeople, infoUtente.idGruppo, out errorMessage);
            return result;
        }

        public static bool DirittoProprietario(string idObj, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.DirittoProprietario(idObj, infoUtente.idPeople, infoUtente.idGruppo);
            return result;
        }

        public static bool RipristinaACL(out string descrizione, DocsPaVO.documento.DirittoOggetto docDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Interfaces.IDocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                result = documentManager.AddPermission(docDiritto);

                if (result)
                    transactionContext.Complete();
            }

            descrizione = docDiritto.description;

            return result;
        }

        public static bool EditingACL(out string descrizione, DocsPaVO.documento.DirittoOggetto docDiritto, string personOrGroup, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Interfaces.IDocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                result = documentManager.RemovePermission(docDiritto);

                if (result)
                    transactionContext.Complete();
            }

            descrizione = docDiritto.description;

            return result;
        }
        #endregion

        public static bool annullaPredisposizione(DocsPaVO.utente.InfoUtente infoUt, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            bool result = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                if (schedaDocumento != null)
                {
                    result = doc.annullaPredisposizione(infoUt, schedaDocumento);
                }


                if (result)
                    transactionContext.Complete();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile">Id del documento da inoltrare</param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento InoltraSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string idProfile)
        {
            // Creazione scheda documento finale (con utilizzo del repository context, indipendentemente dal fatto che sia disabilito o meno)
            DocsPaVO.documento.SchedaDocumento documento = NewSchedaDocumento(infoUtente, true);
            documento.tipoProto = "P";
            documento.predisponiProtocollazione = true;
            documento.protocollo = new DocsPaVO.documento.ProtocolloUscita();
            documento.protocollo.daProtocollare = "1";

            DocsPaVO.utente.Corrispondente corr = ruolo.uo;
            ((DocsPaVO.documento.ProtocolloUscita)documento.protocollo).mittente = corr;

            SessionRepositoryFileManager repositoryFileManager = SessionRepositoryFileManager.GetFileManager(documento.repositoryContext);

            // Reperimento dettagli doc da inoltrare
            DocsPaVO.documento.SchedaDocumento documentoDaInoltrare = getDettaglio(infoUtente, idProfile, idProfile);

            // Nel documento finale, sarà riportato l'oggetto del documento inoltrato
            documento.oggetto = documentoDaInoltrare.oggetto;

            // Reperimento versione corrente del doc principale
            DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)documentoDaInoltrare.documenti[0];
            versioneCorrente.descrizione = documentoDaInoltrare.oggetto.descrizione;

            int fileSize;
            Int32.TryParse(versioneCorrente.fileSize, out fileSize);

            // Impostazione del documento da inoltrare come allegato al documento finale
            DocsPaVO.documento.Allegato allegatoPrincipale = new DocsPaVO.documento.Allegato
            {
                descrizione = "Inoltro documento principale",
                fileName = versioneCorrente.fileName,
                fileSize = versioneCorrente.fileSize,
                firmato = versioneCorrente.firmato,
                path = versioneCorrente.path,
                repositoryContext = versioneCorrente.repositoryContext,
                subVersion = versioneCorrente.subVersion,
                version = "1",
                versionId = versioneCorrente.versionId,
                versionLabel = DocsPaDB.Query_DocsPAWS.Documenti.FormatCodiceAllegato(documento.allegati.Count + 1),
                numeroPagine = 0,
                dataInserimento = string.Empty
            };

            if (fileSize > 0 && !string.IsNullOrEmpty(versioneCorrente.fileName))
            {
                // Reperimento del file associato alla versione corrente
                DocsPaVO.documento.FileDocumento file = FileManager.getFileFirmato(versioneCorrente, infoUtente, false);

                // Copia del file nel repository temporaneo
                repositoryFileManager.SetFile(allegatoPrincipale, file);
            }

            allegatoPrincipale.repositoryContext = documento.repositoryContext;
            documento.allegati.Add(allegatoPrincipale);

            // Reperimento allegati del documento (allegati ricevuti per interoperabilità non devono essere inseriti)
            DocsPaDB.Query_DocsPAWS.Documenti docWs = new DocsPaDB.Query_DocsPAWS.Documenti();
            foreach (DocsPaVO.documento.Allegato allegato in
                ((Allegato[])documentoDaInoltrare.allegati.ToArray(typeof(Allegato))).Where(
                a => AllegatiManager.getIsAllegatoIS(a.versionId) != "1" &&
                    AllegatiManager.getIsAllegatoPEC(a.versionId) != "1" &&
                   docWs.GetTipologiaAllegato(a.versionId)!="D"
                    ))
            {
                allegato.descrizione = string.Format("Inoltro {0}", allegato.descrizione);
                allegato.dataInserimento = string.Empty;

                if (allegato.descrizione.Length > 2000)
                    allegato.descrizione = string.Format("{0}...", allegato.descrizione.Substring(0, 1900));

                // Reperimento versionlabel per l'allegato
                allegato.versionLabel = DocsPaDB.Query_DocsPAWS.Documenti.FormatCodiceAllegato(documento.allegati.Count + 1);

                Int32.TryParse(allegato.fileSize, out fileSize);

                if (fileSize > 0 && !string.IsNullOrEmpty(allegato.fileName))
                {
                    // Reperimento del file associato alla versione corrente
                    DocsPaVO.documento.FileDocumento file = FileManager.getFileFirmato(allegato, infoUtente, false);

                    // Copia del file nel repository temporaneo
                    repositoryFileManager.SetFile(allegato, file);
                }

                allegato.repositoryContext = documento.repositoryContext;
                //Mev Gestione eccezioni - nell'inoltra il segnatura.xml non deve essere preso tra gli allegati
                if (!string.IsNullOrEmpty(documentoDaInoltrare.interop) &&
                    (documentoDaInoltrare.interop.Equals("S") || documentoDaInoltrare.interop.Equals("E")) &&
                    allegato.descrizione.ToLower().Contains("segnatura.xml"))
                    continue;
                //End Mev Gestione Eccezioni
                documento.allegati.Add(allegato);
            }

            return documento;
        }

        /// <summary>
        /// Creazione di una nuova scheda documento già predisposta con i metadati
        /// di un documento già esistente nel sistema documentale
        /// </summary>
        /// <param name="infoUtente">
        /// Contesto utente
        /// </param>
        /// <param name="idProfile">
        /// Id del documento cui reperire i metadati
        /// </param>
        /// <param name="inoltra">
        /// booleano per definire se il metodo è stato chiamato dal tasto inoltra documento
        /// oppure dal tasto riproponi avanzato.
        /// </param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento CloneSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            // Reperimento dei metadati relativi al documento già esistente nel sistema 
            DocsPaVO.documento.SchedaDocumento template = getDettaglio(infoUtente, idProfile, idProfile);

            DocsPaVO.documento.SchedaDocumento newDocumento = null;
            DocsPaVO.documento.SchedaDocumento newDocumentoInoltra = new DocsPaVO.documento.SchedaDocumento();

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                // Serializzazione scheda documento da copiare
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, template);
                stream.Position = 0;

                // Deserializzazione scheda documento in una nuova istanza
                newDocumento = (DocsPaVO.documento.SchedaDocumento)formatter.Deserialize(stream);
            }

            // Creazione di un nuovo contesto per gestire il repository temporaneo per il documento
            DocsPaVO.documento.SessionRepositoryContext repositoryContext = BusinessLogic.Documenti.SessionRepositoryFileManager.NewRepository(infoUtente, true);

            SessionRepositoryFileManager repositoryFileManager = SessionRepositoryFileManager.GetFileManager(repositoryContext);

            newDocumento.repositoryContext = repositoryContext;

            // Rimozione dei dati identificativi del precedente documento dal documento clonato, su cui non vanno riportati
            newDocumento.systemId = null;
            newDocumento.docNumber = null;
            newDocumento.dataCreazione = null;
            newDocumento.oraCreazione = null;
            newDocumento.checkOutStatus = null;
            newDocumento.accessRights = null;
            newDocumento.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
            newDocumento.rispostaDocumento = null;
            newDocumento.riferimentoMittente = null;
            newDocumento.checkOutStatus = null;
            newDocumento.autore = null;
            newDocumento.creatoreDocumento = null;
            newDocumento.protocollatore = null;

            if (newDocumento.protocollo != null)
            {
                newDocumento.protocollo.numero = string.Empty;
                newDocumento.protocollo.segnatura = string.Empty;
                newDocumento.protocollo.anno = string.Empty;
                newDocumento.protocollo.dataProtocollazione = string.Empty;
                newDocumento.protocollo.daProtocollare = "1";
                newDocumento.protocollo.protocolloAnnullato = null;
            }

            // La serializzazione comporta la copia di tutte le versioni del documento principale,
            // la funzione di Clone richiede solamente la copia della versione corrente
            if (newDocumento.documenti.Count > 0)
            {
                newDocumento.documenti = new ArrayList { newDocumento.documenti[0] };
                //Giordano Iacozzilli 02/04/2013:
                //L'errore si prensentava nel momento in cui si creava una nuova versione 
                //di un documento già protocollato, a quella versione si associava 
                //un file la si riproponeva e poi la si protocollava.
                //Soluzione:
                //Imposto la versione a 1 nel codice:
                ((DocsPaVO.documento.FileRequest)(newDocumento.documenti[0])).version = "1";
                ((DocsPaVO.documento.FileRequest)(newDocumento.documenti[0])).versionLabel = "1";
                //FINE
            }

            // Impostazione dell'oggetto repository context per tutte le versioni del documento 
            foreach (DocsPaVO.documento.FileRequest item in newDocumento.documenti)
            {
                int fileSize;
                Int32.TryParse(item.fileSize, out fileSize);

                if (fileSize > 0 && !string.IsNullOrEmpty(item.fileName))
                {
                    // Reperimento del file associato alla versione corrente
                    DocsPaVO.documento.FileDocumento file = FileManager.getFile(item, infoUtente);

                    // Copia del file nel repository temporaneo
                    repositoryFileManager.SetFile(item, file);
                }

                item.dataInserimento = string.Empty;
                item.repositoryContext = repositoryContext;
            }

            // Impostazione dell'oggetto repository context per tutti gli allegati del documento 
            foreach (DocsPaVO.documento.FileRequest item in newDocumento.allegati)
            {
                int fileSize;
                Int32.TryParse(item.fileSize, out fileSize);

                if (fileSize > 0 && !string.IsNullOrEmpty(item.fileName))
                {
                    // Reperimento del file associato alla versione corrente
                    DocsPaVO.documento.FileDocumento file = FileManager.getFile(item, infoUtente);

                    // Copia del file nel repository temporaneo
                    repositoryFileManager.SetFile(item, file);
                }

                item.dataInserimento = string.Empty;
                item.repositoryContext = repositoryContext;
            }

            return newDocumento;
        }

        /// <summary>
        ///  Factory method di una nuova scheda documento predisposta 
        ///  per l'utilizzo del session repository context
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="useSessionRepositoryContext">
        /// Se true, indica che la scheda documento viene predisposta per l'utilizzo del SessionRepositoryContext
        /// </param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento NewSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente, bool useSessionRepositoryContext)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();

            schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
            schedaDocumento.systemId = null;
            schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
            schedaDocumento.idPeople = infoUtente.idPeople;
            schedaDocumento.userId = infoUtente.userId;

            string docType = System.Configuration.ConfigurationManager.AppSettings["DOC_TYPE"];
            if (string.IsNullOrEmpty(docType))
                docType = "LETTERA"; //default
            schedaDocumento.typeId = docType;
            schedaDocumento.appId = "ACROBAT";
            schedaDocumento.privato = "0";  // doc non privato

            // Creazione di un nuovo contesto per gestire il repository temporaneo per il documento
            schedaDocumento.repositoryContext = BusinessLogic.Documenti.SessionRepositoryFileManager.NewRepository(infoUtente, useSessionRepositoryContext);

            // Creazione di un filerequest relativo alla prima versione del documento
            DocsPaVO.documento.Documento documento = new DocsPaVO.documento.Documento();
            documento.repositoryContext = schedaDocumento.repositoryContext;
            documento.applicazione = null;

            documento.autore = infoUtente.userId;
            documento.cartaceo = false;
            documento.daAggiornareFirmatari = false;
            documento.dataInserimento = DateTime.Now.ToString("dd/MM/yyyy");
            documento.descrizione = string.Empty;
            documento.docNumber = string.Empty;
            documento.docServerLoc = string.Empty;
            documento.fileName = string.Empty;
            documento.fileSize = "0";
            documento.fNversionId = string.Empty;
            documento.idPeople = infoUtente.idPeople;
            documento.msgErr = string.Empty;
            documento.path = string.Empty;
            documento.subVersion = "!";
            documento.version = "1";
            documento.versionId = string.Empty;
            documento.versionLabel = "1";
            documento.daInviare = "1";
            documento.dataArchiviazione = null;
            documento.dataArrivo = string.Empty;
            schedaDocumento.documenti = new ArrayList() { documento };

            schedaDocumento.allegati = new ArrayList();

            return schedaDocumento;
        }

        /// <summary>
        ///  Factory method di una nuova scheda documento predisposta 
        ///  senza l'utilizzo del session repository context
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento NewSchedaDocumento(DocsPaVO.utente.InfoUtente infoUtente)
        {
            return NewSchedaDocumento(infoUtente, false);
        }

        ///// <summary>
        ///// Factory method di una nuova scheda per un documento grigio
        ///// </summary>
        ///// <param name="infoUtente"></param>
        ///// <returns>
        ///// Scheda documento predisposta per l'inserimento
        ///// </returns>
        //public static DocsPaVO.documento.SchedaDocumento NewDocumentoGrigio(DocsPaVO.utente.InfoUtente infoUtente)
        //{
        //    DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();

        //    schedaDocumento = new DocsPaVO.documento.SchedaDocumento();
        //    schedaDocumento.systemId = null;
        //    schedaDocumento.tipoProto = "G";
        //    schedaDocumento.oggetto = new DocsPaVO.documento.Oggetto();
        //    schedaDocumento.idPeople = infoUtente.idPeople;
        //    schedaDocumento.userId = infoUtente.userId;

        //    // Creazione di un nuovo contesto per gestire il repository temporaneo per il documento
        //    DocsPaVO.documento.SessionRepositoryContext sessionRepositoryContext = new DocsPaVO.documento.SessionRepositoryContext(infoUtente);
        //    sessionRepositoryContext.IsDocumentoGrigio = true;

        //    string docType = System.Configuration.ConfigurationManager.AppSettings["DOC_TYPE"];
        //    if (string.IsNullOrEmpty(docType))
        //        docType = "LETTERA"; //default
        //    schedaDocumento.typeId = docType;
        //    schedaDocumento.appId = "ACROBAT";
        //    schedaDocumento.privato = "0";  // doc non privato
        //    schedaDocumento.repositoryContext = sessionRepositoryContext;

        //    // Creazione di un filerequest relativo alla prima versione del documento
        //    DocsPaVO.documento.Documento documento = new DocsPaVO.documento.Documento();
        //    documento.repositoryContext = sessionRepositoryContext;
        //    documento.applicazione = null;
        //    documento.autore = infoUtente.userId;
        //    documento.cartaceo = false;
        //    documento.daAggiornareFirmatari = false;
        //    documento.dataInserimento = DateTime.Now.ToString("dd/MM/yyyy");
        //    documento.descrizione = string.Empty;
        //    documento.docNumber = string.Empty;
        //    documento.docServerLoc = string.Empty;
        //    documento.fileName = string.Empty;
        //    documento.fileSize = "0";
        //    documento.fNversionId = string.Empty;
        //    documento.idPeople = infoUtente.idPeople;
        //    documento.msgErr = string.Empty;
        //    documento.path = string.Empty;
        //    documento.subVersion = "!";
        //    documento.version = "1";
        //    documento.versionId = string.Empty;
        //    documento.versionLabel = "1";
        //    documento.daInviare = "1";
        //    documento.dataArchiviazione = null;
        //    documento.dataArrivo = string.Empty;
        //    schedaDocumento.documenti = new ArrayList() { documento };

        //    schedaDocumento.allegati = new ArrayList();

        //    return schedaDocumento;
        //}

        /// <summary>
        /// Reperimento scheda documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento getDettaglio(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
        {
            logger.Info("BEGIN");
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc = doc.GetDettaglio(infoUtente, idProfile, docNumber, true);

            if (schedaDoc == null)
            {
                throw new Exception();
            }
            else
            {
                // Reperimento informazioni se il documento è in stato checkout,
                // solo per i documenti non di tipo stampa registro
                schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
            }

            try
            {
                DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
                string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                app.estensione = ext;
                docPrinc.applicazione = app;
                schedaDoc.documenti[0] = docPrinc;
            }
            catch (Exception)
            {
            }
            logger.Info("END");
            return schedaDoc;
        }

        /// <summary>
        /// set datavista per documento "D" o fasc "F"
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="systemId"></param>
        /// <param name="DocOrFasc"></param>
        /// <returns></returns>
        /// <summary>
        /// set datavista per documento "D" o fasc "F"
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="systemId"></param>
        /// <param name="DocOrFasc"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static bool setDataVistaSP_TV(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, string DocOrFasc, String idRegistro)
        {
            bool rtn = false;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            string systemId = string.Empty;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    if (DocOrFasc == "D")
                    {
                        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                        {

                            string commandText = "SELECT SYSTEM_ID FROM PROFILE WHERE DOCNUMBER=" + docNumber;
                            logger.Debug(commandText);

                            using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                            {
                                if (reader.Read())
                                {
                                    systemId = reader.GetValue(0).ToString();
                                }
                            }
                        }
                    }
                    else if (DocOrFasc == "F")
                    {
                        systemId = docNumber;
                    }


                    if (!string.IsNullOrEmpty(systemId))
                        rtn = doc.SetDataVistaSP_TASTOVISTO(infoUtente, systemId, DocOrFasc);

                    transactionContext.Complete();

                    // Se il documento è stato ricevuto per IS, bisogna selezionare gli RF del ruolo che sono
                    // destinatari della spedizione e per questi inviare una conferma di ricezione al mittente
                    if (DocOrFasc == "D" && InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(docNumber) && !String.IsNullOrEmpty(doc.GetDocumentSignatureByProfileId(systemId)))
                    {
                        SimplifiedInteroperabilityProtoManager.SendDocumentReceivedProofToSender(
                            docNumber,
                            infoUtente,
                            idRegistro);
                    }

                    return rtn;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore SetDataVistaSP_TV", e);
                    return false;
                }
            }

        }






        /// <summary>
        /// set datavista per documento "D" o fasc "F"
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="systemId"></param>
        /// <param name="DocOrFasc"></param>
        /// <returns></returns>
        public static bool setDataVistaSP(DocsPaVO.utente.InfoUtente infoUtente, string docNumber, string DocOrFasc)
        {
            bool rtn = false;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            string systemId = string.Empty;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {

                        string commandText = "SELECT SYSTEM_ID FROM PROFILE WHERE DOCNUMBER=" + docNumber;
                        logger.Debug(commandText);

                        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                        {
                            if (reader.Read())
                            {
                                systemId = reader.GetValue(0).ToString();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(systemId))
                        rtn = doc.SetDataVistaSP(infoUtente, systemId, DocOrFasc);

                    transactionContext.Complete();

                    return rtn;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore SetDataVistaSP", e);
                    return false;
                }
            }

        }

        /// <summary>
        /// ritorna una schedadocumento ma non viene fatto set datavista, altrimenti in interoperabilità non si ha todolist
        /// </summary>
        /// 
        public static DocsPaVO.documento.SchedaDocumento getDettaglioPerNotificaAllegati(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc = doc.GetDettaglio(infoUtente, idProfile, docNumber, false);

            if (schedaDoc == null)
            {
                throw new Exception();
            }

            return schedaDoc;
        }


        /// <summary>
        /// ritorna una schedadocumento ma non viene fatto set datavista, altrimenti in interoperabilità non si ha todolist, e senza i diritti altrimenti non può inviare la mail in caso di cedi diritti
        /// </summary>
        /// 
        public static DocsPaVO.documento.SchedaDocumento getDettaglioPerNotificaAllegatiNoSecurity(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc = doc.GetDettaglioNoSecurity(infoUtente, idProfile, docNumber);

            if (schedaDoc == null)
            {
                throw new Exception();
            }

            return schedaDoc;
        }
        #region Metodi Commentati
        /*public static DocsPaVO.documento.SchedaDocumento getSchedaDocumento(DocsPaWS.Utils.Database db, DataRow dataRow) {
			// inizio creazione SchedaDoc
			DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
			return getProfilo(db, dataRow, schedaDoc);
		}*/


        /*public static DocsPaVO.documento.SchedaDocumento getProfilo(DocsPaWS.Utils.Database db, DataRow dataRow, DocsPaVO.documento.SchedaDocumento schedaDoc) {
            schedaDoc.systemId = dataRow["SYSTEM_ID"].ToString();
            schedaDoc.dataCreazione =  dataRow["CREATION_DATE"].ToString();
            schedaDoc.note = dataRow["VAR_NOTE"].ToString();
            schedaDoc.docNumber =  dataRow["DOCNUMBER"].ToString();
            schedaDoc.accessRights = dataRow["ACCESSRIGHTS"].ToString();
            schedaDoc.tipoProto = dataRow["CHA_TIPO_PROTO"].ToString(); 
            schedaDoc.modOggetto = dataRow["CHA_MOD_OGGETTO"].ToString();			
            schedaDoc.assegnato = dataRow["CHA_ASSEGNATO"].ToString();
            schedaDoc.fascicolato = dataRow["CHA_FASCICOLATO"].ToString();
            schedaDoc.privato = dataRow["CHA_PRIVATO"].ToString();
            schedaDoc.evidenza = dataRow["CHA_EVIDENZA"].ToString();

            // Registro
            schedaDoc.registro = getRegistro(dataRow["ID_REGISTRO"].ToString(), db);

            // Oggetto
            schedaDoc.oggetto = getOggetto(dataRow);	
				
            // Protocollo
            string tipoDoc = dataRow["CHA_TIPO_PROTO"].ToString();
            if(tipoDoc.Equals("A") || tipoDoc.Equals("P")) {
                if (tipoDoc.Equals("A"))
                    schedaDoc.protocollo = getProtocolloArrivo(db, dataRow);
                else if (tipoDoc.Equals("P"))
                    schedaDoc.protocollo = getProtocolloUscita(db, dataRow);
                schedaDoc.protocollo = getDatiProtocollo(db, dataRow, schedaDoc.protocollo);
            }								
				
            // Documenti
            schedaDoc.documenti = getDocumenti(db, dataRow);
				
            // Allegati
            schedaDoc.allegati = getAllegati(db, dataRow);

            // TipologiaAtto
            schedaDoc.tipologiaAtto = getTipologiaAtto(db,dataRow);

            // ParoleChiave
            schedaDoc.paroleChiave = getParoleChiave(db,dataRow);

            return schedaDoc;
        }*/


        /*private static DocsPaVO.utente.Registro getRegistro (string idRegistro, DocsPaWS.Utils.Database db) 
        {
            logger.Debug("getRegistro");
            if (!(idRegistro != null && !idRegistro.Equals("")))
                return null;
            DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
            string queryString = 
                "SELECT SYSTEM_ID, VAR_CODICE, CHA_STATO,  ID_AMM, VAR_DESC_REGISTRO, VAR_EMAIL_REGISTRO, " +
                DocsPaWS.Utils.dbControl.toChar("DTA_OPEN",false) + " AS DTA_OPEN, " +
                DocsPaWS.Utils.dbControl.toChar("DTA_CLOSE",false) + " AS DTA_CLOSE, " +
                DocsPaWS.Utils.dbControl.toChar("DTA_ULTIMO_PROTO",false) + " AS DTA_ULTIMO_PROTO " +
                "FROM DPA_EL_REGISTRI WHERE SYSTEM_ID=" + idRegistro;
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
            if (dr.Read()) {
                reg.systemId = dr.GetValue(0).ToString();
                reg.codRegistro = dr.GetValue(1).ToString();
                reg.stato = dr.GetValue(2).ToString();
                reg.idAmministrazione = dr.GetValue(3).ToString();
                reg.descrizione = dr.GetValue(4).ToString();
                reg.email = dr.GetValue(5).ToString();
                reg.dataApertura = dr.GetValue(6).ToString().Trim();
                reg.dataChiusura = dr.GetValue(7).ToString().Trim();
                reg.dataUltimoProtocollo = dr.GetValue(8).ToString();
            } 
            dr.Close();
            return reg;
        }*/


        /*private static DocsPaVO.documento.Oggetto getOggetto(DataRow dataRow) {
            logger.Debug("getOggetto");
            DocsPaVO.documento.Oggetto oggetto = new DocsPaVO.documento.Oggetto();				
            oggetto.systemId = dataRow["ID_OGGETTO"].ToString();
            oggetto.descrizione = dataRow["VAR_DESC_OGGETTO"].ToString();
            return oggetto;
        }*/


        /*private static DocsPaVO.documento.TipologiaAtto getTipologiaAtto(DocsPaWS.Utils.Database db, DataRow dataRow) {
            DocsPaVO.documento.TipologiaAtto tipoAtto = null;
            string idTipoAtto = dataRow["ID_TIPO_ATTO"].ToString();
            if(!(idTipoAtto != null && !idTipoAtto.Equals("")))
            {
                return null;
            }
            string queryString =
                "SELECT SYSTEM_ID, VAR_DESC_ATTO FROM DPA_TIPO_ATTO " +
                "WHERE SYSTEM_ID =" + idTipoAtto;
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
            if (dr.Read()) 
            {
                tipoAtto = new DocsPaVO.documento.TipologiaAtto();
                tipoAtto.systemId = dr.GetValue(0).ToString();
                tipoAtto.descrizione = dr.GetValue(1).ToString();
            }
            dr.Close();

            return tipoAtto;
        }*/


        /*internal static ArrayList getDocumenti(DocsPaWS.Utils.Database db, DataRow dataRow) {			
            logger.Debug("getDocumenti");
            ArrayList listaDocumenti = new ArrayList();
			
            string queryString = getFileVersionsQuery("D", dataRow["DOCNUMBER"].ToString() );
					
            DataSet dataSet = new DataSet();
            db.fillTable(queryString, dataSet, "VERSIONI");
            foreach(DataRow versionRow in dataSet.Tables["VERSIONI"].Rows) {
                DocsPaVO.documento.Documento documento = new DocsPaVO.documento.Documento();
                documento = (DocsPaVO.documento.Documento)getFileVersionDetails((DocsPaVO.documento.FileRequest)documento,versionRow);
                documento.docServerLoc = dataRow["DOCSERVER_LOC"].ToString();
                documento.path = dataRow["PATH"].ToString();
                documento.dataArrivo = versionRow["DTA_ARRIVO"].ToString();
                if (versionRow["CHA_DA_INVIARE"] != null)
                    documento.daInviare = versionRow["CHA_DA_INVIARE"].ToString();
                documento.firmatari = getFirmatari(db, documento.versionId);
                documento.tipologia = getTipologiaDocumento(dataRow);
                listaDocumenti.Add(documento);
            }
			
            dataSet.Dispose();
				
            return listaDocumenti;
        }	*/


        /*private static DocsPaVO.documento.TipologiaCanale getTipologiaDocumento(DataRow dataRow) {
            DocsPaVO.documento.TipologiaCanale tipoDoc = new DocsPaVO.documento.TipologiaCanale();
            tipoDoc.codice = dataRow["TYPE_ID"].ToString();
            tipoDoc.codice = dataRow["DESCRIPTION"].ToString();
            return tipoDoc;
        }*/


        /*private static ArrayList getAllegati(DocsPaWS.Utils.Database db, DataRow dataRow) {
            logger.Debug("getAllegati");
            ArrayList listaAllegati = new ArrayList();
			
            string queryString = getFileVersionsQuery("A", dataRow["DOCNUMBER"].ToString() );
					
            DataSet dataSet = new DataSet();
            db.fillTable(queryString, dataSet, "VERSIONI");
            foreach(DataRow versionRow in dataSet.Tables["VERSIONI"].Rows) 
            {
                DocsPaVO.documento.Allegato allegato = new DocsPaVO.documento.Allegato();
                allegato = (DocsPaVO.documento.Allegato) getFileVersionDetails((DocsPaVO.documento.FileRequest)allegato, versionRow);
                allegato.docServerLoc = dataRow["DOCSERVER_LOC"].ToString();
                allegato.path = dataRow["PATH"].ToString();			
                allegato.firmatari = getFirmatari(db, allegato.versionId);				
                allegato.numeroPagine = 0;
                try {
                    if(versionRow["NUM_PAG_ALLEGATI"] != null)
                        allegato.numeroPagine = Int32.Parse(versionRow["NUM_PAG_ALLEGATI"].ToString());	
                } catch (Exception e) {
                    logger.Debug("NUM_PAG_ALLEGATI = " + versionRow["NUM_PAG_ALLEGATI"].ToString() + " - " + e.Message);
                }
                listaAllegati.Add(allegato);
            }
			
            dataSet.Dispose();
				
            return listaAllegati;
        }*/


        // tipo = D:documento, A:allegato
        /*private static string getFileVersionsQuery (string tipo, string docNumber) 
        {
            logger.Debug("getFileVersionsQuery");
            string queryString =
                "SELECT " +
                "VERSIONS.VERSION_ID, COMPONENTS.DOCNUMBER, VERSIONS.VERSION, " +
                "VERSIONS.SUBVERSION, VERSIONS.VERSION_LABEL, VERSIONS.AUTHOR, VERSIONS.COMMENTS, " +
                "COMPONENTS.PATH, COMPONENTS.FILE_SIZE, VERSIONS.NUM_PAG_ALLEGATI, VERSIONS.CHA_DA_INVIARE, "+DocsPaWS.Utils.dbControl.toChar("VERSIONS.DTA_ARRIVO",false)+" AS DTA_ARRIVO " +
                "FROM COMPONENTS , VERSIONS " + 
                "WHERE COMPONENTS.VERSION_ID = VERSIONS.VERSION_ID " +
                "AND COMPONENTS.DOCNUMBER = VERSIONS.DOCNUMBER " +
                "AND COMPONENTS.DOCNUMBER=" + docNumber;

            if(tipo.Equals("D")) {
                queryString += " AND VERSIONS.VERSION > 0";
                queryString += " ORDER BY VERSIONS.VERSION_ID DESC";
            } else {
                queryString += " AND VERSIONS.VERSION = 0";
                queryString += " ORDER BY VERSIONS.VERSION_ID";
            }
			
        }*/


        /*private static DocsPaVO.documento.FileRequest getFileVersionDetails(DocsPaVO.documento.FileRequest fileRequest, DataRow dataRow) {
            //DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.FileRequest();
			
            fileRequest.docNumber = dataRow["DOCNUMBER"].ToString();
            fileRequest.versionId = dataRow["VERSION_ID"].ToString();
            fileRequest.version = dataRow["VERSION"].ToString();
            fileRequest.subVersion = dataRow["SUBVERSION"].ToString();
            fileRequest.versionLabel = dataRow["VERSION_LABEL"].ToString();
            fileRequest.idPeople = dataRow["AUTHOR"].ToString();
            fileRequest.descrizione = dataRow["COMMENTS"].ToString();
            fileRequest.fileName = dataRow["PATH"].ToString();
            fileRequest.fileSize = dataRow["FILE_SIZE"].ToString();	

            return fileRequest;
        }*/


        /*private static ArrayList getFirmatari(DocsPaWS.Utils.Database db, string versionId) 
        {
            logger.Debug("getFirmatari");
            ArrayList listaFirmatari = new ArrayList();
            string queryString =
                "SELECT  DPA_FIRMA_VERS.ID_FIRMATARIO, DPA_FIRMATARI.VAR_NOME, " +
                "DPA_FIRMATARI.VAR_COGNOME, DPA_FIRMA_VERS.ID_VERSIONE " +
                "FROM DPA_FIRMA_VERS, DPA_FIRMATARI " +
                "WHERE DPA_FIRMA_VERS.ID_FIRMATARIO =  DPA_FIRMATARI.SYSTEM_ID " +
                "AND DPA_FIRMA_VERS.ID_VERSIONE=" + versionId;
            logger.Debug(queryString);
            IDataReader dr = db.executeReader(queryString);
            while(dr.Read()) {
                DocsPaVO.documento.Firmatario firmatario = new DocsPaVO.documento.Firmatario();
                firmatario.systemId = dr.GetValue(0).ToString();
                firmatario.nome = dr.GetValue(1).ToString();
                firmatario.cognome = dr.GetValue(2).ToString();
                listaFirmatari.Add(firmatario);
            }
            dr.Close();
            return listaFirmatari;
        }*/


        /*private static ArrayList getParoleChiave(DocsPaWS.Utils.Database db, DataRow dataRow) 
        {
            logger.Debug("getParoleChiave");
            ArrayList listaParoleChiave = new ArrayList();
			
            string queryString = 
                "SELECT DPA_PROF_PAROLE.ID_PAROLA, DPA_PAROLE.VAR_DESC_PAROLA, " +
                "DPA_PAROLE.ID_AMM, DPA_PROF_PAROLE.ID_PROFILE " +
                "FROM DPA_PROF_PAROLE, DPA_PAROLE " +
                "WHERE DPA_PROF_PAROLE.ID_PAROLA = DPA_PAROLE.SYSTEM_ID " +
                "AND DPA_PROF_PAROLE.ID_PROFILE=" + dataRow["SYSTEM_ID"].ToString();
            logger.Debug(queryString);		
            IDataReader dr = db.executeReader(queryString);
            while(dr.Read()) {
                DocsPaVO.documento.ParolaChiave pc = new DocsPaVO.documento.ParolaChiave();
                pc.systemId = dr.GetValue(0).ToString();
                pc.descrizione = dr.GetValue(1).ToString();
                pc.idAmministrazione = dr.GetValue(2).ToString();
                listaParoleChiave.Add(pc);
            }
			
            dr.Close();
				
            return listaParoleChiave;
        }*/


        /*private static DocsPaVO.documento.Protocollo getProtocolloArrivo (DocsPaWS.Utils.Database db, DataRow dataRow) {
            logger.Debug("getProtocolloArrivo");
            DocsPaVO.documento.ProtocolloEntrata protocollo = new DocsPaVO.documento.ProtocolloEntrata();
            if(dataRow["DTA_PROTO_IN"] != null)
                protocollo.dataProtocolloMittente = dataRow["DTA_PROTO_IN"].ToString().Trim();
            if(dataRow["VAR_PROTO_IN"] != null)
                protocollo.descrizioneProtocolloMittente = dataRow["VAR_PROTO_IN"].ToString();

            protocollo = getCorrispondentiEntrata(protocollo, dataRow["SYSTEM_ID"].ToString(), db);
            return protocollo;
        }*/
        #endregion

        public static string getQueryCorrispondente(string idProfile)
        {
            #region Codice Commentato
            /*string queryString = 
				"SELECT A.SYSTEM_ID, A.VAR_COD_RUBRICA, A.ID_AMM, " +
				"A.CHA_TIPO_IE, A.CHA_TIPO_CORR, B.CHA_TIPO_MITT_DEST, A.VAR_DESC_CORR " +
				"FROM DPA_CORR_GLOBALI A, DPA_DOC_ARRIVO_PAR B  " +
				"WHERE A.SYSTEM_ID=B.ID_MITT_DEST AND B.ID_PROFILE=" + idProfile;*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            string queryString = doc.GetQueryCorrispondente(idProfile);
            return queryString;
        }

        #region Metodi Commentati
        /*private static DocsPaVO.utente.Corrispondente getCorrispondente_Prot(string idProfile, IDataReader dr) 
		{
			DocsPaVO.utente.Corrispondente cor = getCorrispondente(dr);
			if (cor != null)
				return ProtocolloUscitaManager.getDatiSpedizione(cor, idProfile);
			return cor;
		}*/


        /*public static DocsPaVO.utente.Corrispondente getCorrispondente(IDataReader dr) {
            logger.Debug("getCorrispondente");
            DocsPaVO.utente.Corrispondente corrispondente = null;
            if(dr.GetValue(3) != null) 
            {
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.codiceRubrica = dr.GetValue(1).ToString();;
                qco.idAmministrazione = dr.GetValue(2).ToString();
                qco.getChildren = false;
                qco.fineValidita = false;
                logger.Debug("getCorrispondente");
                if (dr.GetValue(3).ToString().Equals("I"))
                {
                    corrispondente = (DocsPaVO.utente.Corrispondente)addressBookManager.listaCorrispondentiIntMethod(qco)[0];
                }
                else if (dr.GetValue(3).ToString().Equals("E"))
                {
                    corrispondente = (DocsPaVO.utente.Corrispondente)addressBookManager.listaCorrEstSciolti(qco)[0];
                }
            }
            if(corrispondente == null) 
            {
                corrispondente = new DocsPaVO.utente.Corrispondente();
                corrispondente.systemId = dr.GetValue(0).ToString();
                corrispondente.codiceRubrica = dr.GetValue(1).ToString();
                corrispondente.idAmministrazione =  dr.GetValue(2).ToString();
                corrispondente.descrizione =  dr.GetValue(6).ToString();
            }
            corrispondente.tipoCorrispondente = dr.GetValue(4).ToString();
            logger.Debug("fine getCorrispondente");
            return corrispondente;
        }*/


        /*private static DocsPaVO.documento.ProtocolloEntrata getCorrispondentiEntrata(DocsPaVO.documento.ProtocolloEntrata protocolloEntrata, string idProfile, DocsPaWS.Utils.Database db) {
            logger.Debug("getCorrispondentiEntrata");
            string queryString = getQueryCorrispondente(idProfile);
            logger.Debug (queryString);
            IDataReader dr = db.executeReader(queryString);
            DocsPaWS.Utils.Logger.log("Dopo query DocManager.getCorrispondentiEntrata", logLevelTime);
            while (dr.Read()) {		
                // Verifico se si tratta di un mittente o di un mittente intermedio
                if (dr.GetValue(5).ToString().Equals("M")) 
                    protocolloEntrata.mittente = getCorrispondente(dr);
                else if (dr.GetValue(5).ToString().Equals("I")) 
                    protocolloEntrata.mittenteIntermedio = getCorrispondente(dr);
				
            }
            dr.Close();
            return protocolloEntrata;
        }	*/


        /*private static DocsPaVO.documento.Protocollo getProtocolloUscita (DocsPaWS.Utils.Database db, DataRow dataRow) {
            logger.Debug("getProtocolloUscita");
            DocsPaVO.documento.ProtocolloUscita protocollo = new DocsPaVO.documento.ProtocolloUscita();
			
            protocollo = getCorrispondentiUscita(protocollo, dataRow["SYSTEM_ID"].ToString(), db);
			
            return protocollo;
        }*/


        /*private static DocsPaVO.documento.ProtocolloUscita getCorrispondentiUscita(DocsPaVO.documento.ProtocolloUscita protocolloUscita, string idProfile, DocsPaWS.Utils.Database db) {
            ArrayList destinatari = new ArrayList();
            ArrayList destinatariConoscenza = new ArrayList();
            string queryString = getQueryCorrispondente(idProfile);
            logger.Debug (queryString);
            IDataReader dr = db.executeReader(queryString);
            DocsPaWS.Utils.Logger.log("Dopo query DocManager.getCorrispondentiUscita", logLevelTime);
            while (dr.Read()) {	
                // Verifico se si tratta di un destinatario o di un destinatario per conoscenza
                if (dr.GetValue(5).ToString().Equals("D")) 
                    destinatari.Add(getCorrispondente_Prot(idProfile, dr));
                else if (dr.GetValue(5).ToString().Equals("C")) 
                    destinatariConoscenza.Add(getCorrispondente_Prot(idProfile, dr));
            }
            dr.Close();

            if (destinatari.Count > 0)
                protocolloUscita.destinatari = destinatari;
            if (destinatariConoscenza.Count > 0)
                protocolloUscita.destinatariConoscenza = destinatariConoscenza;

            return protocolloUscita;
        }*/


        /*private static DocsPaVO.documento.Protocollo getDatiProtocollo(DocsPaWS.Utils.Database db, DataRow dataRow,  DocsPaVO.documento.Protocollo protocollo) {
            logger.Debug("getDatiProtocollo");
			
            protocollo.numero = dataRow["NUM_PROTO"].ToString();
            protocollo.segnatura = dataRow["VAR_SEGNATURA"].ToString();
            protocollo.dataProtocollazione = dataRow["DTA_PROTO"].ToString().Trim();
            protocollo.anno = dataRow["NUM_ANNO_PROTO"].ToString();
            protocollo.daProtocollare = dataRow["CHA_DA_PROTO"].ToString();
            protocollo.invioConferma = dataRow["CHA_INVIO_CONFERMA"].ToString();
            protocollo.modMittDest = dataRow["CHA_MOD_MITT_DEST"].ToString();
            protocollo.modMittInt = dataRow["CHA_MOD_MITT_INT"].ToString();

            protocollo.rispostaProtocollo = getRispostaAlProtocollo(db, dataRow);
            protocollo.datiEmergenza = getDatiEmergenza(db, dataRow);
            protocollo.protocolloAnnullato = getDatiAnnullamento(db, dataRow);
            return protocollo;
        }*/


        /*private static DocsPaVO.documento.InfoDocumento getRispostaAlProtocollo(DocsPaWS.Utils.Database db, DataRow dataRow) {
            logger.Debug("getRispostaAlProtocollo");
            DocsPaVO.documento.InfoDocumento infoDoc = null;
            string idParent = "0";
            if(dataRow["ID_PARENT"] != null)
                idParent = dataRow["ID_PARENT"].ToString();
            if (!idParent.Equals("0")) {
                try {
                    infoDoc = new DocsPaVO.documento.InfoDocumento();
                    infoDoc.idProfile = idParent;
                    string queryString = 
                        "SELECT VAR_SEGNATURA FROM PROFILE WHERE SYSTEM_ID=" + idParent;
                    logger.Debug(queryString);
                    infoDoc.segnatura = db.executeScalar(queryString).ToString();
                    } catch (Exception) {}
            }

            return infoDoc;			
        }*/


        /*private static DocsPaVO.documento.DatiEmergenza getDatiEmergenza (DocsPaWS.Utils.Database db, DataRow dataRow) {
            logger.Debug("getDatiEmergenza");
            DocsPaVO.documento.DatiEmergenza datiEmergenza = null;
            string protoEmergenza = null;
            string dataProtoEmergenza = null;
            string cognomeProtocollatoreEmergenza = null;
            string nomeProtocollatoreEmergenza = null;

            if (dataRow["VAR_PROTO_EME"] != null && !dataRow["VAR_PROTO_EME"].ToString().Equals(""))
                protoEmergenza = dataRow["VAR_PROTO_EME"].ToString();
			
            if (dataRow["DTA_PROTO_EME"] != null && !dataRow["DTA_PROTO_EME"].ToString().Equals(""))
                dataProtoEmergenza = dataRow["DTA_PROTO_EME"].ToString();
			
            if (dataRow["VAR_COGNOME_EME"] != null && !dataRow["VAR_COGNOME_EME"].ToString().Equals(""))
                cognomeProtocollatoreEmergenza = dataRow["VAR_COGNOME_EME"].ToString();
			
            if (dataRow["VAR_NOME_EME"] != null && !dataRow["VAR_NOME_EME"].ToString().Equals("")) 				
                nomeProtocollatoreEmergenza += dataRow["VAR_NOME_EME"].ToString();
			
				
            if (protoEmergenza != null || dataProtoEmergenza != null || cognomeProtocollatoreEmergenza != null || nomeProtocollatoreEmergenza != null) {
                datiEmergenza = new DocsPaVO.documento.DatiEmergenza();
                datiEmergenza.protocolloEmergenza = protoEmergenza;
                datiEmergenza.dataProtocollazioneEmergenza = dataProtoEmergenza;
                datiEmergenza.cognomeProtocollatoreEmergenza = cognomeProtocollatoreEmergenza;
                datiEmergenza.nomeProtocollatoreEmergenza = nomeProtocollatoreEmergenza;
            }
					
            return datiEmergenza;
        }*/


        /*private static DocsPaVO.documento.ProtocolloAnnullato getDatiAnnullamento (DocsPaWS.Utils.Database db, DataRow dataRow) {
            logger.Debug("getDatiAnnullamento");
            DocsPaVO.documento.ProtocolloAnnullato datiAnnullamento = null;
            string autorizzazione = null;
            string dataAnnullamento = null;

            if (dataRow["VAR_AUT_ANNULLA"] != null && !dataRow["VAR_AUT_ANNULLA"].ToString().Equals(""))
                autorizzazione = dataRow["VAR_AUT_ANNULLA"].ToString();
			
            if (dataRow["DTA_ANNULLA"] != null && !dataRow["DTA_ANNULLA"].ToString().Equals(""))
                dataAnnullamento = dataRow["DTA_ANNULLA"].ToString();
			
            if (autorizzazione != null || dataAnnullamento != null) {
                datiAnnullamento = new DocsPaVO.documento.ProtocolloAnnullato();
                datiAnnullamento.autorizzazione = autorizzazione;
                datiAnnullamento.dataAnnullamento = dataAnnullamento;
            }
					
            return datiAnnullamento;
        }*/
        #endregion

        public static ArrayList getTipologiaAtto()
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetTipoAtto();

            #region Codice Commentato
            /*logger.Debug("getTiplogiaAtto");
			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			ArrayList lista = new ArrayList();
			string sqlString = 
				"SELECT SYSTEM_ID, VAR_DESC_ATTO FROM DPA_TIPO_ATTO ORDER BY VAR_DESC_ATTO";
			try {
				db.openConnection();
				logger.Debug(queryString);
				IDataReader dr = db.executeReader(queryString);
				while (dr.Read())
				{
					DocsPaVO.documento.TipologiaAtto tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
					tipologiaAtto.systemId = dr.GetValue(0).ToString();
					tipologiaAtto.descrizione = dr.GetValue(1).ToString();
					lista.Add(tipologiaAtto);
				}

				db.closeConnection();
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}
			return lista;*/
            #endregion
        }

        public static ArrayList getTipologiaAtto(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetTipoAtto(idAmministrazione);
        }

        public static ArrayList ARCHIVE_getTipologiaAtto(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.ARCHIVE_GetTipoAtto(idAmministrazione);
        }

        public static ArrayList getTipoAttoPDInsRic(string idAmministrazione, string idGruppo, string diritti)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.getTipoAttoPDInsRic(idAmministrazione, idGruppo, diritti);
        }

        #region Metodo Commentato
        //		public static ArrayList getTipologiaCanale() 
        //		{
        //			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        //			return doc.GetTipoCanale();
        //			/*logger.Debug("getTiplogiaCanale");
        //			DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
        //			ArrayList lista = new ArrayList();
        //			string sqlString = 
        //				"SELECT SYSTEM_ID, DESCRIPTION, TYPE_ID FROM DOCUMENTTYPES ORDER BY DESCRIPTION";
        //			try 
        //			{
        //				db.openConnection();
        //				IDataReader dr = db.executeReader(queryString);
        //				while (dr.Read()) 
        //				{
        //					DocsPaVO.documento.TipologiaCanale tipologiaCanale = new DocsPaVO.documento.TipologiaCanale();
        //					tipologiaCanale.systemId = dr.GetValue(0).ToString();
        //					tipologiaCanale.descrizione = dr.GetValue(1).ToString();
        //					tipologiaCanale.codice = dr.GetValue(2).ToString();
        //					lista.Add(tipologiaCanale);
        //				}
        //
        //				db.closeConnection();
        //			} 
        //			catch (Exception e) 
        //			{
        //				logger.Debug (e.Message);				
        //				db.closeConnection();
        //				throw new Exception("F_System");
        //			}
        //			return lista;*/
        //		}
        #endregion

        #region Metodo Commentato
        /*private static void setDataVista (DocsPaWS.Utils.Database db, DocsPaVO.documento.InfoDocumento objInfoDocumento, DocsPaVO.utente.InfoUtente objSicurezza) {
			string updateString =
				"UPDATE DPA_TRASM_UTENTE SET CHA_VISTA = '1', DTA_VISTA=" + 
				DocsPaWS.Utils.dbControl.getDate() + " WHERE CHA_VISTA='0' AND ID_PEOPLE=" + objSicurezza.idPeople + 
				" AND ID_TRASM_SINGOLA IN (SELECT A.SYSTEM_ID FROM DPA_TRASM_SINGOLA A, DPA_TRASMISSIONE B " +
				"WHERE A.ID_TRASMISSIONE=B.SYSTEM_ID AND ID_PROFILE=" + objInfoDocumento.idProfile + 
				")" ;
			logger.Debug(updateString);
			db.executeNonQuery(updateString);
		}*/
        #endregion

        #region DOCUMENTI RIMOSSI-CESTINATI

        //Vecchio metodo per la cancellazione fisica di un documento
        public static string ExecRimuoviSchedaMethod(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

            if (documentManager.Remove(new DocsPaVO.documento.InfoDocumento(schedaDoc)))
                return "Del";
            else
                return string.Format("Errore nella cancellazione del documento con docNumber {0}", schedaDoc.docNumber);
        }

        //Rimuove fisicamente tutti i documenti o una lista filtrata di documenti dal cestino
        public static bool SvuotaCestino(out bool docInCestino, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento[] listaDoc)
        {
            docInCestino = (GetListaDocInCestino(infoUtente).Count != listaDoc.Length);

            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                retValue = documentManager.Remove(listaDoc);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        //Rimuove fisicamente il documento selezionato
        public static bool EliminaDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento infoDoc)
        {
            // Verifica stato di consolidamento del documento
            DocumentConsolidation.CanExecuteAction(infoUtente, infoDoc.idProfile, DocumentConsolidation.ConsolidationActionsDeniedEnum.DeleteDocument, true);

            // Verifica se il documento risulta ricevuto per IS ed è già protocollato non si può procedere con l'eliminazione
            if (InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(infoDoc.idProfile) &&
                !String.IsNullOrEmpty(ProtoManager.GetDocumentSignatureByProfileId(infoDoc.idProfile)))
                throw new ApplicationException("Non è possibile rifiutare il documento in quanto risulta essere stato protocollato");


            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                RecordInfo sender = null, receiver = null;
                String senderUrl = String.Empty, receiverCode = String.Empty;
                // Caricamento delle informazioni sul mittente del documento
                if (InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(infoDoc.idProfile))
                    InteroperabilitaSemplificataManager.LoadSenderDocInfo(infoDoc.idProfile, out sender, out receiver, out senderUrl, out receiverCode);

                retValue = documentManager.Remove(infoDoc);

                // Se l'esito è positivo ed il documento è stato ricevuto per IS deve partire una notifica di 
                // eccezione verso il mittente
                if (retValue && sender != null && receiver != null)
                    SimplifiedInteroperabilityRecordDroppedAndExceptionManager.SendDocumentDroppedOrExceptionProofToSender(
                        sender,
                        receiver,
                        "Non di competenza dell’Amministrazione",
                        false,
                        infoUtente.idAmministrazione,
                        senderUrl,
                        receiverCode);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

        //Verifica se l'utente può o meno rimuovere (mettere in cestino) il documento 
        public static string VerificaDiritti(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.VerificaDirittiCestinaDocumento(infoUtente.idPeople, schedaDoc);
        }

        //Pone un documento nel cestino (nello stato in cestino)
        public static bool CestinaDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, string tipoDoc, string note, out string errorMsg)
        {
            bool retValue = false;
            errorMsg = string.Empty;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    // Prima di inserire il documento nel cestino, verifica se risulta in checkout
                    DocsPaDocumentale.Interfaces.ICheckInOutDocumentManager checkInOutManager = new DocsPaDocumentale.Documentale.CheckInOutDocumentManager(infoUtente);
                    DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus = checkInOutManager.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber);

                    bool canDelete = false;

                    if (checkOutStatus != null && checkOutStatus.UserName.ToLowerInvariant().Equals(infoUtente.userId.ToLowerInvariant()))
                        // Undocheckout del documento, solamente se chi l'ha impostato 
                        // in checkout è lo stesso utente che tenta di rimuoverlo
                        canDelete = checkInOutManager.UndoCheckOut(checkOutStatus);
                    else
                        canDelete = true;

                    if (canDelete)
                    {
                        DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                        // Creazione di un oggetto di tipo InfoDocumento
                        DocsPaVO.documento.InfoDocumento infoDocumento = new DocsPaVO.documento.InfoDocumento(schedaDoc);
                        infoDocumento.noteCestino = note;

                        retValue = documentManager.AddDocumentoInCestino(infoDocumento);

                        if (retValue)
                            transactionContext.Complete();
                        else
                            errorMsg = "Non è stato possibile rimuovere il documento. Riprovare più tardi";

                    }
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in Documenti  - metodo: CestinaDocumento", e);
                    errorMsg = "Non è stato possibile rimuovere il documento. Riprovare più tardi";
                    retValue = false;
                }
            }
            return retValue;
        }

        //Restituisce la lista di tutti i documenti in cestino
        public static System.Collections.ArrayList GetListaDocInCestino(DocsPaVO.utente.InfoUtente infoUtente)
        {
            ArrayList listaDoc = new ArrayList();
            //DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            listaDoc = GetListaDocInCestino(infoUtente, null);
            return listaDoc;

        }

        //Restituisce la lista di tutti i documenti in cestino in base al filtro impostato dall'utente
        public static System.Collections.ArrayList GetListaDocInCestino(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca)
        {
            ArrayList listaDoc = new ArrayList();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            listaDoc = doc.GetListaDocInCestino(infoUtente, filtriRicerca);
            return listaDoc;

        }

        //Pone lo stato del documento da in cestino ad attivo
        public static bool RiattivaDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

                retValue = documentManager.RestoreDocumentoDaCestino(infoDocumento);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }
        #endregion

        public static string getDKPath(string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Documentale doc = new DocsPaDB.Query_DocsPAWS.Documentale();
            string root = System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"] + "\\";
            return root + doc.getDKPath(docNumber);

        }

        public static string getIdDK(string collid)
        {
            DocsPaDB.Query_DocsPAWS.Documentale doc = new DocsPaDB.Query_DocsPAWS.Documentale();
            return doc.getIdDK(collid);

        }

        public static bool getSeDocFascicolato(string idDocumento)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = documentiDb.GetSeDocFascicolato(idDocumento);
            return result;
        }

        /// <summary>
        /// Verifica se il Documento con idDocumento è fascicolato nel fascicolo con ID_FASCICOLO = idFascicolo
        /// </summary>
        /// <param name="idDocumento">id del documento che si vuole verificare</param>
        /// <param name="idFascicolo">id fascicolo della project</param>
        /// <returns></returns>
        public static bool GetSeDocFascicolato(string idDocumento, string idFascicolo)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();
            if (!string.IsNullOrEmpty(idDocumento) && !string.IsNullOrEmpty(idFascicolo))
            {
                result = documentiDb.GetSeDocFascicolato(idDocumento, idFascicolo);
            }
            return result;
        }

        public static DocsPaVO.documento.InfoDocumento getInfoDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
            infoDoc.idProfile = schedaDocumento.systemId;
            infoDoc.oggetto = schedaDocumento.oggetto.descrizione;
            infoDoc.docNumber = schedaDocumento.docNumber;
            infoDoc.tipoProto = schedaDocumento.tipoProto;
            infoDoc.evidenza = schedaDocumento.evidenza;

            if (schedaDocumento.registro != null)
            {
                infoDoc.codRegistro = schedaDocumento.registro.codRegistro;
                infoDoc.idRegistro = schedaDocumento.registro.systemId;
            }

            if (schedaDocumento.protocollo != null)
            {
                infoDoc.numProt = schedaDocumento.protocollo.numero;
                infoDoc.daProtocollare = schedaDocumento.protocollo.daProtocollare;
                infoDoc.dataApertura = schedaDocumento.protocollo.dataProtocollazione;
                infoDoc.segnatura = schedaDocumento.protocollo.segnatura;

                if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
                {
                    infoDoc.mittDest = new System.Collections.ArrayList();
                    DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                    infoDoc.mittDest.Add(pe.mittente.descrizione);
                }
                else if (schedaDocumento.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloUscita)))
                {
                    DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                    infoDoc.mittDest = new System.Collections.ArrayList();
                    for (int i = 0; i < pu.destinatari.Count; i++)
                        infoDoc.mittDest.Add(((DocsPaVO.utente.Corrispondente)pu.destinatari[i]).descrizione);
                }

            }
            else
            {
                infoDoc.dataApertura = schedaDocumento.dataCreazione;
            }

            return infoDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <param name="fetchCorrispondenti"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.InfoDocumento GetInfoDocumento(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber, bool fetchCorrispondenti)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documentiDb = new DocsPaDB.Query_DocsPAWS.Documenti();

            return documentiDb.GetInfoDocumento(infoUtente.idGruppo, infoUtente.idPeople, idProfile, fetchCorrispondenti);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.InfoDocumento GetInfoDocumento(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
        {
            return GetInfoDocumento(infoUtente, idProfile, docNumber, false);
        }

        public static bool DocumentoIsAcquisito(string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.Documentale doc = new DocsPaDB.Query_DocsPAWS.Documentale();
            return doc.DocumentoIsAcquisito(docnumber);
        }

        public static bool DocumentoIsAcquisito(string docnumber, out string segnatura)
        {
            segnatura = "";
            DocsPaDB.Query_DocsPAWS.Documentale doc = new DocsPaDB.Query_DocsPAWS.Documentale();
            return doc.DocumentoIsAcquisito(docnumber, out segnatura);
        }

        /// <summary>
        /// Reperimento del numero delle trasmissioni effettuate dal documento
        /// </summary>
        /// <param name="idProfile"></param>
        /// <returns></returns>
        public static int GetCountTrasmissioniDocumento(int idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetCountTrasmissioniDocumento(idProfile);
        }

        public static void cambiaDirittiDocumenti(int accessRight, string idDocumento)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.cambiaDirittiDocumenti(accessRight, idDocumento);
        }

        public static bool DocumentoCheckUserVisibility(string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            return documentale.DocumentoCheckUserVisibility(docNumber, infoUtente.idPeople, infoUtente.idGruppo);
        }

        /// <summary>
        /// Reperimento tipologia del documento richiesto
        /// </summary>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static string GetTipoDocumento(string docNumber)
        {
            string tipoDocumento;

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_TIPO_DOCUMENTO_FROM_DOCNUMBER");

            queryDef.setParam("docNumber", docNumber);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            if (!dbProvider.ExecuteScalar(out tipoDocumento, commandText))
            {
                logger.Debug("Errore nel reperimento del tipo documento, QUERY " + commandText);
                throw new ApplicationException("Errore nel reperimento del tipo documento");
            }

            return tipoDocumento;
        }

        /// <summary>
        /// Reperimento DocNumber dai dati identificati del protocollo
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="registroProtocollo"></param>
        /// <param name="protocollo"></param>
        /// <param name="annoProtocollo"></param>
        /// <returns></returns>
        public static string GetDocNumber(string idAmministrazione, string registroProtocollo, string protocollo, string annoProtocollo)
        {
            string docNumber;

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DOCNUMBER");

            queryDef.setParam("idAmministrazione", idAmministrazione);
            queryDef.setParam("registroProtocollo", registroProtocollo);
            queryDef.setParam("protocollo", protocollo);
            queryDef.setParam("annoProtocollo", annoProtocollo);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            if (!dbProvider.ExecuteScalar(out docNumber, commandText))
            {
                logger.Debug("Errore nel reperimento del docNumber");
            }

            if (docNumber == null)
                docNumber = string.Empty;

            return docNumber;
        }

        public static bool insDocRichiestaConversionePdf(string idProfile)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                // Verifica se il file è già in stato di conversione in PDF
                if (doc.isDocInConversionePdf(idProfile))
                    return false; // Documento già in conversione
                else
                    return doc.insDocRichiestaConversionePdf(idProfile);
            }
        }

        public static void delDocRichiestaConversionePdf(string idProfile)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    doc.delDocRichiestaConversionePdf(idProfile);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in Documenti  - metodo: delDocRichiestaConversionePdf", e);
                }
            }
        }


        /// <summary>
        /// Verifica se la conversione pdf lato server è attiva o meno
        /// </summary>
        /// <returns></returns>
        public static bool isEnabledConversionePdfServer()
        {
            bool retValue;
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["CONVERSIONE_PDF_LATO_SERVER"], out retValue);
            return retValue;
        }

        public static bool isDocInConversionePdf(string idProfile)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
                return doc.isDocInConversionePdf(idProfile);
        }

        public ArrayList documentoGetTipoProtoRisposta(string docNumber)
        {
            ArrayList result = null;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.getTipoProtoRisposta(docNumber);
            return result;
        }

        public static ArrayList getListaExtFileAcquisiti(string idamm)
        {
            ArrayList result = null;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.getExtFileAqcuisiti(FormatiDocumento.Configurations.SupportedFileTypesEnabled, idamm);
            return result;
        }

        public static string getProtocolloTitolario(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    string protocolloTitolario = doc.getProtocolloTitolario(schedaDoc);
                    transactionContext.Complete();
                    return protocolloTitolario;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in Documenti  - metodo: getProtocolloTitolario", e);
                    return null;
                }
            }
        }

        public static int IsDocInADL(string idDoc, string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.IsDocInADL(idDoc, idPeople);
        }

        /// <summary>
        /// Funzione per il reprimento delle informazioni di base su un documento e sui suoi
        /// eventuali allegati
        /// </summary>
        /// <param name="idProfile">L'id profile del documento di cui reperire le informazioni</param>
        /// <param name="docNumber">Il numero del documento da visualizzare</param>
        /// <param name="versionNumber">Il numero di versione del documento da ricercare. Se non specificato viene presa l'ultima versione</param>
        /// <returns>Le informazioni di base sul documento richiesto e sui suoi allegati</returns>
        public static List<DocsPaVO.documento.BaseInfoDoc> GetBaseInfoForDocument(string idProfile, string docNumber, string versionNumber)
        {
            // Le informazioni di base del documento
            List<DocsPaVO.documento.BaseInfoDoc> result = null;

            try
            {
                // Creazione dell'oggetto a cui richiedere le informazioni sul documento
                DocsPaDB.Query_DocsPAWS.Documenti infoDoc = new DocsPaDB.Query_DocsPAWS.Documenti();

                // Richiesta delle informazioni sul documento specificato
                result = infoDoc.GetBaseInfoForDocument(idProfile, docNumber, versionNumber);

            }
            catch (Exception e)
            {
                logger.Debug("GetBaseInfoForDocument - BusinessLogic");
                logger.Debug(e);

                // In caso di eccezione viene rilanciata al livello superiore
                throw e;

            }

            // Restiuzione delle informazioni sul documento richiesto
            return result;

        }

        public static string getTipoDocObbl(string idAmministrazione)
        {
            string result = "0";
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                result = amm.getTipoDocObbl(idAmministrazione);
            }
            catch (Exception e)
            {
                logger.Debug("getTipoDocObbl - BusinessLogic");
                logger.Debug(e);
                throw e;
            }
            return result;
        }
        public static ArrayList getListaStoricoDataArrivo(string docnumber)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetListaStoricoDtaArrivo(docnumber);
        }

        /// <summary>
        /// Verifica se sono abilitati i mittenti multipli per il protocollo in entrata
        /// </summary>
        /// <returns></returns>
        public static bool isEnableMittentiMultipli()
        {
            return DocsPaDB.Query_DocsPAWS.Documenti.isEnableMittentiMultipli;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool esistiExt(string filename)
        {


            bool retval = true;
            if (!string.IsNullOrEmpty(filename))
            {
                string ext = Path.GetExtension(filename);
                if (!string.IsNullOrEmpty(ext))
                {
                    ext = ext.Remove(0, 1);
                    DocsPaDB.Query_DocsPAWS.Documenti db = new DocsPaDB.Query_DocsPAWS.Documenti();
                    retval = db.EsisteExt(ext);
                }
            }
            return retval;
        }

        public static bool insertExtNonGenerico(string filename, string mime_type)
        {

            bool retval = true;
            if (!string.IsNullOrEmpty(filename))
            {
                string ext = Path.GetExtension(filename);
                if (!string.IsNullOrEmpty(ext))
                {
                    ext = ext.Remove(0, 1);
                    DocsPaDB.Query_DocsPAWS.Documenti db = new DocsPaDB.Query_DocsPAWS.Documenti();
                    retval = db.InsertAppNonGenerica(ext, mime_type);
                }
            }
            return retval;
        }

        public static bool getIfDocOrFascIsInToDoList(string idUtente, string idTrasmissione)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
                return doc.getIfDocOrFascIsInToDoList(idUtente, idTrasmissione);
        }

        public static string VerificaDocErrati(string idAmm)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
                return doc.VerificaDocErrati(idAmm);
        }

        /// <summary>
        /// Questa funzione si occupa di creare un documento in uscita per l'inoltro massivo.
        /// Il documento creato avrà come allegati, per ogni documento, l'immagine del documento
        /// principale e tutti i suoi allegati.
        /// </summary>
        /// <param name="idProfiles">Lista degli identificativi univoci dei documenti</param>
        /// <param name="userInfo">Le informazioni sull'utente mittente del documento</param>
        /// <param name="userRole">Le informazioni sul ruolo mittente del documento</param>
        /// <param name="error">L'eventuale errore verificatosi un fase di creazione della scheda</param>
        /// <returns>La scheda documento pronta per essere compilata nei dettagli e spedita</returns>
        public static SchedaDocumento GetSchedaDocumentoInoltroMassivo(
            List<String> idProfiles,
            InfoUtente userInfo,
            Ruolo userRole,
            out String error)
        {
            // La scheda documento da restituire
            SchedaDocumento toReturn;

            // La uo mittente del documento
            Corrispondente sender;

            // Il responsabile per il reperimento dei file da allegare al documento
            SessionRepositoryFileManager repositoryFileManager;

            // La lista delle informazioni di base sui documenti da allegare al documento
            List<BaseInfoDoc> baseInfoDocs;

            // Booleano utilizzato per indicare che si è verificata un'eccezione
            bool haveError = false;

            // L'errore da restituire
            StringBuilder errorInCreation = new StringBuilder("Si sono verificati i seguenti errori durante la creazione del documento:\n");

            // Creazione della scheda documento
            toReturn = NewSchedaDocumento(userInfo, true);

            // La scheda deve essere un protocollo in uscita, predisposto e da protocollare
            toReturn.tipoProto = "P";
            toReturn.predisponiProtocollazione = true;
            toReturn.protocollo = new ProtocolloUscita();
            toReturn.protocollo.daProtocollare = "1";

            // Il mittente del documento è la UO cui appartiene l'utente che ha lanciato la procedura
            sender = userRole.uo;
            ((ProtocolloUscita)toReturn.protocollo).mittente = sender;

            // Inizializzazione del file manager
            repositoryFileManager = SessionRepositoryFileManager.GetFileManager(toReturn.repositoryContext);

            // Per ogni idProfile vengono recuperare le informazioni sul documento
            // e quindi viene creato un allegato per il file principale ed un allegato
            // per ogni allegato del documento
            int position = 1;
            foreach (String idProfile in idProfiles)
            {
                baseInfoDocs = null;

                try
                {
                    baseInfoDocs = GetBaseInfoForDocument(idProfile, null, null);
                }
                catch (Exception e)
                {
                    haveError = true;
                    errorInCreation.AppendFormat(
                        " - Non è stato possibile reperire le informazioni sul documento con identificativo {0}\n",
                        idProfile);
                }

                // Se le informazioni sono state reperite correttamente, si procede con la creazione
                try
                {
                    if (baseInfoDocs != null)
                        AddAttachmentForDocumentWithBaseInformationObject(
                            baseInfoDocs,
                            toReturn,
                            userInfo,
                            repositoryFileManager, ref position);
                }
                catch (Exception e)
                {
                    haveError = true;
                    errorInCreation.AppendLine(e.Message);

                }
            }

            // Se non si sono verificati errori viene reinizializzata la variabile in modo da elimintare 
            // il messaggio di segnalazione errori
            if (!haveError)
                errorInCreation = new StringBuilder();

            errorInCreation.AppendLine("\nCliccare su chiudi per mostrare il documento creato.");

            error = errorInCreation.ToString();

            // Restituzione del risultato
            return toReturn;
        }

        /// <summary>
        /// Funzione per la generazione e l'aggiunta degli allegati ad un documento Inoltrato.
        /// Questa funzionalità fa arte del pacchietto per Corte dei Conti
        /// </summary>
        /// <param name="baseInfoDocs">Lista delle informazioni di base su un documento ed i suoi allegati</param>
        /// <param name="document">Oggetto a cui aggiungere gli eventuali allegati</param>
        /// <param name="userInfo">Le informazioni sull'utente</param>
        private static void AddAttachmentForDocumentWithBaseInformationObject(
            List<BaseInfoDoc> baseInfoDocs,
            SchedaDocumento document,
            InfoUtente userInfo,
            SessionRepositoryFileManager repositoryFileManager, ref int position)
        {
            // L'allegato da aggiungere al documento
            Allegato attachment;

            // Descrizione da assegnare all'allegato
            String description;

            // Il file request
            FileRequest fileRequest;

            // Informazioni di base sul documento pricipale
            BaseInfoDoc doc = baseInfoDocs.Where(e => e.IsAttachment == false).First();

            // Nome con cui referenziare il documento principale di ogni documento da inoltrare
            // Sarà impostato pari al system id del documento se questo è un grigio o alla segnatura
            // in caso di protocollo
            String documentIdentificationData = doc.Name;

            // Il documento da allegare
            FileDocumento documentFile;
            foreach (BaseInfoDoc baseInfoDoc in baseInfoDocs)
            {
                if (!baseInfoDoc.IsAttachment)
                    description = String.Format("Inoltro documento principale {0}", documentIdentificationData);
                else
                    description = String.Format("Inoltro allegato \"{0}\" del documento {1}", baseInfoDoc.Description, documentIdentificationData);

                // Creazione dell'allegato
                attachment = new Allegato()
                {
                    descrizione = description,
                    fileName = baseInfoDoc.FileName,
                    fileSize = baseInfoDoc.FileSize.ToString(),
                    firmato = baseInfoDoc.Firmato,
                    path = baseInfoDoc.Path,
                    subVersion = baseInfoDoc.VersionNumber.ToString(),
                    version = "1",
                    versionId = baseInfoDoc.VersionId,
                    //versionLabel = "" + position,
                    versionLabel = string.Format("A{0:0#}", position),
                    numeroPagine = 0,
                    dataInserimento = string.Empty,
                    repositoryContext = document.repositoryContext,
                    ForwardingSource = doc.IdProfile,
                    position = position,
                    TypeAttachment = 1
                };

                if (baseInfoDoc.HaveFile)
                {
                    // Costruzione dell'oggetto file request
                    fileRequest = new FileRequest()
                    {
                        fileName = baseInfoDoc.FileName,
                        path = baseInfoDoc.Path,
                        version = baseInfoDoc.VersionNumber.ToString(),
                        versionId = baseInfoDoc.VersionId,
                        versionLabel = "" + position,
                        docNumber = baseInfoDoc.DocNumber
                    };
                    // Recupero del contenuto del file
                    try
                    {
                        documentFile = FileManager.getFileFirmato(
                            fileRequest,
                            userInfo,
                            false);
                        // Impostazione del file per il documento
                        repositoryFileManager.SetFile(attachment, documentFile);

                    }
                    catch (Exception e)
                    {
                        throw new Exception(String.Format(
                            " - Errore durante il reperimento del file associato al documento {0}.",
                            baseInfoDoc.DocNumber));
                    }

                }

                document.allegati.Add(attachment);
                position++;
            }

        }

        public static string DocumentoGetCodiciClassifica(string idProfile, DocsPaVO.utente.InfoUtente infoUtente)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
                return doc.DocumentoGetCodiciClassifica(idProfile, infoUtente);
        }

        public static string GetSegnaturaRepertorio(string docnumber, string codiceAmm, bool getInHtmlVersion, out String dataAnnullamento)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                return doc.GetSegnaturaRepertorio(docnumber, codiceAmm, getInHtmlVersion, out dataAnnullamento);
            }
        }

        /// <summary>
        /// Metodo per impostare impostare un documento come non privato
        /// </summary>
        /// <param name="docId">Id del del documento</param>
        internal static void SetNotPrivate(string docId)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                doc.SetNotPrivate(docId);

            }
        }

        public static String GetDocumentAttribute(DateTime protoDate, String numProto, String regCode, String ammCode, DocsPaDB.Query_DocsPAWS.Documenti.DocumentAttribute attribute)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                return doc.GetDocumentAttribute(protoDate, numProto, regCode, ammCode, attribute);
            }
        }

        /// <summary>
        /// ricerca protocollo by segnatura
        /// </summary>
        /// <param name="segnatura"></param>
        /// <param name="infoutente"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento GetDettaglioBySignature(string segnatura, DocsPaVO.utente.InfoUtente infoutente)
        {
            DocsPaVO.documento.SchedaDocumento sch = null;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            sch = doc.ricercaProto(segnatura, infoutente);
            return sch;
        }

        public static Dictionary<string, string> GetDatiSegnaturaTimbro(string idDocumento)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.GetDatiSegnaturaTimbro(idDocumento);
            return result;
        }

        public static Dictionary<string, string> GetInfoDocument(string idDocumento)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            result = doc.GetInfoDocument(idDocumento);
            return result;
        }

        /// <summary>
        /// Reperimento scheda documento a partire dal solo "docNumber".
        /// NB: Viene ignorata la gestione della visibilità. 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento getDettaglioNoSecurityByNumProtoEIDRegistro(DocsPaVO.utente.InfoUtente infoUtente, string numProto, string idRegistro)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();

            schedaDoc = doc.GetDettaglioNoSecurityByNumProtoEIDRegistro(infoUtente, numProto, idRegistro);

            if (schedaDoc == null || string.IsNullOrEmpty(schedaDoc.systemId))
            {
                schedaDoc = null;
            }
            else
            {
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

                // Reperimento del token di autenticazione per il superutente del documentale
                string superUserToken = userManager.GetSuperUserAuthenticationToken();
                string oldToken = infoUtente.dst;
                infoUtente.dst = superUserToken;

                try
                {
                    // Reperimento informazioni se il documento è in stato checkout
                    schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // Ripristino token di autenticazione
                    infoUtente.dst = oldToken;
                }

                DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
                string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                app.estensione = ext;
                docPrinc.applicazione = app;
                schedaDoc.documenti[0] = docPrinc;

            }



            return schedaDoc;
        }

        public static DocsPaVO.documento.Tab GetDocumentTab(string documentId, InfoUtente infoUser)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                return doc.GetDocumentTab(documentId, infoUser);
            }
        }

        public static int GetDocumentTrasmToCCDest(string documentId, InfoUtente infoUser)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                return doc.GetDocumentTrasmToCCDest(documentId, infoUser);
            }
        }

        public static DocsPaVO.documento.Documento[] GetVersionsMainDocument(InfoUtente infoUser, string docNumber)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                return doc.GetVersionsMainDoc(infoUser, docNumber).ToArray();
            }
        }

        public static int IsDocInADLRole(string idDoc, string idRole)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.IsDocInADLRole(idDoc, idRole);
        }

        public static int IsFascInADLRole(string idFasc, string idRole)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.IsFascInADLRole(idFasc, idRole);
        }

        /// <summary>
        /// Reperimento scheda documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idProfile"></param>
        /// <param name="docNumber"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.SchedaDocumento GetListVersions(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
        {
            logger.Info("BEGIN");
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc = doc.GetListVersionDetail(infoUtente, idProfile, docNumber, true);

            try
            {
                DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
                string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                app.estensione = ext;
                docPrinc.applicazione = app;
                schedaDoc.documenti[0] = docPrinc;
            }
            catch (Exception)
            {
            }
            logger.Info("END");
            return schedaDoc;
        }

        /// <summary>
        /// Metodo per il recupero dell'id dell'ultimo documento creato con un dato oggetto
        /// </summary>
        /// <param name="subject">Oggetto del documento</param>
        /// <returns>Id del documento</returns>
        public static String GetLastInsertedDocumentWithSubject(String subject)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti docsDb = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                return docsDb.GetLastInsertedDocumentWithSubject(subject);
            }

        }

        /// <summary>
        /// Per concatenare 2 documenti.
        /// </summary>
        /// <param name="idParent">Id del documento padre</param>
        /// <param name="docNumber">Id del documento figlio</param>
        public static void UpdateRispostaProtocollo(string idParent, string docNumber)
        {
            using (DocsPaDB.Query_DocsPAWS.Documenti docsDb = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                docsDb.UpdateRispostaProtocollo(idParent,docNumber);
            }
        }

        #region ULTIMI_DOCUMENTI_VISUALIZZATI
        public static void UpdateLastDocumentsView(string idProfile, InfoUtente infoUtente)
        {
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_NUM_ULTIMI_DOC_VISUALIZZATI"))
                                            && !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_NUM_ULTIMI_DOC_VISUALIZZATI").Equals("0"))
            {
                DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                doc.UpdateLastDocumentsView(idProfile, infoUtente);
            }
        }

        public static List<DocumentoVisualizzato> GetLastDocumentsView(InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetLastDocumentsView(infoUtente);
        }

        #endregion

        public static bool CheckDocumentIsSent(string idDocument)
        {
            bool retval = false;
            DocsPaDB.Query_DocsPAWS.Documenti dbInt = new DocsPaDB.Query_DocsPAWS.Documenti();
            retval = dbInt.CheckDocumentIsSent(idDocument, "DOCUMENTOSPEDISCI");
            return retval;
        }
    }
}
