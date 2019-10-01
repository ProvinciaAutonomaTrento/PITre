using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using log4net;
using System.Web;
using DocsPaWS.VtDocsWS;
using VtDocsWS.Services;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione dei documenti
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class LibroFirma : ILibroFirma
    {
        private ILog logger = LogManager.GetLogger(typeof(LibroFirma));
   
        /// <summary>
        /// A partire dai dati del passo attivato, crea l'elemento in libro firma
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.LibroFirma.AddElementoInLF.AddElementoInLFResponse AddElementoInLF(Services.LibroFirma.AddElementoInLF.AddElementoInLFRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.AddElementoInLF.AddElementoInLFResponse response = Manager.LibroFirmaManager.AddElementoInLF(request);

            // Log
            //
            DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
            DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");

            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "ADDELEMINLF", request.IdPasso, "Creazione nuovo elemento in libro firma per il passo. " + request.IdPasso + " in modalità " + request.Modalita, esito);

            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// A partire dai dati del passo attivato, crea l'elemento in libro firma
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextResponse ClosePassoAndGetNext (Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.ClosePassoAndGetNext.ClosePassoAndGetNextResponse response = Manager.LibroFirmaManager.ClosePassoAndGetNext(request);
            if (response.Success)
            {
                
                DocsPaVO.Logger.CodAzione.Esito esito = (response != null && response.Success ? DocsPaVO.Logger.CodAzione.Esito.OK : DocsPaVO.Logger.CodAzione.Esito.KO);
                DocsPaVO.utente.InfoUtente infoUtente = Utils.CheckAuthentication(request, "");

                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "ADDELEMINLF", request.IdIstanzaPasso, "Creazione nuovo elemento in libro firma per il passo. " + request.IdIstanzaPasso + " in modalità " + request.OrdinePasso, esito);
            }
            logger.Info("END");

            Utils.CheckFaultException(response);

            return response;
        }

        public Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesResponse GetSignatureProcesses(Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.GetSignatureProcesses.GetSignatureProcessesResponse response = Manager.LibroFirmaManager.GetSignatureProcesses(request);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        public Services.LibroFirma.GetSignatureProcess.GetSignatureProcessResponse GetSignatureProcess(Services.LibroFirma.GetSignatureProcess.GetSignatureProcessRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.GetSignatureProcess.GetSignatureProcessResponse response = Manager.LibroFirmaManager.GetSignatureProcess(request);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        public Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceResponse GetSignProcessInstance(Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.GetSignProcessInstance.GetSignProcessInstanceResponse response = Manager.LibroFirmaManager.GetSignProcessInstance(request);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        public Services.LibroFirma.GetInstanceSearchFilters.GetInstanceSearchFiltersResponse GetInstanceSearchFilters(Request request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.GetInstanceSearchFilters.GetInstanceSearchFiltersResponse response = Manager.LibroFirmaManager.GetInstanceSearchFilters(request);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        public Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesResponse SearchSignProcessInstances(Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.SearchSignProcessInstances.SearchSignProcessInstancesResponse response = Manager.LibroFirmaManager.SearchSignProcessInstances(request);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        public Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessResponse InterruptSignatureProcess(Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.InterruptSignatureProcess.InterruptSignatureProcessResponse response = Manager.LibroFirmaManager.InterruptSignatureProcess(request);

            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }

        public Services.LibroFirma.StartSignatureProcess.StartSignatureProcessResponse StartSignatureProcess(Services.LibroFirma.StartSignatureProcess.StartSignatureProcessRequest request)
        {
            logger.Info("BEGIN");

            Services.LibroFirma.StartSignatureProcess.StartSignatureProcessResponse response = Manager.LibroFirmaManager.StartSignatureProcess(request);
            
            logger.Info("END");
            Utils.CheckFaultException(response);

            return response;
        }
    }
}
