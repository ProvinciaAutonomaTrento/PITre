using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Documenti;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaVO.Interoperabilita.Semplificata;
using BusinessLogic.Utenti;
using Interoperability.ProofGenerator;
using DocsPaVO.DatiCert;
using Interoperability.Domain.Proof;
using Interoperability.Domain;
using log4net;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Questa classe fornisce metodi di utilità per la gestione delle ricevute di 
    /// avvenuta consegna relative all'interoperabilità semplificata
    /// </summary>
    public class SimplifiedInteroperabilityMessageDeliveredProofManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SimplifiedInteroperabilityMessageDeliveredProofManager));

        /// <summary>
        /// Metodo per la generazione della ricevuta di avvenuta consegna
        /// </summary>
        /// <param name="interoperabilityMessage">Messaggio per cui generare la ricevuta</param>
        /// <param name="messageId">Id del messaggio ricevuto</param>
        /// <param name="receiver">Destinatario raggiunto</param>
        public static void GenerateProof(Interoperability.Domain.InteroperabilityMessage interoperabilityMessage, String messageId, ReceiverInfo receiver, InfoDocumentDelivered documentDelivered)
        {
            // Inizializzazione della ricevuta
            SuccessfullyDeliveredGenerator proof = new SuccessfullyDeliveredGenerator(
                interoperabilityMessage.Record.Subject, documentDelivered);

            // Generazione ricevuta
            ProofMessage message = proof.GenerateProof(
                interoperabilityMessage.Sender.Code,
                receiver.Code,
                messageId);

            AddProofToDocument(
                interoperabilityMessage.MainDocument.DocumentNumber,
                messageId,
                message.ProofContent,
                message.ProofDate,
                message.Receiver);
            // PEC 4 Modifica Maschera Caratteri
            InteroperabilitaSemplificataManager.AggiornaStatusMask("ANVAAAN", receiver.AOOCode, receiver.AdministrationCode, interoperabilityMessage.MainDocument.DocumentNumber);
        
        }

        /// <summary>
        /// Metodo per l'aggiunta della ricevuta di avvenuta consegna al documento spedito
        /// </summary>
        /// <param name="documentId">Id del documento cui aggiungere l'allegato</param>
        /// <param name="messageId">Identificativo del messaggio ricevuto</param>
        /// <param name="data">Contenuto della ricevuta</param>
        /// <param name="proofDate">Data di generazione della ricevuta</param>
        /// <param name="receiverCode">Codice del destinatario raggiunto</param>
        private static void AddProofToDocument(String documentId, String messageId, byte[] data, DateTime proofDate, String receiverCode)
        {
            SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, false, "Aggiunta della Ricevuta di avvenuta consegna al documento");

            String authorId, creatorRole;
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata dbInterop = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                // Recupero dell'id del documento e degli id di utente e ruolo con cui creare l'allegato
                dbInterop.LoadDataForDeliveredProof(documentId, out authorId, out creatorRole);

                // Recupero delle informazioni sull'utente
                Utente user = UserManager.getUtenteById(authorId);
                user.dst = UserManager.getSuperUserAuthenticationToken();
                Ruolo role = UserManager.getRuoloById(creatorRole);
                InfoUtente userInfo = UserManager.GetInfoUtente(user, role);

                // Creazione e aggiunta dell'allegato
                AddAttachmentToDocument(documentId, data, userInfo, messageId, proofDate, receiverCode, InteroperabilitaSemplificataManager.GetUrl(userInfo.idAmministrazione));

            }
        }

        /// <summary>
        /// Metodo per l'aggiunta dell'allegato al documento
        /// </summary>
        /// <param name="documentId">Id del documento cui aggiungere l'allegato</param>
        /// <param name="recData">Contenuto della ricevuta</param>
        /// <param name="userInfo">Inormazioni sull'utente utilizzato per la generazione dell'allegato</param>
        /// <param name="messageId">Identificativo del messaggio ricevuto</param>
        /// <param name="proofDate">Data di generazione della ricevuta</param>
        /// <param name="receiverCode">Codice del destinatario per cui generare la ricevuta</param>
        /// <param name="senderUrl">Url del mittente</param>
        private static void AddAttachmentToDocument(String documentId, byte[] recData, InfoUtente userInfo, String messageId, DateTime proofDate, String receiverCode, String senderUrl)
        {
            // Creazione dell'oggetto allegato
            Allegato a = new Allegato();

            // Impostazione delle proprietà dell'allegato
            a.descrizione = String.Format("Ricevuta di avvenuta consegna - " + receiverCode);

            a.docNumber = documentId;
            a.version = "0";
            a.numeroPagine = 1;
            a.TypeAttachment = 3;
            try
            {
                bool usingTransactionContext = string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOT_USING_TRANSACTION_CONTEXT")) ||
                    !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOT_USING_TRANSACTION_CONTEXT").Equals("1");
                // Aggiunta dell'allegato al documento principale
                if (usingTransactionContext)
                {
                    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                    {
                        logger.Debug("INIZIO aggiungiAllegato per il documento " + documentId);
                        a = AllegatiManager.aggiungiAllegato(userInfo, a);
                        logger.Debug("FINE aggiungiAllegato per il documento " + documentId + ". Id allegato creato: " + a.docNumber + " con id versione: " + a.versionId);

                        // Set del flag in CHA_ALLEGATI_ESTERNO in Versions
                        logger.Debug("INIZIO setFlagAllegati_PEC_IS_EXT per l'allegato con docnumber" + a.docNumber + "e version id: " + a.versionId + ". Documento principale:" + documentId);
                        bool resultSetFlag = BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(a.versionId, a.docNumber, "I");
                        logger.Debug("FINE setFlagAllegati_PEC_IS_EXT per l'allegato " + a.docNumber + "con esito: " + resultSetFlag + ". Documento principale:" + documentId);

                        // Associazione dell'immagine all'allegato
                        FileDocumento fileDocument = new FileDocumento() { content = recData, length = recData.Length, name = "AvvenutaConsegna.pdf" };
                        FileRequest request = a as FileRequest;
                        String err;
                        logger.Debug("INIZIO putFile per l'allegato " + a.docNumber);
                        FileManager.putFileRicevuteIs(ref request, fileDocument, userInfo, out err);
                        logger.Debug("FINE putFile per l'allegato " + a.docNumber);

                        // Aggiunta della notifica alla tabella delle notifiche
                        SaveProofInRegistry(a, messageId, receiverCode, senderUrl, proofDate, documentId);


                        if (!String.IsNullOrEmpty(err))
                        {
                            logger.Error("Errore durante la procedura di putfile per l'allegato: " + a.docNumber + ". Testo dell'errore: " + err);
                            SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, true,
                                String.Format("Errore durante l'associazione della ricevuta di avvenuta consegna inviata da al documento"));
                        }
                        else
                        {
                            logger.Debug("Putfile avvenuta correttamente per l'allegato  " + a.docNumber);
                            SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, false,
                                String.Format("Ricevuta di avvenuta consegna associata correttamente al documento"));
                        }

                        if (resultSetFlag && string.IsNullOrEmpty(err))
                        {
                            transactionContext.Complete();
                        }
                    }
                }
                else
                {
                    logger.Debug("INIZIO aggiungiAllegato per il documento " + documentId);
                    a = AllegatiManager.aggiungiAllegato(userInfo, a);
                    logger.Debug("FINE aggiungiAllegato per il documento " + documentId + ". Id allegato creato: " + a.docNumber + " con id versione: " + a.versionId);

                    // Set del flag in CHA_ALLEGATI_ESTERNO in Versions
                    logger.Debug("INIZIO setFlagAllegati_PEC_IS_EXT per l'allegato con docnumber" + a.docNumber + "e version id: " + a.versionId + ". Documento principale:" + documentId);
                    bool resultSetFlag = BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(a.versionId, a.docNumber, "I");
                    logger.Debug("FINE setFlagAllegati_PEC_IS_EXT per l'allegato " + a.docNumber + "con esito: " + resultSetFlag + ". Documento principale:" + documentId);

                    // Associazione dell'immagine all'allegato
                    FileDocumento fileDocument = new FileDocumento() { content = recData, length = recData.Length, name = "AvvenutaConsegna.pdf" };
                    FileRequest request = a as FileRequest;
                    String err;
                    logger.Debug("INIZIO putFile per l'allegato " + a.docNumber);
                    FileManager.putFileRicevuteIs(ref request, fileDocument, userInfo, out err);
                    logger.Debug("FINE putFile per l'allegato " + a.docNumber);

                    // Aggiunta della notifica alla tabella delle notifiche
                    SaveProofInRegistry(a, messageId, receiverCode, senderUrl, proofDate, documentId);


                    if (!String.IsNullOrEmpty(err))
                    {
                        logger.Error("Errore durante la procedura di putfile per l'allegato: " + a.docNumber + ". Testo dell'errore: " + err);
                        SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, true,
                            String.Format("Errore durante l'associazione della ricevuta di avvenuta consegna inviata da al documento"));
                    }
                    else
                    {
                        logger.Debug("Putfile avvenuta correttamente per l'allegato  " + a.docNumber);
                        SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, false,
                            String.Format("Ricevuta di avvenuta consegna associata correttamente al documento"));
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in AddAttachmentToDocument per l'allegato: " + a.docNumber + ". Errore: " + e.Message);
                string message = e.Message + " " + e.StackTrace;
                message = message.Length > 3700 ? message.Substring(0, 3700) : message;
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, true,
                    String.Format("Errore durante l'aggiunta dell'allegato con la ricevuta di avvenuta consegna al documento " + documentId + " : " + message));
            }
        }

        /// <summary>
        /// Metodo per inserire la ricevuta nel registro delle ricevute
        /// </summary>
        /// <param name="attachment">Allegato da aggiungere</param>
        /// <param name="messageId">Identificativo del messaggio di interoperabilità</param>
        /// <param name="receiverCode">Codice del destinatario per cui è stata generata la ricevuta</param>
        /// <param name="senderUrl">Url del mittente della spedizione</param>
        /// <param name="proofDate">Data di generazione della ricevuta</param>
        /// <param name="documentId">Id del documento cui aggiungere l'allegato</param>
        private static void SaveProofInRegistry(Allegato attachment, String messageId, String receiverCode, String senderUrl, DateTime proofDate, String documentId)
        {
            TipoNotifica not = InteroperabilitaManager.ricercaTipoNotificaByCodice("avvenuta-consegna");

            InteroperabilitaManager.inserimentoNotifica(
                new DocsPaVO.DatiCert.Notifica()
                {
                    consegna = "1",
                    data_ora = proofDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    destinatario = receiverCode,
                    docnumber = documentId,
                    idTipoNotifica = not.idTipoNotifica,
                    mittente = senderUrl,
                    msgid = messageId,
                    risposte = String.Empty,
                    oggetto = "Ricevuta di avvenuta consegna"
                },
                attachment.versionId);
        }

    }
}
