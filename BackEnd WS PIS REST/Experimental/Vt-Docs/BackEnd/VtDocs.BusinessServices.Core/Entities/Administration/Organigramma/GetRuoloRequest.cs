using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Oggetto request relativo al servizio di reperimento dei dati di un ruolo in organigramma
    /// </summary>
    [Serializable()]
    public class GetRuoloRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del ruolo
        /// </summary>
        public string Id
        {
            get;
            set;
        }
    }
}
