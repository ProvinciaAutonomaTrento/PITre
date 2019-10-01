using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.EditDocument
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "EditDocumentResponse"
    /// </summary>
    [DataContract]
    public class EditDocumentResponse : Response
    {
        /// <summary>
        /// Documento modificato
        /// </summary>
         [DataMember]
        public VtDocsWS.Domain.Document Document
        {
            get;
            set;
        }
    }
}