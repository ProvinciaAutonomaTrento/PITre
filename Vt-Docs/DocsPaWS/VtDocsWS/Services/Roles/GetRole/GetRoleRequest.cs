using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Roles.GetRole
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetRoleRequest"
    /// </summary>
  [DataContract]
    public class GetRoleRequest : Request
    {
        /// <summary>
        /// Codice del ruolo da cercare
        /// </summary>
         [DataMember]
        public string CodeRole
        {
            get;
            set;
        }

        /// <summary>
        /// Id del ruolo da cercare
        /// </summary>
         [DataMember]
        public string IdRole
        {
            get;
            set;
        }
    }
}