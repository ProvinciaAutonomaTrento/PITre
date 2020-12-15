using System;
using Firebase.Core;
using Foundation;
using InformaticaTrentinaPCL.AnalyticsCore;
using Firebase.Analytics;

namespace InformaticaTrentinaPCL.iOS
{
    public class IOSAnalyticsManager : IAnalyticsManager
    {

        private static IOSAnalyticsManager instance;
        

        public static IOSAnalyticsManager Instance(){
            if(instance == null){
                instance = new IOSAnalyticsManager();
            }
            return instance;
        }

        private IOSAnalyticsManager()
        {
            App.Configure();
        }

        public void LoginEvent(LoginEventInfo info)
        {
            NSString[] keys = { new NSString(EventParams.USERNAME.ToString()) };
            NSObject[] values = { new NSString(info.username) };
            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
            Analytics.LogEvent(Events.LOGIN.ToString(), parameters);
        }

        public void LogoutEvent(LogoutEventInfo info)
        {
            NSString[] keys = { new NSString(EventParams.USERNAME.ToString()) };
            NSObject[] values = { new NSString(info.username) };
            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(keys, values, keys.Length);
            Analytics.LogEvent(Events.LOGOUT.ToString(), parameters);
        }
    }
}
