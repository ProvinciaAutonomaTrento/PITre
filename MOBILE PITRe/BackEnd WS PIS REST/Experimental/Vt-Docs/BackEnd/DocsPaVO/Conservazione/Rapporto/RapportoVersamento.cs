using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.Rapporto
{
    [Serializable]
    public class RapportoVersamento
    {
        public string Versione { get; set; }

        public string URNRapportoVersamento { get; set; }

        public string DataRapportoVersamento { get; set; }

        public string EsitoGenerale { get; set; }

        public Versatore Versatore { get; set; }

        public Documento[] SIP { get; set; }
    }
}
