using System;
using InformaticaTrentinaPCL.AnalyticsCore;

namespace InformaticaTrentinaPCL.AnalyticsCore
{
    public interface IAnalyticsManager
    {
        void LoginEvent(LoginEventInfo info);
        void LogoutEvent(LogoutEventInfo info);
    }
}
