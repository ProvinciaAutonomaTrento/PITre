using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetDocumentoCondivisoResponseModel : BaseResponseModel
    {
        [JsonProperty("Allegati")]
        public List<DocInfo> allegati { get; set; }

        [JsonProperty("DocInfo")]
        public DocInfo docInfo { get; set; }

        public GetDocumentoCondivisoResponseModel()
        {
        }

        public GetDocumentoCondivisoResponseModel(List<DocInfo> allegati, DocInfo docInfo)
        {
            this.allegati = allegati;
            this.docInfo = docInfo;
        }

        public List<DocInfo> GetDocumentsList()
        {
            List<DocInfo> list = new List<DocInfo>();
            list.Add(docInfo); //Info relative al documento stesso

            if (null != allegati && allegati.Count >0)
            {
                allegati.RemoveAt(0);
                list.AddRange(allegati);
            }
            return list;
        }
    }
}
