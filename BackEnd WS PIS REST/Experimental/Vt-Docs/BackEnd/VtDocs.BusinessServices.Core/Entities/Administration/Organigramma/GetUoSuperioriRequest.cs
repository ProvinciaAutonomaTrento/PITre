using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetUOSuperioriRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'utente
        /// </summary>
        public string User
        {
            get;
            set;
        }

        /// <summary>
        /// Codice della qualifica per discriminare l'UO
        /// </summary>
        public string CodiceQualifica
        {
            get;
            set;
        }
    }
}
