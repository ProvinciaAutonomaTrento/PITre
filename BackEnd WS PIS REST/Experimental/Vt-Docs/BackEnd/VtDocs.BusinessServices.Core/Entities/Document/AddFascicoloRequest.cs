using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto request relativo al servizio di inserimento del documento da un fascicolo
    /// </summary>
    [Serializable()]
    public class AddFascicoloRequest : Request
    {
        /// <summary>
        /// Id del documento da rimuovere dal fascicolo
        /// </summary>
        public string IdProfile
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascicolo da cui rimuovere il documento
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
