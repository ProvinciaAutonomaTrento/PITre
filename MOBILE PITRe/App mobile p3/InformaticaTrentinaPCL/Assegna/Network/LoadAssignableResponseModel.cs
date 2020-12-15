using System;
using System.Collections.Generic;
using System.Net;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assign.Network
{
    public class LoadAssignableResponseModel : BaseResponseModel
    {
		public List<AssigneeModel> assignables { get; set; }


        public LoadAssignableResponseModel()
        {

        }

        public LoadAssignableResponseModel(List<AssigneeModel> assignables)
		{
			this.assignables = assignables;
            StatusCode = HttpStatusCode.OK;
		}
    }
}
