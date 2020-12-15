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
    public class GetListUOByQualificheUtenteResponse : Response
    {
        /// <summary>
        /// Lista delle UO contenenti almeno un utente con la qualifica richiesta
        /// </summary>
        public List<UOByQualifica> UOs
        {
            get;
            set;
        }
    }
}
