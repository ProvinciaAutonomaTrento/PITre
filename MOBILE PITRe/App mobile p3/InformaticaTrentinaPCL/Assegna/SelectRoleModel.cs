using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Assegna
{
    public class SelectRoleModel: WS, ISelectRoleModel
    {
        public SelectRoleModel()
        {
        }

        public async Task<ListaRuoliUserResponseModel> ListaRuoliUtente(ListaRuoliUserRequestModel request)
        {
            ListaRuoliUserResponseModel response;
            try
            {
                response = await api.GetListaRuoliUser(request.body.idUtente, request.token, getCancellationToken());
            }
            catch (Exception e)
            {
                response = new ListaRuoliUserResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
