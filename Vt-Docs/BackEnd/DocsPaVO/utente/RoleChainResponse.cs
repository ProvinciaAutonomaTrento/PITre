using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Response relativa al servizio per il recupero della catena di storicizzazione del ruolo
    /// </summary>
    [Serializable()]
    public class RoleChainResponse
    {
        public List<String> RoleChain { get; set; }

    }

    /// <summary>
    /// Request relativa al servizio per il recupero della catena di storicizzazione del ruolo
    /// </summary>
    [Serializable()]
    public class RoleChainRequest
    {
        public String IdCorrGlobRole { get; set; }

    }

}
