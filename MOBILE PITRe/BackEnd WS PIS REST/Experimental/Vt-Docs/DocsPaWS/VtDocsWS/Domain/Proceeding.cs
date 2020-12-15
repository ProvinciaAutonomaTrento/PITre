using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Procedimento
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Proceeding
    {
        /// <summary>
        /// Utente proprietario del procedimento
        /// </summary>
        [DataMember]
        public Domain.Correspondent User
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione istanza
        /// </summary>
        [DataMember]
        public String Description
        {
            get;
            set;
        }

        /// <summary>
        /// Codice di classifica del fascicolo da creare
        /// </summary>
        [DataMember]
        public String FolderCode
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia fascicolo
        /// </summary>
        [DataMember]
        public String FolderTypology
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia documento
        /// </summary>
        [DataMember]
        public String DocumentTypology
        {
            get;
            set;
        }

        /// <summary>
        /// Oggetto del documento
        /// </summary>
        [DataMember]
        public String DocumentObject
        {
            get;
            set;
        }
    }
}