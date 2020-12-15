using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface IAssegnaModel : IBaseModel
    {
        Task<SmistaResponseModel> Smista(SmistaRequestModel request);
        Task<ListaRagioniResponseModel> GetListaRagioni(ListaRagioniRequestModel request);
    }
}
