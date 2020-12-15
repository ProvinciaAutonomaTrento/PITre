using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;

namespace BusinessLogic.Reporting
{
    [ReportGenerator(ContextName="FindAndReplace", Key="FindAndReplace", Name="Report esecuzione trova e sostituisci")]
    class FindAndReplaceRuoliInModTrasmReportGeneratorCommand : ReportGeneratorCommand
    {
        protected override PrintReportResponse GenerateReport(PrintReportRequest request)
        {
            IReportDataExtractionBehavior reportDataExtractor = new ObjectTrasformationDataExtractionBehavior();
            IReportGeneratorBehavior formatGenerator = base.CreateReportGenerator(request);

            // Generazione del report
            return base.GenerateReport(request, reportDataExtractor, formatGenerator);
        }

        protected override HeaderColumnCollection GetExportableFieldsCollection()
        {
            return null;
        }
    }
}
