using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.CommonAction.Network;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.CommonAction.MVP
{
    public interface IActionModel : IBaseModel
    {
        Task<AccettaRifiutaResponseModel> DoActionAccetta(AccettaRifiutaRequestModel request);
        Task<ActionADLResponseModel> DoActionAddRemoveADL(ActionADLRequestModel request);
        Task<AccettaRifiutaResponseModel> DoActionRifiuta(AccettaRifiutaRequestModel request);
        Task<ViewedResponseModel> DoViewed(ViewedRequestModel request);
    }
}
