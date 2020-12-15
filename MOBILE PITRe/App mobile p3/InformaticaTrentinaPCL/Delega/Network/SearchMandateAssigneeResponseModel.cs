using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class SearchMandateAssigneeResponseModel : BaseResponseModel
    {
        [JsonProperty("Risultati")]
        public List<SearchMandateAssignee> Risultati { get; set; }

        public SearchMandateAssigneeResponseModel()
        {
        }
    }
}
