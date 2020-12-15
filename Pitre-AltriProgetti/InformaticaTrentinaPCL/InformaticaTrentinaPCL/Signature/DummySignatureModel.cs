using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Signature.MVP;
using InformaticaTrentinaPCL.Signature.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Signature
{
    public class DummySignatureModel: WS, ISignatureModel
    {
        public DummySignatureModel()
        {
        }

        public Task<CambiaStatoElementoResponseModel> ChangeDocumentsState(CambiaStatoElementoRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<RespingiElementiResponseModel> RejectDocuments(LibroFirmaRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<RequestOTPResponseModel> RequestOTP(RequestOTPRequestModel request)
        {
            //TODO Implement when work ok
            throw new NotImplementedException();
        }


        public Task<SignDocumentResponseModel> SignDocument(SignDocumentRequestModel request)
        {
            //TODO Implement when work ok
            throw new NotImplementedException();
        }

        public Task<FirmaElementiResponseModel> SignDocuments(LibroFirmaRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<SignDocumentResponseModel> SignDocumentsHSM(SignDocumentRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<SignDocumentResponseModel> SignDocumentsHSM(RequestOTPRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}
