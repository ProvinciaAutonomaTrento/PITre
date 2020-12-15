using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// Response per il metodo web di recupero delle impostazioni relative ad un registro di repertorio
    /// </summary>
    [Serializable()]
    public class RegistroRepertorioSettingsResponse
    {
        /// <summary>
        /// Registro di repertorio con le impostazioni relative alle stampe
        /// </summary>
        public RegistroRepertorioSingleSettings RegistroRepertorioSingleSettings { get; set; }
    }
}
