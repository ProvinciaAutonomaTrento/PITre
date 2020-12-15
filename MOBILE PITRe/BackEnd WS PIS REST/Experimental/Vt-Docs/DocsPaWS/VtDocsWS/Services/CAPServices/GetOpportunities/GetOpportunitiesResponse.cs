using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.CAPServices.GetOpportunities
{
    [DataContract]
    public class GetOpportunitiesResponse : Response
    {
        [DataMember]
        public Domain.Project[] Opportunities { get; set; }

    }
}