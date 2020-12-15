using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using log4net;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione della rubrica
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class AdLDocument : IAdLDocument
    {

        private ILog logger = LogManager.GetLogger(typeof(Administration));

        public Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloResponse AddDocumentInAdLRuolo(Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloRequest request)
        {
            logger.Info("BEGIN");
            
            Services.AdL.AddDocumentInAdLRuolo.AddDocumentInAdLRuoloResponse response = Manager.AdLDocumentManager.AddDocumentInAdLRuolo(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        public Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteResponse AddDocumentInAdLUtente(Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteRequest request)
        {
            logger.Info("BEGIN");

            Services.AdL.AddDocumentInAdLUtente.AddDocumentInAdLUtenteResponse response = Manager.AdLDocumentManager.AddDocumentInAdlUtente(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }
    }
}
