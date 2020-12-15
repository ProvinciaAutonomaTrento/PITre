
using DocsPaVO.Report;
namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione del report per i Decreti in esame
    /// </summary>
    [ReportGeneratorAttribute(Name = "Scadenzario - Decreti in esame", ContextName = "ProspettiRiepilogativi", Key = "DecretiInEsame")]
    public class DecretiInEsameReportGeneratorCommand : ReportGeneratorCommand
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
            // Segnatura (128px, Stringa)
            header["Segnatura"].ColumnSize = 96;
            header["Segnatura"].ColumnName = "Segnatura";
            header["Segnatura"].DataType = HeaderProperty.ContentDataType.String;

            // Num Dec. (65 px, numerico)
            header["NumDec"].ColumnSize = 48;
            header["NumDec"].ColumnName = "Num Dec.";
            header["NumDec"].DataType = HeaderProperty.ContentDataType.Integer;

            // Data Dec. (59 px, Data)
            header["DataDec"].ColumnSize = 44;
            header["DataDec"].ColumnName = "Data dec.";
            header["DataDec"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Oggetto (223 px)
            header["Oggetto"].ColumnSize = 167;
            header["Oggetto"].DataType = HeaderProperty.ContentDataType.String;

            // Data Arrivo (59 px, data)
            header["DataArrivo"].ColumnSize = 44;
            header["DataArrivo"].ColumnName = "Data\nArrivo";
            header["DataArrivo"].DataType = HeaderProperty.ContentDataType.DateTime;

            // TIP (40 px)
            header["Tip"].ColumnSize = 30;
            header["Tip"].ColumnName = "TIP";
            header["Tip"].DataType = HeaderProperty.ContentDataType.String;

            // Primo revisore (68 px)
            header["PrimoRevisore"].ColumnSize = 51;
            header["PrimoRevisore"].ColumnName = "Primo\nRevisore";
            header["PrimoRevisore"].DataType = HeaderProperty.ContentDataType.String;

            // Secondo revisore (56 px)
            header["SecondoRevisore"].ColumnSize = 42;
            header["SecondoRevisore"].ColumnName = "Secondo\nRevisore";
            header["SecondoRevisore"].DataType = HeaderProperty.ContentDataType.String;

            // Magistrato istruttore (72 px)
            header["MagistratoIstruttore"].ColumnSize = 54;
            header["MagistratoIstruttore"].ColumnName = "Magistrato\nIstruttore";
            header["MagistratoIstruttore"].DataType = HeaderProperty.ContentDataType.String;

            // Data primo rilievo (59 px, data)
            header["DataPrimoRilievo"].ColumnSize = 44;
            header["DataPrimoRilievo"].ColumnName = "Data primo\nrilievo";
            header["DataPrimoRilievo"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Data ritorno primo rilievo (59 px, data)
            header["DataRitPrimoRil"].ColumnSize = 44;
            header["DataRitPrimoRil"].ColumnName = "Data ritorno\nprimo rilievo";
            header["DataRitPrimoRil"].DataType = HeaderProperty.ContentDataType.DateTime;

            // GIAC (50 px, Numero)
            header["Giac"].ColumnSize = 37;
            header["Giac"].ColumnName = "GIAC";
            header["Giac"].DataType = HeaderProperty.ContentDataType.Number;

            // Data scad. controllo (59 px, Data)
            header["DataScadContr"].ColumnSize = 44;
            header["DataScadContr"].ColumnName = "Data Scad.\ncontrollo";
            header["DataScadContr"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Data scad. Amm. (59 px, Data)
            header["DataScadAmm"].ColumnSize = 44;
            header["DataScadAmm"].ColumnName = "Data scad. Amm.";
            header["DataScadAmm"].DataType = HeaderProperty.ContentDataType.DateTime;

            return header;
        }
    }
}
