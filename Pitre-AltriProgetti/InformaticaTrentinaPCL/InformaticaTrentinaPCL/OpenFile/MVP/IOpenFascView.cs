using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.OpenFile.MVP
{
    public interface IOpenFascView : IOpenDocumentView
    {
        void OpenDocumentBundle(SharedDocumentBundle bundle);
    }
}
