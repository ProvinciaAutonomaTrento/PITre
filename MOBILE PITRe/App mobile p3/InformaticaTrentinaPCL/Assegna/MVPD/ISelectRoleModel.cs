using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface ISelectRoleModel : IBaseModel
    {
        Task<ListaRuoliUserResponseModel> ListaRuoliUtente(ListaRuoliUserRequestModel model);
    }
}
