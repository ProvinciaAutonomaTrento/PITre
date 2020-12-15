using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Login.Network;
using Newtonsoft.Json;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Login
{
    public enum TypeLogin
    {
        DEFAULT_LOGIN,
        EXPIRED_LOGIN
    }

    public class DummyLoginModel : WS, ILoginModel
    {
        string loginReponseCustom;
        TypeLogin typeLogin;

        public DummyLoginModel(TypeLogin typeLogin, String loginResponseCustom)
        {
            this.typeLogin = typeLogin;
            this.loginReponseCustom = loginResponseCustom;
        }

        public async Task<LoginResponseModel> GetDoLogin(LoginRequestModel request)
        {
            LoginResponseModel response = await getResponseMockedOK();
            return response;
        }

        private async Task<LoginResponseModel> getResponseMockedKO()
        {
            await Task.Delay(1000);
            LoginResponseModel response = new LoginResponseModel(0);
            response.StatusCode = HttpStatusCode.NotFound;

            return response;
        }

        private async Task<LoginResponseModel> getResponseMockedOK()
        {
            LoginResponseModel response;
            try
            {
                await Task.Delay(1000 * 2, getCancellationToken());
                //loginResponseModel = new LoginResponseModel();

                //loginResponseModel.StatusCode = HttpStatusCode.OK;
                //UserInfo mockUser = new UserInfo();
                //mockUser.username = "username";
                //mockUser.descrizione = "Paolo Rossi";
                //mockUser.idPeople = "A00001";
                //RuoloInfo ruolo = new RuoloInfo("R0001", "Code Role", "Segreteria dip. Organizzazione Personale e Affari Generali");
                //mockUser.ruoli = new List<RuoloInfo>();
                //mockUser.ruoli.Add(ruolo);
                //mockUser.URLImageUser = "https://goo.gl/hRxESQ";

                //loginResponseModel.userInfo = mockUser;

                response = JsonConvert.DeserializeObject<LoginResponseModel>(loginReponseCustom);

            }
            catch (Exception e)
            {
                response = new LoginResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}

