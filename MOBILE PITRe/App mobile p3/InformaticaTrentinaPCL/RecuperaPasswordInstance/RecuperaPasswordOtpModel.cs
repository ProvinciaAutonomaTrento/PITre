using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.RecuperaPasswordInstance
{
    public class RecuperaPasswordOtpModel : WS, IRecuperaPasswordOtpModel
    {

        public RecuperaPasswordOtpModel()
        {
        }

        public async Task<LoginResponseModel> GetDoRecuperaPasswordOTP(LoginRequestModel request)
        {
            LoginResponseModel loginResponseModel;
            try{
                loginResponseModel = await api.DoRecuperaPasswordOTP(request.body, getCancellationToken());
            }catch (Exception e)
            {
                loginResponseModel = new LoginResponseModel();
                ResolveError(loginResponseModel, e);
            }
            return loginResponseModel;
        }
    }
}
