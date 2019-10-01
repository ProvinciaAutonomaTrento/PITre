using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class EmailAttachment
    {

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public string ContentType
        {
            get;
            set;
        }

        [DataMember]
        public string SourceFile
        {
            get;
            set;
        }

        [DataMember]
        public byte[] AttachmentData
        {
            get;
            set;
        }
    }
}