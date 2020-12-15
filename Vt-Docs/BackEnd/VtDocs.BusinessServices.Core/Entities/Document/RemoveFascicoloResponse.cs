using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Classe response per la rimozione del documento da un fascicolo
    /// </summary>
    [Serializable()]
    public class RemoveFascicoloResponse : Response
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
