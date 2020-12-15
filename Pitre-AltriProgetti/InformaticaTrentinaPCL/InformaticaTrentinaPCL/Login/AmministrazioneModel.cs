using System;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Login
{
    public class AmministrazioneModel
    {
		[JsonProperty(PropertyName = "systemId")]
		public string systemId { get; set; }

        [JsonProperty(PropertyName = "descrizione")]
        public string descrizione { get; set; }

        [JsonProperty(PropertyName = "codice")]
        public string codice { get; set; }

        [JsonProperty(PropertyName = "libreria")]
        public string libreria { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }

        public AmministrazioneModel(string systemId, string descrizione, string codice, string libreria, string email)
        {
            this.systemId = systemId;
            this.descrizione = descrizione;
            this.codice = codice;
            this.libreria = libreria;
            this.email = email;
        }
    }
}
