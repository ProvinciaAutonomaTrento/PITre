using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.ChangeRole.MVPD
{
    public interface IRoleModel : IBaseModel
	{
        Task<ChangeRoleResponseModel> ChangeRolesAsync(ChangeRoleRequestModel request);
	}
}
