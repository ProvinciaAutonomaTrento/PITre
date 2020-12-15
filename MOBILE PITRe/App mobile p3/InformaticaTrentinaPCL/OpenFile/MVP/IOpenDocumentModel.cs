using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.OpenFile.Network;

namespace InformaticaTrentinaPCL.OpenFile.MVP
{
    public interface IOpenDocumentModel : IBaseModel
    {
        Task<GetDocInfoResponseModel> GetDocInfo(GetDocInfoRequestModel request);
        Task<GetFileResponseModel> GetFile(GetFileRequestModel request);
    }
}
