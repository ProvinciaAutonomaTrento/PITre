using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services
{
    /// <summary>
    /// Classe per l'incapsulamento degli eventuali errori riscontrati
    /// nell'esecuzione del servizio
    /// </summary>
    [DataContract]
    public class ResponseError
    {
        /// <summary>
        /// Codice dell'errore
        /// </summary>
        [DataMember]
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dell'errore
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }
    }
}