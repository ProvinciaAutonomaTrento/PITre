using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.OpenFile.Network;
using InformaticaTrentinaPCL.Signature.Network;

namespace InformaticaTrentinaPCL.Home.MVP
{
    public interface IDocumentListModel : IBaseModel
    {
        Task<LoadToDoDocumentsResponseModel> LoadToDoDocuments(LoadToDoDocumentsRequestModel request);
        Task<ADLListResponseModel> LoadADLDocuments(ADLListRequestModel request);
        Task<LibroFirmaResponseModel> LoadSignDocuments(LibroFirmaRequestModel request);
        Task<CondividiResponseModel> Condividi(CondividiRequestModel request);
        Task<GetDocumentoCondivisoResponseModel> GetDocumentoCondiviso(GetDocumentoCondivisoRequestModel request);
        Task<CambiaStatoElementoResponseModel> ChangeDocumentsState(CambiaStatoElementoRequestModel request);
        Task<RespingiElementiResponseModel> RejectDocuments(LibroFirmaRequestModel request);
        Task<FirmaElementiResponseModel> SignDocuments(LibroFirmaRequestModel request);
    }
}
