using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using InformaticaTrentinaPCL.ChangePassword;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.Droid.Home;
using InformaticaTrentinaPCL.Droid.Login;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.UpdatePassword
{
    [Activity(Label = "@string/app_name")]
    public class UpdatePasswordActivity : BaseActivity, CustomAdministrationView.CustomAdministrationViewListener, ILoginView
    {
        CustomLoaderUtility loaderUtility;

        TextView user;
        EditText oldPwd;
        EditText newPwd;
        EditText repeatPwd;
        CustomAdministrationView administrationView;
        Button login;

        string userParam;

        IChangePasswordPresenter presenter;

        protected override int LayoutResource => Resource.Layout.activity_update_password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            userParam = Intent.GetStringExtra(LoginActivity.KEY_USERNAME);

            if (string.IsNullOrEmpty(userParam))
                throw new Exception("UserParam missing in UpdatePasswordActivity invocation");

            loaderUtility = new CustomLoaderUtility();
            presenter = new ChangePasswordPresenter(this, AndroidNativeFactory.Instance(),userParam);

            user = FindViewById<TextView>(Resource.Id.textView_userName);
            oldPwd = FindViewById<EditText>(Resource.Id.editText_old_pwd);
            newPwd = FindViewById<EditText>(Resource.Id.editText_new_pwd);
            repeatPwd = FindViewById<EditText>(Resource.Id.editText_repeat_pwd);
            administrationView = FindViewById<CustomAdministrationView>(Resource.Id.customAdministrationView);
            login = FindViewById<Button>(Resource.Id.button_login);

            user.Text = userParam;

            administrationView.listener = this;

            oldPwd.TextChanged += delegate
            {
                presenter.UpdateOldPassword(oldPwd.Text);
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
            presenter.OnViewReady();
        }

        public void OnServerChanged(string message)
        {
            throw new NotImplementedException();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == (int)ActivityRequestCodes.SelectAdministrator && resultCode == Result.Ok)
            {
                string serializedModel = data.GetStringExtra(SelectAdministratorActivity.KEY_SELECTED_ADMINISTRATOR);

                AmministrazioneModel model = JsonConvert.DeserializeObject<AmministrazioneModel>(serializedModel);
                administrationView.showAdministration(model);

            }
        }

        public override void OnBackPressed()
        {

        }

        #region CustomAdministrationView.CustomAdministrationViewListener

        public void OnSelectAdministrationClick()
        {
            ShowListAdministration();
        }

        public void OnUpdateAdministrationViewState(AmministrazioneModel model, LoginAdministrationState state, bool isUserAction = true)
        {
            presenter.UpdateAdministration(model, state, isUserAction);
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
            throw new NotImplementedException();
        }

        public void EnableButton(bool enabled)
        {
            login.Enabled = enabled;
        }

        public void ShowListAdministration()
        {
            Intent intent = new Intent(this, typeof(SelectAdministratorActivity));
            intent.PutExtra(LoginActivity.KEY_USERNAME, userParam);
            StartActivityForResult(intent, (int)ActivityRequestCodes.SelectAdministrator);
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
