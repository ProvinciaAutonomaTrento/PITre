using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.CommonAction.Network
{
    public class AccettaRifiutaRequestModel : BaseRequestModel
    {
        public Body body;

        public AccettaRifiutaRequestModel(Body requestBody, string token)
        {
            this.body = requestBody;
            this.token = token;
        }

        public class Body
        {
            [JsonProperty(PropertyName = "Action")]
            public string action { get; set; }

            [JsonProperty(PropertyName = "Note")]
            public string note { get; set; }

            [JsonProperty(PropertyName = "IdTrasmissione")]
            public string idTrasmissione { get; set; }

            public Body(string action, string note, string idTrasmissione)
            {
                this.action = action;
                this.note = note;
                this.idTrasmissione = idTrasmissione;
            }
        }
    }
}
