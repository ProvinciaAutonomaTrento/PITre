using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// Utente
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class User
    {
        /// <summary>
        /// Id dell'utente (idPeople)
        /// </summary>
        [DataMember]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        ///Codice dell'utente
        /// </summary>
        [DataMember]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione con nome e cognome
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Nome 
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome
        /// </summary>
        [DataMember]
        public string Surname
        {
            get;
            set;
        }

        /// <summary>
        /// Codice fiscale/p.iva
        /// </summary>
        [DataMember]
        public string NationalIdentificationNumber
        {
            get;
            set;
        }

    }
}