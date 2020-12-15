using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface ISelectRolePresenter : IBasePresenter
    {
        void setFavorite(bool favorite);
        void OnViewReady();
        void OnBackPressed();
    }
}
