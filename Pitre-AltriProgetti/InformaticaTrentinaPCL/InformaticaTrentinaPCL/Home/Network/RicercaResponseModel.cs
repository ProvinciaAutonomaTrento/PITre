using System;
using System.Collections.Generic;
using System.Linq;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class RicercaResponseModel : DocumentResponseModel
    {
        [JsonProperty(PropertyName = "IdRicercaSalvata")]
        public string idRicercaSalvata { get; set; }

        [JsonProperty(PropertyName = "Risultati")]
        public List<RicercaDocumentModel> elements { get; set; }

        [JsonProperty(PropertyName = "TotalRecordCount")]
        public int totalRecordCount { get; set; }

        public RicercaResponseModel(string idRicercaSalvata, List<RicercaDocumentModel> risultati, int totalRecordCount)
        {
            this.idRicercaSalvata = idRicercaSalvata;
            this.elements = risultati;
            this.totalRecordCount = totalRecordCount;
        }

        public RicercaResponseModel() { }

        #region DocumentResponseModel
        public override List<AbstractDocumentListItem> GetResults()
        {
			if (null != elements)
				return elements.ToList<AbstractDocumentListItem>();
			else
				return new List<AbstractDocumentListItem>();
        }

        public override int GetTotalRecordCount()
        {
            return totalRecordCount;
        }
        #endregion
    }
}
