using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public abstract class DocumentResponseModel : BaseResponseModel
    {
        public abstract List<AbstractDocumentListItem> GetResults();
        public abstract int GetTotalRecordCount();

        public DocumentResponseModel()
        {

        }
    }
}
