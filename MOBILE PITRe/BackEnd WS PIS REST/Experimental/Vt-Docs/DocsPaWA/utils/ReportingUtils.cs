using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPAWA.DocsPaWR;
using System.Web.UI;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.utils
{
    public class ReportingUtils
    {
        private static DocsPaWebService docsPaWS = new DocsPaWebService() { Timeout = System.Threading.Timeout.Infinite };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static String GetOpenReportPageScript(bool readOnlySubTitle)
        {
            string readOnlySubT = string.Empty;
            if(readOnlySubTitle)
                readOnlySubT = "?readOnlySubTitle=true";
            return String.Format(
                "window.showModalDialog('{0}/popup/Reporting/ReportGenerator.aspx" + readOnlySubT + "', '', 'dialogHeight: 400px; dialogWidth:745px; resizable: no;status:no;scroll:yes;help:no;close:no;center:yes;');",
                Utils.getHttpFullPath());
        }

        /// <summary>
        /// Funzione per la generazione dell'anagrafica dei report
        /// </summary>
        /// <param name="contextName">Nome del contesto</param>
        /// <returns>Anagrafica dei report</returns>
        public static ReportMetadata[] GetReportRegistry(String contextName)
        {
            PrintReportResponse response = null;

            try
            {
                response = docsPaWS.GetReportRegistry(contextName);
            }
            catch (Exception e)
            {
                DocsPaUtils.Exceptions.SoapExceptionParser.ThrowOriginalException(e);

            }

            return response.ReportMetadata;

        }

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
            catch (Exception e)
            {
                DocsPaUtils.Exceptions.SoapExceptionParser.ThrowOriginalException(e);
            }

            return response.Document;

        }

        /// <summary>
        /// Request da utilizzare per la richiesta del report
        /// </summary>
        public static PrintReportRequest PrintRequest
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["reportRequest"] as PrintReportRequest;
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("Reporting");
                CallContextStack.CurrentContext.ContextState["reportRequest"] = value;
            }

        }

    }
}
