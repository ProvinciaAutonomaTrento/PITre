using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Oggetto request relativo al servizio di reperimento della todolist
    /// </summary>
    [Serializable()]
    public class GetMyToDoListRequest : Request
    {
        /// <summary>
        /// Contesto di paginazione di input
        /// </summary>
        public PagingContext PagingContext
        {
            get;
            set;
        }

        /// <summary>
        /// Filtro per ragione
        /// </summary>
        public string Ragione
        {
            get;
            set;
        }
        /// <summary>
        /// Filtro per tipo
        /// </summary>
        public string TipoDocumento
        {
            get;
            set;
        }
    }
}
