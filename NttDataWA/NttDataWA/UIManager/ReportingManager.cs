using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using System.Web.UI;


namespace NttDataWA.UIManager
{
    public class ReportingManager
    {
        private static DocsPaWebService docsPaWS = new DocsPaWebService() { Timeout = System.Threading.Timeout.Infinite };
        /// <summary>
        /// Funzione per la generazione di un report
        /// </summary>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <returns>Report prodotto</returns>
        public static FileDocumento GenerateReport(PrintReportRequest request)
        {
            PrintReportResponse response = null;

            try
            {
                response = docsPaWS.GenerateReport(request);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return response.Document;
        }
    }
}
