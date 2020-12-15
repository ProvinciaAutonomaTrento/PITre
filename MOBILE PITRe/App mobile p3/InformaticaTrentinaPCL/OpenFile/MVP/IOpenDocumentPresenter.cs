using System;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.MVP
{
    public interface IOpenDocumentPresenter : IBasePresenter
    {
        void OnSelect(AbstractDocumentListItem file);
        void OnViewReady();
        void CancelDownload();
        string GetDocumentTitle(AbstractDocumentListItem file);
    }
}
