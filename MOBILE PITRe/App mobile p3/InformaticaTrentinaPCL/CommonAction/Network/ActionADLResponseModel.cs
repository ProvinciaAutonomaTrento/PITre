using System;
using System.Net;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.CommonAction.Network
{
    public class ActionADLResponseModel : BaseResponseModel
    {
        public ActionADLResponseModel()
        {

        }

        public ActionADLResponseModel(int code)
        {
            this.Code = code;
        }
    }
}
