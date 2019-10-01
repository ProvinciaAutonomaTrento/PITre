using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.GetTransmissionModel
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetTransmissionModelRequest"
    /// </summary>
    [DataContract]
    public class GetTransmissionModelRequest : Request
    {
        /// <summary>
        /// Id del modello di trasmissione
        /// </summary>
        [DataMember]
        public string IdModel
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del modello di trasmissione
        /// </summary>
         [DataMember]
        public string CodeModel
        {
            get;
            set;
        }

    }
}