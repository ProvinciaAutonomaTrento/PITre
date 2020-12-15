using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Interoperability.Interfaces;
using Interoperability.Domain;
using BusinessLogic.interoperabilita.Semplificata;
using Interoperability.Domain.FaultException;
using System.Runtime.Remoting.Contexts;
using Interoperability.Domain.Proof;
using BusinessLogic.interoperabilita.Semplificata.Exceptions;
using System.Web;
using BusinessLogic.Utenti;
using BusinessLogic.Amministrazione;
using Interoperability.Domain.FileExchange;
using System.IO;
using BusinessLogic.NotificationCenter;

namespace DocsPaWS
{
    public class InteroperabilityService : IInteroperabilityService
    {
        /// <summary>
        /// Elaborazione di un messaggio di richiesta interoperabilità
        /// </summary>
        /// <param name="interoperabilityMessage">Dettaglio della richiesta</param>
        /// <returns>Id del messaggio di interoperabilità</returns>
        /// <exception cref="Interoperability.Domain.FaultException.InteroperabilityFault">Eccezione generata durante l'analisi della richiesta</exception>
        [FaultContract(typeof(InteroperabilityFault))]
        public ElaborateInteroperabilityMessageResult ElaborateNewInteroperabilityMessage(InteroperabilityMessage interoperabilityMessage)
        {
            ElaborateInteroperabilityMessageResult retVal = new ElaborateInteroperabilityMessageResult();
            try
            {
                // Salvataggio del messaggio nel registro dei messaggi ricevuti
                retVal.MessageId = SimplifiedInteroperabilityRequestManager.SaveMessageReference(interoperabilityMessage);

            }
            catch (SaveMessageReferenceException e)
            {
                // Si è verificato un errore durante il salvataggio di un riferimento alla richiesta
                // di interoperabilità.
                retVal.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage()
                {
                    ErrorMessage = e.Message,
                    Receivers = interoperabilityMessage.Receivers
                });

            }

