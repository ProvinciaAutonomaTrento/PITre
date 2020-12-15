using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Signature.MVP;
using InformaticaTrentinaPCL.Signature.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Signature
{
    public class SignatureModel : WS, ISignatureModel
    {

        public async Task<RequestOTPResponseModel> RequestOTP(RequestOTPRequestModel request)
        {
            RequestOTPResponseModel response;
            try
            {
                response = await api.RequestOTP(request.body, request.token, getCancellationToken());
                return response;
            }
            catch (Exception e)
            {
                response = new RequestOTPResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<SignDocumentResponseModel> SignDocument(SignDocumentRequestModel request)
        {
            SignDocumentResponseModel response;
            try
            {
                await Task.Delay(3000);
                //response = await api.FirmaHSM(request.body, request.token, getCancellationToken());
                //TODO to remove when service is OK
                response = new SignDocumentResponseModel();
                response.Code = 0;
            }
            catch (Exception e)
            {
                response = new SignDocumentResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<SignDocumentResponseModel> SignDocumentsHSM(RequestOTPRequestModel request)
        {
            SignDocumentResponseModel response;
            try
            {
                response = await api.SignDocumentsHSM(request.body, request.token, getCancellationToken(0));
                return response;
            }
            catch (Exception e)
            {
                response = new SignDocumentResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
