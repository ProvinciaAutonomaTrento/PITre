using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;
using static InformaticaTrentinaPCL.Login.LoginResponseModel;

namespace InformaticaTrentinaPCL.Signature.Network
{
    public class RequestOTPResponseModel : BaseResponseModel
    {
        [JsonProperty("memento")]
        public InfoMemento Memento { get; set; }

		public RequestOTPResponseModel() { }

    }
}
