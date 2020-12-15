using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface ISelectMandateAssigneeModel : IBaseModel
    {
        Task<FavoritesResponseModel> GetListFavorites(FavoritesRequestModel request);
        Task<SearchMandateAssigneeResponseModel> SearchAssignee(SearchMandateAssigneeRequestModel request);
        Task<SetFavoriteResponseModel> SetFavorite(SetFavoriteRequestModel request);
    }
}
