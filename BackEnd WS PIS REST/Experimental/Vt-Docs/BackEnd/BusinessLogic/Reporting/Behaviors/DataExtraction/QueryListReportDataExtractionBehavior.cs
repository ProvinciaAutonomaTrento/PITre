using System;
using System.Data;
using System.Reflection;
using BusinessLogic.Reporting.Behaviors.DataExtraction.Exceptions;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting.Behaviors.DataExtraction
{
    /// <summary>
    /// Questa classe realizza un estrattore di dati sfruttando il query list
    /// </summary>
    public class QueryListReportDataExtractionBehavior : IReportDataExtractionBehavior
    {
      
        /// <summary>
        /// Metodo per l'estrazione dati da query list
        /// </summary>
        /// <param name="request">Informazioni sull'operazione da compiere</param>
        /// <returns>DataSet con i dati estratti</returns>
        public DataSet ExtractData(PrintReportRequest request)
        {
            MethodInfo reportMethod = DocsPaDB.Query_DocsPAWS.Reporting.ReportMethodFinder.FindMethod(request.ReportKey);

            if (reportMethod == null)
                throw new DataExtractorNotFoundException();

             object instance = Activator.CreateInstance(reportMethod.DeclaringType);

             // Invocazione nome per reflection e recupero del dataset con il risultato
             return reportMethod.Invoke(instance, new object[] { request.UserInfo, request.SearchFilters }) as DataSet;
        }
    }
}
