using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using BusinessLogic.Reporting.Exceptions;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using log4net;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(Name="Export Ricerca Rubrica", ContextName="ExportRubricaSearch", Key="ExportRubricaSearch")]
    public class ExportRubricaSearchReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(ExportRubricaSearchReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request, IReportDataExtractionBehavior dataExtractor, IReportGeneratorBehavior reportGeneration)
        {

            // Il DataSet da utilizzare nel report è contenuto all'interno della request
            // per non replicare tutti i metodi necessari alla ricerca in rubrica

            PrintReportResponse response = null;
            try
            {
                // eseguo il cast della request
                PrintReportRequestDataset casted = request as PrintReportRequestDataset;

                if (casted == null)
                    throw new RequestNotValidException();

                response = new PrintReportResponse();

                List<DocsPaVO.Report.Report> report = new List<DocsPaVO.Report.Report>();

                if (casted.InputDataset != null)
                    report.AddRange(this.CreateReport(casted.InputDataset, request));

                response.Document = reportGeneration.GenerateReport(request, report);

            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella generazione del report - ", ex);
                throw new ReportGenerationFailedException(ex.Message);
            }

            return response;
            
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            report.Title = request.Title;
            report.CreationDate = DateTime.Now;
            report.Subtitle = request.SubTitle;
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();
            report.SectionName = "RUBRICA";
            report.ShowHeaderRow = false;
            report.Summary = string.Empty;

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport);
                report.ReportMapRow.Rows.AddRange(base.GenerateReportRows(dataSet, report.ReportHeader));
            }

            return new List<DocsPaVO.Report.Report>() { report };
        }

        protected override HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = base.GenerateReportHeaderFromDataSet(dataSet);

            // modifico le dimensioni delle colonne
            if (header["Storicizza"] != null)
                header["Storicizza"].ColumnSize = 60;

            header["Cod. Registro"].ColumnSize = 50;
            header["Cod. Rubrica"].ColumnSize = 80;
            header["Cod. Amm."].ColumnSize = 60;
            header["Cod. AOO"].ColumnSize = 60;
            header["Tipo"].ColumnSize = 40;
            header["Descrizione"].ColumnSize = 180;
            header["Cognome"].ColumnSize = 70;
            header["Nome"].ColumnSize = 70;
            header["Indirizzo"].ColumnSize = 120;
            header["CAP"].ColumnSize = 40;
            header["Città"].ColumnSize = 60;
            header["Provincia"].ColumnSize = 50;
            header["Nazione"].ColumnSize = 50;
            header["Cod. Fiscale"].ColumnSize = 80;
            header["P. IVA"].ColumnSize = 80;
            header["Tel 1"].ColumnSize = 60;
            header["Tel 2"].ColumnSize = 60;
            header["Fax"].ColumnSize = 60;
            header["Email"].ColumnSize = 100;
            header["Località"].ColumnSize = 60;
            header["Note"].ColumnSize = 120;
            header["Nuovo registro"].ColumnSize = 80;
            header["Canale preferenziale"].ColumnSize = 100;

            return header;
        }


    }
}
