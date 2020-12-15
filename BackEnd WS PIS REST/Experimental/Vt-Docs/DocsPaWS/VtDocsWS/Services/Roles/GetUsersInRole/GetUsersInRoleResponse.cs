using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Roles.GetUsersInRole
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetUsersInRoleResponse"
    /// </summary>
   [DataContract]
    public class GetUsersInRoleResponse : Response
    {
       /// <summary>
       /// Utenti nel ruolo
       /// </summary>
        [DataMember]
        public Domain.User[] Users
        {
            get;
            set;
        }
    }
}