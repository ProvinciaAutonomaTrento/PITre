using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.OpenFile.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class OpenDocumentModel : WS, IOpenDocumentModel
    {
        public OpenDocumentModel()
        {
        }

       
        public async Task<GetDocInfoResponseModel> GetDocInfo(GetDocInfoRequestModel request)
        {
            GetDocInfoResponseModel response;
            try
            {
                response = await api.GetDocInfo(request.body.idDoc, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new GetDocInfoResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<GetFileResponseModel> GetFile(GetFileRequestModel request)
        {

            GetFileResponseModel response;
            try
            {
                //Qui il timeout deve essere più alto perché potresti dover scaricare diversi MB. Attualmente è 0 --> illimitato
                response = await api.GetFile(request.body.idDoc, request.token, getCancellationToken(0));
            }
            catch (Exception e)
            {
                response = new GetFileResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
