using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Home
{
    public class DummyHomeModel : WS, IHomeModel
    {
        public DummyHomeModel()
        {
        }

        public async Task<LogoutResponseModel> DoLogOut(LogoutRequestModel request)
        {
            return await getResponseMocked();
        }

        private async Task<LogoutResponseModel> getResponseMocked()
        {
            LogoutResponseModel response;
            try
            {
                await Task.Delay(3000);
                return new LogoutResponseModel(0);
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
