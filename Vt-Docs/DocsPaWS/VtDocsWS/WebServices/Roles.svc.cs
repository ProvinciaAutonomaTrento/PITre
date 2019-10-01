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
    public class Roles : IRoles
    {
        private ILog logger = LogManager.GetLogger(typeof(Roles));

        /// <summary>
        /// Servizio per il reperimento dei ruoli che possono effettuare una determinata azione (Corr_globali: VAR_COD_TIPO)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse GetRolesForEnabledActions(Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsRequest request)
        {
            Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse response = Manager.RolesManager.GetRolesForEnabledActions(request);

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei ruoli che hanno configurata una determinata micro funzione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse GetRolesForEnabledSingleFunction(Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsRequest request)
        {
            Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse response = Manager.RolesManager.GetRolesForEnabledSingleFunction(request);

            Utils.CheckFaultException(response);

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un ruolo dato il codice o l'id
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Roles.GetRole.GetRoleResponse GetRole(Services.Roles.GetRole.GetRoleRequest request)
        {
            logger.Info("BEGIN");
            Services.Roles.GetRole.GetRoleResponse response = Manager.RolesManager.GetRole(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei ruoli dato un utente
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Roles.GetRoles.GetRolesResponse GetRoles(Services.Roles.GetRoles.GetRolesRequest request)
        {
            logger.Info("BEGIN");
            Services.Roles.GetRoles.GetRolesResponse response = Manager.RolesManager.GetRoles(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento degli utenti in un ruolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Roles.GetUsersInRole.GetUsersInRoleResponse GetUsersInRole(Services.Roles.GetUsersInRole.GetUsersInRoleRequest request)
        {
            logger.Info("BEGIN");
            Services.Roles.GetUsersInRole.GetUsersInRoleResponse response = Manager.RolesManager.GetUsersInRole(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }
    }
}
