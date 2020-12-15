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
    public class GetDettaglioElementoRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'elemento da estrarre
        /// </summary>
        public string Id
        {
            get;
            set;
        }
    }
}
