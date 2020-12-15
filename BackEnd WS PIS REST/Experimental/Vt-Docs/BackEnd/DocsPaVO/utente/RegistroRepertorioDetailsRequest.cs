using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Richiesta per il metodo web di recupero del dettaglio di un registro di repertorio
    /// </summary>
    [Serializable()]
    public class RegistroRepertorioDetailsRequest
    {
        /// <summary>
        /// Id del registro di repertorio
        /// </summary>
        public String RegistryId { get; set; }
    }
}
