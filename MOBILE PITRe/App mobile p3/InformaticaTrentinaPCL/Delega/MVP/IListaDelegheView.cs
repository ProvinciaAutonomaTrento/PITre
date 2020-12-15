using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface IListaDelegheView: IGeneralView
    {
        void UpdateList(List<DelegaDocumentModel> list);
        void OnRevokeOk();
    }
}
