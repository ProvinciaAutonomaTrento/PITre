using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assign.Network
{
    public class LoadAssignableRequestModel : BaseRequestModel
    {
		public string ID_Agent { get; set; }
        public AssigneeModel.AssignableTypeEnum AssignableType { get; set; }

        public LoadAssignableRequestModel(string ID_Agent, AssigneeModel.AssignableTypeEnum assignableType)
		{
			this.ID_Agent = ID_Agent;
            this.AssignableType = assignableType;
		}
    }
}
