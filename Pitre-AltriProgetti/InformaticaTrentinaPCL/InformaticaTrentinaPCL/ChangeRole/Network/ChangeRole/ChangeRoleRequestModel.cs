using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.ChangeRole.Network
{
    public class ChangeRoleRequestModel:BaseRequestModel
	{
        public RuoloInfo role;

        public ChangeRoleRequestModel(Login.UserInfo user,RuoloInfo role)
		{
            this.role = role;
            this.token = user.token;
		}
     }
}
