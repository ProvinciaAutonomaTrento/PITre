using System;
using System.Net;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.ChangeRole.Network
{
    public class ChangeRoleResponseModel : LoginResponseModel
    {
        public string token { get; set; }

        public ChangeRoleResponseModel() { }

        public ChangeRoleResponseModel(string token)
        {
            this.userInfo.token = token;
        }
    }
}
