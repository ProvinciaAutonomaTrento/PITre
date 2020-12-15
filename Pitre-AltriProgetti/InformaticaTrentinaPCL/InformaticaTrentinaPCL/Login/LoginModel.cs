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

namespace InformaticaTrentinaPCL.Login
{
    public class LoginModel : WS, ILoginModel
    {

        public LoginModel()
        {
        }

        public async Task<LoginResponseModel> GetDoLogin(LoginRequestModel request)
        {
            LoginResponseModel loginResponseModel;
            try{
                loginResponseModel = await api.DoLogin(request.body, getCancellationToken());
            }catch (Exception e)
            {
                loginResponseModel = new LoginResponseModel();
                ResolveError(loginResponseModel, e);
            }
            return loginResponseModel;
        }
    }
}
