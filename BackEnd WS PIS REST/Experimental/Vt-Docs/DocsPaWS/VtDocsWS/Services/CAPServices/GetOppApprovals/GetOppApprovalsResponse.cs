using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.CAPServices.GetOppApprovals
{
    [DataContract]
    public class GetOppApprovalsResponse : Response
    {
        [DataMember]
        public string[] OppApprovals { get; set; }
        
    }
}