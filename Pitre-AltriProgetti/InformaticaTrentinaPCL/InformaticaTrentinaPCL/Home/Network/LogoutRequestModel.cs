using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class LogoutRequestModel : BaseRequestModel
    {
        public string dst;

        public LogoutRequestModel(string dst, string token)
        {
            this.token = token;
            this.dst = dst;
        }
    }
}
