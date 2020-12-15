using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.EditProject
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "EditProjectRequest"
    /// </summary>
    [DataContract]
    public class EditProjectRequest : Request
    {
        /// <summary>
        /// Fascicolo da modificare
        /// </summary>
         [DataMember]
        public VtDocsWS.Domain.Project Project
        {
            get;
            set;
        }
    }
}