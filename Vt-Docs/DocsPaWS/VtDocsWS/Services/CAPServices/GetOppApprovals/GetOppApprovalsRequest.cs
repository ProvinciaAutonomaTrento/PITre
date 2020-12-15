using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.CAPServices.GetOppApprovals
{
    [DataContract]
    public class GetOppApprovalsRequest
    {
        [DataMember]
        public string AuthenticationToken { get; set; }

        [DataMember]
        public string IdOppDocspa { get; set; }
    }
}