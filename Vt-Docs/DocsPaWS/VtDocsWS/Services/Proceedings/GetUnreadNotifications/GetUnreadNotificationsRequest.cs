using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.GetUnreadNotifications
{
    [DataContract]
    public class GetUnreadNotificationsRequest : Request
    {
        /// <summary>
        /// Codice rubrica dell'utente
        /// </summary>
        [DataMember]
        public String CodeCorrespondent
        {
            get;
            set;
        }
    }
}