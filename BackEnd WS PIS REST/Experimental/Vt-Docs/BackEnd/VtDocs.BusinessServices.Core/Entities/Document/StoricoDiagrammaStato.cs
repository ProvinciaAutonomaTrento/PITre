using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    [Serializable()]
    public class StoricoDiagrammaStato
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
        /// Data del cambio di stato
        /// </summary>
        public DateTime Data
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del vecchio stato
        /// </summary>
        public string VecchioStato
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del nuovo stato
        /// </summary>
        public string NuovoStato
        {
            get;
            set;
        }
    }
}
