using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assegna.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Assegna
{
    public class DummySelectRoleModel : WS, ISelectRoleModel
    {
        public DummySelectRoleModel()
        {
        }

        public Task<ListaRuoliUserResponseModel> ListaRuoliUtente(ListaRuoliUserRequestModel model)
        {
            //TODO IMPLEMENTS
            return null;
        }
    }
}
