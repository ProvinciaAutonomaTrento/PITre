using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Roles.GetUsersInRole
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetUsersInRoleRequest"
    /// </summary>
    [DataContract]
    public class GetUsersInRoleRequest : Request
    {
        /// <summary>
        /// Codice del ruolo
        /// </summary>
         [DataMember]
        public string CodeRole
        {
            get;
            set;
        }
    }
}