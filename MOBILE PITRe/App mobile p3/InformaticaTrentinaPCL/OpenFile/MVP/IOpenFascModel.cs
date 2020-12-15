using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.OpenFile.Network;

namespace InformaticaTrentinaPCL.OpenFile.MVP
{
    public interface IOpenFascModel : IBaseModel
    {
        Task<GetFascRicercaResponseModel> GetFascRicerca(GetFascRicercaRequestModel request);
    }
}
