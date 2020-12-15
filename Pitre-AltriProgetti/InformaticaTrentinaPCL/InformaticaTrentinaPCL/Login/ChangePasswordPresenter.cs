using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.ChangePassword
{
    public class ChangePasswordPresenter : LoginPresenter, IChangePasswordPresenter
    {


        public ChangePasswordPresenter(ILoginView view,INativeFactory nativeFactory, string username) : base(view, nativeFactory)
        {
            this.loginObject = new LoginObject();
            this.loginObject.username = username;
#if CUSTOM
            this.model = new DummyLoginModel(TypeLogin.DEFAULT_LOGIN,loginResponse_multiAmm);
#else
			this.model = new LoginModel();
#endif
        }

       public void UpdateOldPassword(string oldPassword){
            loginObject.oldPassword = oldPassword;
            this.CheckCredentials();
        }
       
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

        public override void CheckCredentials()
        {
            bool isPwdEquals = (loginObject.password.Trim() == loginObject.repeatedPassword.Trim() && !string.IsNullOrEmpty(loginObject.password.Trim()));
            bool isAdministrationValueValid = loginObject.labelAdministrationState == LoginAdministrationState.DEFAULT
                                              || (loginObject.administration != null);

            view.EnableButton(isPwdEquals && isAdministrationValueValid && !string.IsNullOrEmpty(loginObject.password.Trim()));
        }
    }
}
