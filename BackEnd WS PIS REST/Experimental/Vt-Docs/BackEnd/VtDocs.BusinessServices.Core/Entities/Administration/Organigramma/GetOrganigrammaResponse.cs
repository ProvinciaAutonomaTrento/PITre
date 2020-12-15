using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Oggetto response del servizio di reperimento delle unità organizzative
    /// </summary>
    [Serializable()]
    public class GetOrganigrammaResponse : Response
    {
        /// <summary>
        /// Lista delle unità organizzative restituite
        /// </summary>
        public UO UnitaOrganizzativa
        {
            get;
            set;
        }
    }
}
