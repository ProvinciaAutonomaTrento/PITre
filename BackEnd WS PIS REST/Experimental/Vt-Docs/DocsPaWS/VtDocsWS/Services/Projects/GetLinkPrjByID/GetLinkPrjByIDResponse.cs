using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetLinkPrjByID
{
    [DataContract]
    public class GetLinkPrjByIDResponse : Response
    {
        [DataMember]
        public string ProjectLink
        {
            get;
            set;
        }
    }
}