using System;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.CommonAction.MVP
{
    public interface IActionPresenter : IBasePresenter
    {
        void UpdateNote(string note);
        void ButtonConfirm(AbstractDocumentListItem abstractDocument);
        void OnViewReady();
    }
}
