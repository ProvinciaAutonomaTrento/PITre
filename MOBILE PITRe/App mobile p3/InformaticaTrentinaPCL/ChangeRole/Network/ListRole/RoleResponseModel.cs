using System;
using System.Collections.Generic;
using System.Net;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.ChangeRole.Network
{
	public class RoleResponseModel : BaseResponseModel
	{
        public List<RuoloInfo> roles { get; set; }

        public RoleResponseModel()
        {

        }

        public RoleResponseModel(List<RuoloInfo> roles)
		{
			this.roles = roles;
            StatusCode = HttpStatusCode.OK;
		}
	}
}
