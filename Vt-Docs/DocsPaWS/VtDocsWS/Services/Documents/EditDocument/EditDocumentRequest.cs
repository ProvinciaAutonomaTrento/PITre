using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.EditDocument
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "EditDocumentRequest"
    /// </summary>
    [DataContract]
    public class EditDocumentRequest : Request
    {
        /// <summary>
        /// Documento di modifica
        /// </summary>
        [DataMember]
        public VtDocsWS.Domain.Document Document
        {
            get;
            set;
        }
    }
}