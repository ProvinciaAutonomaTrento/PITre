using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetDocumentsInProject
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetDocumentsInProjectResponse"
    /// </summary>
    [DataContract]
    public class GetDocumentsInProjectResponse : Response
    {
        /// <summary>
        /// Documenti contenuti nel fascicolo
        /// </summary>
        [DataMember]
        public Domain.Document[] Documents
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei documenti trovati
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? TotalDocumentsNumber
        {
            get;
            set;
        }
    }
}