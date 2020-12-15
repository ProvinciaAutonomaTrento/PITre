using System.Linq;
using System;
using DocsPaVO.Report;
using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;
namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione dei report relativi ai modelli di trasmissione
    /// </summary>
    [ReportGenerator(Name = "Risultati ricerca trasmissioni", ContextName = "ModelliTrasmissioneUtente", Key = "ModelliTrasmissioneUtente")]
    [ReportGenerator(Name = "Risultati ricerca trasmissioni", ContextName = "ModelliTrasmissione", Key = "ModelliTrasmissione")]
    public class ModelliTrasmissioneReportGeneratorCommand : ReportGeneratorCommand
    {

        /// <summary>
        /// Metodo per la creazione delle righe del report
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
        /// Metodo per la generazione di una nuova riga del report
        /// </summary>
        /// <param name="dataRow">DataRow da cui estrarre i dati per inizializzare la riga</param>
        /// <param name="reportHeader">Header del report</param>
        /// <returns>Riga inizializzata</returns>
        private ReportMapRowProperty GenerateNewRow(DataRow dataRow, HeaderColumnCollection reportHeader)
        {
            ReportMapRowProperty row = new ReportMapRowProperty();

            // Aggiunta del codice del modello
            if(reportHeader["Codice"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["system_id"].ToString(), "Codice Modello", "Codice"));

            // Aggiunta della descrizione del modello
            if (reportHeader["Descrizione"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["nome"].ToString(), "Descrizione Modello", "Descrizione"));

            // Aggiunta delle informazioni sul mittente (se la riga contiene informazioni sul mittente)
            String mitt = String.Empty;
            if(!String.IsNullOrEmpty(dataRow["var_desc_corr"].ToString()))
                mitt = dataRow["var_desc_corr"].ToString() + "; ";
            if (reportHeader["Mittenti"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["cha_tipo_mitt_dest"].ToString().Trim() == "M" ? mitt : String.Empty, "Visibilità", "Mittenti"));

            // Aggiunta del tipo di oggetto
            String obj = dataRow["cha_tipo_oggetto"].ToString() == "D" ? "Documento" : "Fascicolo";
            if (reportHeader["DocOrFasc"] != null)
                row.Columns.Add(this.GenerateRow(obj, "Documento o Fascicolo", "DocOrFasc"));

            // Aggiunta delle informazioni sul registro
            if (reportHeader["Registro"] != null)
                row.Columns.Add(this.GenerateRow(dataRow["var_desc_registro"].ToString(), "Registro", "Registro"));

            // Aggiunta delle informazioni sul destinatario
            String dest = String.Empty;
            dest = String.Format("{0} - {1}; ", dataRow["var_desc_ragione"].ToString().ToUpper(), dataRow["var_desc_corr"].ToString());
            if (reportHeader["Destinatari"] != null)
                row.Columns.Add(this.GenerateRow(
                    dataRow["cha_tipo_mitt_dest"].ToString() == "D" ?
                        dest : String.Empty, "Destinatari", "Destinatari"));

            // Aggiunta delle informazioni sui ruoli disabilitati
            if (reportHeader["Disabled"] != null)
                row.Columns.Add(this.GenerateRow(
                    !String.IsNullOrEmpty(dataRow["dta_fine"].ToString()) ?
                        dataRow["var_desc_corr"].ToString() + "; " : String.Empty, "Ruoli disabilitati", "Disabled"));

            // Aggiunta delle informazioni sui ruoli inibiti
            if (reportHeader["Inhibited"] != null)
                row.Columns.Add(this.GenerateRow(
                    !String.IsNullOrEmpty(dataRow["cha_disabled_trasm"].ToString()) && dataRow["cha_disabled_trasm"].ToString().Trim() == "1" ?
                        dataRow["var_desc_corr"].ToString() + "; " : String.Empty, "Ruoli inibiti alla ricezione trasmissioni", "Inhibited"));

            return row;

        }

        /// <summary>
        /// Metodo per la generazione di una riga del report
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
        /// Metodo per l'aggiornamento di una riga del report
        /// </summary>
        /// <param name="row">Riga da aggiornare</param>
        /// <param name="dataRow">DataRow da cui estrarre i dati con cui aggiornare la riga</param>
        private void UpdateRow(ReportMapRowProperty row, DataRow dataRow)
        {

            // Modifica delle informazioni su mittente / destinatario
            if (dataRow["cha_tipo_mitt_dest"].ToString() == "M")
            {
                String mitt = String.Empty;
                if (!String.IsNullOrEmpty(dataRow["var_desc_corr"].ToString().Trim()))
                    mitt = dataRow["var_desc_corr"].ToString() + "; ";

                this.UpdateColumn(row["Mittenti"], mitt);
            }
            else
                this.UpdateColumn(row["Destinatari"], String.Format("{0} - {1}; ", dataRow["var_desc_ragione"].ToString().ToUpper(), dataRow["var_desc_corr"].ToString()));

            // Modifica delle informazioni sui ruoli disabilitati
            if (!String.IsNullOrEmpty(dataRow["dta_fine"].ToString()))
                this.UpdateColumn(row["Disabled"], dataRow["var_desc_corr"].ToString() + "; ");

            // Modifica delle informazioni sui ruoli inibiti
            if (!String.IsNullOrEmpty(dataRow["cha_disabled_trasm"].ToString()) && dataRow["cha_disabled_trasm"].ToString() == "1")
                this.UpdateColumn(row["Inhibited"], dataRow["var_desc_corr"].ToString() + "; ");

        }

        /// <summary>
        /// Metodo per l'aggiornamento del valore di una colonna
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
            retList.Add(this.GetHeaderColumn("Cod. Modello", 108, "Codice"));
            retList.Add(this.GetHeaderColumn("Descr. Modello", 320, "Descrizione"));
            retList.Add(this.GetHeaderColumn("Visibilità", 390, "Mittenti"));
            retList.Add(this.GetHeaderColumn("Doc. o Fasc", 158, "DocOrFasc"));
            retList.Add(this.GetHeaderColumn("Registro", 180, "Registro"));
            retList.Add(this.GetHeaderColumn("Ragione Tram. - Destinatari", 390, "Destinatari"));
            retList.Add(this.GetHeaderColumn("Ruoli disabilitati", 390, "Disabled"));
            retList.Add(this.GetHeaderColumn("Ruoli inibiti alla ricezione di trasmissione", 390, "Inhibited"));

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
        
    }
}
