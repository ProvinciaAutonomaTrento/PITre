using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Search.MVP;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Search
{
    public class SearchModel : WS, ISearchModel
    {
        public SearchModel()
        {
        }

        public async Task<RicercaResponseModel> GetRicercaElementList(RicercaRequestModel request)
        {
            RicercaResponseModel response;
            try
            {
                response = await api.Search(request.body, request.token, getCancellationToken());
                return response;
            }
            catch (Exception e)
            {
                response = new RicercaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
