using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.GetStampAndSignature
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetStampAndSignatureResponse"
    /// </summary>
    [DataContract]
    public class GetStampAndSignatureResponse : Response
    {
        /// <summary>
        /// Dettaglio del timbro e della segnatura
        /// </summary>
        [DataMember]
        public Domain.Stamp Stamp
        {
            get;
            set;
        }
    }
}