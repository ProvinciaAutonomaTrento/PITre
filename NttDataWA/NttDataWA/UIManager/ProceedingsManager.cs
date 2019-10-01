using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class ProceedingsManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static List<string> GetTipiProcedimentoAmministrazione()
        {
            try
            {
                return docsPaWS.GetTipiProcedimentoAmministrazione(UserManager.GetInfoUser().idAmministrazione).ToList();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.ReportProcedimentoResponse GetProcedimentiReport(DocsPaWR.ReportProcedimentoRequest request)
        {
            try
            {
                return docsPaWS.GetReportProcedimento(request);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}