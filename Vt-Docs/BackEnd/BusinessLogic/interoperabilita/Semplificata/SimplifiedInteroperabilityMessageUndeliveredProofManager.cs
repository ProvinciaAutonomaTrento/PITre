using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interoperability.ProofGenerator;
using DocsPaVO.DatiCert;
using Interoperability.Domain.Proof;
using DocsPaVO.utente;
using BusinessLogic.Utenti;
using DocsPaVO.documento;
using BusinessLogic.Documenti;
using Interoperability.Domain;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Questa classe gestisce le ricevute di mancata consegna di un documento spedito per IS
    /// </summary>
    public class SimplifiedInteroperabilityMessageUndeliveredProofManager
    {
        /// <summary>
        /// Metodo per la generazione della ricevuta di mancata consegna
        /// </summary>
        /// <param name="interoperabilityMessage">Messaggio di interoperabilità</param>
        /// <param name="messageId">Id del messaggio ricevuto</param>
        /// <param name="exception">Errore verificatosi</param>
        /// <param name="receiver">Destinatario per cui si è verificata la mancata consegna</param>
        public static void GenerateProof(Interoperability.Domain.InteroperabilityMessage interoperabilityMessage, String messageId, String exception, ReceiverInfo receiver)
        {
            // Inizializzazione della ricevuta
            UnsuccessfullyDeliveredGenerator proof = new UnsuccessfullyDeliveredGenerator(
                interoperabilityMessage.Record.Subject,
                exception);
            
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
            InteroperabilitaSemplificataManager.AggiornaStatusMask("XNXNNNN", receiver.AOOCode, receiver.AdministrationCode, interoperabilityMessage.MainDocument.DocumentNumber);
        }

        /// <summary>
        /// Metodo per l'aggiunta della ricevuta al documento
        /// </summary>
        /// <param name="documentId">Id del documento cui associare la ricevuta</param>
        /// <param name="messageId">Id del messaggio di interoperabilità</param>
        /// <param name="data">content della ricevuta</param>
        /// <param name="proofDate">Data di generazione della ricevuta</param>
        /// <param name="receiverCode">Codice del corrispondente cui non è stato possibile spedire</param>
        private static void AddProofToDocument(String documentId, String messageId, byte[] data, DateTime proofDate, String receiverCode)
        {
            SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, false, "Aggiunta della Ricevuta di mancata consegna al documento");

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
        /// Metodo per l'aggiunta dell'allegato al documento spedito
        /// </summary>
        /// <param name="documentId">Id del documento a aggiungere l'allegato</param>
        /// <param name="recData">Contenuto della ricevuta</param>
        /// <param name="userInfo">Informazioni sull'utente da utilizzare per l'aggiunta dell'allegato</param>
        /// <param name="messageId">Identificativo del messaggio</param>
        /// <param name="proofDate">Data di generazione della ricevuta</param>
        /// <param name="receiverCode">Codice del destinatario che ha generato la mancata consegna</param>
        /// <param name="senderUrl">Url del mittente</param>
        private static void AddAttachmentToDocument(String documentId, byte[] recData, InfoUtente userInfo, String messageId, DateTime proofDate, String receiverCode, String senderUrl)
        {
            // Creazione dell'oggetto allegato
            Allegato a = new Allegato();

            // Impostazione delle proprietà dell'allegato
            a.descrizione = String.Format("Ricevuta di mancata consegna - " + receiverCode);

            a.docNumber = documentId;
            a.version = "0";
            a.numeroPagine = 1;
            a.TypeAttachment = 3;
            try
            {
                // Aggiunta dell'allegato al documento principale
                a = AllegatiManager.aggiungiAllegato(userInfo, a);

                // Set del flag in CHA_ALLEGATI_ESTERNO in Versions
                BusinessLogic.Documenti.AllegatiManager.setFlagAllegati_PEC_IS_EXT(a.versionId, a.docNumber, "I");

                // Associazione dell'immagine all'allegato
                FileDocumento fileDocument = new FileDocumento() { content = recData, length = recData.Length, name = "MancataConsegna.pdf" };
                FileRequest request = a as FileRequest;
                String err;
                FileManager.putFileRicevuteIs(ref request, fileDocument, userInfo, out err);

                // Aggiunta della notifica alla tabella delle notifiche
                SaveProofInRegistry(a, messageId, receiverCode, senderUrl, proofDate, documentId);


                if (!String.IsNullOrEmpty(err))
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, true,
                        String.Format("Errore durante l'associazione della ricevuta di mancata consegna al documento"));
                else
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, false,
                        String.Format("Ricevuta di mancata consegna associata correttamente al documento"));

            }
            catch (Exception e)
            {
                string message = e.Message + " " + e.StackTrace;
                message = message.Length > 3700 ? message.Substring(0, 3700) : message;
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, true,
                    String.Format("Errore durante l'aggiunta dell'allegato con la ricevuta di mancata consegna al documento " + documentId + " : " + message));
            }

        }

        /// <summary>
        /// Metodo per il salvataggio della ricevuta nel registro delle ricevute di interoperabilità
        /// </summary>
        /// <param name="attachment">Allegato da inserire nel registro</param>
        /// <param name="messageId">Identificativo del messaggio</param>
        /// <param name="receiverCode">Codice del destinatario che non è stato possibile raggiungere</param>
        /// <param name="senderUrl">Url del mittente</param>
        /// <param name="proofDate">Data di generazione della ricevuta</param>
        /// <param name="documentId">Id del documento cui aggiungere l'allegato</param>
        private static void SaveProofInRegistry(Allegato attachment, String messageId, String receiverCode, String senderUrl, DateTime proofDate, String documentId)
        {
            TipoNotifica not = InteroperabilitaManager.ricercaTipoNotificaByCodice("errore-consegna");

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
                    oggetto = "Ricevuta di mancata consegna"
                },
                attachment.versionId);
        }
    }
}
