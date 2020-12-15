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
    /// Classe per la generazione del report per per le verifiche periodiche delle istanze
    /// </summary>
    [ReportGeneratorAttribute(Name = "Verifiche di integrità e leggibilità delle istanze", ContextName = "ReportConservazione", Key = "VerifichePeriodiche")]
    class ReportVerifichePeriodicheIstanzeGeneratorCommand : ReportGeneratorCommand
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
            header["ID_ISTANZA"].ColumnSize = 80;
            header["ID_ISTANZA"].ColumnName = "Id Istanza";
            header["ID_ISTANZA"].DataType = HeaderProperty.ContentDataType.Integer;

            // id del supporto
            header["ID_SUPPORTO"].ColumnSize = 80;
            header["ID_SUPPORTO"].ColumnName = "Id supporto";
            header["ID_SUPPORTO"].DataType = HeaderProperty.ContentDataType.Integer;

            // descrizione del supporto
            header["VAR_DESCRIZIONE"].ColumnSize = 150;
            header["VAR_DESCRIZIONE"].ColumnName = "Tipo supporto";
            header["VAR_DESCRIZIONE"].DataType = HeaderProperty.ContentDataType.String;

            // descrizione del della verifica
            header["DESC_VER"].ColumnSize = 150;
            header["DESC_VER"].ColumnName = "Tipo verifica";
            header["DESC_VER"].DataType = HeaderProperty.ContentDataType.String;

            // data invio
            header["DATA_INVIO"].ColumnSize = 75;
            header["DATA_INVIO"].ColumnName = "Data invio";
            header["DATA_INVIO"].DataType = HeaderProperty.ContentDataType.String;

            // data creazione supporto 
            header["DATA_CREAZIONE_SUPP"].ColumnSize = 70;
            header["DATA_CREAZIONE_SUPP"].ColumnName = "Data creazione supporto";
            header["DATA_CREAZIONE_SUPP"].DataType = HeaderProperty.ContentDataType.String;

            // data chiusura istanza 
            header["DATA_CHIUSURA"].ColumnSize = 90;
            header["DATA_CHIUSURA"].ColumnName = "Data chiusura istanza";
            header["DATA_CHIUSURA"].DataType = HeaderProperty.ContentDataType.String;

            // data verifica
            header["DATA_VERIFICA"].ColumnSize = 70;
            header["DATA_VERIFICA"].ColumnName = "Data verifica";
            header["DATA_VERIFICA"].DataType = HeaderProperty.ContentDataType.String;

            //// tipo verifica
            //header["CHA_TIPO_VER"].ColumnSize = 80;
            //header["CHA_TIPO_VER"].ColumnName = "tipo verifica";
            //header["CHA_TIPO_VER"].DataType = HeaderProperty.ContentDataType.String;

            // percentuale documenti verificati
            header["PERCENTUALE"].ColumnSize = 80;
            header["PERCENTUALE"].ColumnName = "% documenti verificati";
            header["PERCENTUALE"].DataType = HeaderProperty.ContentDataType.Integer;

            // esito della verifica
            header["ESITO"].ColumnSize = 80;
            header["ESITO"].ColumnName = "Esito della verifica";
            header["ESITO"].DataType = HeaderProperty.ContentDataType.String;
             
            return header;
        }


    }
}
