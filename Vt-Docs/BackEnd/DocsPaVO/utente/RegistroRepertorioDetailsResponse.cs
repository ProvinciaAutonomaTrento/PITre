using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Response per il metodo web di recupero del dettaglio di un registro di repertorio
    /// </summary>
    [Serializable()]
    public class RegistroRepertorioDetailsResponse
    {
        /// <summary>
        /// Dettaglio del registro di repertorio
        /// </summary>
        public RegistroRepertorio Registry { get; set; }
    }
}
