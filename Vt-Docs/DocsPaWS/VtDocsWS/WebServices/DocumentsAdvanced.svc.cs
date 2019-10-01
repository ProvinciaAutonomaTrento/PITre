using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using System.Diagnostics;
using System.Reflection;
using log4net;
using DocsPaWS.VtDocsWS;

namespace VtDocsWS.WebServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DocumentsAdvanced" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class DocumentsAdvanced : IDocumentsAdvanced
    {

        private ILog logger = LogManager.GetLogger(typeof(DocumentsAdvanced));

        
        public Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentResponse RemoveDocument(Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentRequest request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.RemoveDocument.RemoveDocumentResponse response = VtDocsWS.Manager.DocumentsAdvancedManager.RemoveDocument(request);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedResponse SendDocumentAdvanced(Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedRequest request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.SendDocumentAdvanced.SendDocumentAdvancedResponse response = VtDocsWS.Manager.DocumentsAdvancedManager.SendDocumentAdvanced(request);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaResponse FatturaEsitoNotifica(Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaRequest request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.FatturaEsitoNotifica.FatturaEsitoNotificaResponse response = VtDocsWS.Manager.DocumentsAdvancedManager.FatturaEsitoNotifica(request);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse NuovaFattura(Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaRequest request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse response = VtDocsWS.Manager.DocumentsAdvancedManager.NuovaFattura(request);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response CaricaLottoInPi3(Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Request request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response response = VtDocsWS.Manager.DocumentsAdvancedManager.CaricaLottoInPi3(request);
            
            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public Services.DocumentsAdvanced.C3GetDocs.C3GetDocsResponse C3GetDocs(Services.DocumentsAdvanced.C3GetDocs.C3GetDocsRequest request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.C3GetDocs.C3GetDocsResponse response = Manager.DocumentsAdvancedManager.C3GetDocs(request);
            
            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public bool BachecaCircolarePubblicata(Services.DocumentsAdvanced.CircolarePubblicata.CircolarePubblicataRequest request)
        {
            logger.Info("BEGIN");

            bool response = Manager.DocumentsAdvancedManager.BachecaCircolarePubblicata(request);

            logger.Info("END");

            return response;
        }

        public Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse NuovaFatturaAttiva(Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaRequest request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.NuovaFattura.NuovaFatturaResponse response = VtDocsWS.Manager.DocumentsAdvancedManager.NuovaFatturaAttiva(request);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response NuovoLottoAttivo(Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Request request)
        {
            logger.Info("BEGIN");

            Services.DocumentsAdvanced.CaricaLottoInPi3.CaricaLottoInPi3Response response = VtDocsWS.Manager.DocumentsAdvancedManager.NuovoLottoAttivo(request);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }
    }
}
