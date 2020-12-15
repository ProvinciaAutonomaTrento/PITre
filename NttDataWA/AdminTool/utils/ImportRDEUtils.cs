using System;
using System.Configuration;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.utils
{
    /// <summary>
    /// Questa classe fornisce funzionalità di utilità per l'importazione dell'RDE
    /// </summary>
    public class ImportRDEUtils
    {
        public static ResultsContainer ImportRDE(byte[] content,
            string fileName,
            string serverPath,
            InfoUtente userInfo,
            Ruolo userRole)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;

            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (string.IsNullOrEmpty(valoreChiaveFasc))
                valoreChiaveFasc = "false";
            Boolean.TryParse(
                valoreChiaveFasc,
                out isClassificationRequired);

            try
            {
                // Chiamata del webservice per l'importazione RDE e restituzione 
                // della lista dei risultati
                return ws.ImportRDE(
                    content,
                    fileName,
                    serverPath,
                    userInfo,
                    userRole,
                    isClassificationRequired,
                    Utils.getAbilitazioneSmistamento() == "1",
                    IsEnabledOwnerProto() ? 2 : 1);
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }

        }

        /// <summary>
        /// Funzione per l'estrazione dei dati dal foglio Excel
        /// </summary>
        /// <param name="content">Il contenuto del foglio Excel</param>
        /// <param name="fileName">Il nome da attribuire al file</param>
        /// <returns>L'oggetto con i dati sui documenti da importare</returns>
        public static DocumentRowDataContainer ReadRDEDataFromExcel(
            byte[] content,
            string fileName)
        {
            // Istanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout
            ws.Timeout = System.Threading.Timeout.Infinite;

            // L'oggetto da restiture
            DocumentRowDataContainer toReturn = null;

            // L'eventuale errore avvenuto in fase di estrazione dati
            string errorMessage = String.Empty;

            try
            {
                // Chiamata del metodo web per l'estrazione dei dati dal foglio Excel
                toReturn = ws.ReadRDEDataFromExcel(
                    content,
                    fileName,
                    IsEnabledOwnerProto() ? 2 : 1,
                    out errorMessage);

                // Se si è verificato un errore un esecuzione, viene lanciata un'eccezione con il
                // dettaglio
                if (toReturn == null ||
                    !String.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);

            }
            catch (Exception e)
            {
                throw e;
            }

            return toReturn;
        }


        /// <summary>
        /// Funzione per l'importazione di un documento RDE
        /// </summary>
        /// <param name="rowData">L'oggetto con i dati sul documento da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="serverPath">L'indirizzo della WA</param>
        /// <param name="isSmistamentoEnabled">True se è abilito lo smistamento</param>
        /// <param name="protoType">Il tipo di documento da creare</param>
        /// <returns>Il riusltato dell'importazione</returns>
        public static ImportResult ImportRDEDocument(
            DocumentRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            ProtoType protoType) 
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;

            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (string.IsNullOrEmpty(valoreChiaveFasc))
                valoreChiaveFasc = "false";
            Boolean.TryParse(
                valoreChiaveFasc,
                out isClassificationRequired);

            try
            {
                // Chiamata del webservice per l'importazione RDE e restituzione 
                // della lista dei risultati
                return ws.ImportRDEDocument(
                    rowData,
                    userInfo,
                    role,
                    serverPath,
                    isClassificationRequired,
                    Utils.getAbilitazioneSmistamento() == "1",
                    false,
                    protoType);
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }

        }

        /// <summary>
        /// Funzione per il reperimento del valore associato al flag di presenza dei
        /// protocolli interni
        /// </summary>
        /// <returns>True se devono essere presi in considerazione i protocolli interni</returns>
        public static bool IsEnabledOwnerProto()
        {
            // Il valore da restituire
            bool toReturn = false;

            // Lettura del valore di configurazione
            Boolean.TryParse(
                ConfigurationManager.AppSettings["ENABLE_OWNER_PROTO_FOR_IMPORT_PROCEDURES"],
                out toReturn);

            // Restituzione del risultato
            return toReturn;

        }

    }

}
