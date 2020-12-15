using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.Import.Fascicoli
{
    /// <summary>
    /// Classe per l'esecuzione, attraverso thread, dell'importazione fascicoli
    /// </summary>
    public class AsyncImportProjectExecutor
    {
        #region Dichiarazione Variabili

        // Oggetto utilizzato per la sincronizzazione del thread.
        // Il thread viene messo in attesa al termine dell'esecuzione.
        // Il risveglio avviene non appena qualcuno preleva il report
        // o automaticamente dopo un numero prestabilito di minuti.
        ManualResetEvent waitReading;

        // L'oggetto con il report di esecuzione
        List<ImportResult> report;

        // Il numero di fascicoli totali da importare ed il numero di
        // fascicoli analizzati
        int totalNumberOfProjectToImport, totalNumberOfAnalyzedProjects;

        #endregion

        /// <summary>
        /// Costruttore di default.
        /// </summary>
        public AsyncImportProjectExecutor()
        {
            // Inizializzazione del report
            this.report = new List<ImportResult>();

            // Inizializzazione del numero di fascicoli importati
            this.totalNumberOfAnalyzedProjects = 0;

            // Inizializzazione del numero di fascicoli da importare
            this.totalNumberOfProjectToImport = 0;

            // Inizializzazione dell'oggetto di sincronizzazione manuale
            this.waitReading = new ManualResetEvent(false);

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
                    this.report.Add(ImportProjectsUtils.ImportProject(
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