using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.Rapporto
{
    [Serializable]
    public class Allegato
    {
        public string IDDocumento { get; set; }

        public string Descrizione { get; set; }

        public string FirmatoDigitalmente { get; set; }

        public File File { get; set; }
    }
}
