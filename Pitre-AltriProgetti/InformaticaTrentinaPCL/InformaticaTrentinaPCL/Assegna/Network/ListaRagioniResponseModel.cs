using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaRagioniResponseModel : BaseResponseModel
    {
        [JsonProperty("Ragioni")]
        public List<Ragione> ragioni { get; set; }

        public ListaRagioniResponseModel()
        {
        }

        public ListaRagioniResponseModel(List<Ragione> ragioni)
        {
            this.ragioni = ragioni;
        }
    }
}
