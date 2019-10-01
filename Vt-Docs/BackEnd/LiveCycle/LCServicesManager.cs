using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using log4net;

namespace LiveCycle
{
    class LCServicesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(LCServicesManager));
        static string serverLiveCycle = ConfigurationManager.AppSettings["LIVECYCLE_SERVICE_SERVER"].ToString();
        static string username = ConfigurationManager.AppSettings["LIVECYCLE_USERNAME"].ToString();
        static string password = ConfigurationManager.AppSettings["LIVECYCLE_PASSWORD"].ToString();
        static string processForm = ConfigurationManager.AppSettings["LIVECYCLE_SERVICE_PROCESS_FORM"].ToString();
        static string processBarcodeForm = ConfigurationManager.AppSettings["LIVECYCLE_SERVICE_PROCESS_BARCODE_FORM"].ToString();
        static string generatePdfService = ConfigurationManager.AppSettings["LIVECYCLE_SERVICE_GENERATE_PDF_SERVICE"].ToString();


        public static ProcessFormService.ProcessFormService getProcessFormService()
        {
            try
            {
                if (!string.IsNullOrEmpty(serverLiveCycle) && 
                    !string.IsNullOrEmpty(username) && 
                    !string.IsNullOrEmpty(password) &&
                    !string.IsNullOrEmpty(processForm)
                    )
                {
                    //Creo il servizio processForm
                    ProcessFormService.ProcessFormService serviceProcessForm = new ProcessFormService.ProcessFormService();
                    serviceProcessForm.Timeout = System.Threading.Timeout.Infinite;

                    //Imposto l'URL dei web services
                    //processFormPdf.Url = serverLiveCycle + "ProcessForm?wsdl&blob=base64";
                    serviceProcessForm.Url = processForm + "&blob=base64";

                    //Autenticazione
                    serviceProcessForm.Credentials = new System.Net.NetworkCredential(username, password);

                    return serviceProcessForm;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in LiveCycle  - LCServicesManager - metodo: getProcessFormService()", e);
                return null;
            }
        }

        public static GeneratePdfService.GeneratePDFServiceService getGeneratePdfService()
        {
            try
            {
                if (!string.IsNullOrEmpty(serverLiveCycle) && 
                    !string.IsNullOrEmpty(username) && 
                    !string.IsNullOrEmpty(password) &&
                    !string.IsNullOrEmpty(generatePdfService)
                    )
                {
                    //Creo il servizio generatePdfService
                    GeneratePdfService.GeneratePDFServiceService serviceGeneratePdf = new GeneratePdfService.GeneratePDFServiceService();
                    serviceGeneratePdf.Timeout = System.Threading.Timeout.Infinite;
                    
                    //Imposto l'URL dei web services
                    //generatePdfService.Url = serverLiveCycle + "GeneratePdfService?wsdl&blob=base64";
                    serviceGeneratePdf.Url = generatePdfService + "&blob=base64";

                    //Autenticazione
                    serviceGeneratePdf.Credentials = new System.Net.NetworkCredential(username, password);

                    return serviceGeneratePdf;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in LiveCycle  - LCServicesManager - metodo: getGeneratePdfService()", e);
                return null;
            }
        }

        public static ProcessBarcodeForm.ProcessBarcodeFormService getProcessBarcodeFormService()
        {
            try
            {
                if (!string.IsNullOrEmpty(serverLiveCycle) && 
                    !string.IsNullOrEmpty(username) && 
                    !string.IsNullOrEmpty(password) &&
                    !string.IsNullOrEmpty(processBarcodeForm)
                    )
                {
                    //Creo il servizio generatePdfService
                    ProcessBarcodeForm.ProcessBarcodeFormService serviceProcessBarcodeForm = new LiveCycle.ProcessBarcodeForm.ProcessBarcodeFormService();
                    serviceProcessBarcodeForm.Timeout = System.Threading.Timeout.Infinite;

                    //Imposto l'URL dei web services
                    //processBarcodeForm.Url = serverLiveCycle + "ProcessBarcodeForm?wsdl&blob=base64";
                    serviceProcessBarcodeForm.Url = processBarcodeForm + "&blob=base64";

                    //Autenticazione
                    serviceProcessBarcodeForm.Credentials = new System.Net.NetworkCredential(username, password);

                    return serviceProcessBarcodeForm;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in LiveCycle  - LCServicesManager - metodo: getProcessBarcodeForm()", e);
                return null;
            }
        }



    }
}
