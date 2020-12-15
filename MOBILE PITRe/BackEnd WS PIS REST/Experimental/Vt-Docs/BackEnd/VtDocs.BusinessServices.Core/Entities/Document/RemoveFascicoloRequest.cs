using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto request relativo al metodo di rimozione del documento in un fascicolo
    /// </summary>
    [Serializable()]
    public class RemoveFascicoloRequest : Request
    {
        /// <summary>
        /// Id del documento da inserire nel fascicolo
        /// </summary>
        public string IdProfile
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascicolo in cui inserire il documento
        /// </summary>
        public string IdFascicolo
        {
            get;
            set;
        }

        /// <summary>
        /// Se true, indica di restituire la lista dei fascicoli aggiornata dopo l'inserimento del documento
        /// </summary>
        public bool GetFascicoli
        {
            get;
            set;
        }
    }
}
