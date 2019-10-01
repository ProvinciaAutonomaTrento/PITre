using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.AddDocInProject
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "AddDocInProjectRequest"
    /// </summary>
    [DataContract]
    public class AddDocInProjectRequest : Request
    {
        /// <summary>
        /// Id del documento che si desidera inserire nel fascicolo
        /// </summary>
         [DataMember]
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi
        /// </summary>
         [DataMember]
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascicolo nel quale fascicolare il documento
        /// </summary>
         [DataMember]
        public string IdProject
        {
            get;
            set;
        }
    }
}