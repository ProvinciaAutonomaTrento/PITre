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

namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione del report sulla storia dei documenti
    /// </summary>
    [ReportGeneratorAttribute(Name = "Report sulla storia dei documenti", ContextName = "ReportConservazione", Key = "ReportStoriaDocumenti")]
    class ReportStoriaDocumentiGeneratorCommand:ReportGeneratorCommand
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
            // Id documento
            header["ID_DOCUMENTO"].ColumnSize = 80;
            header["ID_DOCUMENTO"].ColumnName = "Id documento";
            header["ID_DOCUMENTO"].DataType = HeaderProperty.ContentDataType.Integer;

            // Impostazione proprietà delle colonne
            // Id istanza
            header["ID_ISTANZA"].ColumnSize = 80;
            header["ID_ISTANZA"].ColumnName = "Id Istanza";
            header["ID_ISTANZA"].DataType = HeaderProperty.ContentDataType.Integer;

            // data invio
            header["DATA_OPERAZIONE"].ColumnSize = 80;
            header["DATA_OPERAZIONE"].ColumnName = "Data operazione ";
            header["DATA_OPERAZIONE"].DataType = HeaderProperty.ContentDataType.String;

            // Identificativo dell'operatore
            header["OPERATORE"].ColumnSize = 80;
            header["OPERATORE"].ColumnName = "Operatore";
            header["OPERATORE"].DataType = HeaderProperty.ContentDataType.String;

            // dettagli
            header["DETTAGLI"].ColumnSize = 160;
            header["DETTAGLI"].ColumnName = "Dettaglio";
            header["DETTAGLI"].DataType = HeaderProperty.ContentDataType.String;

            // esito della verifica 
            header["ESITO"].ColumnSize = 70;
            header["ESITO"].ColumnName = "ESITO";
            header["ESITO"].DataType = HeaderProperty.ContentDataType.String;



            return header;
        }

    }
}
