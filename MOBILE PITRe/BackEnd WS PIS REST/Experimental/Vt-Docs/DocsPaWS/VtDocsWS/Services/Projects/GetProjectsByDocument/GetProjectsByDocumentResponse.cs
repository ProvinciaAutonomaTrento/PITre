using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.GetProjectsByDocument
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetProjectsByDocumentResponse"
    /// </summary>
   [DataContract]
    public class GetProjectsByDocumentResponse : Response
    {
        /// <summary>
        /// Fascicoli in cui il documento è classificato
        /// </summary>
         [DataMember]
        public Domain.Project[] Projects
        {
            get;
            set;
        }
    }
}