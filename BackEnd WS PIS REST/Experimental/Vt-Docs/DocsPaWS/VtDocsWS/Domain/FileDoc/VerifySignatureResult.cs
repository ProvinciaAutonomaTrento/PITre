using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class VerifySignatureResult
    {
        //TO DO Struttura VerifySignatureResult
        /// <summary>
        /// Status Code - è un int
        /// </summary>
        [DataMember]
        public string StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// Status Description
        /// </summary>
        [DataMember]
        public string StatusDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Final Document Name
        /// </summary>
        [DataMember]
        public string FinalDocumentName
        {
            get;
            set;
        }

        /// <summary>
        /// CRLOnlineCheck - bool
        /// </summary>
        [DataMember]
        public string CRLOnlineCheck
        {
            get;
            set;
        }

        /// <summary>
        /// PKCS7Documents
        /// </summary>
        [DataMember]
        public PKCS7Document[] PKCS7Documents
        {
            get;
            set;
        }
        
    }
}