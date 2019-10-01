using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Classe response per il metodo "Ricerca"
    /// </summary>
    [Serializable()]
    public class RicercaResponse : Response
    {
        /// <summary>
        /// Contesto di paginazione corrente
        /// </summary>
        public PagingContext PagingContext
        {
            get;
            set;
        }

        /// <summary>
        /// Lista dei risultati di ricerca
        /// </summary>
        public DocsPaVO.Grids.SearchObject[] List
        {
            get;
            set;
        }
    }
}
