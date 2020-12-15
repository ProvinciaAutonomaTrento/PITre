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
    [ReportGenerator(Name="Esecuzione Policy PARER", ContextName="EsecuzionePolicyPARER", Key="EsecuzionePolicyPARER")]
    public class EsecuzionePolicyPARERReportGeneratorCommand : ReportGeneratorCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(EsecuzionePolicyPARERReportGeneratorCommand));

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
            string tipoReport = request.SearchFilters.Where(f => f.argomento.Equals("type")).FirstOrDefault().valore;

            foreach (DocsPaVO.Report.Report r in retVal)
            {
                if (tipoReport.Equals("SUCCESS"))
                    r.Summary = "Documenti inseriti in coda di versamento: " + r.ReportMapRow.Rows.Count;
                else
                    r.Summary = string.Empty;
            }

            return retVal;
        }
    }
}
