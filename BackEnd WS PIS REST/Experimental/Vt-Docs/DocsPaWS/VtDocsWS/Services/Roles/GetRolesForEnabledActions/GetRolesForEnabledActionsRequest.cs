using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Roles.GetRolesForEnabledActions
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetRolesRequest"
    /// </summary>
    [DataContract]
    public class GetRolesForEnabledActionsRequest : Request
    {
        /// <summary>
        /// Codice dell'utente
        /// </summary>
        [DataMember]
        public string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'utente
        /// </summary>
        [DataMember]
        public string CodiceTipoFunzione
        {
            get;
            set;
        }

    }
}