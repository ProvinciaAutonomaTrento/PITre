using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Signature.Network;

namespace InformaticaTrentinaPCL.Signature.MVP
{
    public interface ISignatureModel : IBaseModel
    {
        Task<RequestOTPResponseModel> RequestOTP(RequestOTPRequestModel request);
        Task<SignDocumentResponseModel> SignDocument(SignDocumentRequestModel request); 
        Task<SignDocumentResponseModel> SignDocumentsHSM(RequestOTPRequestModel request);
       
    }
}
