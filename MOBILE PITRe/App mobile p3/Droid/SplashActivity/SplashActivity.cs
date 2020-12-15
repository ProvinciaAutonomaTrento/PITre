using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Preferences;
using Android.Support.V7.App;
using InformaticaTrentinaPCL.Droid.ChooseInstance;
using InformaticaTrentinaPCL.Droid.Home;
using InformaticaTrentinaPCL.Droid.Login;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.Network;

namespace InformaticaTrentinaPCL.Droid.SplashActivity
{
  [Activity(Theme = "@style/MasterDetailTheme.Splash", MainLauncher = true, NoHistory = true, LaunchMode = LaunchMode.SingleInstance, ClearTaskOnLaunch = true)]
  [IntentFilter(new[] {Android.Content.Intent.ActionView},
    DataScheme = NetworkConstants.APP_PROTOCOL,
    DataHost = NetworkConstants.APP_BASE_URL,
    Categories = new[] {Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable})]
  public class SplashActivity : AppCompatActivity
  {
    static readonly string TAG = "X:" + typeof(SplashActivity).Name;

    // Launches the startup task
    protected override void OnResume()
    {
      base.OnResume();
      Task startupWork;

      if (!string.IsNullOrEmpty(Intent.DataString) && Intent.DataString.StartsWith(NetworkConstants.APP_BASE_SHARE_URL))
      {
        startupWork = new Task(() => { OpenDocumentTask(Intent.DataString); });
      }
      else
      {
        startupWork = new Task(() => { LoginTask(); });
      }

      startupWork.Start();
    }

    async void LoginTask()
    {
      if (VerifyInstanceSaved())
      {
        await Task.Delay(2000);
        StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
      }
      else
      {
        await Task.Delay(2000);
        var intent = new Intent(Application.Context, typeof(ChooseInstanceActivity));
        StartActivity(intent);
      }
    }

    /// <summary>
    /// Verifies the instance saved.
    /// </summary>
    /// <returns>The instance saved.</returns>
    public bool VerifyInstanceSaved()
    {
      ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
      var descriptionSaved = prefs.GetString(ChooseInstanceActivity.KEY_INSTANCE_SAVED, null);
      return !string.IsNullOrEmpty(descriptionSaved);
    }

    async void OpenDocumentTask(string url)
    {
      Intent intent;

      if (MainApplication.GetSessionData().IsInitialized)
      {
        intent = new Intent(Application.Context, typeof(HomeActivity));
      }
      else
      {
        await Task.Delay(2000);
        intent = new Intent(Application.Context, typeof(LoginActivity));
      }

      MainApplication.GetSessionData().urlToOpen = url;

      intent.AddFlags(ActivityFlags.ClearTop);
      StartActivity(intent);
    }
  }
}