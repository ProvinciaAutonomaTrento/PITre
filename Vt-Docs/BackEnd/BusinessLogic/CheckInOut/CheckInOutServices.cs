using System;
using System.Data;
using System.IO;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.CheckInOut;
using DocsPaVO.Validations;
using DocsPaDocumentale.Documentale;
using DocsPaDocumentale.Interfaces;
using log4net;

namespace BusinessLogic.CheckInOut
{
    /// <summary>
    /// Gestione dei servizi relativi al CheckIn / CheckOut dei documenti
    /// </summary>
    public sealed class CheckInOutServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(CheckInOutServices));
        /// <summary>
        /// Costanti che identificano le tipologie di errore
        /// </summary>
        private const string DOCUMENT_CONSOLIDATED = "DOCUMENT_CONSOLIDATED";
        private const string INVALID_ID_DOCUMENT = "INVALID_ID_DOCUMENT";
        private const string INVALID_DOCUMENT_NUMBER = "INVALID_DOCUMENT_NUMBER";
        private const string INVALID_PATH_DOCUMENT = "INVALID_PATH_DOCUMENT";
        private const string INVALID_USER = "INVALID_USER";
        private const string MISSING_CHECK_OUT_STATUS = "MISSING_CHECK_OUT_STATUS";
        private const string CHECK_OUT_ERROR = "CHECK_OUT_ERROR";
        private const string CHECK_IN_ERROR = "CHECK_IN_ERROR";
        private const string UNDO_CHECK_OUT_ERROR = "UNDO_CHECK_OUT_ERROR";
        private const string DOCUMENT_ALREADY_CHECKED_OUT = "DOCUMENT_ALREADY_CHECKED_OUT";
        private const string GET_FILE_ERROR = "GET_FILE_ERROR";
        private const string CREATE_VERSION_ERROR = "CREATE_VERSION_ERROR";

        /// <summary>
        /// Costanti che identificano i messaggi di validazione
        /// </summary>
        private const string MSG_DOCUMENT_CONSOLIDATED = "Il documento risulta in stato consolidato, pertanto non può essere bloccato";
        private const string MSG_MISSING_ID_DOCUMENT = "IDDocumento non valido";
        private const string MSG_MISSING_DOCUMENT_NUMBER = "DocumentNumber non valido";
        private const string MSG_MISSING_PATH_DOCUMENT = "Percorso del documento non valido";
        private const string MSG_MISSING_USER = "Oggetto utente non valido";
        private const string MSG_MISSING_CHECK_OUT_STATUS = "Oggetto CheckOutStatus non valido";
        private const string MSG_DOCUMENT_NOT_CHECKED_OUT_BY_USER = "Il documento non risulta bloccato dall'utente '{0}'";
        private const string MSG_DOCUMENT_ALREADY_CHECKED_OUT = "Il documento, o almeno uno dei suoi allegati, risulta già bloccato da un altro utente";
        private const string MSG_CHECK_OUT_ERROR = "Si è verificato un errore nel blocco del documento";
        private const string MSG_CHECK_IN_ERROR = "Si è verificato un errore nel rilascio del documento";
        private const string MSG_CHECK_IN_ERROR_DOCUMENTALE = "Si è verificato un errore nel rilascio del documento nel sistema documentale";
        private const string MSG_UNDO_CHECK_OUT_ERROR = "Si è verificato un errore nell'annullamento del blocco del documento";
        private const string MSG_GET_FILE_ERROR = "Si è verificato un errore nel reperimento del file";
        private const string MSG_CREATE_VERSION_ERROR = "Si è verificato un errore in fase di creazione di una nuova versione del documento";
        private const string MSG_DOCUMENT_MODEL_NOT_EXISTS = "Per il tipo di documento richiesto non è presente alcun modello predefinito.\nContattare l'amministratore per installare il modello di documento richiesto";

        private CheckInOutServices()
        {
        }

        #region Public methods


        /// <summary>
        /// Reperimento del file in stato checkedout
        /// </summary>
        /// <param name="checkedOutStatus"></param>
        /// <param name="checkOutOwner">Utente che ha in checkout il documento</param>
        /// <returns></returns>
        public static byte[] GetCheckedOutFile(CheckOutStatus checkedOutStatus, InfoUtente checkOutOwner)
        {
            /*
                Precondizioni:
                Il documento deve essere bloccato dall'utente che effettua la richiesta.
				
                PostCondizioni:
                Restituzione del documento
            */
            if (checkedOutStatus == null)
                throw new ApplicationException(MSG_MISSING_CHECK_OUT_STATUS);

            byte[] retValue = null;

            // Verifica se il documento è bloccato dall'utente stesso che ha richiesto il file
            if (IsOwnerCheckedOut(checkedOutStatus.IDDocument, checkedOutStatus.DocumentNumber, checkOutOwner))
            {
                FileDocumento fileDocument = GetFileDocument(checkedOutStatus.IDDocument, checkOutOwner);

                if (fileDocument != null)
                {
                    retValue = fileDocument.content;
                }
                else
                {
                    // Se non è presente alcun documento,
                    // viene reperito il modello predefinito
                    // per il tipo di file richiesto
                    retValue = CheckInOutModels.GetDocumentModelContent(checkOutOwner.idAmministrazione, checkedOutStatus.DocumentLocation);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento delle informazioni di stato realtivamente ad un documento checkedout
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        /// <remarks>
        /// Le informazioni di stato possono essere reperite solamente dall'utente che ha bloccato il documento
        /// </remarks>
        public static CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber, InfoUtente utente)
        {
            /*
                Precondizioni:
                IDDocumento valido.
                Oggetto "utente" valido.
                Il documento deve essere in stato checked out.
                L'utente fornito deve essere lo stesso che ha impostato lo stato checked out per il documento.
				
                PostCondizioni:
                Restituzione dello stato del documento.
            */
            if (idDocument == null || idDocument == string.Empty)
                throw new ApplicationException(MSG_MISSING_ID_DOCUMENT);

            if (documentNumber == null || documentNumber == string.Empty)
                throw new ApplicationException(MSG_MISSING_DOCUMENT_NUMBER);

            CheckOutStatus retValue = null;

            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(utente);

            string ownerUser;
            if (documentManagement.IsCheckedOut(idDocument, documentNumber, out ownerUser))
            {
                retValue = documentManagement.GetCheckOutStatus(idDocument, documentNumber);

                if (retValue != null)
                {
                    // Se il documento è in checkout, reperimento dello stato di conversione PDF
                    retValue.InConversionePdf = IsInConversionePdf(idDocument);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se un documento è in checkout
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="documentNumber"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static bool IsCheckedOut(string idDocument, string documentNumber, InfoUtente utente)
        {
            /*
                Precondizioni:
                IDDocumento valido.
                Il documento deve essere in stato checked out.
				
                Postcondizioni:
                Esito dell'operazione	
            */
            if (idDocument == null || idDocument == string.Empty)
                throw new ApplicationException(MSG_MISSING_ID_DOCUMENT);

            if (documentNumber == null || documentNumber == string.Empty)
                throw new ApplicationException(MSG_MISSING_DOCUMENT_NUMBER);

            if (utente == null)
                throw new ApplicationException(MSG_MISSING_USER);

            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(utente);

            return documentManagement.IsCheckedOut(idDocument, documentNumber);
        }

        /// <summary>
        /// Verifica se il documento principale o uno dei suoi allegati è in checkout, relativamente al parametro checkAllegati
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="checkedOutUser"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente">Utente che ha fatto il checkout del documento</param>
        /// <returns></returns>
        public static bool IsCheckedOut(string idDocument, string documentNumber, InfoUtente utente, bool checkAllegati)
        {
            string ownerUser;
            return IsCheckedOut(idDocument, documentNumber, utente, checkAllegati, out ownerUser);
        }

        /// <summary>
        /// Verifica se il documento principale o uno dei suoi allegati è in checkout, relativamente al parametro checkAllegati
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="checkedOutUser"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente">Utente che ha fatto il checkout del documento</param>
        /// <returns></returns>
        public static bool IsCheckedOutSimple(string idDocument, string documentNumber, InfoUtente utente, bool checkAllegati, SchedaDocumento doc)
        {
            string ownerUser;
            return IsCheckedOutSimple(idDocument, documentNumber, utente, checkAllegati, doc, out ownerUser);
        }

        /// <summary>
        /// Verifica se il documento principale o uno dei suoi allegati è in checkout, relativamente al parametro checkAllegati
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente"></param>
        /// <param name="checkAllegati"></param>
        /// <param name="ownerUser">
        /// Nome utente proprietario del blocco sul documento
        /// </param>
        /// <returns></returns>
        public static bool IsCheckedOut(string idDocument, string documentNumber, InfoUtente utente, bool checkAllegati, out string ownerUser)
        {
            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(utente);

            ownerUser = string.Empty;


            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(utente, idDocument, documentNumber);


            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
            {
                // Impostazione utente che ha bloccato il documento
                ownerUser = schedaDocumento.checkOutStatus.UserName;

                return (!string.IsNullOrEmpty(schedaDocumento.checkOutStatus.ID));
            }

            //Controllo se un allegato è bloccato
            if (checkAllegati)
            {

                foreach (Allegato a in schedaDocumento.allegati)
                {

                    if (documentManagement.IsCheckedOut(a.docNumber, a.docNumber, out ownerUser))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Verifica se il documento principale o uno dei suoi allegati è in checkout, relativamente al parametro checkAllegati
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente"></param>
        /// <param name="checkAllegati"></param>
        /// <param name="ownerUser">
        /// Nome utente proprietario del blocco sul documento
        /// </param>
        /// <returns></returns>
        public static bool IsCheckedOutSimple(string idDocument, string documentNumber, InfoUtente utente, bool checkAllegati, SchedaDocumento doc, out string ownerUser)
        {
            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(utente);

            ownerUser = string.Empty;

            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            bool retval = false;

            if (doc == null || string.IsNullOrEmpty(doc.systemId) || !doc.systemId.Equals(idDocument))
            {
                //schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(utente, idDocument, documentNumber);
                retval = documentManagement.IsCheckedOut(idDocument, documentNumber, out ownerUser);
            }
            else
            {
                schedaDocumento = doc;
            }

            //?
            // schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
         
            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
            {
                // Impostazione utente che ha bloccato il documento
                ownerUser = schedaDocumento.checkOutStatus.UserName;

                return (!string.IsNullOrEmpty(schedaDocumento.checkOutStatus.ID));
            }
            else if (retval)
            {
                return retval;
            }

            //Controllo se un allegato è bloccato
            if (checkAllegati)
            {
                if (schedaDocumento != null)
                {
                    foreach (Allegato a in schedaDocumento.allegati)
                    {
                        if (documentManagement.IsCheckedOut(a.docNumber, a.docNumber, out ownerUser))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    System.Collections.ArrayList allegati = BusinessLogic.Documenti.AllegatiManager.getAllegatiSuperSimple(idDocument);
                    foreach (string a in allegati)
                    {
                        if (documentManagement.IsCheckedOut(a, a, out ownerUser))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Verifica se un documento è in checkout
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="checkedOutUser"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente">Utente che ha fatto il checkout del documento</param>
        /// <returns></returns>
        public static bool IsCheckedOut(string idDocument, string documentNumber, InfoUtente user, out string ownerUser)
        {
            /*
                Precondizioni:
                IDDocumento valido.
                Il documento deve essere in stato checked out.
				
                Postcondizioni:
                Esito dell'operazione.
                Utente che ha impostato lo stato checked out del documento.
            */
            if (idDocument == null || idDocument == string.Empty)
                throw new ApplicationException(MSG_MISSING_ID_DOCUMENT);

            if (documentNumber == null || documentNumber == string.Empty)
                throw new ApplicationException(MSG_MISSING_DOCUMENT_NUMBER);

            if (user == null)
                throw new ApplicationException(MSG_MISSING_USER);

            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(user);

            return documentManagement.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        /// <summary>
        /// Verifica se il documento è stato messo in checkout dall'utente richiesto
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static bool IsOwnerCheckedOut(string idDocument, string documentNumber, InfoUtente user)
        {
            /*
                Precondizioni:
                IDDocumento valido.
                Oggetto "utente" valido.
                Il documento deve essere in stato checked out.
				
                Postcondizioni:
                Esito dell'operazione.
            */
            if (idDocument == null || idDocument == string.Empty)
                throw new ApplicationException(MSG_MISSING_ID_DOCUMENT);

            if (documentNumber == null || documentNumber == string.Empty)
                throw new ApplicationException(MSG_MISSING_DOCUMENT_NUMBER);

            if (user == null)
                throw new ApplicationException(MSG_MISSING_USER);

            bool retValue = false;

            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(user);

            string ownerUser;
            if (documentManagement.IsCheckedOut(idDocument, documentNumber, out ownerUser))
                retValue = (ownerUser.ToUpper() == user.userId.ToUpper());

            return retValue;
        }

        /// <summary>
        /// CheckOut di un documento
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="documentNumber"></param>
        /// <param name="documentLocation">Percorso in cui deve essere estratto il documento</param>
        /// <param name="machineName"></param>
        /// <param name="utente">Utente che effettua il checkout</param>
        /// <param name="checkOutStatus">Status del documento CheckedOut</param>
        /// <returns></returns>
        public static ValidationResultInfo CheckOut(string idDocument,
                                                    string documentNumber,
                                                    string documentLocation,
                                                    string machineName,
                                                    InfoUtente user,
                                                    out CheckOutStatus checkOutStatus)
        {
            /*
                PreCondizioni:
                Documento in stato NON consolidato
                IDDocumento valido.
                Percorso del file valido.
                Oggetto "utente" valido.
                La funzione può essere utilizzata solamente se il documento richiesto
                non è già in stato CheckOut.
				
                PostCondizioni:
                Viene impostato il blocco sul documento. 
                Per gli altri utenti non sarà possibile effettuare tutte le operazioni
                inerenti all'inserimento di una nuova versione.
            */
            ValidationResultInfo retValue = new ValidationResultInfo();
            checkOutStatus = null;

            // Verifica stato di consolidamento del documento
            if (!BusinessLogic.Documenti.DocumentConsolidation.CanExecuteAction(user, idDocument, Documenti.DocumentConsolidation.ConsolidationActionsDeniedEnum.AddVersions))
            {
                retValue.Value = false;
                retValue.BrokenRules.Add(CreateBrokenRule(DOCUMENT_CONSOLIDATED, MSG_DOCUMENT_CONSOLIDATED));
            }
            else
            {
                // Validazione parametri di input
                if (idDocument == null || idDocument == string.Empty)
                    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_ID_DOCUMENT, MSG_MISSING_ID_DOCUMENT));

                if (documentNumber == null || documentNumber == string.Empty)
                    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_DOCUMENT_NUMBER, MSG_MISSING_DOCUMENT_NUMBER));

                //if (documentLocation==null || documentLocation==string.Empty)
                //    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_PATH_DOCUMENT,MSG_MISSING_PATH_DOCUMENT));

                if (user == null)
                    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_USER, MSG_MISSING_USER));

                retValue.Value = (retValue.BrokenRules.Count == 0);

                if (retValue.Value)
                {
                    try
                    {
                        checkOutStatus = null;

                        CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(user);

                        // Verifica se è il documento, o uno dei suoi componenti, non sia già in stato CheckOut
                        string ownerUser;

                        if (!IsCheckedOut(idDocument, documentNumber, user, true, out ownerUser))
                        {
                            if (!documentManagement.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatus))
                            {
                                retValue.BrokenRules.Add(CreateBrokenRule(CHECK_OUT_ERROR, MSG_CHECK_OUT_ERROR));
                            }
                            else
                            {
                                // Se il documento è in checkout, reperimento dello stato di conversione PDF
                                checkOutStatus.InConversionePdf = IsInConversionePdf(idDocument);
                            }
                        }
                        else
                        {
                            retValue.BrokenRules.Add(CreateBrokenRule(DOCUMENT_ALREADY_CHECKED_OUT, MSG_DOCUMENT_ALREADY_CHECKED_OUT));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.Message);

                        retValue.BrokenRules.Add(CreateBrokenRule(CHECK_OUT_ERROR, ConcatErrorMessage(MSG_CHECK_OUT_ERROR, ex)));
                    }

                    retValue.Value = (retValue.BrokenRules.Count == 0);
                }
            }

            return retValue;
        }

        /// <summary>
        /// CheckOut di un documento
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="documentNumber"></param>
        /// <param name="documentLocation">Percorso in cui deve essere estratto il documento</param>
        /// <param name="machineName"></param>
        /// <param name="user">Utente che effettua il checkout</param>
        /// <param name="checkOutStatus">Status del documento CheckedOut</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ValidationResultInfo CheckOut(string idDocument,
                                                    string documentNumber,
                                                    string documentLocation,
                                                    string machineName,
                                                    InfoUtente user,
                                                    out CheckOutStatus checkOutStatus,
                                                    out byte[] content)
        {
            /*
                PreCondizioni:
                La funzione può essere utilizzata solamente se il documento richiesto
                non è già in stato CheckOut.
				
                PostCondizioni:
                Viene impostato il blocco sul documento. 
                Viene restituita l'ultima versione del file del documento.
                Per gli altri utenti non sarà possibile effettuare tutte le operazioni
                inerenti all'inserimento di una nuova versione.
            */
            checkOutStatus = null;
            content = null;

            ValidationResultInfo retValue = new ValidationResultInfo();

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = CheckOut(idDocument, documentNumber, documentLocation, machineName, user, out checkOutStatus);

                if (retValue.Value)
                {
                    // Se il documento è in checkout, reperimento dello stato di conversione PDF
                    checkOutStatus.InConversionePdf = IsInConversionePdf(idDocument);

                    try
                    {
                        // Se blocco impostato correttamente, 
                        // reperimento dell'ultima versione del documento
                        FileDocumento checkedOutFile = GetFileDocument(idDocument, user);

                        if (checkedOutFile != null)
                        {
                            content = checkedOutFile.content;
                        }
                        else
                        {
                           
                            SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglio(user, idDocument, documentNumber);
                            if (sd != null && sd.template != null && sd.template.PATH_MODELLO_1 != null && (sd.template.PATH_MODELLO_1_EXT.ToUpper().Equals("PPT") || (sd.template.PATH_MODELLO_1_EXT.ToUpper().Equals("PPTX"))))
                            {
                                BusinessLogic.Modelli.AsposeModelProcessor.PptModelProcessor processor = new Modelli.AsposeModelProcessor.PptModelProcessor();
                                content = processor.GetProcessedTemplate(user, sd);
                            }
                            else
                            {
                                // Se non è presente alcun documento,
                                // viene reperito il modello predefinito
                                // per il tipo di file richiesto
                                content = CheckInOutModels.GetDocumentModelContent(Convert.ToInt32(user.idAmministrazione), documentLocation);
                            }
                            if (content == null)
                            {
                                // Se non è presente alcun modello predefinito per il documento
                                // richiesto, viene annullata l'operazione di CheckOut
                                UndoCheckOut(checkOutStatus, user);

                                retValue.BrokenRules.Add(CreateBrokenRule(CHECK_OUT_ERROR, MSG_DOCUMENT_MODEL_NOT_EXISTS));

                                checkOutStatus = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.Message);

                        retValue.BrokenRules.Add(CreateBrokenRule(GET_FILE_ERROR, ConcatErrorMessage(MSG_GET_FILE_ERROR, ex)));

                        // UndoCheckOut del documento in caso di errore nel reperimento del file
                        UndoCheckOut(checkOutStatus, user);

                        checkOutStatus = null;
                    }
                }

                retValue.Value = (retValue.BrokenRules.Count == 0);

                if (retValue.Value)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// CheckIn di un documento
        /// </summary>
        /// <param name="checkOutStatus">Dati sullo stato del CheckOut</param>
        /// <param name="checkOutOwner">Utente che ha effettuato il CheckIn</param>
        /// <param name="checkedOutFile">Documento CheckedOut, che comprende il content del documento eventualmente modificato</param>
        /// <param name="checkInComments">Eventuale commento per il CheckIn</param>
        /// <returns></returns>
        public static ValidationResultInfo CheckIn(CheckOutStatus checkOutStatus,
                                                    InfoUtente checkOutOwner,
                                                    byte[] content,
                                                    string checkInComments)
        {
            /*
                PreCondizioni:
                Oggetto "checkOutStatus" valido.
                La funzione può essere utilizzata solamente se il documento richiesto
                è già in stato CheckOut dall'utente richiesto.
				
                PostCondizioni:
                Viene rilasciato il blocco sul documento e viene creata una nuova
                versione con il documento eventualmente modificato.
            */

            ValidationResultInfo retValue = new ValidationResultInfo();

            if (checkOutStatus == null)
                retValue.BrokenRules.Add(CreateBrokenRule(MISSING_CHECK_OUT_STATUS, MSG_MISSING_CHECK_OUT_STATUS));

            // Verifica se il formato del file può essere sbloccato
            if (!CanCheckInFormat(checkOutStatus))
                retValue.BrokenRules.Add(CreateBrokenRule(MISSING_CHECK_OUT_STATUS, MSG_MISSING_CHECK_OUT_STATUS));

            retValue.Value = (retValue.BrokenRules.Count == 0);

            if (retValue.Value)
            {
                // Creazione contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    try
                    {
                        // Verifica se il documento è stato bloccato dall'utente che richiede il servizio
                        if (IsOwnerCheckedOut(checkOutStatus.IDDocument, checkOutStatus.DocumentNumber, checkOutOwner))
                        {
                            //modifica
                            //DocsPaVO.Caching.CacheConfig info = Documenti.CacheFileManager.getConfigurazioneCache(checkOutOwner.idAmministrazione);//server.MachineName);

                            // if(info != null && info.caching == 1)
                            if (Documenti.CacheFileManager.isActiveCaching(checkOutOwner.idAmministrazione))
                            {
                                if (!CheckInOutCache.CheckIn(checkOutStatus, content, checkInComments, checkOutOwner))
                                    throw new ApplicationException();
                            }
                            else
                            {
                                //modifica
                                CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(checkOutOwner);

                                if (!documentManagement.CheckIn(checkOutStatus, content, checkInComments))
                                {
                                    retValue.BrokenRules.Add(CreateBrokenRule(CHECK_IN_ERROR, MSG_CHECK_IN_ERROR_DOCUMENTALE));
                                }
                                else
                                {
                                    DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

                                    // Reperimento dell'ultima versione del documento
                                    FileRequest fileRequest = checkInOutDb.GetFileRequest(checkOutStatus.IDDocument);

                                    BusinessLogic.Documenti.FileManager.processFileInformation(fileRequest, checkOutOwner);
                                }
                            }
                        }
                        else
                        {
                            retValue.BrokenRules.Add(CreateBrokenRule(CHECK_IN_ERROR, string.Format(MSG_DOCUMENT_NOT_CHECKED_OUT_BY_USER, checkOutStatus.UserName)));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.Message);

                        retValue.BrokenRules.Add(CreateBrokenRule(CHECK_IN_ERROR, ConcatErrorMessage(MSG_CHECK_IN_ERROR, ex)));
                    }

                    retValue.Value = (retValue.BrokenRules.Count == 0);

                    if (retValue.Value)
                        transactionContext.Complete();
                }
            }

            return retValue;
        }

        /// <summary>
        /// Annullamento dello stato CheckedOut per un documento
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        public static ValidationResultInfo UndoCheckOut(CheckOutStatus checkOutStatus, InfoUtente checkOutOwner)
        {
            /*
                PreCondizioni:
                Oggetto "checkOutStatus" valido.
                La funzione può essere utilizzata solamente se il documento richiesto
                è già in stato CheckOut dall'utente richiesto.
				
                PostCondizioni:
                Viene rilasciato il blocco sul documento e vengono annullate le eventuali
                modifiche effettuate
            */
            ValidationResultInfo retValue = new ValidationResultInfo();

            if (checkOutStatus == null)
                retValue.BrokenRules.Add(CreateBrokenRule(MISSING_CHECK_OUT_STATUS, MSG_MISSING_CHECK_OUT_STATUS));

            retValue.Value = (retValue.BrokenRules.Count == 0);

            if (retValue.Value)
            {
                // Creazione contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    try
                    {
                        // Verifica se l'utente fornito è il proprietario del blocco sul documento
                        if (IsOwnerCheckedOut(checkOutStatus.IDDocument, checkOutStatus.DocumentNumber, checkOutOwner))
                        {
                            // Gestione UndoCheckOut nel documentale
                            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(checkOutOwner);

                            if (!documentManagement.UndoCheckOut(checkOutStatus))
                                throw new ApplicationException(MSG_UNDO_CHECK_OUT_ERROR);

                            if (BusinessLogic.Documenti.DocManager.isDocInConversionePdf(checkOutStatus.IDDocument))
                            {
                                // Rimozione del documento dalla coda di conversione PDF qualora il blocco sia stato iniziato da una conversione PDF
                                BusinessLogic.Documenti.DocManager.delDocRichiestaConversionePdf(checkOutStatus.IDDocument);
                            }
                        }
                        else
                        {
                            retValue.BrokenRules.Add(CreateBrokenRule(CHECK_IN_ERROR, string.Format(MSG_DOCUMENT_NOT_CHECKED_OUT_BY_USER, checkOutStatus.UserName)));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.Message);

                        retValue.BrokenRules.Add(CreateBrokenRule(UNDO_CHECK_OUT_ERROR, ex.Message));
                    }

                    retValue.Value = (retValue.BrokenRules.Count == 0);

                    if (retValue.Value)
                        transactionContext.Complete();
                }
            }

            return retValue;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        private static bool CanCheckInFormat(CheckOutStatus checkOutStatus)
        {
            FileInfo fileInfo = new FileInfo(checkOutStatus.DocumentLocation);

            string extension = fileInfo.Extension.Replace(".", string.Empty);


            return true;
        }

        /// <summary>
        /// Reperimento oggetto InfoDocumento
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        private static InfoDocumento GetInfoDocumento(string idDocument, string documentNumber, InfoUtente utente)
        {
            InfoDocumento infoDocumento = null;

            try
            {
                SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(utente, idDocument, documentNumber);

                infoDocumento = BusinessLogic.Documenti.DocManager.getInfoDocumento(schedaDocumento);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new ApplicationException("Documento inesistente oppure l'utente potrebbe non avere i diritti sufficienti per accedevi", ex);
            }

            return infoDocumento;
        }

        /// <summary>
        /// Reperimento oggetto "FileRequest" relativamente all'ultima
        /// versione del documento corrente
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        private static FileRequest GetFileRequest(string idDocument)
        {
            FileRequest retValue = null;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_GET_LAST_VERSION_DATA");
                queryDef.setParam("idDocument", idDocument);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            retValue = new FileRequest();

                            retValue.fileSize = reader.GetValue(reader.GetOrdinal("FILE_SIZE")).ToString();
                            retValue.docNumber = reader.GetValue(reader.GetOrdinal("DOCNUMBER")).ToString();
                            retValue.versionId = reader.GetValue(reader.GetOrdinal("VERSION_ID")).ToString();
                            retValue.fileName = reader.GetValue(reader.GetOrdinal("PATH")).ToString();
                            retValue.docServerLoc = GetDocRootPath();
                            retValue.version = reader.GetValue(reader.GetOrdinal("VERSION")).ToString();
                            retValue.subVersion = reader.GetValue(reader.GetOrdinal("SUBVERSION")).ToString();
                            retValue.versionLabel = reader.GetValue(reader.GetOrdinal("VERSION_LABEL")).ToString();
                            retValue.firmatari = new System.Collections.ArrayList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel reperimento dell'oggetto 'FileRequest'. IDDocument: " + idDocument, ex);
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se per l'ultima versione del documento corrente è stato acquisito un file
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        private static bool IsAcquired(FileRequest fileRequest)
        {
            return (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != string.Empty &&
                    fileRequest.fileSize != null && fileRequest.fileSize != "0");
        }

        /// <summary>
        /// Reperimento oggetto "FileDocumento".
        /// Il file può anche non essere stato acquisito
        /// </summary>
        /// <param name="documentNumber"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static FileDocumento GetFileDocument(string idDocument, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.documento.FileDocumento retValue = null;

            FileRequest fileRequest = GetFileRequest(idDocument);
            //modifica
            if (fileRequest.fileName == "" || fileRequest.fileName == null)
            {
                DocsPaVO.Caching.InfoFileCaching info = Documenti.CacheFileManager.getFileDaCache(fileRequest.docNumber, fileRequest.versionId, infoUtente.idAmministrazione);
                if (info != null)
                {
                    fileRequest.fileName = info.CacheFilePath;
                    fileRequest.fileSize = info.file_size.ToString();
                }
            }
            //modifica
            // Controllo se il file è stato acquisito o meno
            if (IsAcquired(fileRequest))
            {
                try
                {
                    retValue = Documenti.FileManager.getFile(fileRequest, infoUtente);
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message, ex);

                    throw ex;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento nome ruolo
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        private static string GetRoleName(string idRole)
        {
            string retValue = string.Empty;

            if (idRole != null && idRole != string.Empty)
            {
                Ruolo role = GetRole(idRole);

                retValue = role.codice;
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento oggetto Ruolo
        /// </summary>
        /// <param name="idRole"></param>
        /// <returns></returns>
        private static Ruolo GetRole(string idRole)
        {
            return BusinessLogic.Utenti.UserManager.getRuolo(idRole);
        }

        /// <summary>
        /// Reperimento oggetto InfoUtente proprietario di un blocco
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        private static InfoUtente GetInfoUtente(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            InfoUtente retValue = null;

            Utente user = null;

            if (!string.IsNullOrEmpty(checkOutStatus.IDUser))
                user = BusinessLogic.Utenti.UserManager.getUtente(checkOutStatus.IDUser);
            else if (!string.IsNullOrEmpty(checkOutStatus.UserName))
                user = BusinessLogic.Utenti.UserManager.getUtente(checkOutStatus.UserName, string.Empty);

            if (user != null)
            {
                Ruolo currentRole = null;

                if (!string.IsNullOrEmpty(checkOutStatus.IDRole))
                    currentRole = GetRole(checkOutStatus.IDRole.ToString());

                try
                {
                    retValue = new InfoUtente(user, currentRole);
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);

                    throw ex;
                }
            }
            else
            {
                throw new ApplicationException("Utente non trovato");
            }

            return retValue;
        }

        /// <summary>
        /// Creazione di una nuova versione del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="checkOutStatus"></param>
        /// <param name="checkedOutFile"></param>
        /// <param name="checkInComments"></param>
        /// <returns></returns>
        private static bool CreateDocumentVersion(string idDocument, CheckOutStatus checkOutStatus, byte[] checkedOutFileContent, string checkInComments, InfoUtente checkOutOwner)
        {
            bool retValue = false;

            // Reperimento dell'ultima versione del documento
            FileRequest fileRequest = GetFileRequest(idDocument);

            FileDocumento fileDocument = CreateFileDocument(checkOutStatus.DocumentLocation, checkedOutFileContent);

            if (IsAcquired(fileRequest))
            {
                // Se per l'ultima versione del documento è stato acquisito un file,
                // viene creata nuova versione per il documento
                fileRequest = new FileRequest();
                fileRequest.fileName = checkOutStatus.DocumentLocation;
                fileRequest.docNumber = checkOutStatus.DocumentNumber;

                // Impostazione degli eventuali commenti da aggiungere alla versione
                fileRequest.descrizione = checkInComments;

                fileRequest = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, checkOutOwner, false);
            }
            else
            {
                // Se per l'ultima versione del documento non è stato acquisito un file,
                // il file viene acquisito per l'ultima versione
                fileRequest.fileName = fileDocument.fullName;

                // Impostazione degli eventuali commenti da aggiungere alla versione
                fileRequest.descrizione = checkInComments;
            }

            retValue = (fileRequest != null);

            if (retValue && fileDocument != null &&
                fileDocument.content != null &&
                fileDocument.content.Length > 0)
            {
                // Inserimento del nuovo file per la versione
                fileRequest = BusinessLogic.Documenti.FileManager.putFile(fileRequest, fileDocument, checkOutOwner, false);

                retValue = (fileRequest != null);
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento dello stato di conversione pdf del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        private static bool IsInConversionePdf(string idDocument)
        {
            if (BusinessLogic.Documenti.DocManager.isEnabledConversionePdfServer())
            {
                // Se il documento è in checkout, reperimento dello stato di conversione PDF
                return BusinessLogic.Documenti.DocManager.isDocInConversionePdf(idDocument);
            }
            else
                return false;
        }

        /// <summary>
        /// Reperimento nome libreria per l'amministrazione richiesta
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        private static string GetLibrary(string idAmministrazione)
        {
            return DocsPaDB.Utils.Personalization.getInstance(idAmministrazione).getLibrary();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private static BrokenRule CreateBrokenRule(string id, string description)
        {
            BrokenRule brokenRule = new BrokenRule();
            brokenRule.ID = id;
            brokenRule.Description = description;
            return brokenRule;
        }

        /// <summary>
        /// Creazione di un oggetto FileDocumento a partire dal percorso del file
        /// </summary>
        /// <param name="documentLocation"></param>
        /// <returns></returns>
        private static FileDocumento CreateFileDocument(string documentLocation, byte[] content)
        {
            FileDocumento fileDocument = new FileDocumento();

            FileInfo fileInfo = new FileInfo(documentLocation);
            fileDocument.fullName = fileInfo.FullName;
            fileDocument.name = fileInfo.Name;
            fileDocument.estensioneFile = fileInfo.Extension;
            if (content != null)
            {
                fileDocument.content = content;
                fileDocument.length = content.Length;
            }
            fileDocument.path = GetDocRootPath();

            return fileDocument;
        }

        /// <summary>
        /// Reperimento percorso principale del documentale
        /// </summary>
        /// <returns></returns>
        private static string GetDocRootPath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
        }

        /// <summary>
        /// Creazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string ConcatErrorMessage(string errorMessage, Exception ex)
        {
            string retValue = errorMessage;

            if (retValue != string.Empty)
                retValue += ":" + Environment.NewLine;

            retValue += ex.Message;

            return retValue;
        }

        #endregion


        #region MEV 1.5 F02_01

        /// <summary>
        /// CheckOut di un documento
        /// NON CONSIDERA LA SECURITY
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="documentNumber"></param>
        /// <param name="documentLocation">Percorso in cui deve essere estratto il documento</param>
        /// <param name="machineName"></param>
        /// <param name="utente">Utente che effettua il checkout</param>
        /// <param name="checkOutStatus">Status del documento CheckedOut</param>
        /// <returns></returns>
        public static ValidationResultInfo CheckOutNoSecurity(string idDocument,
                                                    string documentNumber,
                                                    string documentLocation,
                                                    string machineName,
                                                    InfoUtente user,
                                                    out CheckOutStatus checkOutStatus)
        {
            /*
                PreCondizioni:
                Documento in stato NON consolidato
                IDDocumento valido.
                Percorso del file valido.
                Oggetto "utente" valido.
                La funzione può essere utilizzata solamente se il documento richiesto
                non è già in stato CheckOut.
				
                PostCondizioni:
                Viene impostato il blocco sul documento. 
                Per gli altri utenti non sarà possibile effettuare tutte le operazioni
                inerenti all'inserimento di una nuova versione.
            */
            ValidationResultInfo retValue = new ValidationResultInfo();
            checkOutStatus = null;

            // Verifica stato di consolidamento del documento
            if (!BusinessLogic.Documenti.DocumentConsolidation.CanExecuteAction(user, idDocument, Documenti.DocumentConsolidation.ConsolidationActionsDeniedEnum.AddVersions))
            {
                retValue.Value = false;
                retValue.BrokenRules.Add(CreateBrokenRule(DOCUMENT_CONSOLIDATED, MSG_DOCUMENT_CONSOLIDATED));
            }
            else
            {
                // Validazione parametri di input
                if (idDocument == null || idDocument == string.Empty)
                    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_ID_DOCUMENT, MSG_MISSING_ID_DOCUMENT));

                if (documentNumber == null || documentNumber == string.Empty)
                    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_DOCUMENT_NUMBER, MSG_MISSING_DOCUMENT_NUMBER));

                //if (documentLocation==null || documentLocation==string.Empty)
                //    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_PATH_DOCUMENT,MSG_MISSING_PATH_DOCUMENT));

                if (user == null)
                    retValue.BrokenRules.Add(CreateBrokenRule(INVALID_USER, MSG_MISSING_USER));

                retValue.Value = (retValue.BrokenRules.Count == 0);

                if (retValue.Value)
                {
                    try
                    {
                        checkOutStatus = null;

                        CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(user);

                        // Verifica se è il documento, o uno dei suoi componenti, non sia già in stato CheckOut
                        string ownerUser;

                        if (!IsCheckedOutNoSecurity(documentNumber, user, true, out ownerUser))
                        {
                            if (!documentManagement.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatus))
                            {
                                retValue.BrokenRules.Add(CreateBrokenRule(CHECK_OUT_ERROR, MSG_CHECK_OUT_ERROR));
                            }
                            else
                            {
                                // Se il documento è in checkout, reperimento dello stato di conversione PDF
                                checkOutStatus.InConversionePdf = IsInConversionePdf(idDocument);
                            }
                        }
                        else
                        {
                            retValue.BrokenRules.Add(CreateBrokenRule(DOCUMENT_ALREADY_CHECKED_OUT, MSG_DOCUMENT_ALREADY_CHECKED_OUT));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Debug(ex.Message);

                        retValue.BrokenRules.Add(CreateBrokenRule(CHECK_OUT_ERROR, ConcatErrorMessage(MSG_CHECK_OUT_ERROR, ex)));
                    }

                    retValue.Value = (retValue.BrokenRules.Count == 0);
                }
            }

            return retValue;
        }



        /// <summary>
        /// Verifica se il documento principale o uno dei suoi allegati è in checkout, relativamente al parametro checkAllegati,
        /// SENZA CONSIDERARE LA SECURITY
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente"></param>
        /// <param name="checkAllegati"></param>
        /// <param name="ownerUser">
        /// Nome utente proprietario del blocco sul documento
        /// </param>
        /// <returns></returns>
        public static bool IsCheckedOutNoSecurity(string documentNumber, InfoUtente utente, bool checkAllegati, out string ownerUser)
        {
            CheckInOutDocumentManager documentManagement = new CheckInOutDocumentManager(utente);

            ownerUser = string.Empty;


            DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(utente, documentNumber);


            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
            {
                // Impostazione utente che ha bloccato il documento
                ownerUser = schedaDocumento.checkOutStatus.UserName;

                return (!string.IsNullOrEmpty(schedaDocumento.checkOutStatus.ID));
            }

            //Controllo se un allegato è bloccato
            if (checkAllegati)
            {

                foreach (Allegato a in schedaDocumento.allegati)
                {

                    if (documentManagement.IsCheckedOut(a.docNumber, a.docNumber, out ownerUser))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}