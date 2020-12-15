using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VtDocsWS.Services.Documents.InvalidateDocument
{
    [System.Runtime.Serialization.DataContract]
    public class InvalidateDocumentResponse:Response
    {
        /// <summary>
        /// Esito dell'operazione
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public bool Success { get; set; }
        /// <summary>
        /// Messaggio di errore
        /// </summary>
        //[System.Runtime.Serialization.DataMember]
        public PisException Error { get; set; }
    }
}