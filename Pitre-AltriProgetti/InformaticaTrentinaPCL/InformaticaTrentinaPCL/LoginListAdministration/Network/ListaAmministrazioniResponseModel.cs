using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Login.Network
{
    public class ListaAmministrazioniResponseModel : BaseResponseModel
    {
		public List<AmministrazioneModel> amministrazioni { get; set; }

		public ListaAmministrazioniResponseModel() { }

        public ListaAmministrazioniResponseModel(List<AmministrazioneModel> list)
        {
            this.amministrazioni = list;
        }
    }
}