            try
            {
                // Analisi del messaggio
                retVal.DocumentDelivered = SimplifiedInteroperabilityRequestManager.AnalyzeInteroperabilityMessage(interoperabilityMessage, retVal.MessageId);
            }
            catch (RetrivingSenderInfoException retrivingSenderInfo)
            {
                // Il mittente non è stato individuato, una delle possibili cause è che il mittente non è in rubrica
                // comune
                retVal.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage()
                {
                    ErrorMessage = retrivingSenderInfo.Message,
                    Receivers = interoperabilityMessage.Receivers
                });

            }
            catch (SimplifiedInteroperabilityLoadSettingsException simplifiedInteroperabilityLoadSettings)
            {
                // Il sistema non è riuscito a recuperare le impostazioni sui registri. 
                retVal.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage()
                {
                    ErrorMessage = simplifiedInteroperabilityLoadSettings.Message,
                    Receivers = interoperabilityMessage.Receivers
                });

            }
            catch (SimplifiedInteroperabilityException simplifiedInteroperabilityException)
            {
                // Per alcuni destinatari si è verificata qualche eccezione
                simplifiedInteroperabilityException.Requests.ForEach(request =>
                    retVal.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage()
                    {
                        ErrorMessage = request.ErrorMessage,
                        Receivers = request.ReceiverInfoes
                    }));
            }
            catch (Exception notRecognized)
            {
                // Si è verificata un errore non identificato, quindi viene considerata non soddisfatta la richiesta
                // di interoperabilità
                retVal.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage()
                {
                    ErrorMessage = "Errore non identificato",
                    Receivers = interoperabilityMessage.Receivers
                });
                
            }

            // Restituzione del risultato dell'elaborazione
            return retVal;

        }

        /// <summary>
        /// Metodo per l'analisi della ricevuta di conferma ricezione al mittente
        /// </summary>
        /// <param name="senderRecordInfo">Informazioni sul protocollo inviato dal mittente</param>
        /// <param name="receiverRecordInfo">Informazioni sul protocollo creato dal destinatario</param>
        /// <param name="receiverUrl">Url da utilizzare per contattare il mittente</param>
        /// <param name="receiverCode">Codice del destinatario per cui generare la conferma di ricezione</param>
        public void AnalyzeDocumentReceivedProof(
            RecordInfo senderRecordInfo,
            RecordInfo receiverRecordInfo,
            String receiverUrl,
            String receiverCode)
        {
            try
            {
                SimplifiedInteroperabilityProtoManager.SaveReceivedProofData(
                    new DocsPaVO.Interoperabilita.Semplificata.RecordInfo()
                    {
                        AdministrationCode = senderRecordInfo.AdministrationCode,
                        AOOCode = senderRecordInfo.AOOCode,
                        RecordDate = senderRecordInfo.RecordDate,
                        RecordNumber = senderRecordInfo.RecordNumber
                    },
                    new DocsPaVO.Interoperabilita.Semplificata.RecordInfo()
                    {
                        AdministrationCode = receiverRecordInfo.AdministrationCode,
                        AOOCode = receiverRecordInfo.AOOCode,
                        RecordDate = receiverRecordInfo.RecordDate,
                        RecordNumber = receiverRecordInfo.RecordNumber
                    },
                    receiverUrl,
                    receiverCode);
            }
            catch (Exception e)
            {
                throw new FaultException<InteroperabilityFault>(
                    new InteroperabilityFault
                    {
                        Exceptions = new List<Exception>() { e }
                    },
                    new FaultReason(e.Message));
            }

        }

        /// <summary>
        /// Metodo per l'analisi di una richiesta di generazione di una ricevuta di eccezione o di eliminazione
        /// di un documento
        /// </summary>
        /// <param name="senderRecordInfo">Informazioni sul protocollo mittente</param>
        /// <param name="receiverRecordInfo">Informazioni sul protocollo destinatario</param>
        /// <param name="reason">Ragione della cancellazione del documento o dettaglio errore</param>
        /// <param name="receiverUrl">Url del mittente della spedizione</param>
        /// <param name="operation">Tipo di ricevuta da generare</param>
        /// <param name="receiverCode">Codice del destinatario per cui generare la ricevuta</param>
        public void AnalyzeDocumentDroppedOrErrorMessageProof(
            RecordInfo senderRecordInfo,
            RecordInfo receiverRecordInfo,
            String reason,
            String receiverUrl,
            OperationDiscriminator operation,
            String receiverCode)
        {
            try
            {
                SimplifiedInteroperabilityRecordDroppedAndExceptionManager.SaveProofData(
                    new DocsPaVO.Interoperabilita.Semplificata.RecordInfo()
                    {
                        AdministrationCode = senderRecordInfo.AdministrationCode,
                        AOOCode = senderRecordInfo.AOOCode,
                        RecordDate = senderRecordInfo.RecordDate,
                        RecordNumber = senderRecordInfo.RecordNumber
                    },
                    new DocsPaVO.Interoperabilita.Semplificata.RecordInfo()
                    {
                        AdministrationCode = receiverRecordInfo.AdministrationCode,
                        AOOCode = receiverRecordInfo.AOOCode,
                        RecordDate = receiverRecordInfo.RecordDate,
                        RecordNumber = receiverRecordInfo.RecordNumber
                    },
                    reason,
                    receiverUrl,
                    operation == OperationDiscriminator.Drop,
                    receiverCode);


                // Se l'operazione richiesta non è la segnalazione di eliminazione di un documento
                // viene inviato un item al centro notifiche
                if (operation != OperationDiscriminator.Drop && NotificationCenterHelper.IsEnabled(OrganigrammaManager.GetIDAmm(senderRecordInfo.AdministrationCode)))
                {
                    // Recupero dell'id del ruolo creatore del documento e dell'id del documento
                    String userId = BusinessLogic.Documenti.DocManager.GetDocumentAttribute(
                        senderRecordInfo.RecordDate,
                        senderRecordInfo.RecordNumber,
                        senderRecordInfo.AOOCode,
                        senderRecordInfo.AdministrationCode,
                        DocsPaDB.Query_DocsPAWS.Documenti.DocumentAttribute.UserId);
                    String documentId = BusinessLogic.Documenti.DocManager.GetDocumentAttribute(
                        senderRecordInfo.RecordDate,
                        senderRecordInfo.RecordNumber,
                        senderRecordInfo.AOOCode,
                        senderRecordInfo.AdministrationCode,
                        DocsPaDB.Query_DocsPAWS.Documenti.DocumentAttribute.IdProfile);

                    NotificationCenterHelper.InsertItem(
                        receiverCode,
                        reason,
                        "Notifica di eccezione",
                        Convert.ToInt32(userId),
                        "IS",
                        Int32.Parse(documentId),
                        Int32.Parse(senderRecordInfo.RecordNumber),
                        senderRecordInfo.AdministrationCode);
                }

            }
            catch (Exception e)
            {
                throw new FaultException<InteroperabilityFault>(
                    new InteroperabilityFault
                    {
                        Exceptions = new List<Exception>() { e }
                    },
                    new FaultReason(e.Message));
            }

        }

    }
}
