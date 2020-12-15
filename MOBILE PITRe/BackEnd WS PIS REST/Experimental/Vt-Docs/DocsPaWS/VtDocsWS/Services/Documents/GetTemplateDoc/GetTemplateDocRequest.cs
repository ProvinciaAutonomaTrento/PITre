using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetTemplateDoc
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetTemplateDocRequest"
    /// </summary>
    [DataContract]
    public class GetTemplateDocRequest : Request
    {
        /// <summary>
        /// Descrizione della tipologia del documento
        /// </summary>
         [DataMember]
        public string DescriptionTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Id della tipologia di documento
        /// </summary>
         [DataMember]
        public string IdTemplate
        {
            get;
            set;
        }
    }
}