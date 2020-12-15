using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class EseguiTrasmissioneModelloRequestModel : BaseRequestModel
    {
        public Body body;

        public EseguiTrasmissioneModelloRequestModel(Body requestBody, string token)
        {
			this.body = requestBody;
			this.token = token;
        }

        public class Body
        {
            [JsonProperty("IdDoc")]
			public string IdDoc { get; set; }

			[JsonProperty("IdModelloTrasm")]
			public string IdModelloTrasm { get; set; }

			[JsonProperty("IdCorrGlobali")]
			public string IdCorrGlobali { get; set; }

			[JsonProperty("IdFasc")]
			public string IdFasc { get; set; }

			[JsonProperty("Note")]
			public string Note { get; set; }

			[JsonProperty("Path")]
			public string Path { get; set; }

			public Body(bool isFasc, string id, string idModelloTrasm, string note)
			{
                if (isFasc)
                    IdFasc = id;
                else
                    IdDoc = id;
				IdModelloTrasm = idModelloTrasm;
				Note = note;
			}
        }
    }
}
