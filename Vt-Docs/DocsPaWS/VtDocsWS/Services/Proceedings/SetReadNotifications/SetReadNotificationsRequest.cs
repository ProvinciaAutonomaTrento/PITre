using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.SetReadNotifications
{
    [DataContract]
    public class SetReadNotificationsRequest : Request
    {
        [DataMember]
        public String IdDoc
        {
            get;
            set;
        }

        [DataMember]
        public String IdProceeding
        {
            get;
            set;
        }
    }
}