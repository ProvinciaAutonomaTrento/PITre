using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;
using static InformaticaTrentinaPCL.Login.LoginResponseModel;

namespace InformaticaTrentinaPCL.Signature.Network
{
    public class SignDocumentResponseModel : BaseResponseModel
    {
        [JsonProperty("infoFirma")]
        public List<InfoFirma> InfoFirma { get; set; }

        public SignDocumentResponseModel() { }


    }
}
