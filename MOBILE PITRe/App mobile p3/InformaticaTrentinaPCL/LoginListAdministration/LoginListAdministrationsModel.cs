using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.LoginListAdministration.MVP;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.LoginListAdministration
{
    public class LoginListAdministrationsModel : WS, ILoginListAdministrationsModel
    {
        public LoginListAdministrationsModel()
        {
        }

        public async Task<ListaAmministrazioniResponseModel> GetListaAmministrazioniByUser(ListaAmministrazioniRequestModel request)
        {
            ListaAmministrazioniResponseModel response;
            try
            {
                response = new ListaAmministrazioniResponseModel(await api.GetListaAmministrazioniByUser(request.userId, getCancellationToken()));
            }
            catch (Exception e)
            {
                response = new ListaAmministrazioniResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
