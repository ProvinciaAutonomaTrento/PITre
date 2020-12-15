using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Proceedings.StartProceeding
{
    [DataContract]
    public class StartProceedingRequest : Request
    {

        /// <summary>
        /// Procedimento da avviare
        /// </summary>
        [DataMember]
        public Domain.Proceeding Proceeding { get; set; }

    }
}