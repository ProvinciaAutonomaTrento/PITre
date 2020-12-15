using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.Import.Rubrica
{
    /// <summary>
    /// Questa classe viene utilizzata come esecutrice dell'importazione di corrispondenti
    /// nella rubrica
    /// </summary>
    public class AsyncImportAddressBookExecutor
    {
        #region Dichiarazione Variabili

        // Oggetto utilizzato per la sincronizzazione del thread.
        // Il thread viene messo in attesa al termine dell'esecuzione.
        // Il risveglio avviene non appena qualcuno preleva il report
        // o automaticamente dopo 5 minuti.
        ManualResetEvent waitReading;

        // L'oggetto con il report di esecuzione
        AddressBookImportResultContainer report;

        // Il numero di corrispondenti totali da importare ed il numero di
        // corrispondenti analizzati
        int totalNumberOfElementToImport, totalNumberOfAnalyzedElements;

        #endregion

        /// <summary>
        /// Costruttore di default.
        /// </summary>
        public AsyncImportAddressBookExecutor()
        {
            // Inizializzazione del report
            this.report = new AddressBookImportResultContainer();

            // Inizializzazione del numero di corrispondenti importati
            this.totalNumberOfAnalyzedElements = 0;

            // Inizializzazione del numero di corrispondenti da importare
            this.totalNumberOfElementToImport = 0;

            // Inizializzazione dell'oggetto di sincronizzazione manuale
            this.waitReading = new ManualResetEvent(false);

        }

        /// <summary>
        /// Funzione richiamata dal Thread per l'esecuzione dell'importazione
        /// </summary>
        /// <param name="prameters">Un array di oggetti. Sono attesi 4 elementi:
        ///     - Il primo deve essere un oggetto di tipo AddressBookRowDataContainer
        ///     - Il secondo deve essere un oggetto di tipo string
        ///     - Il terzo deve essere un oggetto di tipo InfoUtente
        ///     - Il quarto deve essere un oggetto di tipo Ruolo
        /// </param>
        public void ExecuteImport(object parameters)
        {
            #region Dichiarazione variabili

            // L'array dei parametri
            object[] param;

            // L'oggetto con le informazioni sui corrispondenti da importare
            AddressBookRowDataContainer abrdc;

            // L'url del frontend
            string url;

            // Le informazioni sull'utente che ha lanciato la procedura
            InfoUtente userInfo;

            // Il ruolo con cui è stata lanciata la procedura
            Ruolo role;

            // Lista in cui depositare temporaneamente il report
            // di importazione relativo ad un insieme di corrispondenti
            // su cui è stata compiuta la stessa operazione
            List<AddressBookImportResult> tempReport;

            #endregion

            // Casting dell'oggetto a array di oggetti
            param = parameters as object[];

            // Se l'array non contiene quattro elementi, fine esecuzione
            if (param.Length != 4)
                return;

            // Prelevamento delle informazioni sui corrispondenti da creare
            abrdc = param[0] as AddressBookRowDataContainer;

            // Prelevamento dell'url del Fontend
            url = param[1] as string;

            // Prelevamento delle informazioni sull'utente
            userInfo = param[2] as InfoUtente;

            // Prelevamento del ruolo
            role = param[3] as Ruolo;

            // Calcolo del numero totale di corrispondenti su cui compiere operazioni
            this.totalNumberOfElementToImport =
                abrdc.ToDelete.Length +
                abrdc.ToInsert.Length +
                abrdc.ToModify.Length;

            #region Inserimento nuovi corrispondenti

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sull'inserimento
            // dei corrispondenti
            tempReport = new List<AddressBookImportResult>();

            // Inserimento dei corrispondenti
            foreach (AddressBookRowData rowData in abrdc.ToInsert)
            {
                try
                {
                    tempReport.Add(ImportAddressBookUtils.ImportAddressBookElement(
                        userInfo,
                        role,
                        OperationEnum.I,
                        rowData));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new AddressBookImportResult()
                    {
                        Result = ResultEnum.KO,
                        Message = e.Message
                    });

                }

                // Un corrispondente è stato analizzato
                totalNumberOfAnalyzedElements += 1;

            }

            // Aggiunta di una voce di report con il totale dei corrispondenti
            // importati e non importati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i corrispondenti da inserire
            report.Inserted = tempReport.ToArray();

            #endregion

            #region Modifica dei corrispondenti

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sulla modifica dei
            // corrispondenti
            tempReport = new List<AddressBookImportResult>();

            // Modifica dei corrispondenti
            foreach (AddressBookRowData rowData in abrdc.ToModify)
            {
                try
                {
                    tempReport.Add(ImportAddressBookUtils.ImportAddressBookElement(
                        userInfo,
                        role,
                        OperationEnum.M,
                        rowData));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new AddressBookImportResult()
                        {
                            Result = ResultEnum.KO,
                            Message = e.Message
                        });
                }

                // Un corrispondente è stato analizzato
                totalNumberOfAnalyzedElements += 1;

            }

            // Aggiunta di una voce di report con il totale dei corrispondenti
            // modificati e non modificati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i corrispondenti modificati
            report.Modified = tempReport.ToArray();

            #endregion

            #region Corrispondenti da cancellare

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sulla cancellazione
            // dei corrispondenti
            tempReport = new List<AddressBookImportResult>();

            // Cancellazione dei corrispondenti
            foreach (AddressBookRowData rowData in abrdc.ToDelete)
            {
                try
                {
                    tempReport.Add(ImportAddressBookUtils.ImportAddressBookElement(
                        userInfo,
                        role,
                        OperationEnum.C,
                        rowData));
                }
                catch (Exception e) 
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new AddressBookImportResult()
                    {
                        Message = e.Message,
                        Result = ResultEnum.KO
                    });
                }

                // Un corrispondente è stato analizzato
                totalNumberOfAnalyzedElements += 1;

            }

            // Aggiunta di una voce di report con il totale dei corrispondenti
            // cancellati e non cancellati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i corrispondenti cancellati
            report.Deleted = tempReport.ToArray();

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
        private AddressBookImportResult GetSummaryElement(List<AddressBookImportResult> tempReport)
        {
            // L'oggetto da restituire
            AddressBookImportResult toReturn;

            // Il numero di risultati negativi
            int unsuccessNumber = 0;

            // Calcolo dei risultati negativi
            unsuccessNumber = tempReport.Where(e => e.Result == ResultEnum.KO).Count();

            // Inizializzazione dell'elemento
            toReturn = new AddressBookImportResult()
            {
                Result = ResultEnum.OK,
                Message = String.Format(
                    "Corrispondenti analizzati correttamente: {0}. Corrispondenti analizzati con errori: {1}",
                    tempReport.Count - unsuccessNumber, unsuccessNumber)
            };

            // Restituzione dell'elemento creato
            return toReturn;

        }

        /// <summary>
        /// Funzione richiamabile per conoscere il numero di corrispondenti analizzati
        /// ed il numero di corrispondenti totali
        /// </summary>
        /// <param name="analyzed">Numero di corrispondenti analizzati</param>
        /// <param name="total">Numero totale di corrispondenti</param>
        public void GetStatistics(out int analyzed, out int total)
        {
            // Impostazione dei valori richiesti
            analyzed = this.totalNumberOfAnalyzedElements;
            total = this.totalNumberOfElementToImport;

        }

        /// <summary>
        /// Funzione per la restituzione del report
        /// </summary>
        /// <returns></returns>
        public AddressBookImportResultContainer GetReport()
        {
            // Viene risvegliato il thread
            //waitReading.Set();

            // Viene restituito il report
            return this.report;
        }
    }
}