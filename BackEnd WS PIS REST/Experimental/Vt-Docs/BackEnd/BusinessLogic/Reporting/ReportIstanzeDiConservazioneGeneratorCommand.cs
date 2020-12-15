using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using System.Data;
using BusinessLogic.Reporting.Exceptions;
using BusinessLogic.Documenti;
using DocsPaVO.utente;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione del report per le istanze di conservazione
    /// </summary>
    [ReportGeneratorAttribute(Name = "Istanze di conservazione", ContextName = "ReportConservazione", Key = "IstanzeConservazione")]
    class ReportIstanzeDiConservazioneGeneratorCommand: ReportGeneratorCommand
    {
        /// <summary>
        /// Questo report non prevede la possibilità di selezionare le colonne da esportare
        /// </summary>
        /// <returns>Null</returns>
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        /// <summary>
        /// Viene fatto un override dell'header in modo da impostare le dimensioni delle colonne
        /// </summary>
        /// <param name="dataSet">Dataset con i dati estratti</param>
        /// <param name="fieldsToExport">Campi da esportare (tutti per questo report)</param>
        /// <returns>Header del report</returns>
        protected override HeaderColumnCollection GenerateReportHeader(System.Data.DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = base.GenerateReportHeaderFromDataSet(dataSet);

            // Impostazione proprietà delle colonne
            // Id istanza
            header["ID_ISTANZA"].ColumnSize = 50;
            header["ID_ISTANZA"].ColumnName = "Id Istanza";
            header["ID_ISTANZA"].DataType = HeaderProperty.ContentDataType.Integer;

            // descrizione
            header["DESCRIZIONE"].ColumnSize = 150;
            header["DESCRIZIONE"].ColumnName = "Descrizione";
            header["DESCRIZIONE"].DataType = HeaderProperty.ContentDataType.String;

            // numero documenti
            header["NUMERO_DOCUMENTI"].ColumnSize = 50;
            header["NUMERO_DOCUMENTI"].ColumnName = "N. documenti";
            header["NUMERO_DOCUMENTI"].DataType = HeaderProperty.ContentDataType.Integer;

            // dimensioni in megabyte
            header["DIMENSIONE"].ColumnSize = 60;
            header["DIMENSIONE"].ColumnName = "Dimensione in MB";
            header["DIMENSIONE"].DataType = HeaderProperty.ContentDataType.Integer;

            // data invio 
            header["DATA_INVIO"].ColumnSize = 70;
            header["DATA_INVIO"].ColumnName = "Data Invio";
            header["DATA_INVIO"].DataType = HeaderProperty.ContentDataType.String;

            // data chiusura 
            header["DATA_CHIUSURA"].ColumnSize = 90;
            header["DATA_CHIUSURA"].ColumnName = "Data Chiusura";
            header["DATA_CHIUSURA"].DataType = HeaderProperty.ContentDataType.String;

            // data rifiuto 
            header["DATA_RIFIUTO"].ColumnSize = 70;
            header["DATA_RIFIUTO"].ColumnName = "Data rifiuto";
            header["DATA_RIFIUTO"].DataType = HeaderProperty.ContentDataType.DateTime;

            // giorni lavorazione istanza conservata
            header["GIORNI_LAVORAZIONE_C"].ColumnSize = 70;
            header["GIORNI_LAVORAZIONE_C"].ColumnName = "Giorni di lavorazione - istanza conservata";
            header["GIORNI_LAVORAZIONE_C"].DataType = HeaderProperty.ContentDataType.Integer;

            // giorni lavorazione istanza rifiutata
            header["GIORNI_LAVORAZIONE_R"].ColumnSize = 70;
            header["GIORNI_LAVORAZIONE_R"].ColumnName = "Giorni di lavorazione - istanza rifiutata";
            header["GIORNI_LAVORAZIONE_R"].DataType = HeaderProperty.ContentDataType.Integer;


            return header;
        }

    }
}
