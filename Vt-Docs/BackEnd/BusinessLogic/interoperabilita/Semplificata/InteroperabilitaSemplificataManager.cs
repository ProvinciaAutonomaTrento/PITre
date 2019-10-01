using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Interoperabilita.Semplificata;
using log4net;
using BusinessLogic.interoperabilita.Semplificata.Exceptions;
using DocsPaVO.utente;
using Interoperability.Domain;
using BusinessLogic.Utenti;
using DocsPaDB.Query_DocsPAWS;
using BusinessLogic.Amministrazione;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Classe per la gestione della logica di business relativa all'interoperabilità semplificata
    /// </summary>
    public class InteroperabilitaSemplificataManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaSemplificataManager));

        public const String InteroperabilityCode = "SIMPLIFIEDINTEROPERABILITY";

        /// <summary>
        /// Metodo per il salvataggio delle impostazioni relative all'interoperabilità semplificata
        /// </summary>
        /// <param name="interoperabilitySettings">Impostazioni da salvare</param>
        /// <returns>Esito del sdalvataggio</returns>
        public static bool SaveSettings(InteroperabilitySettings interoperabilitySettings)
        {
            logger.DebugFormat("Salvataggio impostazioni di interoperabilità semplificata per il registro con id {0}",
                interoperabilitySettings.RegistryId);

            // Validazione dei dati prima di salvare
            ValidateData(interoperabilitySettings);

            bool saved = false;
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interoperabilitaSemplificataDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                saved = interoperabilitaSemplificataDb.SaveSettings(interoperabilitySettings);
            }

            logger.DebugFormat("Salvataggio impostazioni di interoperabilità semplificata per il registro con id {0}, {1}",
                interoperabilitySettings.RegistryId,
                saved ? "effettuato con successo" : "non riuscito.");

            return saved;

        }

        /// <summary>
        /// Metodo per la validazione dei dati da salvare
        /// </summary>
        /// <param name="interoperabilitySettings">Impostazioni da salvare</param>
        private static void ValidateData(InteroperabilitySettings interoperabilitySettings)
        {
            SimplifiedInteroperabilitySaveSettingsException exc = new SimplifiedInteroperabilitySaveSettingsException();

            // Se è attiva l'interoperabilità ma non c'è nè un ruolo nè un utente impostato per la creazione del 
            // predisposto
            if (interoperabilitySettings.IsEnabledInteroperability && interoperabilitySettings.RoleId == 0 &&
                interoperabilitySettings.UserId == 0)
                exc.AddInvalidField("Specificare ruolo e utente da utilizzare per la creazione del documento predisposto");

            // Se c'è almeno un avviso, viene lanciata l'eccezione
            if (exc.GetInvalidFields().Count > 0)
                throw exc;
        }

        /// <summary>
        /// Metodo per il caricamento delle impostazioni di interoperabilità semplificata per un registro / RF
        /// </summary>
        /// <param name="registryId">Id del registro / RF</param>
        /// <returns>Impostazioni</returns>
        public static InteroperabilitySettings LoadSettings(String registryId)
        {
            logger.DebugFormat("Caricamento impostazioni di interoperabilità semplificata per il registro con id {0}",
                registryId);

            InteroperabilitySettings loadedData = null;
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interoperabilitaSemplificataDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                loadedData = interoperabilitaSemplificataDb.LoadSettings(registryId);
            }

            logger.DebugFormat("Caricamento impostazioni di interoperabilità semplificata per il registro con id {0}, {1}",
                registryId,
                loadedData != null ? "effettuato con successo" : "non riuscito.");

            return loadedData;

        }

        /// <summary>
        /// Metodo per la verifica di abilitazione di un oggetto all'IS
        /// </summary>
        /// <param name="objectId">Id dell'oggetto</param>
        /// <param name="rf">Booleano che indica se si desidera verificare un RF</param>
        /// <returns>Esito della verifica</returns>
        public static bool IsElementInteroperable(String objectId, bool rf)
        {
            logger.DebugFormat("Caricamento stato di abilitazione dell'{0} con id {1} all'interoperabilità semplificata",
                rf ? "RF" : "UO",
                objectId);

            bool retVal = false;
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interoperabilitaSemplificataDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                retVal = interoperabilitaSemplificataDb.IsElementInteroperable(objectId, rf);
            }

            logger.DebugFormat("Caricamento stato di abilitazione dell'{0} con id {1} all'interoperabilità semplificata, {2}",
                rf ? "RF" : "UO",
                objectId,
                retVal ? "effettuato con successo" : "non riuscito.");

            return retVal;

        }

        /// <summary>
        /// Metodo utilizzato per verificare se, per l'amministrazione corrente è attiva la funzionalità di 
        /// interoperabilità semplificata
        /// </summary>
        /// <param name="adminId">Id dell'amministrazione</param>
        /// <returns>Flag che indica lo stato di attivazione dell'interoperabilità</returns>
        public static bool IsEnabledSimplifiedInteroperability(String adminId)
        {
            string enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(adminId, "INTEROP_SERVICE_ACTIVE");
            if (String.IsNullOrEmpty(enabled))
                enabled = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "INTEROP_SERVICE_ACTIVE");

            return enabled == "1";

        }

        /// <summary>
        /// Metodo per il recupero dell'url di accesso all'IS per questa istanza
        /// </summary>
        /// <param name="administrationId">Id dell'amministrazione</param>
        /// <returns>Url del servizio di accesso</returns>
        public static String GetUrl(String administrationId)
        {
            String valoreChiaveInteropUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(administrationId, "INTEROP_SERVICE_URL");
            if (string.IsNullOrEmpty(valoreChiaveInteropUrl))
                valoreChiaveInteropUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "INTEROP_SERVICE_URL");

            return valoreChiaveInteropUrl;
        }

        public static String GetFileManagerUrl(String administrationId)
        {
            String valoreChiaveInteropUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(administrationId, "FILE_SERVICE_URL");
            if (string.IsNullOrEmpty(valoreChiaveInteropUrl))
                valoreChiaveInteropUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FILE_SERVICE_URL");

            return valoreChiaveInteropUrl;

 
        }

        /// <summary>
        /// Metodo per l'analisi delle informazioni sulla spedizione del documento. Al termine dell'esecuzione di questo
        ///  metodo saranno caricate le impostazioni relative alle AOO cui è indirizzato il documento e una descrizione
        ///  dei destinatari del documento
        /// </summary>
        /// <param name="receiverInfo">Lista dei destinatari della spedizione</param>
        /// <param name="adminCode">Codice dell'amministrazione mittente</param>
        /// <returns>Dizionario che ha per chiave le impostazioni relative al registro e per valore la lista dei destinatari sul registro</returns>
        internal static Dictionary<InteroperabilitySettings, List<ReceiverInfo>> LoadSettings(List<ReceiverInfo> receiverInfo, String adminCode)
        {
            // Dictionary da restituire
            Dictionary<InteroperabilitySettings, List<ReceiverInfo>> retVal = new Dictionary<InteroperabilitySettings, List<ReceiverInfo>>();

            // Splitting dei corrispondenti per codice AOO
            List<List<ReceiverInfo>> splittedReceivers = receiverInfo.GroupBy(r => r.AOOCode).Select(group => group.ToList()).ToList();

            // Analisi degli insiemi di corrispondenti
            foreach (var receiver in splittedReceivers)
                retVal.Add(
                    LoadSettings(RegistriManager.getIdRegistroIS(adminCode, receiver[0].AOOCode)),
                    receiver);

            return retVal;
        }

        /// <summary>
        /// Metodo per il caricamento delle informazioni sui destinatari interni cui effettuare le trasmissioni del 
        /// documento.
        /// </summary>
        /// <param name="settings">Impostazioni del registro per cui caricare i destinatari della trasmissione interna</param>
        /// <param name="privateDocument">Flag utilizzato per indicare se il documento è stato rivevuto come privato</param>
        /// <param name="receiversInfo">Lista dei destinatari della spedizione per cui caricare i destinatari interni cui trasmettere il documento</param>
        /// <param name="uneachableReceivers">Lista dei destinatari non raggiungibili (per il registro / RF non esiste alcun ruolo con la microfunzione per la ricezione della trasmissione del predisposto</param>
        /// <returns>Lista dei corrispondenti raggiunti dalla spedizione</returns>
        internal static List<Corrispondente> LoadTransmissionReceivers(InteroperabilitySettings settings, bool privateDocument, List<ReceiverInfo> receiversInfo, out List<ReceiverInfo> uneachableReceivers)
        {
            logger.Debug("RAFFREDDORE - START");
            logger.Debug("RAFFREDDORE - private document " + privateDocument.ToString());
            List<Corrispondente> receivers = new List<Corrispondente>();
            List<Corrispondente> tempReceivers = new List<Corrispondente>();
            uneachableReceivers = new List<ReceiverInfo>();

            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interopDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                foreach (ReceiverInfo receiver in receiversInfo)
                {
                    tempReceivers = new List<Corrispondente>();
                    // Il codice del corrispondente pubblicato in Rubrica Comune è composto da CodiceAmministrazione-CodiceCorrispondente
                    String corrCode = receiver.Code;
                    logger.Debug("RAFFREDDORE - receiver.Code " + corrCode);
                    if (receiver.Code.Length - (receiver.AdministrationCode.Length + 1) > 0)
                        corrCode = receiver.Code.Substring(receiver.AdministrationCode.Length + 1);

                    logger.Debug("RAFFREDDORE - receiver.Code " + corrCode);
                    String rfId = RegistriManager.GetSystemIdRFDaDPA_EL_REGISTRI(corrCode);
                    logger.Debug("RAFFREDDORE - rfId " + rfId);
                    
                    // Se il destinatario è una UO, la ricerca viene effettuata sull'id del registro
                    // altirmenti viene effettuata sull'id dell'RF
                    if (!String.IsNullOrEmpty(rfId))
                        tempReceivers = interopDB.LoadTransmissionReceiverData(rfId, privateDocument);
                    else
                    {
                        String corrType = UserManager.GetInternalCorrAttributeByCorrCode(
                            corrCode,
                            DocsPaDB.Query_DocsPAWS.Utenti.CorrAttribute.cha_tipo_urp,
                            OrganigrammaManager.GetIDAmm(receiver.AdministrationCode));

                        if(!String.IsNullOrEmpty(corrType) && corrType == "U")
                            tempReceivers = interopDB.LoadTransmissionReceiverData(settings.RegistryId, privateDocument);
                    }
   
                    // Aggiunta dei destinatari trovati all'insieme dei destinatari
                    receivers.AddRange(tempReceivers);

                    // Se tempReceivers non contiene elementi significa che per il destinatario analizzato non è stato
                    // trovato alcun destinatario per la trasmissione, quindi per il destinatario analizzato deve essere
                    // generata una ricevuta di mancata consegna
                    if (tempReceivers == null || tempReceivers.Count == 0)
                        uneachableReceivers.Add(receiver);

                }
            }

            return receivers;
        }

        /// <summary>
        /// Questo metodo verifica se un documento ricevuto per interoperabilità semplificata
        /// è stato ricevuto marcato come privato
        /// </summary>
        /// <param name="documentId">Id del documento da controllare</param>
        /// <returns>Flag indicante se il documento è stato ricevuto marcato come privato</returns>
        public static bool IsDocumentReceivedPrivate(String documentId)
        {
            bool retVal = false;
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interopDb =  new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                retVal = interopDb.IsDocumentReceivedPrivate(documentId);
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per verificare se un documento è stato ricevuto per IS
        /// </summary>
        /// <param name="documentId">Id del documento da verificare</param>
        /// <returns>Esito della verifica</returns>
        public static bool IsDocumentReceivedWithIS(String documentId)
        {
            bool retVal = false;
            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata interopDb = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
            {
                retVal = interopDb.IsDocumentReceivedWithIS(documentId);
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il caricamento del mittete di una spedizione
        /// </summary>
        /// <param name="documentId">Id del documento</param>
        /// <param name="sender">Verrà valorizzato con le informazioni sul protocollo mittente</param>
        /// <param name="receiver">Verrà popolato con le informazioni sul destinatario della spedizione</param>
        /// <param name="senderUrl">Url del mittente</param>
        /// <param name="receiverCode">Codice del destinatario</param>
        /// <returns>Esito del caricamento</returns>
        public static bool LoadSenderDocInfo(String documentId, out DocsPaVO.Interoperabilita.Semplificata.RecordInfo sender, out DocsPaVO.Interoperabilita.Semplificata.RecordInfo receiver, out String senderUrl, out String receiverCode)
        {
            bool retVal = false;
            using (InteroperabilitaSemplificata dbManager = new InteroperabilitaSemplificata())
                retVal = dbManager.LoadSenderDocInfo(documentId, out sender, out receiver, out senderUrl, out receiverCode);

            return retVal;
        }

        /// <summary>
        /// Metodo per il caricamento del mittente a partire dall'id della UO
        /// </summary>
        /// <param name="uoId">Id della UO</param>
        /// <returns>Codice del mittente</returns>
        public static String LoadSenderInfoFromUoId(String uoId)
        {
            using (InteroperabilitaSemplificata dbManager = new InteroperabilitaSemplificata())
            {
                return dbManager.LoadSenderInfoFromUoId(uoId);
            }
 
        }

        /// <summary>
        /// MEtodo per verificare se un corrispondente è abilitato all'IS
        /// </summary>
        /// <param name="corrId">Id del corrispondente da verificare</param>
        /// <returns>Esito della verifica</returns>
        public static bool IsCorrEnabledToInterop(String corrId)
        {
            using (InteroperabilitaSemplificata dbManager = new InteroperabilitaSemplificata())
            {
                return dbManager.IsCorrEnabledToInterop(corrId);
            }
        }

        /// <summary>
        /// PEC 4 Modifica Maschera Caratteri
        /// Metodo di aggiornamento della status mask per i consegne e mancate consegne IS.
        /// </summary>
        /// <param name="statusmask">Stringa da inserire nella status mask</param>
        /// <param name="codAOO"></param>
        /// <param name="codAmm"></param>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static bool AggiornaStatusMask(string statusmask, string codAOO, string codAmm, string idDocument)
        {
            using (InteroperabilitaSemplificata dbManager = new InteroperabilitaSemplificata())
            {
                return dbManager.AggiornaStatusMask(statusmask, codAOO, codAmm, idDocument);
            }
        }

        /// <summary>
        /// PEC 4 update della status mask per le spedizioni IS
        /// </summary>
        /// <param name="idDoc"></param>
        /// <returns></returns>
        public static bool IS_statusMaskUpdater(string idDoc)
        {
            using (InteroperabilitaSemplificata dbManager = new InteroperabilitaSemplificata())
            {
                return dbManager.IS_statusMaskUpdater(idDoc);
            }
        }

    }
}
