using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Classe response per il metodo "GetFascicolazioniDocumento"
    /// </summary>
    [Serializable()]
    public class GetFascicoliResponse : Response
    {
        /// <summary>
        /// Lista dei fascicoli in cui il documento è contenuto
        /// </summary>
        public DocsPaVO.fascicolazione.Fascicolo[] Fascicoli
        {
            get;
            set;
        }
    }
}
