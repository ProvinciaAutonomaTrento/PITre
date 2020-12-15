using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetResponsabiliResponse : Response
    {
        /// <summary>
        /// Lista dei responsabili per un utente in organigramma
        /// </summary>
        public UtenteResponsabile[] Utenti
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Reppresenta un utente responsabile 
    /// </summary>
    [Serializable()]
    public class UtenteResponsabile
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Matricola
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Cognome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string UO
        {
            get;
            set;
        }
    }
}
