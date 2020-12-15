using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.CAPServices.GetDocsInOpportunity
{
    [DataContract]
    public class GetDocsInOpportunityRequest
    {
        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public string IdPrjOpportunity { get; set; }
    }
}