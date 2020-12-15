using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Signature.Network
{
    public class InfoFirma
    {
        [JsonProperty("IdDocumento")]
        public string IdDocumento{ get; set; }
        
        [JsonProperty("Status")]
        public string Status{ get; set; }
        
        [JsonProperty("ErrorMessage")]
        public string ErrorMessage{ get; set; }

        public InfoFirma()
        {
        }
    }
}
