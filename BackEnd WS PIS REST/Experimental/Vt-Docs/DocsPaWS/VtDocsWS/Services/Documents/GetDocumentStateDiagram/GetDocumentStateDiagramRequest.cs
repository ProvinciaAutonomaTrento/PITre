using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocumentStateDiagram
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetDocumentStateDiagramRequest"
    /// </summary>
   [DataContract]
    public class GetDocumentStateDiagramRequest : Request
    {
        /// <summary>
        /// Docnumber del documento
        /// </summary>
         [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura del protocollo
        /// </summary>
         [DataMember]
        public string Signature
        {
            get;
            set;
        }
    }
}