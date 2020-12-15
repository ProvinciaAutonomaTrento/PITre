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
    }
}
