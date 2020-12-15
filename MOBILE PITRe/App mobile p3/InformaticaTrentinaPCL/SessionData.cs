using System;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL
{
    public class SessionData
    {
        public bool IsInitialized { get; private set; }
        public UserInfo userInfo { get; private set; }
        public string urlToOpen { get; set; }
        public bool isTodoListRemovalManual { get; set; }
        public bool shareAllowed { get; set; }

        public void SetUserInfo(UserInfo info)
        {
            IsInitialized = true;
            userInfo = info;
        }

        public SessionData()
        {
        }

        public void clear()
        {
            IsInitialized = false;
            userInfo = null;
        }

    }
}
