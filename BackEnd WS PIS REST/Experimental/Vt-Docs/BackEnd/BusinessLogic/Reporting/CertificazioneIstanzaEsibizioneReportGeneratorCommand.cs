using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.Report;
using log4net;


namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name="Certificazione Esibizione", ContextName="CertificazioneEsibizione", Key="CertificazioneEsibizione")]
    public class CertificazioneIstanzaEsibizioneReportGeneratorCommand : ReportGeneratorCommand
    {

        private ILog logger = LogManager.GetLogger(typeof(CertificazioneIstanzaEsibizioneReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = base.GenerateReportHeaderFromDataSet(dataSet);

            //INTESTAZIONI COLONNE
            header["SEGNATURA"].ColumnSize = 80;
            header["SEGNATURA"].ColumnName = "ID/Segnatura";
            header["SEGNATURA"].DataType = HeaderProperty.ContentDataType.String;

            header["DATA_PROT_OR_CREA"].ColumnSize = 80;
            header["DATA_PROT_OR_CREA"].ColumnName = "Data creazione/protocollazione";
            header["DATA_PROT_OR_CREA"].DataType = HeaderProperty.ContentDataType.String;

            header["VAR_OGGETTO"].ColumnSize = 250;
            header["VAR_OGGETTO"].ColumnName = "Oggetto";
            header["VAR_OGGETTO"].DataType = HeaderProperty.ContentDataType.String;

            header["VAR_IMPRONTA"].ColumnSize = 400;
            header["VAR_IMPRONTA"].ColumnName = "Impronta";
            header["VAR_IMPRONTA"].DataType = HeaderProperty.ContentDataType.String;

            return header;
        }

    }
}
