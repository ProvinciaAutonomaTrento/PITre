using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.CommonAction.Network
{
    public class ActionADLRequestModel : BaseRequestModel
    {
        public Body body;

        public ActionADLRequestModel(Body requestBody, string token)
        {
            this.body = requestBody;
            this.token = token;
        }
        public class Body
        {
            [JsonProperty(PropertyName = "ADLAction")]
            public string ADLAction { get; set; }

            [JsonProperty(PropertyName = "IdElemento")]
            public string idElemento { get; set; }

            [JsonProperty(PropertyName = "TipoElemento")]
            public string tipoElemento { get; set; }

            public Body(string ADLAction, string idElemento, string tipoElemento)
            {
                this.ADLAction = ADLAction;
                this.idElemento = idElemento;
                this.tipoElemento = tipoElemento;
            }
        }
    }
}
