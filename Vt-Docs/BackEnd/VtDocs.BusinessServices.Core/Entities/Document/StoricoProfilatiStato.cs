using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    [Serializable()]
    public class StoricoProfilatiStato
    {
        /// <summary>
        /// Identificativo utente
        /// </summary>
        public string Utente
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Ruolo
        {
            get;
            set;
        }

        /// <summary>
        /// Data della modifica del campo profilato
        /// </summary>
        public DateTime Data
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del campo profilato
        /// </summary>
        public string Campo
        {
            get;
            set;
        }

        /// <summary>
        /// Valore precedente del campo profilato 
        /// </summary>
        public string VecchioValore
        {
            get;
            set;
        }
    }
}
