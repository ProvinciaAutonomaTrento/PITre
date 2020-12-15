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
    public class GetUtentiConQualificaResponse : Response
    {
        /// <summary>
        /// Lista degli utenti che hanno impostata in organigramma la qualifica richiesta 
        /// </summary>
        public List<DocsPaVO.Qualifica.PeopleQualifica> Utenti
        {
            get;
            set;
        }
    }
}
