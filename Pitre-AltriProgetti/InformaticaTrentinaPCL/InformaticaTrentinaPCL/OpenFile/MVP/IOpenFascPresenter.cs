using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.MVP
{
    public interface IOpenFascPresenter : IOpenDocumentPresenter, IBasePresenter
    {
        void OnSearch(string text);
        void ClearSearch();
        void LoadFileList();
        void DoNextPage();
    }
}
