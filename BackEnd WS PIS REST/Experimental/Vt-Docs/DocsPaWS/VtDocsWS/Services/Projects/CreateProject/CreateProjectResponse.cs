using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Projects.CreateProject
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "CreateProjectResponse"
    /// </summary>
    //[DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class CreateProjectResponse : Response
    {
        /// <summary>
        /// Fascicolo creato
        /// </summary>
         [DataMember]
        public Domain.Project Project
        {
            get;
            set;
        }
    }
}