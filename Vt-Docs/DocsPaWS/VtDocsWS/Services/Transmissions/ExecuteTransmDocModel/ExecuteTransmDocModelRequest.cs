using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.ExecuteTransmDocModel
{
     /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "ExecuteTransmDocModelRequest"
    /// </summary>
    [DataContract]
    public class ExecuteTransmDocModelRequest : Request
    {
        /// <summary>
        /// Codice del modello di trasmissione
        /// </summary>
         [DataMember]
        public string IdModel
        {
            get;
            set;
        }

        /// <summary>
        /// Id del documento
        /// </summary>
         [DataMember]
        public string DocumentId
        {
            get;
            set;
        }
    }
}