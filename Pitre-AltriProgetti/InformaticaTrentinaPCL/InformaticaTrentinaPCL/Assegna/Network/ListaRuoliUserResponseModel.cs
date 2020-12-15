using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaRuoliUserResponseModel : BaseResponseModel
    {
        [JsonProperty("ListaRuoli")]
        public List<RuoloInfo> ListaRuoli { get; set; }

        public ListaRuoliUserResponseModel()
        {
        }
    }
}
