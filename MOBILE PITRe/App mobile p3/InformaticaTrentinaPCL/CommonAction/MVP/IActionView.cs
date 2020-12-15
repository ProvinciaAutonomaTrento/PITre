using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.CommonAction.MVP
{
    public interface IActionView : IGeneralView
    {
        void CompletedActionOK(Dictionary<string, string> extra);
        void EnableConfirmButton(bool enabled);
    }
}
