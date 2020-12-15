using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    [Serializable()]
    public class GetUtenteResponse : Response
    {
        //Oggetto utente
        public DocsPaVO.utente.Utente Utente
        {
            get;
            set;
        }
    }
}
