using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.EditDocStateDiagram
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "EditDocStateDiagramRequest"
    /// </summary>
  [DataContract]
    public class EditDocStateDiagramRequest : Request
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

        /// <summary>
        /// Stato del diagramma che verrà salvato per il documento
        /// </summary>
         [DataMember]
        public string StateOfDiagram
        {
            get;
            set;
        }
    }
}