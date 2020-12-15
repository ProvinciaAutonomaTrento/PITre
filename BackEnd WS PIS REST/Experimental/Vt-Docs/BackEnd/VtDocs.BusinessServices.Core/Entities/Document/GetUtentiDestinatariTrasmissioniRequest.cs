using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Document
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetUtentiDestinatariTrasmissioniRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del documento
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }
    }
}
