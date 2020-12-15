using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.RecuperaPassword
{
    public class RecuperaPasswordModel : BaseResponseModel
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
