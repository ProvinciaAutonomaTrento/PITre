using System.Linq;
using System;
using DocsPaVO.Report;
using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.Reporting.Exceptions;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;

namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Funzione per la generazione dei report relativi alle mail in casella istituzionale    /// </summary>
    [ReportGenerator(Name = "Risultati Mail Casella Istituzionale", ContextName = "CasellaIstituzionale", Key = "RicercaCasellaIstituzionale")]
    class CasellaIstituzionaleReportGenerator : ReportGeneratorCommand
    {

        /// <summary>
        /// Funzione per la creazione delle righe del report
        /// </summary>
        /// <param name="dataSet">Data set da cui estrarre i dati</param>
        /// <param name="reportHeader">Header del report</param>
        /// <returns>Collection con le proprietà delle righe</returns>
        protected override IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader)
        {
            // Lista delle righe
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();

            // Riga in generazione
            ReportMapRowProperty row = null;
            // Id del modello in esame
            String modelId = String.Empty;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                // Se non è stato ancora recuperato un id di modello o
                // se l'id della riga corrente è diverso da quello memorizzato,
                // significa che è cambiato il modello, quindi viene generata una nuova
                // riga
                if (String.IsNullOrEmpty(modelId) || !dataRow["system_id"].ToString().Equals(modelId))
                {
                    // Se la riga attuale è diversa da null, viene aggiunta all'elenco delle righe
                    modelId = dataRow["system_id"].ToString();
                    row = this.GenerateNewRow(dataRow, reportHeader);
                    rows.Add(row);

                }
                else
                    this.UpdateRow(row, dataRow);

            }

            return rows;

        }

        /// <summary>
        /// Funzione per la generazione di una nuova riga del report
        /// </summary>
        /// <param name="dataRow">DataRow da cui estrarre i dati per inizializzare la riga</param>
        /// <param name="reportHeader">Header del report</param>
        /// <returns>Riga inizializzata</returns>
        private ReportMapRowProperty GenerateNewRow(DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            ReportMapRowProperty row = new ReportMapRowProperty();

            // Aggiunta del codice del modello

            if (reportHeader["Type"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["Type"].ToString(), "Tipo", "Type"));

            if (reportHeader["From"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["From"].ToString(), "Mittente", "From"));

            // Aggiunta della descrizione del modello
            if (reportHeader["Subject"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["Subject"].ToString(), "Oggetto", "Subject"));

            if (reportHeader["Date"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["Date"].ToString(), "Data Invio", "Date"));

            if (reportHeader["CountAttatchments"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["CountAttatchments"].ToString(), "Allegati", "CountAttatchments"));

            if (reportHeader["CheckResult"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["CheckResult"].ToString(), "Esito Controllo Messaggio", "CheckResult"));
          
            return row;

        }

        /// <summary>
        /// Funzione per la generazione di una riga del report
        /// </summary>
        /// <param name="value">Valore da assegnare al campo</param>
        /// <param name="columnName">Nome da assegnare alla colonna</param>
        /// <param name="originalName">Nome originale della colonna</param>
        /// <returns>Colonna</returns>
        private ReportMapColumnProperty GenerateRow(string value, string columnName, String originalName)
        {
            return new ReportMapColumnProperty()
            {
                OriginalName = originalName,
                ColumnName = columnName,
                Value = value
            };
        }


        /// <summary>
        /// Funzione per l'aggiornamento di una riga del report
        /// </summary>
        /// <param name="row">Riga da aggiornare</param>
        /// <param name="dataRow">DataRow da cui estrarre i dati con cui aggiornare la riga</param>
        private void UpdateRow(ReportMapRowProperty row, DataRow dataRow)
        {

            // Modifica delle informazioni su mittente / destinatario
       

        }

        /// <summary>
        /// Funzione per l'aggiornamento del valore di una colonna
        /// </summary>
        /// <param name="column">Colonna da aggiornare</param>
        /// <param name="value">Valore da aggiungere ai valori già presenti nella colonna</param>
        private void UpdateColumn(ReportMapColumnProperty column, String value)
        {
            if (column != null)
                column.Value += value;
        }

        /// <summary>
        /// Questo report prevede la possibilità di selezionare i campi da esportare
        /// </summary>
        /// <returns>Collezione con la lista dei campi che costituiscono il report</returns>
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            HeaderColumnCollection retList = new HeaderColumnCollection();
            retList.Add(this.GetHeaderColumn("Tipo", 150, "Type"));
            retList.Add(this.GetHeaderColumn("Mittente", 200, "From"));
            retList.Add(this.GetHeaderColumn("Oggetto", 420, "Subject"));
            retList.Add(this.GetHeaderColumn("Data Invio", 200, "Date"));
            retList.Add(this.GetHeaderColumn("Allegati", 30, "CountAttatchments"));
            retList.Add(this.GetHeaderColumn("Esito Controllo Messaggio", 420, "CheckResult"));
            return retList;
        }

        /// <summary>
        /// Metodo per la creazione di un oggetto con le informazioni su una colonna
        /// </summary>
        /// <param name="columnName">Nome da assegnare alla colonna</param>
        /// <param name="columnWidth">Larghezza da assegnare alla colonna</param>
        /// <returns>Proprietà della colonna</returns>
        private HeaderProperty GetHeaderColumn(String columnName, int columnWidth, String originalColumnName)
        {

            return new HeaderProperty()
            {
                ColumnName = columnName,
                OriginalName = originalColumnName,
                ColumnSize = columnWidth,
                Export = true
            };

        }

        /// Funzione da richiamare per la generazione di un report. Il comportamento strandard prevede
        /// l'utilizzo di un behavior per l'estrazione dati basato su query list. Sovrascrivere questo
        /// metodo per ottenere un comportamento differente.
        /// </summary>
        /// <param name="request">Informazioni utili alla generazione del report</param>
        /// <returns>Outup del processo di generazione</returns>
        protected override PrintReportResponse GenerateReport(PrintReportRequest request)
        {
            IReportDataExtractionBehavior reportDataExtractor = new DatasetReportDataExtractionBehavior();
            IReportGeneratorBehavior formatGenerator = this.CreateReportGenerator(request);

            // Generazione del report
            return this.GenerateReport(request, reportDataExtractor, formatGenerator);
        }

    }
}
