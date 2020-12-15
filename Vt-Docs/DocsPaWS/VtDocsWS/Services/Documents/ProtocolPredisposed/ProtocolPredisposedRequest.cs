using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Documents.ProtocolPredisposed
{
    [DataContract]
    public class ProtocolPredisposedRequest : Request
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
        [DataMember]
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        [DataMember]
        public string CodeRF
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        [DataMember]
        public string IdDocument
        {
            get;
            set;
        }        
    }
}