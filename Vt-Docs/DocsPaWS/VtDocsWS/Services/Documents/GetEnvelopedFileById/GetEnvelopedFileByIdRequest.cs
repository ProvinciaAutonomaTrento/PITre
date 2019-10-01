using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace VtDocsWS.Services.Documents.GetEnvelopedFileById
{
    [DataContract]
    public class GetEnvelopedFileByIdRequest : Request
    {
        /// <summary>
        /// DocNumber del documento
        /// </summary>
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Id della versione, opzionale, se vuoto prende l'ultima versione
        /// </summary>
        [DataMember]
        public string VersionId
        {
            get;
            set;
        }

        /// <summary>
        /// Prelievo solo delle informazioni sul file. False di default
        /// </summary>
        [DataMember]
        public bool GetOnlyFileInfo { get; set; }

        /// <summary>
        /// Prendo il file sbustato. Default false.
        /// </summary>
        [DataMember]
        public bool GetFileWithoutEnvelope
        {
            get;
            set;
        }
    }
}