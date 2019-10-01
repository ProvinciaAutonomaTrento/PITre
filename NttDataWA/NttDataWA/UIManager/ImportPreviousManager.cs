using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Collections.Generic;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.UIManager
{

    public class ImportPreviousManager
    {

        private static DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

        public static DocsPaWR.ReportPregressi[] GetReports()
        {
            ws.Timeout = System.Threading.Timeout.Infinite;
            //Parametro true: fa il get degli item del report; false: non fa il get degli item 
            return ws.GetReports(true, UserManager.GetInfoUser(), false);
        }

        public static void DeleteReportById(string id)
        {
            ws.Timeout = System.Threading.Timeout.Infinite;
            ws.DeleteReportById(id);
        }

        public static DocsPaWR.ReportPregressi GetReportPregressi(string idReport)
        {
            return ws.GetReportPregressi(idReport, true);
        }

        public static FileDocumento ExportPregressiExcel(ReportPregressi reportSelezionato, InfoUtente infoUtente)
        {
            return ws.ExportPregressiExcel(reportSelezionato, infoUtente);
        }

        public static DocsPaWR.EsitoImportPregressi ImportPregresso(byte[] fileToImport)
        {
            ws.Timeout = System.Threading.Timeout.Infinite;
            return ws.importPregresso(UserManager.GetInfoUser(), fileToImport, false);
        }

        public static FileDocumento ExportReportExcel(List<DocsPaWR.ItemReportPregressi> repInErr)
        {
            return ws.ExportReportExcel(repInErr.ToArray());
        }

        public static void AsyncImportPregresso(DocsPaWR.InfoUtente infoUtente, DocsPaWR.EsitoImportPregressi esitoImport, string descrizione)
        {
            ws.Timeout = System.Threading.Timeout.Infinite;
            ws.asyncImportPregresso(infoUtente, esitoImport, descrizione);
        }
    }

}