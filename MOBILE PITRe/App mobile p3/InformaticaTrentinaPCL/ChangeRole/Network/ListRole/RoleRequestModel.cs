using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.ChangeRole.Network
{
	public class RoleRequestModel : BaseRequestModel
	{
		public string IDUser { get; set; }
        public RuoloInfo role { get; set; }

        public RoleRequestModel(string IDUser, RuoloInfo role)
		{
			this.IDUser = IDUser;
            this.role = role;
		}

	}

}
