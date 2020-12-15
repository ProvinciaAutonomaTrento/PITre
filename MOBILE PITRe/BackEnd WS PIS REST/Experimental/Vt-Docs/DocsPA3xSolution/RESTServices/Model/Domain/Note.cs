using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class Note
    {
        /// <summary>
        /// System id della nota
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione della nota
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Utente proprietario della nota
        /// </summary>
        public User User
        {
            get;
            set;
        }
    }
}