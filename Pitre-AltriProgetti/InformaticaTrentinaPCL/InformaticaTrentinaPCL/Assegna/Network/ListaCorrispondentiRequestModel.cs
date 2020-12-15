using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaCorrispondentiRequestModel : BaseRequestModel
    {
		public Body body;

        public ListaCorrispondentiRequestModel(Body body, string token){

            this.body = body;
            this.token = token;
        }


		public class Body{
			[JsonProperty("Descrizione")]
			public string descrizione { get; set; }

			[JsonProperty("Ragione")]
			public string ragione { get; set; }

			public Body(string descrizione, string ragione)
			{
				this.descrizione = descrizione;
				this.ragione = ragione;
			}
        }
    }
}
