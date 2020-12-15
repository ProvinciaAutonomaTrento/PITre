using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using InformaticaTrentinaPCL.ChangePassword;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.Droid.Home;
using InformaticaTrentinaPCL.Droid.Login;
using InformaticaTrentinaPCL.Droid.UpdatePassword;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.RecuperaPassword;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.RecuperaPassword
{
    [Activity(Label = "RecuperaOtpActivity")]
    public class RecuperaOtpActivity : BaseActivity ,  ILoginView
    {
        CustomLoaderUtility loaderUtility;

       
        EditText user;
       
        Button login;

        string userParam;

        IRecuperaOtpPresenter presenter;

        protected override int LayoutResource => Resource.Layout.activity_login_recupera_otp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            loaderUtility = new CustomLoaderUtility();
            presenter = new RecuperaOtpPresenter(this, AndroidNativeFactory.Instance());

            user = FindViewById<EditText>(Resource.Id.editText_userName);

            login = FindViewById<Button>(Resource.Id.button_login);



            user.TextChanged += delegate
            {
               presenter.UpdateUsername(user.Text);
            };

      

            login.Click += delegate
            {
                presenter.GetOTPAsync();
               
                
            };   
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public void OnServerChanged(string message)
        {
         //   throw new NotImplementedException();
        }

       

        #region ILoginView

        public void OnLoginOK(UserInfo userInf)
        {
            var intent = new Intent(Application.Context, typeof(RecuperaPasswordActivity));
            intent.PutExtra(LoginActivity.KEY_USERNAME, user.Text);
            intent.PutExtra("info_email", userInf.email);
            StartActivity(intent);
            Finish();
        }

     
        
        public void EnableButton(bool enabled)
        {
            login.Enabled = enabled;
        }

       

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, e, isLight);
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
            {
                loaderUtility.showLoader(this);
            }
            else
            {
                loaderUtility.hideLoader();
            }
        }

        public void ShowChangePassword()
        {
           
        }

        public void ShowListAdministration()
        {
            
        }

        public void ShowUpdatePopup(string url)
        {
            // throw new System.NotImplementedException();
        }

        #endregion
    }
}
