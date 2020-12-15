using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile.Network
{
    public class GetFileResponseModel : BaseResponseModel
    {
        [JsonProperty("File")]
        public FileInfo file { get; set; }

        public GetFileResponseModel()
        {
        }

        public GetFileResponseModel(FileInfo fileInfo)
        {
            this.file = fileInfo;
        }
    }
}
