using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaModelliTrasmissioneResponseModel : BaseResponseModel
    {
		[JsonProperty(PropertyName = "Modelli")]
        public List<ModelliTrasmissioneModel> modelli { get; set; }

        public ListaModelliTrasmissioneResponseModel()
        {
        }

        public ListaModelliTrasmissioneResponseModel(List<ModelliTrasmissioneModel> modelli)
        {
            this.modelli = modelli;
        }
    }
}
