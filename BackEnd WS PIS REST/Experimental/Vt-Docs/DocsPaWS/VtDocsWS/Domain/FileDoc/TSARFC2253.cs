using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class TSARFC2253
    {
        /// <summary>
        /// nome della TSA in formato RFC2253
        /// </summary>
        [DataMember]
        public string TSARFC2253Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// Nome comune
        /// </summary>
        [DataMember]
        public string CN
        {
            get;
            set;
        }
        
        /// <summary>
        /// nome dell'unità organizzativa
        /// </summary>
        [DataMember]
        public string OU
        {
            get;
            set;
        }
        
        /// <summary>
        /// nome dell'organizzazione
        /// </summary>
        [DataMember]
        public string O
        {
            get;
            set;
        }
        
        /// <summary>
        /// sigla della nazione
        /// </summary>
        [DataMember]
        public string C
        {
            get;
            set;
        }
    }
}