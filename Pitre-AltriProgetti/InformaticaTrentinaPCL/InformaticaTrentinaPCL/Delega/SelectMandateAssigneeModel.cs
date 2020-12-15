using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Delega
{
    public class SelectMandateAssigneeModel : WS, ISelectMandateAssigneeModel
    {
        public SelectMandateAssigneeModel()
        {
        }

        public async Task<FavoritesResponseModel> GetListFavorites(FavoritesRequestModel request)
        {
            FavoritesResponseModel response;
            try
            {
                response = await api.GetFavorites(request.body.soloPers, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new FavoritesResponseModel();
                ResolveError(response, e);
            }
            return response;

        }

        public async Task<SearchMandateAssigneeResponseModel> SearchAssignee(SearchMandateAssigneeRequestModel request)
        {
            SearchMandateAssigneeResponseModel response;
            try
            {
                response = await api.RicercaUtenti(request.body.descrizione, request.body.numMaxResult, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new SearchMandateAssigneeResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<SetFavoriteResponseModel> SetFavorite(SetFavoriteRequestModel request)
        {
            SetFavoriteResponseModel response = new SetFavoriteResponseModel();
            try
            {
                response.success = request.isFavorite ?
                                    await api.PutFavorite(request.body, request.token, getCancellationToken())
                                                            :
                                    await api.DeleteFavorite(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                ResolveError(response, e);
            }
            return response;
        }
    }
}
