using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface INewMandateModel : IBaseModel
    {
        Task<NewMandateResponseModel> DoNewMandate(NewMandateRequestModel request);
    }
}
