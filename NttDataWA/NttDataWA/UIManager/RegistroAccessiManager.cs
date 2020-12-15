using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class RegistroAccessiManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static DocsPaWR.RegistroAccessiReportResponse PubblicaRegistroAccessi(DocsPaWR.RegistroAccessiReportRequest request)
        {
            try
            {
                return docsPaWS.RegistroAccessiPubblicazione(request);
            }
            catch(Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}