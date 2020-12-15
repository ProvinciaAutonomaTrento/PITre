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
    [Activity(Label = "RecuperaPasswordActivity")]
    public class RecuperaPasswordActivity : BaseActivity , ILoginView //CustomAdministrationView.CustomAdministrationViewListener, 
    {
        CustomLoaderUtility loaderUtility;

        TextView infoEmail;
        EditText otp;
        EditText newPwd;
        EditText repeatPwd;
      //  CustomAdministrationView administrationView;
        Button login;

        string userParam;

        IRecuperaPasswordPresenter presenter;

        protected override int LayoutResource => Resource.Layout.activity_login_password_dimenticata;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

              userParam = Intent.GetStringExtra(LoginActivity.KEY_USERNAME);

            infoEmail = FindViewById<TextView>(Resource.Id.infoemail);
            infoEmail.Text =  Intent.GetStringExtra("info_email"); 

            loaderUtility = new CustomLoaderUtility();
            presenter = new RecuperaPasswordPresenter(this, AndroidNativeFactory.Instance(), userParam);

           otp = FindViewById<EditText>(Resource.Id.editText_otp);
            newPwd = FindViewById<EditText>(Resource.Id.editText_new_pwd);
            repeatPwd = FindViewById<EditText>(Resource.Id.editText_repeat_pwd);
            login = FindViewById<Button>(Resource.Id.button_login);
            otp.TextChanged += delegate
            {
               presenter.UpdateOTP(otp.Text);
            };

            newPwd.TextChanged += delegate
            {
                presenter.UpdateNewPassword(newPwd.Text);
            };

            repeatPwd.TextChanged += delegate
            {
                presenter.UpdateRepeatedNewPassword(repeatPwd.Text);
            };

            login.Click += delegate
            {
                presenter.LoginAsync(true);
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

           
        }

   
        #region CustomAdministrationView.CustomAdministrationViewListener

        public void OnSelectAdministrationClick()
        {
       }

        public void OnUpdateAdministrationViewState(AmministrazioneModel model, LoginAdministrationState state, bool isUserAction = true)
        {
        }
        #endregion

        #region ILoginView

        public void OnLoginOK(UserInfo user)
        {
            Intent intent = new Intent(this, typeof(HomeActivity));
            StartActivity(intent);
            Finish();
        }

        public void ShowChangePassword()
        {
         //   throw new NotImplementedException();
        }
        
        public void EnableButton(bool enabled)
        {
            login.Enabled = enabled;
        }

        public void ShowListAdministration()
        {
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

        public void ShowUpdatePopup(string url)
        {
            // throw new System.NotImplementedException();
        }

        #endregion
    }
}
