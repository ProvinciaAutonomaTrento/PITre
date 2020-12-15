using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.DocumentsAdvanced.GetModifiedDocumentAdv
{
    [DataContract]
    public class GetModifiedDocumentAdvResponse : Response
    {
        /// <summary>
        /// Dettaglio del documento creato
        /// </summary>
        [DataMember]
        public string ResultMessage { get; set; }

        [DataMember]
        public Domain.ModifiedDocument[] ModifiedDocuments { get; set; }
        //[DataMember]
        //public System.Collections.ArrayList ModifiedDocuments { get; set; }
}
}