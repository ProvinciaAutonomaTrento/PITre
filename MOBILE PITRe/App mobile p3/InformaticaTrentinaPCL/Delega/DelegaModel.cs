using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Delega
{
    public class DelegaModel : WS, IListaDelegheModel
    {
        public DelegaModel()
        {
        }

        public async Task<DelegaListResponseModel> GetListaModelliDelega(DelegaListRequestModel request)
        {
            DelegaListResponseModel response;
            try
            {
                response = await api.GetListaDelega(request.statoDelega, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new DelegaListResponseModel();
                ResolveError(response, e);

            }
            return response;
        }

        public async Task<DoRevokeResponseModel> DoRevoke(DoRevokeRequestModel request)
        {
            DoRevokeResponseModel response = new DoRevokeResponseModel();
            try
            {
                bool success = await api.DoRevoke(request.body, request.token, getCancellationToken());
                response.revoked = success;
                return response;
            }
            catch (Exception e)
            {
                response = new DoRevokeResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
