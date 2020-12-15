using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Assegna
{
    public class AssegnaModel : WS, IAssegnaModel
    {
        public AssegnaModel()
        {
        }

        public async Task<ListaRagioniResponseModel> GetListaRagioni(ListaRagioniRequestModel request)
        {
            ListaRagioniResponseModel response;
            try
            {
                response = await api.GetListaRagioni(request.body.idObject, request.body.docFasc, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new ListaRagioniResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<SmistaResponseModel> Smista(SmistaRequestModel request)
        {
            SmistaResponseModel response;
            try
            {
                response = await api.Smista(request.body, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new SmistaResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
