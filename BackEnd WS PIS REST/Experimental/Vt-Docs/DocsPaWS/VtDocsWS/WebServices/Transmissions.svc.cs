using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using VtDocsWS.Services;
using log4net;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione delle trasmissioni
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Transmissions : ITransmissions
    {
        private ILog logger = LogManager.GetLogger(typeof(Transmissions));

        /// <summary>
        /// Servizio per l'esecuzione della trasmissione di un documento attraverso un modello di trasmissione
        /// </summary>
        /// <param name="Request"></param>
        /// <returns>Response</returns>
        public Services.Transmissions.ExecuteTransmDocModel.ExecuteTransmDocModelResponse ExecuteTransmDocModel(Services.Transmissions.ExecuteTransmDocModel.ExecuteTransmDocModelRequest request)
        {
            logger.Info("BEGIN");
            Services.Transmissions.ExecuteTransmDocModel.ExecuteTransmDocModelResponse response = Manager.TransmissionsManager.ExecuteTransmDocModel(request);
            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "EXECUTETRANSMDOCMODEL", request.DocumentId, "Trasmissione del documento con id " + request.DocumentId + " con il modello di trasmissione con id " + request.IdModel, esito);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per la trasmissione di un fascicolo attraverso un modello di trasmissione
        /// </summary>
        /// <param name="Request"></param>
        /// <returns>Response</returns>
        public Services.Transmissions.ExecuteTransmPrjModel.ExecuteTransmPrjModelResponse ExecuteTransmPrjModel(Services.Transmissions.ExecuteTransmPrjModel.ExecuteTransmPrjModelRequest request)
        {
            logger.Info("BEGIN");
            Services.Transmissions.ExecuteTransmPrjModel.ExecuteTransmPrjModelResponse response = Manager.TransmissionsManager.ExecuteTransmPrjModel(request);
            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "EXECUTETRANSMPRJMODEL", request.IdProject, "Trasmissione del fascicolo con id " + request.IdProject + " con il modello di trasmissione con id " + request.IdModel, esito);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il dettaglio di un modello di trasmissione dato un id o il codice
        /// </summary>
        /// <param name="Request"></param>
        /// <returns>Response</returns>
        public Services.Transmissions.GetTransmissionModel.GetTransmissionModelResponse GetTransmissionModel(Services.Transmissions.GetTransmissionModel.GetTransmissionModelRequest request)
        {
            logger.Info("BEGIN");
            Services.Transmissions.GetTransmissionModel.GetTransmissionModelResponse response = Manager.TransmissionsManager.GetTransmissionModel(request);
            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "GETTRANSMISSIONMODEL", request.IdModel, "Prelievo dei dettagli del modello di trasmissione con id " + request.IdModel, esito);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un ruolo dato il codice o l'id
        /// </summary>
        /// <param name="Request"></param>
        /// <returns>Response</returns>
        public Services.Transmissions.GetTransmissionModels.GetTransmissionModelsResponse GetTransmissionModels(Services.Transmissions.GetTransmissionModels.GetTransmissionModelsRequest request)
        {
            logger.Info("BEGIN");
            Services.Transmissions.GetTransmissionModels.GetTransmissionModelsResponse response = Manager.TransmissionsManager.GetTransmissionModels(request);
            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "GETTRANSMISSIONMODELS", request.UserName, "Prelievo dei modelli di trasmissione da parte dell'utente " + request.UserName, esito);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per la trasmissione di un documento senza l'utilizzo di un modello di trasmissione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Transmissions.ExecuteTransmissionDocument.ExecuteTransmissionDocumentResponse ExecuteTransmissionDocument(Services.Transmissions.ExecuteTransmissionDocument.ExecuteTransmissionDocumentRequest request)
        {
            logger.Info("BEGIN");
            Services.Transmissions.ExecuteTransmissionDocument.ExecuteTransmissionDocumentResponse response = Manager.TransmissionsManager.ExecuteTransmissionDocument(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "EXECUTETRANSMISSIONDOCUMENT", request.IdDocument, "Trasmissione del documento " + request.IdDocument + " con ragione " + request.TransmissionReason + " verso destinatario con codice " + request.Receiver.Code + " o id " + request.Receiver.Id, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);
            return response;
        }

        /// <summary>
        /// Servizion per la trasmissione di un fascicolo senza l'utilizzo di un modello di trasmissione.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Transmissions.ExecuteTransmissionProject.ExecuteTransmissionProjectResponse ExecuteTransmissionProject(Services.Transmissions.ExecuteTransmissionProject.ExecuteTransmissionProjectRequest request)
        {
            logger.Info("BEGIN");
            Services.Transmissions.ExecuteTransmissionProject.ExecuteTransmissionProjectResponse response = Manager.TransmissionsManager.ExecuteTransmissionProject(request);
            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");
            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "EXECUTETRANSMISSIONPRJ", request.IdProject, "Trasmissione del fascicolo " + request.IdProject + " con ragione " + request.TransmissionReason + " verso destinatario con codice " + request.Receiver.Code + " o id " + request.Receiver.Id, esito);

            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;

        }

        public Services.Transmissions.GiveUpRights.GiveUpRightsResponse GiveUpRights(Services.Transmissions.GiveUpRights.GiveUpRightsRequest request)
        {
            logger.Info("BEGIN");
            Services.Transmissions.GiveUpRights.GiveUpRightsResponse response = Manager.TransmissionsManager.GiveUpRights(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }
    }
}
