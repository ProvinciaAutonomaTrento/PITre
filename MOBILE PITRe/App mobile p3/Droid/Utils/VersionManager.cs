using System;
using InformaticaTrentinaPCL.Login.MVPD;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    public class VersionManager: IVersionManager
    {
        static private VersionManager instance;

        public static VersionManager Instance()
        {
            if (instance == null)
            {
                instance = new VersionManager();
            }

            return instance;
        }

        public string getAppVersion()
        {
            var version = Android.App.Application.Context.PackageManager.GetPackageInfo(Android.App.Application.Context.PackageName, 0).VersionName;
            return version.ToString();
        }

        private VersionManager()
        {
        }
    }
}
