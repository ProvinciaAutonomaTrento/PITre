using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface ISelectMandateAssigneePresenter : IBasePresenter
    {
        void GetListFavorites();
        void SearchAssignee(string text);
        void ClearSearch();
        void SetFavorite(AbstractRecipient user, bool favorite);
    }
}
