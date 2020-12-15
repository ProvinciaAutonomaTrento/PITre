using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using log4net;
using System.Data;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name = "Monitoraggio policy", ContextName = "AmmMonitoraggioPolicy", Key = "AmmMonitoraggioPolicy")]
    public class AmmMonitoraggioPolicyReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(AmmMonitoraggioPolicyReportGeneratorCommand));

       
        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            throw new NotImplementedException();
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            // Report da restiutuire
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            report.CreationDate = DateTime.Now;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();
            report.Title = request.Title;
            report.Subtitle = request.SubTitle;
            report.AdditionalInformation = request.AdditionalInformation;

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader));
            }


            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected override HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            header.Add(new HeaderProperty()
            {
                ColumnName = "CODICE AMM.",
                ColumnSize = 100,
                OriginalName = "VAR_CODICE_AMM",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "DESCRIZIONE AMM.",
                ColumnSize = 300,
                OriginalName = "VAR_DESC_AMM",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "CODICE POLICY",
                ColumnSize = 100,
                OriginalName = "VAR_CODICE",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "DESCRIZIONE POLICY",
                ColumnSize = 300,
                OriginalName = "VAR_DESCRIZIONE",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "TIPO",
                ColumnSize = 30,
                OriginalName = "CHA_TIPO_POLICY",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "ULTIMA ESECUZIONE",
                ColumnSize = 100,
                OriginalName = "ULTIMA_ESECUZIONE",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "N.ESECUZIONI",
                ColumnSize = 50,
                OriginalName = "NUM_ESECUZIONI_PERIODO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "PRESI IN CARICO",
                ColumnSize = 100,
                OriginalName = "PRESI_CARICO",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "RIFIUTATI",
                ColumnSize = 100,
                OriginalName = "RIFIUTATI",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "FALLITI",
                ColumnSize = 100,
                OriginalName = "FALLITI",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "ALTRI",
                ColumnSize = 100,
                OriginalName = "ALTRI",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });
            header.Add(new HeaderProperty()
            {
                ColumnName = "TOTALE",
                ColumnSize = 100,
                OriginalName = "TOT",
                Export = true,
                DataType = HeaderProperty.ContentDataType.String
            });

            return header;
        }
    }
}
