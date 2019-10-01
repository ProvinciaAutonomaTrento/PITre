using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.CreateDocument
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "CreateDocumentResponse"
    /// </summary>
   [DataContract]
    public class CreateDocumentResponse : Response
    {
        /// <summary>
        /// Dettaglio del documento creato
        /// </summary>
        [DataMember]
        public Domain.Document Document
        {
            get;
            set;
        }
    }
}