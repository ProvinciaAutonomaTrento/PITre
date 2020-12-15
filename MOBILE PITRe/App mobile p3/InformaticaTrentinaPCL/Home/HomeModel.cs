using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class HomeModel : WS, IHomeModel
    {
        public HomeModel()
        {
        }

        public async Task<LogoutResponseModel> DoLogOut(LogoutRequestModel request)
        {
            LogoutResponseModel response;
            try
            {
                response = await api.DoLogOut(request.dst, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new LogoutResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
