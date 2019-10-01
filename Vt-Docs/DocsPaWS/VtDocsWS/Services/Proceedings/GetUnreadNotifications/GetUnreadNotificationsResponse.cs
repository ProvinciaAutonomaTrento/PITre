using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.GetUnreadNotifications
{
    public class GetUnreadNotificationsResponse : Response
    {
        public Domain.Proceeding[] Proceedings
        {
            get;
            set;
        }
    }
}