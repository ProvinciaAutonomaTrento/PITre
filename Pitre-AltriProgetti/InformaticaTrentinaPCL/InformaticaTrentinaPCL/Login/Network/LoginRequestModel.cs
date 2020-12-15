using System;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Login
{
    public class LoginRequestModel : BaseRequestModel
    {
        public Body body;
        public LoginRequestModel(Body body)
        {
            this.body = body;
        }

        public class Body
        {
            public Body(string username, string password, string oldPassword = null, AmministrazioneModel adminModel = null , string otp=null)
            {
                this.otp = otp;
                this.username = username;
                this.password = password;
                this.oldPassword = oldPassword;
                this.idAmministrazione = adminModel != null?adminModel.systemId:null;
            }

            [JsonProperty(PropertyName = "Username")]
            public String username { get; set; }

            [JsonProperty(PropertyName = "Password")]
            public String password { get; set; }

            [JsonProperty(PropertyName = "OldPassword")]
            public String oldPassword { get; set; }

            [JsonProperty(PropertyName = "idAmministrazione")]
            public String idAmministrazione { get; set; }

            [JsonProperty(PropertyName = "Otp")]
            public String otp { get; set; }
        }
    }
}