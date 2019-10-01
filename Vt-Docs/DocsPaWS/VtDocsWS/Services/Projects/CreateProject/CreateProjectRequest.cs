using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.CreateProject
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "CreateProjectRequest"
    /// </summary>
    //[DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class CreateProjectRequest : Request
    {
        /// <summary>
        /// Fascicolo da creare
        /// </summary>
         [DataMember]
        public Domain.Project Project
        {
            get;
            set;
        }
    }
}