using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Assegna.Network;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface IAssignableModel : IBaseModel
    {
        Task<ListaModelliTrasmissioneResponseModel> ListaModelliTrasmissone(ListaModelliTrasmissioneRequestModel request);
        Task<FavoritesResponseModel> ListaPreferiti(FavoritesRequestModel request);
        Task<ListaCorrispondentiResponseModel> ListaCorrispondenti(ListaCorrispondentiRequestModel request);
    }
}
