using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class EseguiTrasmissioneRequestModel : BaseRequestModel
    {
		public Body body;

        public EseguiTrasmissioneRequestModel(Body requestBody, string token)
        {
            this.body = requestBody;
            this.token = token;
        }

        public class Body
        {
            [JsonProperty("IdFasc")]
            public string IdFasc { get; set; }

            [JsonProperty("IdDestinatario")]
            public string IdDestinatario { get; set; }

            [JsonProperty("CodiceDestinatario")]
            public string CodiceDestinatario { get; set; }

            [JsonProperty("IdDoc")]
            public string IdDoc { get; set; }

            [JsonProperty("Notify")]
            public bool Notify { get; set; }

            [JsonProperty("Ragione")]
            public string Ragione { get; set; }

            [JsonProperty("Note")]
            public string Note { get; set; }

            [JsonProperty("Path")]
            public string Path { get; set; }

            [JsonProperty("TipoTrasmissione")]
            public string TipoTrasmissione { get; set; }


            public Body(bool isFascicolo, string id, string 
                    idDestinatario, bool notify,
                    string ragione, string note,
                    string tipoTrasmissione)
            {
                if (isFascicolo)
                    IdFasc = id;
                else
                    IdDoc = id;

                IdDestinatario = idDestinatario;
                Notify = notify;
                Ragione = ragione;
                Note = note;
                TipoTrasmissione = tipoTrasmissione;
            }
        }
    }
}
