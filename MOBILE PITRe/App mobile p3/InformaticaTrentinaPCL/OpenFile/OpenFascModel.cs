using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.OpenFile.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class OpenFascModel : WS, IOpenFascModel
    {
        public OpenFascModel()
        {
        }

        public async Task<GetFascRicercaResponseModel> GetFascRicerca(GetFascRicercaRequestModel request)
        {
            GetFascRicercaResponseModel response;
            try
            {
                //GetFascRicerca([AliasAs("IdFascicolo")] string idFascicolo, [AliasAs("PageSize")] int pageSize, [AliasAs("RequestedPage")] int requestedPage, [AliasAs("TestoDaCercare")] string testoDaCercare, [Header("authtoken")] string authtoken, CancellationToken ct);
                response = await api.GetFascRicerca(request.body.idFascicolo, request.body.pageSize, request.body.requestedPage, request.body.searchString, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new GetFascRicercaResponseModel();
                ResolveError(response, e);
            }
            return response;   
        }
    }
}
