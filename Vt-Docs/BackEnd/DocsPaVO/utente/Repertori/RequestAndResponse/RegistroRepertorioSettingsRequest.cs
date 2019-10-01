using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// Richiesta per il metodo web di recupero del dettaglio di un registro di repertorio
    /// </summary>
    [Serializable()]
    public class RegistroRepertorioSettingsRequest
    {
        /// <summary>
        /// Id del contatore di repertorio
        /// </summary>
        public String CounterId { get; set; }

        public String RegistryId { get; set; }

        public String RfId { get; set; }

        public RegistroRepertorio.TipologyKind TipologyKind { get; set; }


        public RegistroRepertorio.SettingsType SettingsType { get; set; }
    }
}
