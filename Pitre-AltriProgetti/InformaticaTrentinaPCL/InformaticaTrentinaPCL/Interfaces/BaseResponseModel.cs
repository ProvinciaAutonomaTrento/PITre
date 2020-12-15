using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Interfaces
{
    public class BaseResponseModel : HttpResponseMessage
    {

        public bool IsCancelled { get; set; } = false;

        [JsonProperty(PropertyName = "Errore")]
        public string Error { get; set; } = null; 

        public BaseResponseModel()
        {
        }

        [JsonProperty(PropertyName = "Code")]
        public int Code { get; set; }
    }
}
