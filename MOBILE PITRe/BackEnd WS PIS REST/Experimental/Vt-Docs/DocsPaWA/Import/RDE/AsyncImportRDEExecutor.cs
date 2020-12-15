using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Threading;

namespace DocsPAWA.Import.RDE
{

    /// <summary>
    /// Questa classe si occupa di effettuare l'importazione di documenti
    ///  di emergenza descritti in un array di oggetti DocumentRowData e di produrre
    /// il report di importazione.
    /// </summary>
    public class AsyncImportRDEExecutor
    {
        #region Dichiarazione Variabili

        // Oggetto utilizzato per la sincronizzazione del thread.
        // Il thread viene messo in attesa al termine dell'esecuzione.
        // Il risveglio avviene non appena qualcuno preleva il report
        // o automaticamente dopo 5 minuti.
        ManualResetEvent waitReading;

        // L'oggetto con il report di esecuzione
        ResultsContainer report;

        // Il numero di documenti totali da importare ed il numero di
        // documenti analizzati
        int totalNumberOfDocumentToImport, totalNumberOfAnalyzedDocuments;

        #endregion

        /// <summary>
        /// Costruttore di default.
        /// </summary>
        public AsyncImportRDEExecutor()
        {
            // Inizializzazione del report
            this.report = new ResultsContainer();

            // Inizializzazione del numero di documenti importati
            this.totalNumberOfAnalyzedDocuments = 0;

            // Inizializzazione del numero di documenti da importare
            this.totalNumberOfDocumentToImport = 0;

            // Inizializzazione dell'oggetto di sincronizzazione manuale
            this.waitReading = new ManualResetEvent(false);

        }

        /// <summary>
        /// Funzione richiamata dal Thread per l'esecuzione dell'importazione
        /// </summary>
        /// <param name="prameters">Un array di oggetti. Sono attesi 4 elementi:
        ///     - Il primo deve essere un oggetto di tipo DocumentRowDataContainer
        ///     - Il secondo deve essere un oggetto di tipo string
        ///     - Il terzo deve essere un oggetto di tipo InfoUtente
        ///     - Il quarto deve essere un oggetto di tipo Ruolo
        /// </param>
        public void ExecuteImport(object parameters)
        {
            #region Dichiarazione variabili

            // L'array dei parametri
            object[] param;

            // L'oggetto con le informazioni sui documenti da importare
            DocumentRowDataContainer drdc;

            // L'url del frontend
            string url;

            // Le informazioni sull'utente che ha lanciato la procedura
            InfoUtente userInfo;

            // Il ruolo con cui è stata lanciata la procedura
            Ruolo role;

            // Lista in cui depositare temporaneamente il report
            // di importazione relativo ad una tipologia di documento
            List<ImportResult> tempReport;

            #endregion

            // Casting dell'oggetto a array di oggetti
            param = parameters as object[];

            // Se l'array non contiene quattro elementi, fine esecuzione
            if (param.Length != 4)
                return;

            // Prelevamento delle informazioni sui documenti da creare
            drdc = param[0] as DocumentRowDataContainer;

            // Prelevamento dell'url del Fontend
            url = param[1] as string;

            // Prelevamento delle informazioni sull'utente
            userInfo = param[2] as InfoUtente;

            // Prelevamento del ruolo
            role = param[3] as Ruolo;

            // Calcolo del numero totale di documenti da importare
            this.totalNumberOfDocumentToImport =
                drdc.InDocument.Length +
                drdc.OutDocument.Length +
                drdc.OwnDocument.Length;

            #region Importazione documenti in arrivo

            // Inizializzazione della lista da utulizzare per
            // memorizzare temporaneamente il report sull'importazione dei
            // documenti in ingresso
            tempReport = new List<ImportResult>();

            // Importazione dei documenti in ingresso
            foreach (DocumentRowData rowData in drdc.InDocument)
            {
                try
                {
                    tempReport.Add(ImportRDEUtils.ImportRDEDocument(
                                rowData,
                                userInfo,
                                role,
                                userInfo.urlWA,
                                ProtoType.A));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new ImportResult()
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Ordinal = rowData.OrdinalNumber,
                        Message = e.Message
                    });

                }

                // Un documenti è stato analizzato
                totalNumberOfAnalyzedDocuments += 1;

            }

            // Aggiunta di una voce di report con il totale dei documenti
            // importati e non importati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i documenti in ingresso
            report.InDocument = tempReport.ToArray();

            #endregion

            #region Importazione documenti in partenza

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sull'importazione dei
            // documenti in partenza
            tempReport = new List<ImportResult>();

