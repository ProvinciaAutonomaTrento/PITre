using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori.RequestAndResponse
{
    [Serializable()]
    public class GetSettingsMinimalInfoRequest
    {
        public String CounterId { get; set; }

        public RegistroRepertorio.TipologyKind TipologyKind { get; set; }

        public string idAmm { get; set; }
    }
}
