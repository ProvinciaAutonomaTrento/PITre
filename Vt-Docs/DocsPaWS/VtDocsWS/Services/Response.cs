using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services
{
    /// <summary>
    /// Classe per l'incapsulamento dei pacchetti di risposta di tutti i servizi Pis
    /// </summary>
    //[DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    [DataContract]
    public class Response
    {
        /// <summary>
        /// Indica l'esito dell'invocazione al servizio
        /// </summary>
        //[DataMember]
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// Eventuale errore riscontrato nell'invocazione al servizio 
        /// </summary>
        /// <remarks>
        /// Valorizzato solo se Success è false
        /// </remarks>
        //[DataMember]
        public ResponseError Error
        {
            get;
            set;
        }
    }
}