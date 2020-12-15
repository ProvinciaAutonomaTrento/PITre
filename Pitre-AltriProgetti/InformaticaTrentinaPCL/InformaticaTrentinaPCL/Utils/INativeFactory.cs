using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.MVPD;
using InformaticaTrentinaPCL.OpenFile.MVP;

namespace InformaticaTrentinaPCL.Utils
{
    public interface INativeFactory
    {
        IAnalyticsManager GetAnalyticsManager();
        SessionData GetSessionData();
        IReachability GetReachability();
        IFileSystemManager GetFileSystemManager();
        //TODO Add INativeResolver Helpers
        IVersionManager GetVersionManager();
    }
}