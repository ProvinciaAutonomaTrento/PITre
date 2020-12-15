using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.MVPD
{
    public interface IHomeModel : IBaseModel
    {
        Task<LogoutResponseModel> DoLogOut(LogoutRequestModel request);
    }
}
