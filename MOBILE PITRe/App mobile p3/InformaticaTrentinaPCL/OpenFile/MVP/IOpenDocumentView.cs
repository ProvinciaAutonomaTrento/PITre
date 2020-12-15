using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.MVP
{
    public interface IOpenDocumentView: IGeneralView
    {
        void ShowList(List<AbstractDocumentListItem> list);
        void OnUpdateLoaderWithAction(bool isShow);
    }
}
