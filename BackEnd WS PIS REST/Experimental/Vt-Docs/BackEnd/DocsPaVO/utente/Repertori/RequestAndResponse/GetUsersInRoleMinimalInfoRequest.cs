using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// Request relativa al servizio di recupero delle imformazioni di base sugli utenti di un ruolo
    /// </summary>
    public class GetUsersInRoleMinimalInfoRequest
    {
        public String RoleId { get; set; }
    }
}
