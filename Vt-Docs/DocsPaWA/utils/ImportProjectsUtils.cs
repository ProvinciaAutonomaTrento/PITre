using System;
using DocsPAWA.DocsPaWR;
using System.Configuration;

namespace DocsPAWA.utils
{
    /// <summary>
    /// Questa classe fornisce funzionalità di utilità per l'importazione di fascicoli
    /// </summary>
    public class ImportProjectsUtils
    {

        public static ImportResult[] ImportProjects(
            byte[] content, 
            string fileName, 
            string serverPath, 
            InfoUtente userInfo,
            Ruolo userRole)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                // Chiamata del webservice per l'importazione dei fascicoli da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportProjects(
                    content,
                    fileName,
                    serverPath,
                    userInfo,
                    userRole,
                    Utils.getAbilitazioneSmistamento() == "1");
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        /// <summary>
        /// Funzione per l'invocazione del metodo web per l'estrazione dei dati 
        /// riguardanti fascicoli da foglio Excel
        /// </summary>
        /// <param name="content">Il contenuto del foglio Excel</param>
        /// <param name="fileName">Il nome da attribuire al file temporaneo</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <returns>La lista dei dati estratti dal foglio Excel passato per parametro</returns>
        public static ProjectRowData[] ReadDataFromExcel(
            byte[] content,
            string fileName,
            InfoUtente userInfo,
            Ruolo role)
        {
            // Istanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                // Chiamata del metodo web per l'estrazione dei dati dal foglio Excel
                return ws.ReadProjectDataFromExcel(
                    content,
                    fileName,
                    userInfo,
                    role);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Funzione per l'invocazione del metodo web per l'importazione di un fascicolo i cui dati
        /// sono descritti in un oggetto specifico
        /// </summary>
        /// <param name="rowData">L'oggetto con la descrizione dei dati riguardanti il fascicolo da importare</param>
        /// <param name="userInfo">Le informazioni sull'utente che ha lanciato la procedura</param>
        /// <param name="role">Il ruolo con cui è stata lanciata la procedura</param>
        /// <param name="serverPath">L'indirizzo web della WA</param>
        /// <returns>Il risultato dell'importazione</returns>
        public static ImportResult ImportProject(
            ProjectRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath)
        {
            // Istanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                // Chiamata del metodo web per l'importazione del fascicolo
                return ws.ImportProject(
                    rowData,
                    userInfo,
                    role,
                    Utils.getAbilitazioneSmistamento() == "1",
                    serverPath);
            }
            catch (Exception e)
            {
                throw e;
            }
 
        }

        /// <summary>
        /// Funzione per il reperimento del massimo numero di fascicoli importabili
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente</param>
        /// <returns>Il numero massimo di fascicoli che è possibile importare</returns>
        public static int GetMaxProjectsNumber(InfoUtente userInfo)
        {
            // Il valore da restituire
            int toReturn;

            // Valore utilizzato per indicare se il reperimento è andato a buon fine
            bool success;

            // Reperimento del valore
            success = Int32.TryParse(ConfigurationManager.AppSettings["MAX_PRJ_CAN_IMPORT"], out toReturn);
            
            // Se non è stato reperito un valore, viene restituita un'eccezione
            if (!success)
                throw new Exception("Errore durante il reperimento del massimo numero di fascicoli che è possibile importare. Contattare l'amministratore.");

            // Restituzione del valore
            return toReturn;

        }

    }

}
