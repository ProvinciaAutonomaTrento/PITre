using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    [Serializable()]
    public class GetUtenteRequest : Request
    {
        /// <summary>
        /// UserId dell'utente
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Matricola utente
        /// </summary>
        public string Matricola
        {
            get;
            set;
        }

        /// <summary>
        /// System id nella tabella People
        /// </summary>
        public int IdPeople
        {
            get;
            set;
        }

    }
}
