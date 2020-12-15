using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Domain
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class ModifiedDocument
    {
        [DataMember]
        public string DocumentId
        {
            get;
            set;
        }

        [DataMember]
        public string ActionDescription
        {
            get;
            set;
        }

        [DataMember]
        public string DataEvento
        {
            get;
            set;
        }

        [DataMember]
        public string Operatore
        {
            get;
            set;
        }

    }
}