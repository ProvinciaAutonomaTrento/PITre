using System;
using System.Collections.Generic;
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
    [ReportGenerator(Name="Report Versamenti PARER Empty", ContextName="ReportVersamentiPAREREmpty", Key="ReportVersamentiPAREREmpty")]
    public class ReportVersamentiPAREREmptyReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(ReportVersamentiPAREREmptyReportGeneratorCommand));

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }

        protected override PrintReportResponse GenerateReport(PrintReportRequest request)
        {
            IReportGeneratorBehavior formatGenerator = base.CreateReportGenerator(request);
            IReportDataExtractionBehavior reportDataExtractor = new DatasetReportDataExtractionBehavior();

            return base.GenerateReport(request, reportDataExtractor, formatGenerator);
        }

        protected override List<DocsPaVO.Report.Report> CreateReport(System.Data.DataSet dataSet, PrintReportRequest request)
        {
            List<DocsPaVO.Report.Report> retVal = base.CreateReport(dataSet, request);

            foreach (DocsPaVO.Report.Report r in retVal)
            {
                r.Summary = string.Empty;
            }

            return retVal;
        }


    }
}
