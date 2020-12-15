using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name = "Elenco Decreti Restituiti", ContextName = "ProspettiRiepilogativi", Key = "ElencoDecretiRestituitiSRC")]
    public class ElencoDecretiRestituitiSRCReportGeneratorCommand : ReportGeneratorCommand
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
            // Segnatura (50, Stringa)
            header["Protocollo"].ColumnSize = 50;
            header["Protocollo"].ColumnName = "Protocollo";
            header["Protocollo"].DataType = HeaderProperty.ContentDataType.String;

            // Data Protocollazione (80 px, data)
            header["Data_proto"].ColumnSize = 70;
            header["Data_proto"].ColumnName = "Data";
            header["Data_proto"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Oggetto (180 px)
            header["Oggetto"].ColumnSize = 180;
            header["Oggetto"].DataType = HeaderProperty.ContentDataType.String;

            // Data Registrazione (80 px, data)
            header["Data decreto"].ColumnSize = 90;
            header["Data decreto"].ColumnName = "Data\nDecreto";
            header["Data decreto"].DataType = HeaderProperty.ContentDataType.DateTime;

            return header;
        }
    }

}
