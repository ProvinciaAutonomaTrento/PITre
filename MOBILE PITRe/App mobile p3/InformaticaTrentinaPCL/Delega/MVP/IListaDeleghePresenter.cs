using System;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface IListaDeleghePresenter : IBasePresenter
    {
        void GetDelegaDocumentsList();
        void OnPullToRefresh();
        void DoRevoke(DelegaDocumentModel delega);
    }
}
