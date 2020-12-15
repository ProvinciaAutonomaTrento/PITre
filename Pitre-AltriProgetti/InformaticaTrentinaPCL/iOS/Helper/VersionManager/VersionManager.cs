using System;
using Foundation;
using InformaticaTrentinaPCL.Login.MVPD;

namespace InformaticaTrentinaPCL.iOS.Helper.VersionManager
{
    public class VersionManager : IVersionManager
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
            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            return version.ToString();
        }

        private VersionManager()
        {
        }
    }
}
