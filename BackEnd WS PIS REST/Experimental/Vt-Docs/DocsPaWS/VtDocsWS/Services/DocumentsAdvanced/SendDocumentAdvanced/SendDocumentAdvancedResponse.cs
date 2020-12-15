using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.DocumentsAdvanced.SendDocumentAdvanced
{
    [DataContract]
    public class SendDocumentAdvancedResponse : Response
    {
        [DataMember]
        public string ResultMessage { get; set; }

        /// <summary>
        /// Placeholder
        /// </summary>
        //[DataMember]
        //public string[] ResultRecipient { get; set; }

        [DataMember]
        public Domain.SendingResult[] SendingResults { get; set; }
    }
}