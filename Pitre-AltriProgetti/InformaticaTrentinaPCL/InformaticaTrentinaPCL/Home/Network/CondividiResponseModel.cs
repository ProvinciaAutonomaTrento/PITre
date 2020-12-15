using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class CondividiResponseModel : BaseResponseModel
    {
        public string key { get; set; }

        public CondividiResponseModel()
        {
        }

        public CondividiResponseModel(string key)
        {
            this.key = key;
        }
    }
}
