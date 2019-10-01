using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetStampAndSignature
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetStampAndSignatureRequest"
    /// </summary>
    [DataContract]
    public class GetStampAndSignatureRequest : Request
    {
        /// <summary>
        /// Id del documento
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