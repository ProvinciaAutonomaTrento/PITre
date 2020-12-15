using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Delega
{
    public class DummyNewMandateModel : WS,INewMandateModel
    {
        public DummyNewMandateModel()
        {
        }

        public async Task<NewMandateResponseModel> DoNewMandate(NewMandateRequestModel request)
        {
            NewMandateResponseModel response;
            try
            {
                await Task.Delay(1000);
                response = new NewMandateResponseModel();
                response.Code = 0;
                return response;
            }
            catch (Exception e)
            {
                response = new NewMandateResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
