using Android.App;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.Utils.Reachability;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.MVPD;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    public class AndroidNativeFactory : INativeFactory
    {
        static private AndroidNativeFactory instance;

        public static AndroidNativeFactory Instance()
        {
            if (instance == null)
            {
                instance = new AndroidNativeFactory();
            }

            return instance;
        }

        private AndroidNativeFactory()
        {
        }
        
        public IAnalyticsManager GetAnalyticsManager()
        {
            return MainApplication.getAnalyticsManager();
        }

        public IReachability GetReachability()
        {
            return AndroidReachabilityManager.GetInstance();
        }

        public IFileSystemManager GetFileSystemManager()
        {
            return new FileSystemManager();
        }

        public SessionData GetSessionData()
        {
            return MainApplication.GetSessionData();
        }

        public IVersionManager GetVersionManager()
        {
            return VersionManager.Instance();
        }
    }
}