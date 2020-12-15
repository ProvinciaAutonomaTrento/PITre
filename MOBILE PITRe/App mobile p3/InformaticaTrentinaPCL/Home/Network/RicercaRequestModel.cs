using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class RicercaRequestModel : BaseRequestModel
    {
        public Body body;

        public RicercaRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            //[JsonProperty(PropertyName = "IdRicercaSalvata")]
            //public string idRicercaSalvata { get; set; }

            //[JsonProperty(PropertyName = "TypeRicercaSalvata")]
            //public int typeRicercaSalvata { get; set; }

            //[JsonProperty(PropertyName = "TipoRicercaSalvata")]
            //public string tipoRicercaSalvata { get; set; }

            [JsonProperty(PropertyName = "Text")]
            public string text { get; set; }

            //[JsonProperty(PropertyName = "TypeRicerca")]
            //public int typeRicerca { get; set; }

            [JsonProperty(PropertyName = "TipoRicerca")]
            public string tipoRicerca { get; set; }

            //[JsonProperty(PropertyName = "ParentFolderId")]
            //public string parentFolderId { get; set; }

            //[JsonProperty(PropertyName = "FascId")]
            //public string fascId { get; set; }

            //[JsonProperty(PropertyName = "EnableProfilazione")]
            //public bool enableProfilazione { get; set; }

            //[JsonProperty(PropertyName = "EnableUfficioRef")]
            //public bool enableUfficioRef { get; set; }

            [JsonProperty(PropertyName = "RequestedPage")]
            public int requestedPage { get; set; }

            [JsonProperty(PropertyName = "PageSize")]
            public int pageSize { get; set; }

            [JsonProperty(PropertyName = "DataDa")]
            public string dataDa { get; set; }

            [JsonProperty(PropertyName = "DataA")]
            public string dataA { get; set; }

            [JsonProperty(PropertyName = "DataProtoDa")]
            public string dataProtoDa { get; set; }

            [JsonProperty(PropertyName = "DataProtoA")]
            public string dataProtoA { get; set; }

            [JsonProperty(PropertyName = "IdDocumento")]
            public string idDocumento { get; set; }

            [JsonProperty(PropertyName = "NumProto")]
            public string numProto { get; set; }

            [JsonProperty(PropertyName = "NumAnnoProto")]
            public string numAnnoProto { get; set; }

            public Body(int pageSize, int requestedPage,  string tipoRicerca)
            {
                this.requestedPage = requestedPage;
                this.pageSize = pageSize;
                this.tipoRicerca = tipoRicerca;
            }

            public Body(int pageSize,  int requestedPage, string text, string tipoRicerca){
                this.requestedPage = requestedPage;
                this.pageSize = pageSize;
                this.text = text;
                this.tipoRicerca = tipoRicerca;
            }

            public Body(int pageSize, int requestedPage, string tipoRicerca, DateTime dataDa, DateTime dataA, DateTime dataProtoDa, DateTime dataProtoA, string idDocumento, string numProto, string numAnnoProto)
            {
                this.requestedPage = requestedPage;
                this.pageSize = pageSize;
                this.text = text;
                this.tipoRicerca = tipoRicerca;
                this.dataDa = DateTime.MinValue != dataDa ? dataDa.ToReadableString() : null;
                this.dataA =  DateTime.MinValue != dataA ? dataA.ToReadableString(): null;
                this.dataProtoDa = DateTime.MinValue != dataProtoDa ? dataProtoDa.ToReadableString() : null;
                this.dataProtoA = DateTime.MinValue != dataProtoA ? dataProtoA.ToReadableString() : null;;
                this.idDocumento = idDocumento;
                this.numProto = numProto;
                this.numAnnoProto = numAnnoProto;
            }
        }
    }
}
