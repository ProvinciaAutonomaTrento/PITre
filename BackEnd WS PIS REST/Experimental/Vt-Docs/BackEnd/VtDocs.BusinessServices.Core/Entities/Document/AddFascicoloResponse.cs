using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto response del servizio di inserimento del documento in un fascicolo
    /// </summary>
    [Serializable()]
    public class AddFascicoloResponse : Response
    {
        /// <summary>
        /// Lista dei fascicoli in cui il documento è contenuto
        /// </summary>
        /// <remarks>
        /// La lista è restituita solamente se l'oggetto request prevede il flag "GetFascicoli"
        /// </remarks>
        public DocsPaVO.fascicolazione.Fascicolo[] Fascicoli
        {
            get;
            set;
        }
    }
}
