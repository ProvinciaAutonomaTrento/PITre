using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class SendingResult
    {
        [DataMember]
        public string CorrespondentId
        {
            get;
            set;
        }

        [DataMember]
        public string CorrespondentDescription
        {
            get;
            set;
        }

        [DataMember]
        public string Mail
        { get; set; }

        [DataMember]
        public string PrefChannel
        { get; set; }

        [DataMember]
        public string SendingRes
        { get; set; }

    }
}