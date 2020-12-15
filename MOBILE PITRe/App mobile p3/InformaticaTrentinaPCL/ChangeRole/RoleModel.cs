using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole.MVPD;
using InformaticaTrentinaPCL.ChangeRole.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.ChangeRole
{
    public class RoleModel : WS, IRoleModel
    {
        public RoleModel()
        {
        }

        public async Task<ChangeRoleResponseModel> ChangeRolesAsync(ChangeRoleRequestModel request)
        {
            ChangeRoleResponseModel response;
            try
            {
                response = await api.ChangeRole(request.role.id, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new ChangeRoleResponseModel();
                ResolveError(response, e);

            }
            return response;
        }
    }
}
