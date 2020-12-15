using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class RespingiElementiResponseModel : BaseResponseModel
    {
        [JsonProperty(PropertyName = "TotalRecordCount")]
        public int totalRecordCount { get; set; }

        public List<SignDocumentModel> Elementi;
    }
}
