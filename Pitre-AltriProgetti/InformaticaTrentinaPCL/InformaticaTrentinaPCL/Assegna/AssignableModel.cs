using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Assegna
{
    public class AssignableModel : WS, IAssignableModel
    {
        public AssignableModel()
        {
        }

        public async Task<ListaCorrispondentiResponseModel> ListaCorrispondenti(ListaCorrispondentiRequestModel request)
        {
            ListaCorrispondentiResponseModel response;
            try
            {
                response = await api.ListaCorrispondenti(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new ListaCorrispondentiResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<ListaModelliTrasmissioneResponseModel> ListaModelliTrasmissone(ListaModelliTrasmissioneRequestModel request)
        {
            ListaModelliTrasmissioneResponseModel response;
            try
            {
                response = await api.GetListaModelliTrasmissione(request.fascicoli, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new ListaModelliTrasmissioneResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<FavoritesResponseModel> ListaPreferiti(FavoritesRequestModel request)
        {
            FavoritesResponseModel response;
            try
            {
                response = await api.GetFavorites(false, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new FavoritesResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