            // Importazione dei documenti in partenza
            foreach (DocumentRowData rowData in drdc.OutDocument)
            {
                try
                {
                    tempReport.Add(ImportRDEUtils.ImportRDEDocument(
                                rowData,
                                userInfo,
                                role,
                                userInfo.urlWA,
                                ProtoType.P));
                }
                catch (Exception e)
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new ImportResult()
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Ordinal = rowData.OrdinalNumber,
                        Message = e.Message
                    });
                }

                // Un documenti è stato analizzato
                totalNumberOfAnalyzedDocuments += 1;

            }

            // Aggiunta di una voce di report con il totale dei documenti
            // importati e non importati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i documenti in partenza
            report.OutDocument = tempReport.ToArray();

            #endregion

            #region Importazione documenti interni

            // Inizializzazione della lista da utilizzare per
            // memorizzare temporaneamente il report sull'importazione dei
            // documenti interni
            tempReport = new List<ImportResult>();

            // Se non è abilitata l'importazione dei documenti interni, viene 
            // cancellata la lista dei documenti interni
            if (!ImportRDEUtils.IsEnabledOwnerProto())
                drdc.OwnDocument = new DocumentRowData[0];

            // Importazione dei documenti interni
            foreach (DocumentRowData rowData in drdc.OwnDocument)
            {
                try
                {
                    tempReport.Add(ImportRDEUtils.ImportRDEDocument(
                        rowData,
                        userInfo,
                        role,
                        userInfo.urlWA,
                        ProtoType.I));
                }
                catch (Exception e) 
                {
                    // In caso di eccezione viene aggiunto un risultato negativo
                    tempReport.Add(new ImportResult()
                    {
                        Outcome = OutcomeEnumeration.KO,
                        Ordinal = rowData.OrdinalNumber,
                        Message = e.Message
                    });
                }

                // Un documenti è stato analizzato
                totalNumberOfAnalyzedDocuments += 1;

            }

            // Aggiunta di una voce di report con il totale dei documenti
            // importati e non importati
            tempReport.Add(this.GetSummaryElement(tempReport));

            // Salvataggio del report per i documenti interni
            report.OwnDocument = tempReport.ToArray();

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
            if (unsuccessNumber > 0)
            {
                toReturn = new ImportResult()
                {
                    Outcome = OutcomeEnumeration.NONE,
                    Message = String.Format(
                        "Documenti importati correttamente: {0}. Documenti non importati {1}",
                        tempReport.Count - unsuccessNumber, unsuccessNumber)
                };
            }
            else
                toReturn = new ImportResult()
                {
                    Outcome = OutcomeEnumeration.NONE,
                    Message = String.Format(
                        "Documenti importati correttamente: {0}. Documenti non importati {1}",
                        tempReport.Count - unsuccessNumber, unsuccessNumber)
                };

            // Restituzione dell'elemento creato
            return toReturn;

        }

        /// <summary>
        /// Funzione richiamabile per conoscere il numero di documenti analizzati
        /// ed il numero di documenti totali
        /// </summary>
        /// <param name="analyzed">Numero di documenti analizzati</param>
        /// <param name="total">Numero totale di documenti</param>
        public void GetStatistics(out int analyzed, out int total)
        {
            // Impostazione dei valori richiesti
            analyzed = this.totalNumberOfAnalyzedDocuments;
            total = this.totalNumberOfDocumentToImport;

        }

        /// <summary>
        /// Funzione per la restituzione del report
        /// </summary>
        /// <returns></returns>
        public ResultsContainer GetReport()
        {
            // Viene risvegliato il thread
            //waitReading.Set();

            // Numero di documenti non importati ed importati con
            // warning
            int importedWithWarningOrNotImported = 0;

            // Calcolo del numero di documenti importati con warning
            importedWithWarningOrNotImported += this.report.Attachment != null ?
                this.report.Attachment.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.GrayDocument != null ?
                this.report.GrayDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.InDocument != null ?
                this.report.InDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.OutDocument != null ?
                this.report.OutDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;
            importedWithWarningOrNotImported += this.report.OwnDocument != null ?
                this.report.OwnDocument.Where(e => e.Outcome == OutcomeEnumeration.Warnings || e.Outcome == OutcomeEnumeration.KO).Count() :
                0;

            ImportResult importResult = new ImportResult();

            // Aggiunta di un messaggio nel report generale
            // Se non ci sono documenti importati con warning o non importati,
            // viene aggiunto un messaggio positivo altrimenti viene aggiunto un
            // messaggio di warning
            if (this.totalNumberOfDocumentToImport == 0)
            {
                importResult.Outcome = OutcomeEnumeration.Warnings;
                importResult.Message = "Non è stato rilevato nessun documento da importare.";
            }
            else
                if (importedWithWarningOrNotImported == 0)
                    importResult.Message = "Nessun messaggio generale da mostrare.";
                else
                {
                    importResult.Message = String.Format("Attenzione! {0} documenti hanno presentato problemi durante l'importazione. Controllare gli altri tab per maggiori informazioni.",
                        importedWithWarningOrNotImported);
                    importResult.Outcome = OutcomeEnumeration.KO;

                }


            this.report.General = new ImportResult[] {
                    importResult
                };

            // Viene restituito il report
            return this.report;
        }
   
    }
}
