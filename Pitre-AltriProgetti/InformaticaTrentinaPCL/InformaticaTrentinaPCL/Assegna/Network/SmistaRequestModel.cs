using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class SmistaRequestModel : BaseRequestModel
    {
        public Body body;

        public SmistaRequestModel(Body requestBody, string token)
        {
            this.body = requestBody;
            this.token = token;
        }

        public class Body
        {
            [JsonProperty("IdTrasmissione")]
            public string IdTrasmissione { get; set; }

            [JsonProperty("IdTrasmissioneUtente")]
            public string IdTrasmissioneUtente { get; set; }

            [JsonProperty("HasWorkflow")]
            public bool HasWorkFlow { get; set; }

            [JsonProperty("IdEvento")]
            public string IdEvento { get; set; }

            [JsonProperty("IdDoc")]
            public string IdDoc { get; set; }

            [JsonProperty("IdFasc")]
            public string IdFasc { get; set; }

            [JsonProperty("IdDestinatario")]
            public string IdDestinatario { get; set; }

            [JsonProperty("CodiceDestinatario")]
            public string CodiceDestinatario { get; set; }

            [JsonProperty("Notify")]
            public bool Notify { get; set; }

            [JsonProperty("TipoTrasmissione")]
            public string TipoTrasmissione { get; set; }

            [JsonProperty("Ragione")]
            public string Ragione { get; set; }

            [JsonProperty("IdModelloTrasm")]
            public string IdModelloTrasm { get; set; }

            [JsonProperty("NoteTrasm")]
            public string NoteTrasm { get; set; }

            [JsonProperty("Path")]
            public string Path { get; set; }

            public Body(string idTrasmissione, 
                        bool isFascicolo,
                        bool hasWorkFlow,
                        string idEvento,
                        string id, 
                        string idDestinatario, 
                        bool notify,
                        string ragione, 
                        string note,
                        string tipoTrasmissione,
                        string idModelloTrasm)
            {
                if (isFascicolo)
                    IdFasc = id;
                else
                    IdDoc = id;

                HasWorkFlow = hasWorkFlow;
                IdEvento = idEvento;
                IdDestinatario = idDestinatario;
                Notify = notify;
                Ragione = ragione;
                NoteTrasm = note;
                TipoTrasmissione = tipoTrasmissione;
                IdModelloTrasm = idModelloTrasm;
                IdTrasmissione = idTrasmissione;
            }
        }
    }
}
