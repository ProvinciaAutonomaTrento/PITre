using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    /// <summary>
    /// Classe per la rappresentazione della request per il servizio di salvataggio delle modifiche apportate ad un registro
    /// di reprtorio
    /// </summary>
    [Serializable()]
    public class SaveRegistroRepertorioSettingsRequest
    {
        public String CounterId { get; set; }

        public RegistroRepertorio.SettingsType Settings { get; set; }

        public RegistroRepertorioSingleSettings SettingsToSave { get; set; }

        public RegistroRepertorio.TipologyKind TipologyKind { get; set; }

        public String IdAdmin { get; set; }

    }
}
