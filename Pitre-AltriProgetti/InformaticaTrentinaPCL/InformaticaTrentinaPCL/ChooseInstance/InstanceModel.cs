using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Login.Network
{
    public class InstanceModel : BaseResponseModel
    {
        [JsonProperty(PropertyName = "Nome")]
        public string nome { get; set; }

        [JsonProperty(PropertyName = "Descrizione")]
        public string descrizione { get; set; }

        [JsonProperty(PropertyName = "Tipo")]
        public string tipo { get; set; }

        [JsonProperty(PropertyName = "Url")]
        public string url { get; set; }
    }
}
