using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.CAPServices.GetOpportunities
{
    [DataContract]
    public class GetOpportunitiesRequest
    {
        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public string OpportunityStatus { get; set; }
    }
}