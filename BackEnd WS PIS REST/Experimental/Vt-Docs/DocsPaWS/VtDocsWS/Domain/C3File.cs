using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// File
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class C3File
    {
        /// <summary>
        /// System id del documento
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// Descrizione del file
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Contenuto del file
        /// </summary>
        [DataMember]
        public byte[] Content
        {
            get;
            set;
        }

        /// <summary>
        /// Mime del file
        /// </summary>
        [DataMember]
        public string MimeType
        {
            get;
            set;
        }

        /// <summary>
        /// Id della versione del file
        /// </summary>
        [DataMember]
        public string VersionId
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del file
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public string PathName
        { get; set; }

        [DataMember]
        public string VersionIdinDB { get; set; }

        [DataMember]
        public string FileSize { get; set; }
    }
}