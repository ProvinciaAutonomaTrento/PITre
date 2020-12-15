using System;
using System.Data;
using System.Reflection;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Behaviors.DataExtraction.Exceptions;
using System.Linq;

namespace BusinessLogic.Reporting.Behaviors.DataExtraction
{
    class DatasetReportDataExtractionBehavior : IReportDataExtractionBehavior
    {

        /// <summary>
        /// </summary>
        /// <param name="request">Informazioni sull'operazione da compiere</param>
        /// <returns>DataSet con i dati estratti</returns>
        public DataSet ExtractData(PrintReportRequest request)
        {
            /*
            MethodInfo reportMethod = DocsPaDB.Query_DocsPAWS.Reporting.ReportMethodFinder.FindMethod(request.ContextName);

            if (reportMethod == null)
                throw new DataExtractorNotFoundException();

            object instance = Activator.CreateInstance(reportMethod.DeclaringType);

            // Invocazione nome per reflection e recupero del dataset con il risultato
            return reportMethod.Invoke(instance, new object[] { request.UserInfo, request.SearchFilters }) as DataSet;
             */
            PrintReportRequestDataset req = (PrintReportRequestDataset)request; 
            return req.InputDataset;
        }
    }
}
