using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaDB.Query_DocsPAWS;
using log4net;
using DocsPaVO.Logger;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Questa classe si occupa della gestione del log e del registro per l'interoperabilità semplificata
    /// </summary>
    public class SimplifiedInteroperabilityLogAndRegistryManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SimplifiedInteroperabilityLogAndRegistryManager));

        /// <summary>
        /// Metodo per l'inserimento di un item nel registro dei messaggi ricevuti per IS
        /// </summary>
        /// <param name="messageId">Identificativo del messaggio di interoperabilità</param>
        /// <param name="receivedPrivate">Flag utilizzato per indicare se il documento è stato spedito come privato</param>
        /// <param name="subject">Oggetto del documento</param>
        /// <param name="senderDescription">Destiazione del mittente</param>
        /// <param name="senderUrl">Url del mittente</param>
        /// <param name="senderAdministrationCode">Codice dell'amministrazione mittente</param>
        /// <param name="senderAOOCode">Codice dell'AOO mittente</param>
        /// <param name="senderRecordDate">Data di registrazione del protocollo mittente</param>
        /// <param name="senderRecordNumber">Numero del protocollo mittente</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        /// <returns>Esito dell'azione di inserimento</returns>
        public static bool InsertItemInRegistry(String messageId, bool receivedPrivate, String subject, 
            String senderDescription, String senderUrl, String senderAdministrationCode, String senderAOOCode,
            DateTime senderRecordDate, String senderRecordNumber, String receiverCode)
        {
            bool retVal = false;

            using (SimplifiedInteroperabilityRegistryAndLogDbManager dbManager = new SimplifiedInteroperabilityRegistryAndLogDbManager())
            {
                retVal = dbManager.InsertItemInRegistry(new InteroperabilityLogItem()
                    {
                        MessageIdentifier = messageId,
                        ReceivedPrivate = receivedPrivate,
                        Subject = subject,
                        SenderDescription = senderDescription,
                        SenderUrl = senderUrl,
                        SenderRecordInfo = new DocsPaVO.Interoperabilita.Semplificata.RecordInfo()
                            {
                                AdministrationCode = senderAdministrationCode,
                                AOOCode = senderAOOCode,
                                RecordDate = senderRecordDate,
                                RecordNumber = senderRecordNumber
                            },
                        ReceiverCode = receiverCode

                    });
            }

            logger.DebugFormat("Informazioni relative al documento con id {0} ricevuto per interoperabilità semplificata {1}",
                messageId,
                retVal ? "inserite correttamente" : "non inserite");

            return retVal;
            
        }

        /// <summary>
        /// Metodo per l'aggiornamento dell'id profile relativo ad un messaggio ricevuto per interoperabilità semplificata
        /// </summary>
        /// <param name="messageId">Identificativo (GUID) del messaggio da aggiornare</param>
        /// <param name="idProfile">Id del documento generato</param>
        /// <returns>Esito dell'operazione di aggiornamento</returns>
        public static bool SetIdProfileForMessage(String messageId, String idProfile)
        {
            bool retVal = false;

            using (SimplifiedInteroperabilityRegistryAndLogDbManager dbManager = new SimplifiedInteroperabilityRegistryAndLogDbManager())
            {
                retVal = dbManager.SetIdProfileForMessage(messageId, idProfile);
            }

            logger.DebugFormat("Informazioni relative al documento con id {0} ricevuto per interoperabilità semplificata {1}",
                idProfile,
                retVal ? "aggiornate correttamente" : "non aggiornate");

            return retVal;
 
        }

        /// <summary>
        /// Metodo per l'inserimento di un item nel log
        /// </summary>
        /// <param name="profileId">Id del documento</param>
        /// <param name="isErrorMessage">Flag utilizzato per indicare se si desidera inserire un errore</param>
        /// <param name="text">Testo del messaggio da loggare</param>
        /// <returns>Esito dell'operazione di inserimento</returns>
        public static bool InsertItemInLog(String profileId, bool isErrorMessage, String text)
        {
            bool retVal = false;
            using (SimplifiedInteroperabilityRegistryAndLogDbManager dbManager = new SimplifiedInteroperabilityRegistryAndLogDbManager())
            {
                retVal = dbManager.InsertItemInLog(new InteroperabilityLogItem()
                {
                    ProfileId = String.IsNullOrEmpty(profileId) ? 0 : Int32.Parse(profileId),
                    IsErrorMessage = isErrorMessage,
                    LogMessage = text
                });
                
            }
            if (isErrorMessage)
            {
                logger.ErrorFormat("Messaggio {0}relativo al documento con id {0} con testo '{1}' {2}",
                    isErrorMessage ? "di eccezione " : "",
                    profileId,
                    text,
                    retVal ? "inserite correttamento" : "non inserito");
            }
            else
            {
                logger.DebugFormat("Messaggio {0}relativo al documento con id {0} con testo '{1}' {2}",
                    isErrorMessage ? "di eccezione " : "",
                    profileId,
                    text,
                    retVal ? "inserite correttamento" : "non inserito");
            }

            return retVal;
        }
    }
}
