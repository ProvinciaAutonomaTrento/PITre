using System;
using System.Net;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.CommonAction.MVP;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Home.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.CommonAction
{
    public class DummyActionModel : WS, IActionModel
    {
        public DummyActionModel()
        {
        }

        public async Task<AccettaRifiutaResponseModel> DoActionAccetta(AccettaRifiutaRequestModel request)
        {
            AccettaRifiutaResponseModel response;
            try
            {
                Task.Delay(1000);
                response = new AccettaRifiutaResponseModel(0);
                response.StatusCode = HttpStatusCode.OK;
                return response;
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
                Task.Delay(1000);
                response = new ActionADLResponseModel(0);
                response.StatusCode = HttpStatusCode.OK;
                return response;
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
                Task.Delay(1000);
                response = new AccettaRifiutaResponseModel(0);
                response.StatusCode = HttpStatusCode.OK;
                return response;
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
                Task.Delay(1000);
                response = new ViewedResponseModel();
                response.Code = 0;
                response.StatusCode = HttpStatusCode.OK;
                return response;
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
