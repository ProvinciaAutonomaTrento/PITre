using System.Threading.Tasks;

using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Search.MVP
{
    public interface ISearchModel : IBaseModel
    {
        Task<RicercaResponseModel> GetRicercaElementList(RicercaRequestModel request);
    }
}
