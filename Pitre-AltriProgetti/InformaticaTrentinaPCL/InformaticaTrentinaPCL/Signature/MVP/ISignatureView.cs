using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Signature.MVP
{
    public interface ISignatureView : IGeneralView
    {
        void SignatureDone(Dictionary<string, string> extra);
        void EnableRequestOTPButton(bool enabled);
        void EnableSignatureButton(bool enabled);
        void OnOTPRequested();
    }
}
