using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.iOS.Login.Session;
using InformaticaTrentinaPCL.Login.MVPD;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class IosNativeFactory : INativeFactory
    {
        static private IosNativeFactory instance;

        public static IosNativeFactory Instance()
        {
            if (instance == null)
            {
                instance = new IosNativeFactory();
            }

            return instance;
        }

        private IosNativeFactory()
        {
        }

        public IFileSystemManager GetFileSystemManager()
        {
            return FileManager.FileManager.Instance();
        }

        public IAnalyticsManager GetAnalyticsManager()
        {
            return IOSAnalyticsManager.Instance();
        }

        public IReachability GetReachability()
        {
            return IosReachabilityManager.Instance();
        }

        public SessionData GetSessionData()
        {
            return SessionDataManager.Instance().GetSessionData();
        }

        public IVersionManager GetVersionManager()
        {
            return VersionManager.VersionManager.Instance();
        }
    }
}