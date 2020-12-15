using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.CAPServices.StoreOpportunity
{
    [DataContract]
    public class StoreOpportunityRequest
    {
        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public VtDocsWS.Domain.OpportunityData[] OppDatas { get; set; }

        [DataMember]
        public VtDocsWS.Domain.File[] Files { get; set; }
    }
}