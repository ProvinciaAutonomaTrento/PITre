
using DocsPaVO.Report;
namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione del report per l'Elenco Decreti
    /// </summary>
    [ReportGeneratorAttribute(Name = "Elenco Decreti", ContextName = "ProspettiRiepilogativi", Key = "ElencoDecretiSRC")]
    public class ElencoDecretiSRCReportGeneratorCommand : ReportGeneratorCommand
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

            // Data Prot. (50 px, Data)
            header["Data_Proto"].ColumnSize = 50;
            header["Data_Proto"].ColumnName = "Data";
            header["Data_Proto"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Oggetto (180 px)
            header["Oggetto"].ColumnSize = 180;
            header["Oggetto"].DataType = HeaderProperty.ContentDataType.String;

            // TIP (75 px)
            header["Registro_Foglio"].ColumnSize = 75;
            header["Registro_Foglio"].ColumnName = "Registro-Foglio";
            header["Registro_Foglio"].DataType = HeaderProperty.ContentDataType.String;

            // Data Registrazione (80 px, data)
            header["data_registrazione"].ColumnSize = 70;
            header["data_registrazione"].ColumnName = "Data\nRegistrazione";
            header["data_registrazione"].DataType = HeaderProperty.ContentDataType.String;

            return header;
        }
    }
}
