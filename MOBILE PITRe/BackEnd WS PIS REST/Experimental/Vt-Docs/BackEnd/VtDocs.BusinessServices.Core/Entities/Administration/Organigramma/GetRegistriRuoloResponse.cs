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
    public class GetRegistriRuoloResponse : Response
    {
        /// <summary>
        /// Lista dei registri del ruolo
        /// </summary>
        public DocsPaVO.utente.Registro[] Registri
        {
            get;
            set;
        }
    }
}
