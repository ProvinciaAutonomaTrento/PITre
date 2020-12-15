using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Assegna
{
    public class DummyAssegnaModel : WS,IAssegnaModel
    {
        public DummyAssegnaModel()
        {
        }

        public async Task<ListaRagioniResponseModel> GetListaRagioni(ListaRagioniRequestModel request)
        {
            Task.Delay(1000);
            ListaRagioniResponseModel response = new ListaRagioniResponseModel(new List<Ragione>());
            response.Code = 0;
            return response;        
        }

        public async Task<SmistaResponseModel> Smista(SmistaRequestModel request)
        {
            Task.Delay(1000);
            SmistaResponseModel response = new SmistaResponseModel();
            response.Code = 0;
            return response;
        }
    }
}
