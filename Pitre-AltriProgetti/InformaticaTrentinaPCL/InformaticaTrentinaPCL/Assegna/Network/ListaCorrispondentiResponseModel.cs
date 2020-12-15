using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaCorrispondentiResponseModel : BaseResponseModel
    {

        [JsonProperty("Elements")]
        public List<CorrispondenteTrasmissioneModel> elements { get; set; }

        public ListaCorrispondentiResponseModel()
        {
        }

        public ListaCorrispondentiResponseModel(List<CorrispondenteTrasmissioneModel> elements)
        {
            this.elements = elements;
        }
    }
}
