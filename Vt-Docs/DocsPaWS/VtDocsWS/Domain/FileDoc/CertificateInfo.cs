using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    /*
     * Certificate information
     */
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class CertificateInfo
    {
        /// <summary>
        /// Stato revoca (CAPICOM) - int
        /// </summary>
        [DataMember]
        public string RevocationStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione stato di revoca 
        /// </summary>
        [DataMember]
        public string RevocationStatusDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Serial number
        /// </summary>
        [DataMember]
        public string SerialNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Algoritmo firma (OID)
        /// </summary>
        [DataMember]
        public string SignatureAlgorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Valido da - DateTime
        /// </summary>
        [DataMember]
        public string ValidFromDate
        {
            get;
            set;
        }

        /// <summary>
        /// Valido fino a - DateTime
        /// </summary>
        [DataMember]
        public string ValidToDate
        {
            get;
            set;
        }

        /// <summary>
        /// DataRevoca - DateTime
        /// </summary>
        [DataMember]
        public string RevocationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Soggetto
        /// </summary>
        [DataMember]
        public string SubjectName
        {
            get;
            set;
        }

        /// <summary>
        /// Nome della CA emittente
        /// </summary>
        [DataMember]
        public string IssuerName
        {
            get;
            set;
        }

        /// <summary>
        /// Impronta SHA-1
        /// </summary>
        [DataMember]
        public string ThumbPrint
        {
            get;
            set;
        }

    }
}