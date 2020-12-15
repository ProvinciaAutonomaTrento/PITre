using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// Classe request per il metodo "GetFascicolazioniDocumento"
    /// </summary>
    [Serializable()]
    public class GetFascicoliRequest : Request
    {
        /// <summary>
        /// Id del documento per cui è necessario reperire i fascicoli che lo contengono
        /// </summary>
        public string IdProfile
        {
            get;
            set;
        }
    }
}
