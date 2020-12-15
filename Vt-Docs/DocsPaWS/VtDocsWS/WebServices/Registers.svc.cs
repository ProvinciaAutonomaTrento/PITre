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
    /// Metodi per la gestione dei registri
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Registers : IRegisters
    {

        private ILog logger = LogManager.GetLogger(typeof(Registers));

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un registor dato un codice o l'id
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Registers.GetRegisterOrRF.GetRegisterOrRFResponse GetRegisterOrRF(Services.Registers.GetRegisterOrRF.GetRegisterOrRFRequest request)
        {
            logger.Info("BEGIN");
            Services.Registers.GetRegisterOrRF.GetRegisterOrRFResponse response = Manager.RegistersManager.GetRegisterOrRF(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei registri/rf visibili da un ruolo dato un codice o l'id di un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Registers.GetRegistersOrRF.GetRegistersOrRFResponse GetRegistersOrRF(Services.Registers.GetRegistersOrRF.GetRegistersOrRFRequest request)
        {
            logger.Info("BEGIN");
            Services.Registers.GetRegistersOrRF.GetRegistersOrRFResponse response = Manager.RegistersManager.GetRegistersOrRF(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }
    }
}
