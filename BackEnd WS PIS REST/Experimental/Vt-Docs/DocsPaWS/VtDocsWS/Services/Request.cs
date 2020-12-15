using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services
{
    /// <summary>
    /// Classe per l'incapsulamento dei pacchetti di richiesta per i servizi dei Pis
    /// </summary>
    //[DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    [DataContract]
    public class Request
    {
        /// <summary>
        /// Nome utente per l'autenticazione
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Codice amministrazione
        /// </summary>
        [DataMember]
        public string CodeAdm
        {
            get;
            set;
        }

        /// <summary>
        /// Token di autenticazione
        /// </summary>
        /// <remarks>
        /// Il token attesta che l'utente si è realmente autenticato al sistema per poter accedere ai servizi
        /// </remarks>
        [DataMember]
        public string AuthenticationToken
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del ruolo (opzionale)
        /// </summary>
        [DataMember]
        public string CodeRoleLogin
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'applicazione (opzionale)
        /// </summary>
        [DataMember]
        public virtual string CodeApplication
        {
            get;
            set;
        }
    }
}