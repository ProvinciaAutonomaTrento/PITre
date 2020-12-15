using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.VerifyUpdate
{
    public class VerifyUpdateModel : WS //, ILoginModel
    {

        public VerifyUpdateModel()
        {
        }

        public async Task<VerifyUpdateResponseModel> GetDoVerifyUpdate(VerifyUpdateRequestModel request)
        {
            VerifyUpdateResponseModel verifyUpdateResponseModel;
            try{
                verifyUpdateResponseModel = await api.DoVerifyUpdate(request.body, getCancellationToken());
            }catch (Exception e)
            {
                verifyUpdateResponseModel = new VerifyUpdateResponseModel();
                ResolveError(verifyUpdateResponseModel, e);
            }
            return verifyUpdateResponseModel;
        }
    }
}
