using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Login
{
    public class LoginResponseModel : BaseResponseModel
    {
        public LoginResponseModel(int code)
        {
            this.Code = code;
        }

        public LoginResponseModel()
        {
            
        }

        [JsonProperty(PropertyName = "UserInfo")]
        public UserInfo userInfo { get; set; }

        //[JsonProperty(PropertyName = "OTPInfo")]
        //public OTPInfo otpInfo { get;  }

        

        [JsonProperty(PropertyName = "TodoListRemoval")]
        public string todoListRemoval { get; set; }

        [JsonProperty(PropertyName = "ShareAllowed")]
        public bool shareAllowed { get; set; }

        [JsonProperty(PropertyName = "OTPAllowed")]
        private bool isOTPAllowed 
        { 
            set
            {
                if(userInfo!=null)
                    userInfo.signConfiguration.isOTPAllowed = value;
            }
        }

        [JsonProperty(PropertyName = "InfoMemento")]
        private InfoMemento infoMemento 
        {
            set
            {
                if (userInfo != null)
                {
                    userInfo.signConfiguration.alias = value?.alias;
                    userInfo.signConfiguration.dominio = value?.dominio;
                }
            }
        }

        public class InfoMemento
        {
            [JsonProperty(PropertyName = "Alias")]
            public string alias { get; set; }

            [JsonProperty(PropertyName = "Dominio")]
            public string dominio { get; set; }
        }
    }
}
