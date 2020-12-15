using System;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna
{
    public class Ragione
    {
        [JsonProperty("systemId")]
        public string systemId { get; set; }

        [JsonProperty("descrizione")]
        public string descrizione { get; set; }

        public Ragione()
        {
        }

        public override string ToString()
        {
            return descrizione;
        }
    }
}
