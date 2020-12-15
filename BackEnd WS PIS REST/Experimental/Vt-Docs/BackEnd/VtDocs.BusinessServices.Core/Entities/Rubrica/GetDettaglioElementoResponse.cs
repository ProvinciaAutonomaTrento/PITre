using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Rubrica
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetDettaglioElementoResponse : Response
    {
        /// <summary>
        /// Dettagli del corrispondenterichiesto
        /// </summary>
        public DettaglioElementoRubrica Elemento
        {
            get;
            set;
        }
    }
}
