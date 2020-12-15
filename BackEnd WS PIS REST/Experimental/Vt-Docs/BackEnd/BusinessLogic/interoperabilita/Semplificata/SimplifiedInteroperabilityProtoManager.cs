using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Interoperabilita.Semplificata;
using log4net;
using Interoperability.Controller;
using BusinessLogic.Amministrazione;
using System.Collections;
using DocsPaVO.utente;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Questa classe fornisce tutti i metodi di utilità per la gestione della fase
    /// in cui un documento viene protocollato e bisogna quindi inviare al mittente
    /// la ricevuta di conferma di ricezione
    /// </summary>
    public class SimplifiedInteroperabilityProtoManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SimplifiedInteroperabilityProtoManager));

        /// <summary>
        /// Metodo per l'invio della ricevuta di conferma di ricezione 
        /// al mittente del messaggio di interoperabilità
        /// </summary>
        /// <param name="idProfile">Id del documento per cui inviare la conferma di ricezione</param>
        /// <param name="userId">Usr id dell'utente mittente dell'invio della ricevuta</param>
        /// <param name="adminId">Id dell'amministrazione mittente della ricevuta</param>
        /// <param name="recCode">Codice del destinatario che invia la conferma di ricezione</param>
        private static void SendDocumentReceivedProofToSender(String idProfile, String userId, String adminId, String recCode)
        {
            logger.DebugFormat("Invio ricevuta di conferma di ricezione per interoperabilità semplificata del documento con id {0}", idProfile);

            // Recupero delle informazioni sul mittente del documento
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata dbManager = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                RecordInfo sender, receiver;
                String senderUrl, receiverCode;
                if (dbManager.LoadSenderDocInfo(idProfile, out sender, out receiver, out senderUrl, out receiverCode))
                {
                    String rec = recCode;
                    // Se la recCode è vuoto, la notifica viene inviata per tutti i destinatari
                    if (String.IsNullOrEmpty(recCode))
                        rec = receiverCode;

                    // Invio della ricevuta di conferma ricezione
                    SendProof(sender, receiver, senderUrl, idProfile, userId, adminId, rec);

                }
                else
                {
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(idProfile, true,
                        String.Format("Errore nel recupero dei dati per la creazione della ricevuta di conferma ricezione"));
                    logger.ErrorFormat("Errore nel recupero dei dati per la creazione della ricevuta di conferma ricezione per il documento con id {0}", idProfile);
                }

            }

        }

        /// <summary>
        /// Metodo per l'invio della ricevuta di conferma ricezione
        /// </summary>
        /// <param name="senderRecordInfo">Informazioni sul protocollo mittente</param>
        /// <param name="receiverRecordInfo">Informaizoni sul protocollo destinatario</param>
        /// <param name="senderUrl">Url del mittente della richiesta di interoperabilità</param>
        /// <param name="idProfile">Id del documento creato a seguito della richiesta di interoperabilità</param>
        /// <param name="userId">User id dell'utente mittente della ricevuta</param>
        /// <param name="adminId">Id dell'amministrazione mittente</param>
        /// <param name="receiverCode">Codice del destinatario per cui generare la ricevuta</param>
        internal static void SendProof(RecordInfo senderRecordInfo, RecordInfo receiverRecordInfo, String senderUrl, String idProfile, String userId, String adminId, String receiverCode)
        {
            InteroperabilityController interoperabilityController = new InteroperabilityController();

            try
            {
                // Invio della ricevuta al mittente
                try
                {
                    interoperabilityController.AnalyzeDocumentReceivedProof(
                        new Interoperability.Domain.RecordInfo()
                        {
                            AdministrationCode = senderRecordInfo.AdministrationCode,
                            AOOCode = senderRecordInfo.AOOCode,
                            RecordDate = senderRecordInfo.RecordDate,
                            RecordNumber = senderRecordInfo.RecordNumber
                        },
                        new Interoperability.Domain.RecordInfo()
                        {
                            AdministrationCode = receiverRecordInfo.AdministrationCode,
                            AOOCode = receiverRecordInfo.AOOCode,
                            RecordDate = receiverRecordInfo.RecordDate,
                            RecordNumber = receiverRecordInfo.RecordNumber
                        },
                        senderUrl,
                        InteroperabilitaSemplificataManager.GetUrl(adminId),
                        receiverCode);


                }
                catch (Exception e)
                {
                    logger.Error("Errore durante l'invio della ricevuta di conferma ricezione al mittente: " + e.Message);
                    SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(idProfile, true,
                        String.Format("Errore durante l'invio della ricevuta di conferma ricezione al mittente da parte dell'utente {0}", userId));
                    logger.ErrorFormat("Errore durante l'invio della ricevuta di conferma ricezione al mittente da parte dell'utente {0}", userId);
                }

                // Aggiunta voce di log
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(idProfile, false,
                    String.Format("Ricevuta di conferma di ricezione al mittente con indirizzo '{0}' da parte dell'utente {1} inviata correttamente.",
                    senderUrl, userId));

            }
            catch (Exception e)
            {
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(idProfile, true,
                    String.Format("Errore durante l'invio della ricevuta di conferma di ricezione al mittente con indirizzo '{0}' da parte dell'utente {1}",
                    senderUrl, userId));

                logger.Error(
                        String.Format("Errore durante l'invio della ricevuta di conferma di ricezione al mittente con indirizzo '{0}' da parte dell'utente {1}",
                            senderUrl, userId),
                        e);
            }

        }

        /// <summary>
        /// Metodo per il salvataggio delle informazioni su una ricevuta di conferma di ricezione
        /// </summary>
        /// <param name="senderRecordInfo">Informazioni sul protocollo mittente</param>
        /// <param name="receiverRecordInfo">Informaizoni sul protocollo destinatario</param>
        /// <param name="receiverUrl">Url del destinatario della spedizione</param>
        /// <param name="receiverCode">Codice del destinatario che ha inviato la conferma di ricezione</param>
        public static void SaveReceivedProofData(
            RecordInfo senderRecordInfo,
            RecordInfo receiverRecordInfo,
            String receiverUrl,
            String receiverCode)
        {
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interopDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                bool retVal = interopDb.SaveReceivedProofData(senderRecordInfo, receiverRecordInfo, receiverUrl, receiverCode);

                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog("", !retVal,
                    String.Format("Salvataggio delle informazioni sulla ricevuta di conferma di ricezione relativa al protocollo {0} creato in data {1}, {2}",
                    senderRecordInfo.RecordNumber, senderRecordInfo.RecordDate,
                    retVal ? "riuscito correttamente" : "non riuscito"));

                logger.DebugFormat("Salvataggio delle informazioni sulla ricevuta di conferma di ricezione, per l'interoperabilità semplificata, relativa al protocollo {0} creato in data {1}, {2}",
                    senderRecordInfo.RecordNumber, senderRecordInfo.RecordDate,
                    retVal ? "riuscito correttamente" : "non riuscito");

            }

        }

        /// <summary>
        /// Metodo per l'invio della ricevuta di conferma di ricezione per singolo destinatario
        /// </summary>
        /// <param name="idProfile">Id del documento per cui inviare la ricevuta</param>
        /// <param name="userInfo">Informazioni sull'utente mittente della ricevuta</param>
        /// <param name="registryId">Id del registro su cui è stato protocollato il documento</param>
        public static void SendDocumentReceivedProofToSender(String idProfile, InfoUtente userInfo, String registryId)
        {
            ArrayList rfs = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(
                                userInfo.idCorrGlobali,
                                "1",
                                registryId);

            StringBuilder receiverCodes = new StringBuilder();
            foreach (Registro rf in rfs)
            {
                String tempCode = SimplifiedInteroperabilityProtoManager.IsRfReceiverOfDocumentTransmission(
                    rf.codRegistro,
                    idProfile,
                    rf.codAmministrazione);
                if (!String.IsNullOrEmpty(tempCode))
                    receiverCodes.AppendFormat("'{0}', ", tempCode);
            }
            
            // Pulizia della lista dei codici corrispondente
            if (receiverCodes.Length != 0)
                receiverCodes = receiverCodes.Remove(receiverCodes.Length - 2, 2);

            // Invio della ricevuta di conferma di ricezione al mittente
            SendDocumentReceivedProofToSender(
                idProfile,
                userInfo.userId,
                userInfo.idAmministrazione,
                receiverCodes.ToString());

        }

        /// <summary>
        /// Metodo per l'invio di una ricevuta di conferma di ricezione per ogni destinatario di una spedizione (utilizzato
        /// nel caso di protocollazione automatica)
        /// </summary>
        /// <param name="interoperabilityMessage">Informazioni sul messaggio di interoperabilità</param>
        /// <param name="receiverRecordInfo">Informazioni sul protocollo creato lato destinazione</param>
        /// <param name="idProfile">Id del documento prodotto dal destinatario</param>
        /// <param name="userInfo">Informazioni sull'utente protocollatore</param>
        public static void SendDocumentReceivedProofToSender(Interoperability.Domain.InteroperabilityMessage interoperabilityMessage, RecordInfo receiverRecordInfo, String idProfile, InfoUtente userInfo)
        {
            RecordInfo senderRecordInfo = new RecordInfo()
            {
                AdministrationCode = interoperabilityMessage.Record.AdministrationCode,
                AOOCode = interoperabilityMessage.Record.AOOCode,
                RecordDate = interoperabilityMessage.Record.RecordDate,
                RecordNumber = interoperabilityMessage.Record.RecordNumber
            };

            interoperabilityMessage.Receivers.ForEach(r =>
                SendProof(senderRecordInfo,
                receiverRecordInfo,
                interoperabilityMessage.Sender.Url,
                idProfile,
                userInfo.userId,
                userInfo.idAmministrazione,
                String.Format("'{0}'", r.Code)));
    
        }

        /// <summary>
        /// Metodo per verificare se un RF compare fra i destinatari di una spedizione per un dato documento. L'individuazione
        /// si basa sul fatto che nel registro delle richieste di interoperabilità, è presente un campo (ReceiverCode) in cui
        /// sono riportati tutti i destinatari con il codice nel formato RC, racchiusi fra singoli apici e separati da virgola
        /// </summary>
        /// <param name="rfCode">Codice RF da verificare</param>
        /// <param name="documentId">Id del documento in cui verficare se compare l'RF come destinatario</param>
        /// <param name="administrationCode">Codice dell'amministrazione</param>
        /// <returns>Codice del corrispondente del destinatario</returns>
        internal static String IsRfReceiverOfDocumentTransmission(String rfCode, String documentId, String administrationCode)
        {
            // Costruzione del codice del corrispondente
            String corrCode = String.Format("{0}-{1}", administrationCode, rfCode);

            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata simpDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                if (simpDb.IsCorrInReceivers(documentId, corrCode))
                    return corrCode;
                else
                    return String.Empty;
            }

        }

    }
}
