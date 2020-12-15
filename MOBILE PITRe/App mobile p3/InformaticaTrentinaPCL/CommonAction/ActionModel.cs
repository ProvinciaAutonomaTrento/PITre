using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Home.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.CommonAction
{
    public class ActionModel : WS, IActionModel
    {
        public ActionModel()
        {
        }

        public async Task<AccettaRifiutaResponseModel> DoActionAccetta(AccettaRifiutaRequestModel request)
        {
            AccettaRifiutaResponseModel response;
            try
            {
                response = await api.DoActionAccettaRifiuta(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new AccettaRifiutaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<ActionADLResponseModel> DoActionAddRemoveADL(ActionADLRequestModel request)
        {
            ActionADLResponseModel response;
            try
            {
                response = await api.DoActionADL(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new ActionADLResponseModel();
                ResolveError(response, e);

            }
            return response;
        }

        public async Task<AccettaRifiutaResponseModel> DoActionRifiuta(AccettaRifiutaRequestModel request)
        {
            AccettaRifiutaResponseModel response;
            try
            {
                response = await api.DoActionAccettaRifiuta(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new AccettaRifiutaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<ViewedResponseModel> DoViewed(ViewedRequestModel request)
        {
            ViewedResponseModel response;
            try
            {
                response = await api.DoViewed(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new ViewedResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
