using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaVO.FriendApplication
{
    public class FriendApplication
    {
        public string codiceApplicazione = string.Empty;
        public string idRegistro = string.Empty;
        public string codiceRegistro = string.Empty;
        public string idPeopleFactory = string.Empty;
        public string idGruppoFactory = string.Empty;
        public DocsPaVO.utente.Registro registro = null;
        public DocsPaVO.utente.Utente utente = null;
        public DocsPaVO.utente.Ruolo ruolo = null;
    }
}
