using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetFascRicercaResponseModel : BaseResponseModel
    {
        [JsonProperty("Risultati")]
        public List<ResultFascRicerca> risultati { get; set; }

        [JsonProperty("TotalRecordCount")]
        public int totalRecordCount { get; set; }

        public GetFascRicercaResponseModel()
        {
        }

        public GetFascRicercaResponseModel(List<ResultFascRicerca> risultati, int totalRecordCount)
        {
            this.risultati = risultati;
            this.totalRecordCount = totalRecordCount;
        }
    }
}
