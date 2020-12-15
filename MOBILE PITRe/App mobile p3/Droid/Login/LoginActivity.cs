using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using InformaticaTrentinaPCL.ChangePassword;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Droid.Home;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Droid.UpdatePassword;
using Newtonsoft.Json;
using InformaticaTrentinaPCL.Droid.ChooseInstance;
using Android.Preferences;
using InformaticaTrentinaPCL.Login.MVPD;
using InformaticaTrentinaPCL.Droid.RecuperaPassword;

namespace InformaticaTrentinaPCL.Droid.Login
{
    [Activity(Label = "@string/app_name")]
    public class LoginActivity : BaseActivity, ILoginView, ILoginViewChooseInstance, CustomAdministrationView.CustomAdministrationViewListener
    {
        
        public const string KEY_USERNAME = "KEY_USERNAME";

        protected override int LayoutResource => Resource.Layout.activity_login;

        CustomLoaderUtility loaderUtility;
        private string textIntanceSaved;
        private EditText username;
        private EditText password;
        CustomAdministrationView administrationView;
        Button signInButton;
        Button changeInstance;
       Button recuperaPassword; 
        TextView instanceSaved;
        TextView appVersion;

        LoginNativePresenter presenter;

        protected override void OnStart()
        {
            base.OnStart();
            presenter.OnViewReady();
            this.UpdateAppVersion();
            SetSavedBaseUrl();
            SetUrlInstanceDescription();
            instanceSaved.Text = textIntanceSaved;
            signInButton.Click += SignInButton_Click;
        }

        protected override void OnStop()
        {
            base.OnStop();
            signInButton.Click -= SignInButton_Click;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            presenter.Dispose();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);     
            loaderUtility = new CustomLoaderUtility();
            presenter = new LoginNativePresenter(this, AndroidNativeFactory.Instance(), this);   
            signInButton = FindViewById<Button>(Resource.Id.button_login);
            username = FindViewById<EditText>(Resource.Id.editText_username);
            password = FindViewById<EditText>(Resource.Id.editText_password);
            administrationView = FindViewById<CustomAdministrationView>(Resource.Id.customAdministrationView);

            changeInstance = FindViewById<Button>(Resource.Id.change_instance_button);
            recuperaPassword = FindViewById<Button>(Resource.Id.password_dimenticata); 
            instanceSaved = FindViewById<TextView>(Resource.Id.texView_instance_selected);

            appVersion = FindViewById<TextView>(Resource.Id.app_version_tv);

            administrationView.listener = this;

            username.TextChanged += delegate
            {
                presenter.UpdateUsername(username.Text);
            };

            password.TextChanged += delegate
            {
                presenter.UpdatePassword(password.Text);
            };

#if DEBUG || SVIL          
            // I campi DEFAULT_USERNAME e DEFAULT_PASSWORD sono valorizzate solo se 
            // NON siamo in Produzione
            username.Text = NetworkConstants.DEFAULT_USERNAME;
            password.Text = NetworkConstants.DEFAULT_PASSWORD;

            signInButton.LongClick += delegate
            {
                presenter.ChangeServer();
            };
#endif            

            changeInstance.Click += delegate
            {
                presenter.OpenViewChooseInstance();
            };
            
           recuperaPassword.Click += delegate
            {
                presenter.OpenViewRecuperaPassword();
            };

            presenter.OnRestoreInstanceState(savedInstanceState);

        }

        private void UpdateAppVersion()
        {
            var version = presenter.GetAppVersion();
            appVersion.Text = version;
        }

        /// <summary>
        /// Gets the URL saved.
        /// </summary>
        public void SetSavedBaseUrl()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var a = prefs.GetString(ChooseInstanceActivity.KEY_INSTANCE_URL, null);
            presenter.SetUrlConstant(prefs.GetString(ChooseInstanceActivity.KEY_INSTANCE_URL, null));
        }

        /// <summary>
        /// Gets the instance saved.
        /// </summary>
        public void SetUrlInstanceDescription()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            textIntanceSaved = prefs.GetString(ChooseInstanceActivity.KEY_INSTANCE_SAVED, null);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutAll(presenter.GetStateToSave());
            base.OnSaveInstanceState(outState);
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

        public void OnServerChanged(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        void SignInButton_Click(object sender, System.EventArgs e)
        {
              presenter.LoginAsync(false);
           // presenter.VerifyUpdate("IOS");
        }

        #region ILoginView
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
        
        public void OnLoginOK(UserInfo user)
        {
            Intent intent = new Intent(this, typeof(HomeActivity));
            StartActivity(intent);
            Finish();
        }

        public void ShowChangePassword()
        {
            Intent intent = new Intent(this, typeof(UpdatePasswordActivity));
            intent.PutExtra(LoginActivity.KEY_USERNAME, username.Text);
            StartActivity(intent);
        }

        public void EnableButton(bool enabled)
        {
            signInButton.Enabled = enabled;
        }

        public void ShowListAdministration()
        {
            Intent intent = new Intent(this, typeof(SelectAdministratorActivity));
            intent.PutExtra(KEY_USERNAME, username.Text);
            StartActivityForResult(intent, (int)ActivityRequestCodes.SelectAdministrator);
        }
        #endregion

        #region CustomAdministrationView.CustomAdministrationViewListener

        public void OnSelectAdministrationClick()
        {
            ShowListAdministration();
        }

        public void OnUpdateAdministrationViewState(AmministrazioneModel model, LoginAdministrationState state, bool isUserAction)
        {
            presenter.UpdateAdministration(model, state, isUserAction);
        }

        /// <summary>
        /// Opens the view change instance.
        /// </summary>
        public void OpenViewChangeInstance()
        {
            var intent = new Intent(Application.Context, typeof(ChooseInstanceActivity));
            StartActivity(intent);
        }


        /// <summary>
        /// Opens the view change instance.
        /// </summary>
        public void OpenViewRecuperaPassword()
        {
            //  var intent = new Intent(Application.Context, typeof(RecuperaPasswordActivity));
            var intent = new Intent(Application.Context, typeof(RecuperaOtpActivity));
            StartActivity(intent);
        }

        public void ShowUpdatePopup(string url)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Aggiornamento disponibile");
            alert.SetMessage("E disponibile una nuova applicazione clicca ok per aggiornare");
            alert.SetPositiveButton("Ok", (senderAlert, args) => {
                //var uri = Android.Net.Uri.Parse(url);
                //var intent = new Intent(Intent.ActionView, uri);
                //StartActivity(intent);

                //Browser.Async(url, BrowserLaunchMode.SystemPreferred);


            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                presenter.LoginAsync(false);
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        #endregion

    }
}
