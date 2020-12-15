using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole.MVPD;
using InformaticaTrentinaPCL.ChangeRole.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.ChangeRole
{
    public class DummyRoleModel : WS, IRoleModel
    {
        public DummyRoleModel()
        {
        }

        public async Task<ChangeRoleResponseModel> ChangeRolesAsync(ChangeRoleRequestModel request)
        {
            ChangeRoleResponseModel response;
            try
            {
                response = new ChangeRoleResponseModel(await this.CreateLocalMockChangeRole(request.role.descrizione));

            }
            catch (Exception e)
            {

                response = new ChangeRoleResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        private async Task<string> CreateLocalMockChangeRole(String newRole)
        {
            await Task.Delay(1000);
            return newRole;  // ritorna il role scelto dall utente 
        }
    }
}
