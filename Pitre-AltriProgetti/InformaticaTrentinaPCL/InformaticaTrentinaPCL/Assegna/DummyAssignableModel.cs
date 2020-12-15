using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Assegna
{
    public class DummyAssignableModel : WS, IAssignableModel
    {
        public DummyAssignableModel()
        {
        }

        public async Task<ListaCorrispondentiResponseModel> ListaCorrispondenti(ListaCorrispondentiRequestModel request)
        {
            ListaCorrispondentiResponseModel response;
            try
            {
                await Task.Delay(1000);
                response = new ListaCorrispondentiResponseModel(new List<CorrispondenteTrasmissioneModel>{
                    new CorrispondenteTrasmissioneModel("descrizione_utente_1","descrizione_ruolo_1","codice_corrispondente_1","descrizioneUO_1","id_UO_1",1, "id_ruolo_1","id_utente_1","id_corrispondente_1", false),
                    new CorrispondenteTrasmissioneModel("descrizione_utente_2","descrizione_ruolo_2","codice_corrispondente_2","descrizioneUO_2","id_UO_2",2, "id_ruolo_2","id_utente_2","id_corrispondente_2", false),
                    new CorrispondenteTrasmissioneModel("descrizione_utente_3","descrizione_ruolo_3","codice_corrispondente_3","descrizioneUO_3","id_UO_3",3, "id_ruolo_3","id_utente_3","id_corrispondente_3", true)
                });
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
                response = new ListaModelliTrasmissioneResponseModel(new List<ModelliTrasmissioneModel>{
                    new ModelliTrasmissioneModel("codice_1","1"),
                    new ModelliTrasmissioneModel("codice_2","3"),
                    new ModelliTrasmissioneModel("codice_3","3"),
                    new ModelliTrasmissioneModel("codice_4","4")
                });
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
                //response = await api.ListaPreferitiTrasmissioneRequestModel(request.token, getCancellationToken());
                response = new FavoritesResponseModel(new List<InfoPreferito>());
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
