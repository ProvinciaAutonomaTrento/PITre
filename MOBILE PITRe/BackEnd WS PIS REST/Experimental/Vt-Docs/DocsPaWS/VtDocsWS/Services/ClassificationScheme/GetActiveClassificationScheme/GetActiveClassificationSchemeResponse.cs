using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.ClassificationScheme.GetActiveClassificationScheme
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetActiveClassificationSchemeResponse"
    /// </summary>
    [DataContract]
    public class GetActiveClassificationSchemeResponse : Response
    {
        /// <summary>
        /// Titolario attivo
        /// </summary>
        [DataMember]
        public Domain.ClassificationScheme ClassificationScheme
        {
            get;
            set;
        }

    }
}