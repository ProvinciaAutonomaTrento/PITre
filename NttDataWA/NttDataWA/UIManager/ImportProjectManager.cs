using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using log4net;

namespace NttDataWA.UIManager
{

    /// <summary>
    /// Questa classe fornisce funzionalità di utilità per l'importazione di fascicoli
    /// </summary>
    public class ImportProjectManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(ImportProjectManager));
        private static DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

        public static ImportResult[] ImportProjects(
            byte[] content,
            string fileName,
            string serverPath,
            InfoUtente userInfo,
            Ruolo userRole)
        {
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
                    false);
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
            // Impostazione del timeout
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                // Chiamata del metodo web per l'importazione del fascicolo
                return ws.ImportProject(
                    rowData,
                    userInfo,
                    role,
                    false,
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
            int toReturn = 500;

            // Reperimento del valore
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MAX_PRJ_CAN_IMPORT.ToString()]))
                toReturn = int.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.MAX_PRJ_CAN_IMPORT.ToString()]);

            // Restituzione del valore
            return toReturn;

        }

    }


    /// <summary>
    /// Classe per l'esecuzione, attraverso thread, dell'importazione fascicoli
    /// </summary>
    public class ImportProjectExecutor
    {
        #region Dichiarazione Variabili

        // L'oggetto con il report di esecuzione
        List<ImportResult> report;

        // Il numero di fascicoli totali da importare ed il numero di
        // fascicoli analizzati
        int totalNumberOfProjectToImport, totalNumberOfAnalyzedProjects;

        #endregion

        /// <summary>
        /// Costruttore di default.
        /// </summary>
        public ImportProjectExecutor()
        {
            // Inizializzazione del report
            this.report = new List<ImportResult>();

            // Inizializzazione del numero di fascicoli importati
            this.totalNumberOfAnalyzedProjects = 0;

            // Inizializzazione del numero di fascicoli da importare
            this.totalNumberOfProjectToImport = 0;
        }

        /// <summary>
        /// Funzione richiamata dal Thread per l'esecuzione dell'importazione
        /// </summary>
        /// <param name="prameters">Un array di oggetti. Sono attesi 4 elementi:
        ///     - Il primo deve essere una lista di oggetti di tipo ProjectRowData
        ///     - Il secondo deve essere un oggetto di tipo string
        ///     - Il terzo deve essere un oggetto di tipo InfoUtente
        ///     - Il quarto deve essere un oggetto di tipo Ruolo
        /// </param>
        public void ExecuteImport(object parameters)
        {
            #region Dichiarazione variabili

            // L'array dei parametri
            object[] param;

            // L'oggetto con le informazioni sui fascicoli da importare
            ProjectRowData[] prds;

            // L'url del frontend
            string url;

            // Le informazioni sull'utente che ha lanciato la procedura
            InfoUtente userInfo;

            // Il ruolo con cui è stata lanciata la procedura
            Ruolo role;

            #endregion

            // Casting dell'oggetto a array di oggetti
            param = parameters as object[];

            // Se l'array non contiene quattro elementi, fine esecuzione
            if (param.Length != 4)
                return;

            // Prelevamento delle informazioni sui fascicoli da creare
            prds = param[0] as ProjectRowData[];

            // Prelevamento dell'url del Fontend
            url = param[1] as string;

            // Prelevamento delle informazioni sull'utente
            userInfo = param[2] as InfoUtente;

            // Prelevamento del ruolo
            role = param[3] as Ruolo;

            // Calcolo del numero totale di fascicoli da importare
            this.totalNumberOfProjectToImport =
                prds.Length;

            #region Importazione fascicoli

            // Inizializzazione della lista da utilizzare per
            // memorizzare il report sull'importazione dei
            // fascicoli
            this.report = new List<ImportResult>();

            // Importazione dei fascicoli
            foreach (ProjectRowData rowData in prds)
            {
                try
                {
                    this.report.Add(ImportProjectManager.ImportProject(
                                rowData,
                                userInfo,
                                role,
                                url));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    this.report.Add(new ImportResult()
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Ordinal = rowData.OrdinalNumber,
                        Message = e.Message
                    });

                }

                // Un documenti è stato analizzato
                this.totalNumberOfAnalyzedProjects += 1;

            }

            // Aggiunta di una voce di report con il totale dei fascicoli
            // importati e non importati
            this.report.Add(this.GetSummaryElement(this.report));

            #endregion

            // Sospensione del thread fino a quando non viene letto
            // il report o non passano 1 minuto
            //waitReading.WaitOne(new TimeSpan(0,1,0));

        }

        /// <summary>
        /// Funzione per la creazione di un elemento di summary
        /// </summary>
        /// <param name="tempReport">L'array con gli elementi di cui creare un summary</param>
        /// <returns>L'elemento con il summary</returns>
        private ImportResult GetSummaryElement(List<ImportResult> tempReport)
        {
            // L'oggetto da restituire
            ImportResult toReturn;

            // Il numero di risultati negativi
            int unsuccessNumber = 0;

            // Calcolo dei risultati negativi
            unsuccessNumber = tempReport.Where(e => e.Outcome == OutcomeEnumeration.KO).Count();

            // Inizializzazione dell'elemento
            toReturn = new ImportResult()
            {
                Outcome = OutcomeEnumeration.OK,
                Message = String.Format(
                    "Fascicoli importati correttamente: {0}. Fascicoli non importati {1}",
                    tempReport.Count - unsuccessNumber, unsuccessNumber)
            };

            // Restituzione dell'elemento creato
            return toReturn;

        }

        /// <summary>
        /// Funzione richiamabile per conoscere il numero di fascicoli analizzati
        /// ed il numero di fascicoli totali
        /// </summary>
        /// <param name="analyzed">Numero di fascicoli analizzati</param>
        /// <param name="total">Numero totale di fascicoli</param>
        public void GetStatistics(out int analyzed, out int total)
        {
            // Impostazione dei valori richiesti
            analyzed = this.totalNumberOfAnalyzedProjects;
            total = this.totalNumberOfProjectToImport;

        }

        /// <summary>
        /// Funzione per la restituzione del report
        /// </summary>
        /// <returns></returns>
        public List<ImportResult> GetReport()
        {
            // Viene risvegliato il thread
            //waitReading.Set();

            // Viene restituito il report
            return this.report;
        }
    }

}