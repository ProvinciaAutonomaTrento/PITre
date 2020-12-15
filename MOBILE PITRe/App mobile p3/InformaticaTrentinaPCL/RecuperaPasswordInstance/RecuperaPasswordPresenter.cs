using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.RecuperaPassword;
using InformaticaTrentinaPCL.RecuperaPasswordInstance;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.ChangePassword
{

    public class RecuperaPasswordPresenter : ChangePasswordPresenter , IRecuperaPasswordPresenter//LoginPresenter, IChangePasswordPresenter
    {


        public RecuperaPasswordPresenter(ILoginView view,INativeFactory nativeFactory, string username) : base(view, nativeFactory , username)
        {
            this.loginObject = new RecuperaPasswordObject();
            this.loginObject.username = username;
#if CUSTOM
            this.model = new DummyLoginModel(TypeLogin.DEFAULT_LOGIN,loginResponse_multiAmm);
#else
			this.model = new LoginModel();
#endif


        }

        public void UpdateOTP(string otp)
        {
            ((RecuperaPasswordObject)loginObject).otp = otp;
            this.CheckCredentials();
        }

        /*     public void UpdateOldPassword(string oldPassword){
                 loginObject.oldPassword = oldPassword;
                 this.CheckCredentials();
             }
*/
             public void UpdateNewPassword(string newPassword)
             {
                 this.loginObject.password = newPassword;
                 this.CheckCredentials();
             }

             public void UpdateRepeatedNewPassword(string repeatedNewPassword)
             {
                 this.loginObject.repeatedPassword = repeatedNewPassword;
                 this.CheckCredentials();
             }

        public async void LoginAsync(bool saveLogin)
        {
            view.OnUpdateLoader(true);
            LoginRequestModel request = new LoginRequestModel(new LoginRequestModel.Body(loginObject.username, loginObject.password, loginObject.oldPassword, loginObject.administration , ((RecuperaPasswordObject)loginObject).otp));
            LoginResponseModel response = await model.GetDoLogin(request);

            view.OnUpdateLoader(false);
            this.ManageLoginResponse(response);
        }

        public override void CheckCredentials()
             {
                 bool isPwdEquals = (loginObject.password.Trim() == loginObject.repeatedPassword.Trim() && !string.IsNullOrEmpty(loginObject.password.Trim())
                    && !string.IsNullOrEmpty(((RecuperaPasswordObject)loginObject).otp.Trim())
                    );
           
            bool isAdministrationValueValid = true; 

            view.EnableButton(isPwdEquals && isAdministrationValueValid && !string.IsNullOrEmpty(loginObject.password.Trim()));
             }

        
    }
}
