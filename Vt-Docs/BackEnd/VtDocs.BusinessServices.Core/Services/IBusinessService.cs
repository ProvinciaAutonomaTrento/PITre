using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBusinessService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Authentication.GetLoginResponse LoginUser(VtDocs.BusinessServices.Entities.Authentication.GetLoginRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.Response LogoutUser(VtDocs.BusinessServices.Entities.Request request);

        /// <summary>
        /// Reperimento valore chiave di configurazione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        VtDocs.BusinessServices.Entities.GetConfigurazioneResponse GetConfigurazione(VtDocs.BusinessServices.Entities.GetConfigurazioneRequest request);
    }
}
