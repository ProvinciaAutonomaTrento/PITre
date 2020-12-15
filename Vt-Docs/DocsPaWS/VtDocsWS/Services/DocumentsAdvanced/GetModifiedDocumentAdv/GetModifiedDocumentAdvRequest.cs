using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VtDocsWS.Services.DocumentsAdvanced.GetModifiedDocumentAdv
{
    [DataContract]
    public class GetModifiedDocumentAdvRequest : Request
    {
        /// <summary>
        /// Limite inferiore della data, formato dd/mm/yyyy hh:mi:ss
        /// </summary>
        [DataMember]
        public string FromDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// Stringa di eventi separati da punto e virgola
        /// </summary>
        [DataMember]
        public string EventString
        {
            get;
            set;
        }


    }
}