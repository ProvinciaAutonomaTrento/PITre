using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Conservazione.PARER.BigFiles
{
    [XmlRoot(ElementName = "invioOggettoAsincrono")]
    public class InvioOggettoAsincrono
    {
        [XmlElement(ElementName = "nmAmbiente")]
        public string Ambiente { get; set; }

        [XmlElement(ElementName = "nmVersatore")]
        public string Versatore { get; set; }

        [XmlElement(ElementName = "cdKeyObject")]
        public string Chiave { get; set; }

        [XmlElement(ElementName = "nmTipoObject")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "flFileCifrato")]
        public bool Cifrato { get; set; }

        [XmlElement(ElementName = "flForzaWarning")]
        public bool ForzaWarning { get; set; }

        [XmlElement(ElementName = "flForzaAccettazione")]
        public bool ForzaAccettazione { get; set; }

    }
}
