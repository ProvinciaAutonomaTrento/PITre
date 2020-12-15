using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VtDocsWS.Services.Documents.InvalidateDocument
{
    [System.Runtime.Serialization.DataContract]
    public class InvalidateDocumentRequest:Request
    {
        /// <summary>
        /// Obbligatorio Segantura del Protocollo
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public string Segnature { get; set; }
        /// <summary>
        /// Obbligatorio Descrizione dell'Annulamento
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public string Description { get; set; }
    }
}