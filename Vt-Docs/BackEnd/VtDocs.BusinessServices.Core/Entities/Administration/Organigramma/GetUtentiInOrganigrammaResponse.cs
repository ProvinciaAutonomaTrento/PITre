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
    public class GetUtentiInOrganigrammaResponse : Response
    {
        /// <summary>
        /// 
        /// </summary>
        public List<UtenteInOrganigramma> Utenti
        {
            get;
            set;
        }

        [Serializable()]
        public class UtenteInOrganigramma
        {
            /// <summary>
            /// 
            /// </summary>
            public int Id
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string UserName
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
            public string Denominazione
            {
                get;
                set;
            }

            /// <summary>
            /// Indica se l'utente è censito in un ruolo responsabile
            /// </summary>
            public bool Responsabile
            {
                get;
                set;
            }

            /// <summary>
            /// Indirizzo mail
            /// </summary>
            public string MailAddress
            {
                get;
                set;
            }
        }
    }
}
