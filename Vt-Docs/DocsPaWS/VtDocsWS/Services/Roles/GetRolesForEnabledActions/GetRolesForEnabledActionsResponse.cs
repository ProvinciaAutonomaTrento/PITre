using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Roles.GetRolesForEnabledActions
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetRolesResponse"
    /// </summary>
    [DataContract]
    public class GetRolesForEnabledActionsResponse : Response
    {
        /// <summary>
        /// Lista dei ruoli dell'utente
        /// </summary>
        [DataMember]
        public Domain.Role[] Roles
        {
            get;
            set;
        }
    }
}