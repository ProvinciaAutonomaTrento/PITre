using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;


namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Classe per la generazione del report per i Decreti pervenuti in un dato intervallo temporale suucessivi
    /// </summary>
    [ReportGeneratorAttribute(Name = "Report Decreti pervenuti in intervallo temporale successivi SCCLA", ContextName = "ProspettiRiepilogativi", Key = "DecretiPervenutiInIntTemporaleSuccSCCLA")]
    class DecretiPervenutiInIntTemporaleSuccSCCLAGeneratorCommand : ReportGeneratorCommand
    {
        /// <summary>
        /// Questo report non prevede possibilità di selezionare i campi da esportare
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
            // Id del documento
            header["system_id"].ColumnName = "Id";

            // Segnatura (128px, Stringa)
            header["Segnatura"].ColumnSize = 96;
            header["Segnatura"].ColumnName = "Segnatura";

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

            // Data Arrivo (59 px, data)
            header["DataArrivo"].ColumnSize = 44;
            header["DataArrivo"].ColumnName = "Data\nArrivo";
            header["DataArrivo"].DataType = HeaderProperty.ContentDataType.DateTime;

            // TIP (40 px)
            header["Tip"].ColumnSize = 30;
            header["Tip"].ColumnName = "TIP";

            // Primo revisore (68 px)
            header["PrimoRevisore"].ColumnSize = 51;
            header["PrimoRevisore"].ColumnName = "Primo\nRevisore";

            // Secondo revisore (56 px)
            header["SecondoRevisore"].ColumnSize = 42;
            header["SecondoRevisore"].ColumnName = "Secondo\nRevisore";

            // Magistrato istruttore (72 px)
            header["MagistratoIstruttore"].ColumnSize = 54;
            header["MagistratoIstruttore"].ColumnName = "Magistrato\nIstruttore";

            // Stato
            header["Stato"].ColumnName = "Stato";

            // Data primo rilievo (59 px, data)
            header["DataPrimoRilievo"].ColumnSize = 44;
            header["DataPrimoRilievo"].ColumnName = "Data primo\nrilievo";
            header["DataPrimoRilievo"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Data ritorno primo rilievo (59 px, data)
            header["dataritornoprimorilievo"].ColumnSize = 44;
            header["dataritornoprimorilievo"].ColumnName = "Data ritorno\nprimo rilievo";
            header["dataritornoprimorilievo"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Data secondo rilievo
            header["data_secondo_rilievo"].ColumnName = "Data secondo rilievo";
            header["data_secondo_rilievo"].ColumnSize = 44;
            header["data_secondo_rilievo"].DataType = HeaderProperty.ContentDataType.DateTime;

            // GIAC (50 px, Numero)
            header["Giac"].ColumnSize = 37;
            header["Giac"].ColumnName = "GIAC";
            header["Giac"].DataType = HeaderProperty.ContentDataType.Number;

            // Data rest amministrazione
            header["datarestamm"].ColumnName = "Data Rest. rilievo";
            header["datarestamm"].ColumnSize = 44;
            header["datarestamm"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Data registrazione
            header["datareg"].ColumnName = "Data Reg.";
            header["datareg"].ColumnSize = 44;
            header["datareg"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Data scad. controllo (59 px, Data)
            header["datascadcontrollo"].ColumnSize = 44;
            header["datascadcontrollo"].ColumnName = "Data Scad.\ncontrollo";
            header["datascadcontrollo"].DataType = HeaderProperty.ContentDataType.DateTime;

            // Data scad. Amm. (59 px, Data)
            header["datascadamm"].ColumnSize = 44;
            header["datascadamm"].ColumnName = "Data scad. Amm.";
            header["datascadamm"].DataType = HeaderProperty.ContentDataType.DateTime;

            return header;
        }
    }
}
