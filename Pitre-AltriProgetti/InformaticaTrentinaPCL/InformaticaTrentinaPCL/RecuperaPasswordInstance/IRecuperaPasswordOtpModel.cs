using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.RecuperaPasswordInstance
{
    public interface IRecuperaPasswordOtpModel : IBaseModel
    {
        Task<LoginResponseModel> GetDoRecuperaPasswordOTP(LoginRequestModel request);
    }
}
