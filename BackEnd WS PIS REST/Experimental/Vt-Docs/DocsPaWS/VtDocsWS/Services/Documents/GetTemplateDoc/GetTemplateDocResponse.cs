using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetTemplateDoc
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetTemplateDocResponse"
    /// </summary>
    [DataContract]
    public class GetTemplateDocResponse : Response
    {
        /// <summary>
        /// Tipologia di documento richiesta
        /// </summary>
        [DataMember]
        public Domain.Template Template
        {
            get;
            set;
        }
    }
}