using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    /// Definizione dei servizi per la gestione dei ruoli dei Product Integration Services.
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IRoles
    {
        [OperationContract]
        VtDocsWS.Services.Roles.GetRole.GetRoleResponse GetRole(VtDocsWS.Services.Roles.GetRole.GetRoleRequest request);

        [OperationContract]
        VtDocsWS.Services.Roles.GetRoles.GetRolesResponse GetRoles(VtDocsWS.Services.Roles.GetRoles.GetRolesRequest request);

        [OperationContract]
        VtDocsWS.Services.Roles.GetUsersInRole.GetUsersInRoleResponse GetUsersInRole(VtDocsWS.Services.Roles.GetUsersInRole.GetUsersInRoleRequest request);

        [OperationContract]
        VtDocsWS.Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse GetRolesForEnabledActions(VtDocsWS.Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsRequest request);

        [OperationContract]
        VtDocsWS.Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsResponse GetRolesForEnabledSingleFunction(Services.Roles.GetRolesForEnabledActions.GetRolesForEnabledActionsRequest request);
    }
}