using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.CreateDocument
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "CreateDocumentRequest"
    /// </summary>
    [DataContract]
    public class CreateDocumentRequest : Request
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
         [DataMember]
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        [DataMember]
        public Domain.Document Document
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
         [DataMember]
        public string CodeRF
        {
            get;
            set;
        }
    }
}