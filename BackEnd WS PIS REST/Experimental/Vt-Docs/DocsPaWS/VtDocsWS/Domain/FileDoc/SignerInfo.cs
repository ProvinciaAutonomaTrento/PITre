using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class SignerInfo
    {
        /// <summary>
        /// ParentSignerCertSN
        /// </summary>
        [DataMember]
        public string ParentSignerCertSN
        {
            get;
            set;
        }

        /// <summary>
        /// CertificateInfo
        /// </summary>
        [DataMember]
        public CertificateInfo CertificateInfo
        {
            get;
            set;
        }

        /// <summary>
        /// SubjectInfo
        /// </summary>
        [DataMember]
        public SubjectInfo SubjectInfo
        {
            get;
            set;
        }
    }
}