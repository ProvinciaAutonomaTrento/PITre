using System;
using Android.Content;
using Android.OS;
using Firebase.Analytics;
using InformaticaTrentinaPCL.AnalyticsCore;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    public class AndroidAnalyticsManager : IAnalyticsManager
    {

        private FirebaseAnalytics firebaseAnalytics;

        public AndroidAnalyticsManager(Context context)
        {
            // Obtain the FirebaseAnalytics instance.
            firebaseAnalytics = FirebaseAnalytics.GetInstance(context);
        }

        public void LoginEvent(LoginEventInfo extra)
        {
            var bundle = new Bundle();
            bundle.PutString(EventParams.USERNAME.ToString(), extra.username);
            firebaseAnalytics.LogEvent(Events.LOGIN.ToString(), bundle);
        }

        public void LogoutEvent(LogoutEventInfo extra)
        {
            var bundle = new Bundle();
            bundle.PutString(EventParams.USERNAME.ToString(), extra.username);
            firebaseAnalytics.LogEvent(Events.LOGOUT.ToString(), bundle);
        }

    }
}
