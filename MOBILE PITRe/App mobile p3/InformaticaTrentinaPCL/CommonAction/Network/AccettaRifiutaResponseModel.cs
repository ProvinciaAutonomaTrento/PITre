using System;
using System.Net;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.CommonAction.Network
{
    public class AccettaRifiutaResponseModel : BaseResponseModel
    {
        public AccettaRifiutaResponseModel()
        {

        }

        public AccettaRifiutaResponseModel(int code)
        {
            this.Code = code;
        }
    }
}
