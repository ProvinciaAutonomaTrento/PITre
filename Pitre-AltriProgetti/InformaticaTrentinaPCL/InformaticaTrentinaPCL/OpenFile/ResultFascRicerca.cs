using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class ResultFascRicerca 
    {
        [JsonProperty("Documenti")]
        public List<DocInfo> documenti { get; set; }

        [JsonProperty("InfoElement")]
        public RicercaDocumentModel infoElement { get; set; }

        public ResultFascRicerca()
        {
        }

        public ResultFascRicerca(List<DocInfo> documenti, RicercaDocumentModel infoElement)
        {
            this.documenti = documenti;
            this.infoElement = infoElement;
        }
    }
}
