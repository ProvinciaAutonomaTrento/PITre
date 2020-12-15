using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Delega
{
    public class NewMandateModel : WS, INewMandateModel
    {
        public NewMandateModel()
        {
        }

        public async Task<NewMandateResponseModel> DoNewMandate(NewMandateRequestModel request)
        {
            NewMandateResponseModel response;
			try
			{
                response = await api.Delega(request.body, request.token, getCancellationToken());
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
