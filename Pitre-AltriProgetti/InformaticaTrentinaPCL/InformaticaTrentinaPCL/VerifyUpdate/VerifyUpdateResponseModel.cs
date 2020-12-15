using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.VerifyUpdate
{
    public class VerifyUpdateResponseModel : BaseResponseModel
    {
        //public VerifyUpdateResponseModel(int code)
        //{
        //    this.Code = code;
        //}

        public VerifyUpdateResponseModel()
        {
            
        }

        [JsonProperty(PropertyName = "Url")]
        public string url { get; set; }

        
    }
}
