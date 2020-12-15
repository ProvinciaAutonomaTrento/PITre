using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Interoperabilita;
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaDB.Query_DocsPAWS;
using log4net;
using Interoperability.Controller;
using DocsPaVO.DatiCert;
using BusinessLogic.Amministrazione;
using System.Collections;
using BusinessLogic.Spedizione;
using DocsPaVO.utente;
using DocsPaVO.Spedizione;
using BusinessLogic.Utenti;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Classe per la gestione delle notifiche di annullamento e di eccezione
    /// </summary>
    public class SimplifiedInteroperabilityRecordDroppedAndExceptionManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SimplifiedInteroperabilityRecordDroppedAndExceptionManager));

        /// <summary>
        /// Metodo per l'invio di una richiesta di generazione di una ricevuta di annullamento o eccezione realtiva ad un documento ricevuto per IS
        /// </summary>
        /// <param name="reason">Ragione di annullamento o dettaglio dell'eccezione</param>
        /// <param name="documentId">Id del documento</param>
        /// <param name="adminCode">Codice dell'amministrazione</param>
        /// <param name="dropped">Flag utilizzato per indicare se il documento è stato cancellato</param>
        /// <returns>Esito dell'operazione</returns>
        public static bool SendDocumentDroppedOrExceptionProofToSender(String reason, String documentId, String adminCode, bool dropped)
        {
            bool sendSucceded = false;

            String ammId = new DocsPaDB.Query_DocsPAWS.Amministrazione().GetIDAmm(adminCode);
            // Se non è attiva l'interoperabilità semplificata non si può procedere
            if (InteroperabilitaSemplificataManager.IsEnabledSimplifiedInteroperability(ammId))
                sendSucceded = GenerateAndSendProof(reason, documentId, ammId, dropped);

            return sendSucceded;
 
        }

        /// <summary>
        /// Metodo per l'invio di una richiesta di generazione di una ricevuta di annullamento o eccezione relativa ad un
        /// documento ricevuto per IS
        /// </summary>
        /// <param name="sender">Informazioni sul mittente</param>
        /// <param name="receiver">Informazioni sul destinatario</param>
        /// <param name="reason">Ragione dell'annullamento o dettaglio dell'eccezione</param>
        /// <param name="dropped">Flag utilizzato per indicare se il documento è stato cancellato</param>
        /// <param name="adminId">Id dell'amministrazione</param>
        /// <param name="senderUrl">Url del mittente</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        /// <returns>Esito della richiesta</returns>
        public static bool SendDocumentDroppedOrExceptionProofToSender(RecordInfo sender, RecordInfo receiver, String reason, bool dropped, String adminId, String senderUrl, String receiverCode)
        {
            // Invio della ricevuta di conferma ricezione
            SendProof(sender, receiver, senderUrl, reason, adminId, dropped, receiverCode);
            return true;

        }

        /// <summary>
        /// Metodo per la generazione della ricevuta e per il suo invio
        /// </summary>
        /// <param name="reason">Ragione di annullamento o dettaglio dell'eccezione</param>
        /// <param name="documentId">Id del documento</param>
        /// <param name="adminId">Id dell'amministrazione</param>
        /// <param name="dropped">Flag utilizzato per indicare se il documento è stato cancellato</param>
        /// <returns>Esito dell'operazione</returns>
        private static bool GenerateAndSendProof(String reason, String documentId, String adminId, bool dropped)
        {
            bool retVal = false;
            using (InteroperabilitaSemplificata dbManager = new InteroperabilitaSemplificata())
            {
                
                RecordInfo sender, receiver;
                String senderUrl, receiverCode;

                if (dbManager.LoadSenderDocInfo(documentId, out sender, out receiver, out senderUrl, out receiverCode))
                {
                    // Invio della ricevuta di conferma ricezione
                    SendProof(sender, receiver, senderUrl, reason, adminId, dropped, receiverCode);
                    retVal = true;
                }
                else
                {
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(documentId, true,
                        String.Format("Errore nel recupero dei dati per la creazione della ricevuta di {0}", dropped ? "annullamento del protocollo" : "rifiuto del documento"));
                    logger.ErrorFormat("Errore nel recupero dei dati per la creazione della ricevuta di {1} per il documento con id {0}", documentId, dropped ? "annullamento del protocollo" : "rifiuto del documento");
                }

            }

            return retVal;
        }

        /// <summary>
        /// Metodo per l'invio della ricevuta
        /// </summary>
        /// <param name="sender">Informazioni sul mittente della spedizione</param>
        /// <param name="receiver">Informazioni sul destinatario della spedizione</param>
        /// <param name="senderUrl">Url del mittente</param>
        /// <param name="reason">Ragione di annullamento o dettaglio dell'eccezione</param>
        /// <param name="adminId">Id dell'amministrazione</param>
        /// <param name="dropped">Flag utilizzato per indicare se si deve inviare una richiesta di eliminazione</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        private static void SendProof(RecordInfo sender, RecordInfo receiver, string senderUrl, String reason, String adminId, bool dropped, String receiverCode)
        {
            InteroperabilityController interoperabilityController = new InteroperabilityController();
            interoperabilityController.AnalyzeDocumentDroppedOrErrorProof(
                new Interoperability.Domain.RecordInfo()
                {
                    AdministrationCode = sender.AdministrationCode,
                    AOOCode = sender.AOOCode,
                    RecordDate = sender.RecordDate,
                    RecordNumber = sender.RecordNumber
                },
                new Interoperability.Domain.RecordInfo()
                {
                    AdministrationCode = receiver.AdministrationCode,
                    AOOCode = receiver.AOOCode,
                    RecordDate = receiver.RecordDate,
                    RecordNumber = receiver.RecordNumber
                },
                reason,
                InteroperabilitaSemplificataManager.GetUrl(adminId),
                senderUrl,
                dropped ? Interoperability.Domain.OperationDiscriminator.Drop : Interoperability.Domain.OperationDiscriminator.Error,
                receiverCode);
            
        }

        /// <summary>
        /// Metodo per il salvataggio dei dati di una ricevuta di eccezione o di annullamento
        /// </summary>
        /// <param name="senderRecordInfo">Informazioni sul protocollo mittente</param>
        /// <param name="receiverRecordInfo">Informazioni sul protocollo destinatario</param>
        /// <param name="reason">Ragione di annullamento o dettaglio dell'eccezione</param>
        /// <param name="receiverUrl">Url del destinatario</param>
        /// <param name="droppedProof">Flag utilizzato per indicare se si tratta di una ricevuta di annullamento</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        public static void SaveProofData(RecordInfo senderRecordInfo, RecordInfo receiverRecordInfo, string reason, string receiverUrl, bool droppedProof, String receiverCode)
        {
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interopDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                bool retVal = interopDb.SaveDocumentDroppedOrExceptionProofData(senderRecordInfo, receiverRecordInfo, reason, receiverUrl, droppedProof, receiverCode);

                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(String.Empty, !retVal,
                    String.Format("Salvataggio delle informazioni sulla ricevuta di {0} relativa al protocollo {1} creato in data {2}, {3}",
                    droppedProof ? "annullamento" : "eccezione",
                    senderRecordInfo.RecordNumber, senderRecordInfo.RecordDate,
                    retVal ? "riuscito correttamente" : "non riuscito"));

                logger.DebugFormat("Salvataggio delle informazioni sulla ricevuta di {0}, per l'interoperabilità semplificata, relativa al protocollo {1} creato in data {2}, {3}",
                    droppedProof ? "annullamento" : "eccezione",
                    senderRecordInfo.RecordNumber, senderRecordInfo.RecordDate,
                    retVal ? "riuscito correttamente" : "non riuscito");

                // Recupero dell'id del documento cui si riferisce l'eccezione
                String idProfile = String.Empty;
                using (DocsPaDB.Query_DocsPAWS.Documenti docDb = new DocsPaDB.Query_DocsPAWS.Documenti())
                {
                    idProfile = docDb.GetIdProfileFromProtoInfo(senderRecordInfo.RecordDate, senderRecordInfo.RecordNumber, senderRecordInfo.AOOCode, senderRecordInfo.AdministrationCode);
                }

                // Se è una eccezione, viene inserita una riga nel registro delle ricevute
                if (!droppedProof)
                {
                    SaveExceptionInRegistry(
                        String.Empty,
                        InteroperabilitaSemplificataManager.GetUrl(OrganigrammaManager.GetIDAmm(senderRecordInfo.AdministrationCode)),
                        DateTime.Now,
                        idProfile,
                        receiverCode,
                        reason);

                   
                    String userId = BusinessLogic.Documenti.DocManager.GetDocumentAttribute(
                        senderRecordInfo.RecordDate,
                        senderRecordInfo.RecordNumber,
                        senderRecordInfo.AOOCode,
                        senderRecordInfo.AdministrationCode,
                        DocsPaDB.Query_DocsPAWS.Documenti.DocumentAttribute.UserId);

                    //Recupero il ruolo che ha effettuato l'ultima spedizione IS, dallo storico delle spedizioni. 
                    ArrayList listHistorySendDoc = SpedizioneManager.GetElementiStoricoSpedizione(idProfile);
                    if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
                    {
                        Object lastSendIs = (from record in listHistorySendDoc.ToArray()
                                             where ((ElStoricoSpedizioni)record).Mail_mittente.Equals("N.A.")
                                             select record).ToList().OrderBy(z => ((ElStoricoSpedizioni)z).Id).LastOrDefault();

                        Ruolo role = UserManager.getRuoloByIdGruppo(((ElStoricoSpedizioni)lastSendIs).IdGroupSender);
                        Utente user = UserManager.getUtente(userId);
                        // LOG per documento
                        string desc = "Notifica di eccezione: " + reason.Replace("’", "'") + "<br/>Destinatario spedizione: " + receiverCode;
                        BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo,
                            user.idAmministrazione, "EXCEPTION_SEND_SIMPLIFIED_INTEROPERABILITY",
                            idProfile, desc, DocsPaVO.Logger.CodAzione.Esito.OK, null, "1");
                    }

                }
                        
            }
        }

        /// <summary>
        /// Metodo per il salvataggio dell'eccezione nel log relativo all'IS
        /// </summary>
        /// <param name="messageId">Identificativo del messaggio</param>
        /// <param name="senderUrl">Url del mittente</param>
        /// <param name="proofDate">Data di generazione della ricevuta</param>
        /// <param name="documentId">Id del documento</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        /// <param name="reason">Ragione di annullamento o dettaglio dell'eccezione</param>
        private static void SaveExceptionInRegistry(String messageId, String senderUrl, DateTime proofDate, String documentId, String receiverCode, String reason)
        {
            TipoNotifica not = InteroperabilitaManager.ricercaTipoNotificaByCodice("eccezione");

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
                    oggetto = reason
                },
                String.Empty);
        }
    }
}
