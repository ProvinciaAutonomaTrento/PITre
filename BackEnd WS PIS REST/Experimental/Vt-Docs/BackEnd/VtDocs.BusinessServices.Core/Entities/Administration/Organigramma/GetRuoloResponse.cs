using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Oggetto response del servizio di reperimento di un ruolo in organigramma
    /// </summary>
    [Serializable()]
    public class GetRuoloResponse : Response
    {
        /// <summary>
        /// Ruolo richiesto
        /// </summary>
        public DocsPaVO.amministrazione.OrgRuolo Ruolo
        {
            get;
            set;
        }
    }
}
