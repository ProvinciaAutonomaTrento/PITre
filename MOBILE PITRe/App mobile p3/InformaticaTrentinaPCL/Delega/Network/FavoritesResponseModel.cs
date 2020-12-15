using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class FavoritesResponseModel : BaseResponseModel
    {
        [JsonProperty("Preferiti")]
        public List<InfoPreferito> preferiti { get; set; }

        public FavoritesResponseModel()
        {
        }

        public FavoritesResponseModel(List<InfoPreferito> preferiti)
        {
            this.preferiti = preferiti;
        }
    }
}
