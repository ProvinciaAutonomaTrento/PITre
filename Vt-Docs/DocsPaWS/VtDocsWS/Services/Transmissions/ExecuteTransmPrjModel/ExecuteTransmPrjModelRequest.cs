using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Transmissions.ExecuteTransmPrjModel
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "ExecuteTransmPrjModelRequest"
    /// </summary>
   [DataContract]
    public class ExecuteTransmPrjModelRequest : Request
    {
        /// <summary>
        /// I del modello di trasmissione
        /// </summary>
         [DataMember]
        public string IdModel
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascciolo
        /// </summary>
         [DataMember]
        public string IdProject
        {
            get;
            set;
        }
    }
}