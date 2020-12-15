using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Domain
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class OpportunityData
    {
        [DataMember]
        public string DataName { get; set; }

        [DataMember]
        public string DataValue { get; set; }
    }
}