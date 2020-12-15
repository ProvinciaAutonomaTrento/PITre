using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    /// <summary>
    /// FileDoc
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class FileDoc
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

        /*
         * Informazioni relative alla firma
         */
        /// <summary>
        /// Risultato di verifica firma
        /// </summary>
        [DataMember]
        public VerifySignatureResult verifySignatureResult
        {
            get;
            set;
        }

        /// <summary>
        /// Risultato di timestamp della marca temporale
        /// </summary>
        [DataMember]
        public OutputResponseMarca timestampResult
        {
            get;
            set;
        }
    }
}