using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.VerifyUpdate
{
    public class VerifyUpdateRequestModel : BaseRequestModel
    {
        public Body body;
        public VerifyUpdateRequestModel(Body body)
        {
            this.body = body;
        }

        public class Body
        {
            public Body(string version, string model="", string brand = "")
            {
                this.version = version;
                this.model = model;
                this.brand = brand;
            }

            [JsonProperty(PropertyName = "Model")]
            public String model { get; set; }

            [JsonProperty(PropertyName = "Brand")]
            public String brand { get; set; }

            [JsonProperty(PropertyName = "Version")]
            public String version { get; set; }

            
        }
    }
}