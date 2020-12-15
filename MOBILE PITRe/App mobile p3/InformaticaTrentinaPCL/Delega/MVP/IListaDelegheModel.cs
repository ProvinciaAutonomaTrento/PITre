using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface IListaDelegheModel : IBaseModel
    {
		Task<DelegaListResponseModel> GetListaModelliDelega(DelegaListRequestModel request);
        Task<DoRevokeResponseModel> DoRevoke(DoRevokeRequestModel request);
	}
}
