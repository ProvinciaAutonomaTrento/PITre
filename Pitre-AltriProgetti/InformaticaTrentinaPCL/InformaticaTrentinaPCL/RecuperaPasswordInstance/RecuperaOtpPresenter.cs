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

    public class RecuperaOtpPresenter : IRecuperaOtpPresenter // : ChangePasswordPresenter , IRecuperaPasswordPresenter//LoginPresenter, IChangePasswordPresenter
    {

        protected LoginObject loginObject;
        protected IRecuperaPasswordOtpModel model;
        protected ILoginView view;
        protected SessionData sessionData;
        protected IAnalyticsManager analyticsManager;
        protected IReachability reachability;

        public RecuperaOtpPresenter(ILoginView view,INativeFactory nativeFactory)// : base(view, nativeFactory , username)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.reachability = nativeFactory.GetReachability();


            this.loginObject = new LoginObject();
            //   this.loginObject.username = username;
            //   this.model = new LoginModel();
            this.model = new RecuperaPasswordOtpModel();

        }

        public async void GetOTPAsync()
        {
            view.OnUpdateLoader(true);
            LoginRequestModel request = new LoginRequestModel(new LoginRequestModel.Body(loginObject.username,"rest", "reset", loginObject.administration));
            LoginResponseModel response = await model.GetDoRecuperaPasswordOTP(request);

            view.OnUpdateLoader(false);
            this.ManageLoginResponse(response);
        }


        public void UpdateUsername(string username)
        {
            loginObject.username = username;
            this.CheckCredentials();
        }

      

        public  void CheckCredentials()
             {
                 bool isvalid = ( !string.IsNullOrEmpty(loginObject.username.Trim())  );
           

            view.EnableButton(isvalid);
             }

        public void ManageLoginResponse(LoginResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                  switch (response.Code)  
               
                {

                    case 5:
                        this.OnLoginOk(response.userInfo, response.todoListRemoval, response.shareAllowed);
                        break;
                    case 6:
                        view.ShowError(LocalizedString.OTP_USER_NOT_FOUND.Get());
                        break;
                    case 7:
                        view.ShowError(LocalizedString.OTP_EMAIL_ERROR.Get());
                        break;
                    case 8:
                        view.ShowError(LocalizedString.OTP_SEND_EMAIL_ERROR.Get());
                        break;

                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        private void OnLoginOk(UserInfo userLogged, string todoListRemoval, bool shareAllowed)
        {
            UserInfo user = userLogged;
            sessionData.SetUserInfo(user);
            sessionData.isTodoListRemovalManual = todoListRemoval == "Manual";
            sessionData.shareAllowed = shareAllowed;
            LoginEventInfo eventInfo = new LoginEventInfo();
            eventInfo.username = user.username;
            view.OnLoginOK(userLogged);
        }


    }
}
