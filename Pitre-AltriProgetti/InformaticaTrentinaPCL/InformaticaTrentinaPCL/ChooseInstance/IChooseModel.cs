using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.Network;

namespace InformaticaTrentinaPCL.ChooseInstance
{
    public interface IChooseModel : IBaseModel
    {
        Task<ListInstanceResponseModel> GetListInstance();
    }
}
