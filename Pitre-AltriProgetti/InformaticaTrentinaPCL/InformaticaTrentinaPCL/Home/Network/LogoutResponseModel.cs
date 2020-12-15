using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class LogoutResponseModel : BaseResponseModel
    {
        public LogoutResponseModel()
        {

        }

        public LogoutResponseModel(int code)
        {
            this.Code = code;
        }
    }
}
