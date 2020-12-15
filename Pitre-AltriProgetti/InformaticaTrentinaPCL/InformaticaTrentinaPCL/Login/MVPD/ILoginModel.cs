using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Login.MVP
{
    public interface ILoginModel : IBaseModel
    {
        Task<LoginResponseModel> GetDoLogin(LoginRequestModel request);
    }
}
