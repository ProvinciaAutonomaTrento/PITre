using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class ViewedRequestModel : BaseRequestModel
    {
        public Body body;
        public ViewedRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            [JsonProperty(PropertyName = "IdTrasmissione")]
            public string idTrasmissione { get; set; }

            [JsonProperty(PropertyName = "IdEvento")]
            public string idEvento { get; set; }

            public Body(string idTrasmissione, string idEvento)
            {
                this.idTrasmissione = idTrasmissione;
                if (String.IsNullOrWhiteSpace(idTrasmissione))
                {
                    this.idEvento = idEvento;
                }
            }
        }
    }
}
