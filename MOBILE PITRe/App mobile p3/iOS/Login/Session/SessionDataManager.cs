using System;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.iOS.Helper;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.iOS.Login.Session
{
    public class SessionDataManager
    {
        public static SessionDataManager sessionDataManager;
        private SessionData sessionData = new SessionData();

        public static String[] LIST_ACTION_TODO_DOCUMENT =
            new String[] 
        {
            Utility.LanguageConvert("document_state_accept"),
            Utility.LanguageConvert("document_state_accept_send"),
            Utility.LanguageConvert("document_state_accept_acl"),
            Utility.LanguageConvert("document_state_refuse"),
            Utility.LanguageConvert("document_state_sharing")
        };

        public static ActionType[] LIST_ACTION_TYPE =
        {
            ActionType.ACCEPT,
            ActionType.ASSIGN,
            ActionType.ACCEPT_AND_ADL,
            ActionType.REFUSE,
            ActionType.SHARE,
        };

        public SessionDataManager()
        {
        }

        static public SessionDataManager Instance()
        {
            if (sessionDataManager == null)
                sessionDataManager = new SessionDataManager();

            return sessionDataManager;
        }

        public SessionData GetSessionData()
        {
            return sessionData;
        }

    }
}
