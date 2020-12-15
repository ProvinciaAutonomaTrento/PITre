using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Calligraphy;
using Firebase.Analytics;
using HockeyApp.Android;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Droid.PinningSSL;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Network;

namespace InformaticaTrentinaPCL.Droid
{
    //You can specify additional application information in this attribute
    [Application]

   

    public class MainApplication : Application
    {

        private SessionData sessionData = new SessionData();
        private IAnalyticsManager analyticsManager;

        public static SessionData GetSessionData()
        {
            return ((MainApplication)Context).sessionData;
        }

        public static IAnalyticsManager getAnalyticsManager()
        {
            if (((MainApplication)Context).analyticsManager == null)
            {
                ((MainApplication)Context).analyticsManager = new AndroidAnalyticsManager(Context);
            }

            return ((MainApplication)Context).analyticsManager;
        }

        public MainApplication(IntPtr handle, JniHandleOwnership transer)
        : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            /**INIT HOCKEYAPP**/
            CrashManager.Register(this, "a991b0611055417ab78cca3195405e64", new AlwaysSendCrashListener());

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
            .SetDefaultFontPath("fonts/SourceSansPro-Regular.ttf")
            .SetFontAttrId(Resource.Attribute.fontPath)
            .Build());

            if (NetworkConstants.IsPinningSSLEnabled())
            {
                ServicePointConfig.SetUp();
            }

            EnableCertificateValidation();
        }

        public void EnableCertificateValidation()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (sender, cert, chain, sslPolicyErrors) =>
            {
                if (cert != null) System.Diagnostics.Debug.WriteLine(cert);
                return true;
            };
        }

    }
    public class AlwaysSendCrashListener : CrashManagerListener
    {
        public override bool ShouldAutoUploadCrashes()
        {
            return true;
        }
    }
}
