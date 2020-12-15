using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Signature.MVP
{
    public interface ISignaturePresenter : IBasePresenter
    {
        void RequestOTP();
        void SignDocument(AbstractDocumentListItem abstractDocument);
        void UpdateAlias(string alias);
        void UpdateDomain(string domain);
        void UpdatePIN(string pin);
        void UpdateOTP(string otp);
        void OnViewReady();
    }
}
