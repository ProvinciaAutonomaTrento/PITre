using System;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.utils
{
    public class ImportAddressBookUtils
    {
        /// <summary>
        /// Funzione per l'estrazione dei dati relativi a corrispondenti da foglio Excel
        /// </summary>
        /// <param name="content">Il contenuto del file Excel</param>
        /// <param name="fileName">Il nome da attribuire al file temporaneo</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <returns>Un oggetto con le informazioni sui corrispondenti su cui compiere le azioni</returns>
        public static AddressBookRowDataContainer ReadDataFromExcel(
            byte[] content,
            string fileName,
            InfoUtente userInfo)
        {
            // Istanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout
            ws.Timeout = System.Threading.Timeout.Infinite;

            // L'oggetto da restiture
            AddressBookRowDataContainer toReturn = null;

            // L'eventuale errore avvenuto in fase di estrazione dati
            string errorMessage = String.Empty;

            try
            {
                // Chiamata del metodo web per l'estrazione dei dati dal foglio Excel
                toReturn = ws.ReadAddressBookImportDataFromExcel(
                    content,
                    fileName,
                    userInfo,
                    out errorMessage);

                // Se si è verificato un errore un esecuzione, viene lanciata un'eccezione con il
                // dettaglio
                if (toReturn == null &&
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
        /// Funzione per l'importazione di un corrispondente nella rubrica
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="operation">L'operazione da compiere sul corrispondente</param>
        /// <param name="rowData">L'oggetto con le informazioni sul corrispondente su cui compiere l'operazione richiesta</param>
        /// <returns>Il risultato dell'elaborazione</returns>
        public static AddressBookImportResult ImportAddressBookElement(
            InfoUtente userInfo,
            Ruolo role,
            OperationEnum operation,
            AddressBookRowData rowData)
        {
            // Il risultato dell'inserimento
            AddressBookImportResult toReturn;

            // Riferimento al web service a cui richiedere l'operazione
            DocsPaWebService ws;

            // Istanziazione di un riferimento al Web Service e impostazione del timeout infinito
            ws = new DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                // Esecuzione dell'importazione del corrispondente
                toReturn = ws.ImportAddressBookElement(
                    userInfo,
                    role,
                    System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"].Equals("1") ? 1 : 0,
                    operation,
                    rowData);
            }
            catch (Exception e)
            {
                throw e;
            }

            // Restituzione del risultato
            return toReturn;
 
        }
    
    }
}