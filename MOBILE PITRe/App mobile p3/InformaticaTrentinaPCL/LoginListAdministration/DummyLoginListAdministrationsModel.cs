using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.LoginListAdministration.MVP;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.LoginListAdministration
{
    public class DummyLoginListAdministrationModel : WS, ILoginListAdministrationsModel
    {
        public DummyLoginListAdministrationModel()
        {
        }

        public async Task<ListaAmministrazioniResponseModel> GetListaAmministrazioniByUser(ListaAmministrazioniRequestModel request)
        {
            ListaAmministrazioniResponseModel response;
            try
            {
                response = new ListaAmministrazioniResponseModel(await this.GetListaAmministrazioniByUserOK());
            }
            catch (Exception e)
            {
                response = new ListaAmministrazioniResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        private async Task<List<AmministrazioneModel>> GetListaAmministrazioniByUserOK()
        {
            await Task.Delay(1000);
            List<AmministrazioneModel> response = new List<AmministrazioneModel>(
                new List<AmministrazioneModel> {
                new AmministrazioneModel("systemId1", "descrizione1", "codice1", "libreria1", "email1@email.it"),
                new AmministrazioneModel("systemId2", "descrizione2", "codice2", "libreria2", "email2@email.it")
            });

            return response;
        }

        private async Task<List<AmministrazioneModel>> GetListaAmministrazioniByUserKO()
        {
            await Task.Delay(1000);
            List<AmministrazioneModel> response = new List<AmministrazioneModel>();

            return response;
        }
    }
}
