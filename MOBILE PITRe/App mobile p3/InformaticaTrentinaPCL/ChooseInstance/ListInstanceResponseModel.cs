using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.Network;

namespace InformaticaTrentinaPCL.ChooseInstance
{
    public class ListInstanceResponseModel: BaseResponseModel
    {
        public List<InstanceModel> instances { get; set; }

        public ListInstanceResponseModel() { }

        public ListInstanceResponseModel(List<InstanceModel> list)
        {
            this.instances = list;
        }
    }
}
