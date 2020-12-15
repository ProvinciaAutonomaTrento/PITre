using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetDocInfoResponseModel : GetDocumentoCondivisoResponseModel
    {
        [JsonProperty("TrasmInfo")]
        public TrasmInfo trasmInfo { get; set; }

        public GetDocInfoResponseModel()
        {
        }

        public GetDocInfoResponseModel(List<DocInfo> allegati, DocInfo docInfo, TrasmInfo trasmInfo)
        {
            this.allegati = allegati;
            this.docInfo = docInfo;
            this.trasmInfo = trasmInfo;
        }
    }
}
