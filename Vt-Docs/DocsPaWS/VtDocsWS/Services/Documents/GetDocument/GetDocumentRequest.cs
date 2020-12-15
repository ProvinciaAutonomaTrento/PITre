using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocument
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetDocumentRequest"
    /// </summary>
    [DataContract]
    public class GetDocumentRequest : Request
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

        /// <summary>
        /// Se true oltre ai metadati restituisce anche il contenuto del file principale e allegati
        /// </summary>
        [DataMember]
        public bool GetFile
        {
            get;
            set;
        }

        /// <summary>
        /// Se pari a uno, restituisce il file con l'estensione della firma (p7m)
        /// </summary>
        [DataMember]
        public string GetFileWithSignature
        {
            get;
            set;
        }
    }
}