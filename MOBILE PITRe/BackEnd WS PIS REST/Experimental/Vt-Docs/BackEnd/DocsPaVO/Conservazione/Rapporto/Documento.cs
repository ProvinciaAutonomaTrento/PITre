using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.Rapporto
{
    [Serializable]
    public class Documento
    {
        public string URNIndiceSIP { get; set; }

        public string HashIndiceSIP { get; set; }

        public string AlgoritmoHashIndiceSIP { get; set; }

        public string EncodingHashIndiceSIP { get; set; }

        public string DataVersamento { get; set; }

        public string IDDocumento { get; set; }

        public string OggettoDocumento { get; set; }

        public Tipologia Tipologia { get; set; }

        public string FirmatoDigitalmente { get; set; }

        public File File { get; set; }

        public Allegato[] Allegati { get; set; }
    }
}
