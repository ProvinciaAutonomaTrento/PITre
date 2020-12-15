using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.ChooseInstance
{
    public class ChooseInstanceModel :WS, IChooseModel
    {
        public async Task<ListInstanceResponseModel> GetListInstance()
        {
            ListInstanceResponseModel response;
            try
            {
                response = new ListInstanceResponseModel(await apiPrelogin.ListIstanze(getCancellationToken()));
            }
            catch (Exception e)
            {
                response = new ListInstanceResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}

