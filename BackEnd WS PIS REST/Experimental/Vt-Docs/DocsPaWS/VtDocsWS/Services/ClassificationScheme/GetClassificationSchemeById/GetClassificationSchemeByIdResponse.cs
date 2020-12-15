using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.ClassificationScheme.GetClassificationSchemeById
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetClassificationSchemeByIdResponse"
    /// </summary>
    [DataContract]
    public class GetClassificationSchemeByIdResponse : Response
    {
        /// <summary>
        /// Titolario richiesto
        /// </summary>
        [DataMember]
        public Domain.ClassificationScheme ClassificationScheme
        {
            get;
            set;
        }
    }
}