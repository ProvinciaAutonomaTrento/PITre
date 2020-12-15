using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.RecuperaPasswordInstance
{
    public interface IRecuperaPasswordModel : IBaseModel
    {
        Task<LoginResponseModel> GetDoRecuperaPassword(LoginRequestModel request);
    }
}
