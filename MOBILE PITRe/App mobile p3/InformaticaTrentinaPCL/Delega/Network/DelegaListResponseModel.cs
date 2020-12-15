using System;
using System.Collections.Generic;
using System.Net;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class DelegaListResponseModel : BaseResponseModel
    {
        [JsonProperty(PropertyName = "Elements")]
        public List<DelegaDocumentModel> elements { get; set; }

        [JsonProperty(PropertyName = "TotalRecordCount")]
        public int totalRecordCount { get; set; }

        public DelegaListResponseModel() { }

        public DelegaListResponseModel(List<DelegaDocumentModel> modelli, int totalRecordCount)
        {
            this.totalRecordCount = totalRecordCount;
            this.elements = modelli;
            this.Code = 0;
            this.StatusCode = HttpStatusCode.OK;
        }
    }
}
