using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Roles.GetRole
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetRoleResponse"
    /// </summary>
   [DataContract]
    public class GetRoleResponse : Response
    {
        /// <summary>
        /// Dettaglio del ruolo richiesto
        /// </summary>
         [DataMember]
        public VtDocsWS.Domain.Role Role
        {
            get;
            set;
        }
    }
}