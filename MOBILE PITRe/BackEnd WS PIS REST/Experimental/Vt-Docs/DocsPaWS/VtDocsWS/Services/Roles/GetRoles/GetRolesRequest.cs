using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Roles.GetRoles
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetRolesRequest"
    /// </summary>
    [DataContract]
    public class GetRolesRequest : Request
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
    }
}