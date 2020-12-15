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
    public class DeleteElementoRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'elemento da elminiare in rubrica
        /// </summary>
        public string Id
        {
            get;
            set;
        }
    }
}
