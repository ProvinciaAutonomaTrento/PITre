using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.SendDocument
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "SendDocumentRequest"
    /// </summary>
    [DataContract]
    public class SendDocumentRequest : Request
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
        /// Segnatura del documento
        /// </summary>
         [DataMember]
        public string Signature
        {
            get;
            set;
        }
    }
}