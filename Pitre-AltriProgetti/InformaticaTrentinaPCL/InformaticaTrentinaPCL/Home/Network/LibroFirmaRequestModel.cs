using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class LibroFirmaRequestModel : BaseRequestModel
    {
        public Body body;

        public LibroFirmaRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            [JsonProperty(PropertyName = "RequestedPage")]
            public int requestedPage { get; set; }

            [JsonProperty(PropertyName = "PageSize")]
            public int pageSize { get; set; }

            [JsonProperty(PropertyName = "Testo")]
            public string testo { get; set; }
            
            [JsonProperty(PropertyName = "Oggetto")]
            public string oggetto { get; set; }

            [JsonProperty(PropertyName = "TipoRicerca")]
            public string tipoRicerca { get; set; }

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

            public Body( int pageSize, int requestedPage, string testo = "", string tipoRicerca = "")
            {
                this.requestedPage = requestedPage;
                this.pageSize = pageSize;
                this.testo = testo;
                this.tipoRicerca = tipoRicerca;
            }

            public Body(int pageSize, int requestedPage, string tipoRicerca, DateTime dataDa, DateTime dataA, DateTime dataProtoDa, DateTime dataProtoA, string idDocumento, string numProto, string numAnnoProto)
            {
                this.requestedPage = requestedPage;
                this.pageSize = pageSize;
                this.tipoRicerca = tipoRicerca;
                this.dataDa = DateTime.MinValue != dataDa ? dataDa.ToReadableString() : null;
                this.dataA = DateTime.MinValue != dataA ? dataA.ToReadableString() : null;
                this.dataProtoDa = DateTime.MinValue != dataProtoDa ? dataProtoDa.ToReadableString() : null;
                this.dataProtoA = DateTime.MinValue != dataProtoA ? dataProtoA.ToReadableString() : null; ;
                this.idDocumento = idDocumento;
                this.numProto = numProto;
                this.numAnnoProto = numAnnoProto;
            }
        }
    }
}
