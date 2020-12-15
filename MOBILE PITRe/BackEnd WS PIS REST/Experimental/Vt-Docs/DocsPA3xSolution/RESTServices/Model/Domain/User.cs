using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class User
    {
        /// <summary>
        /// Id dell'utente (idPeople)
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        ///Codice dell'utente
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione con nome e cognome
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Nome 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome
        /// </summary>
        public string Surname
        {
            get;
            set;
        }

        /// <summary>
        /// Codice fiscale/p.iva
        /// </summary>
        public string NationalIdentificationNumber
        {
            get;
            set;
        }
    }
}