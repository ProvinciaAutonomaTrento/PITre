using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.RegistroAccessi.FOIA
{
    [Serializable]
    [XmlType("richiestadiaccesso")]
    public class RichiestaDiAccesso
    {
        [XmlElement(ElementName = "datacreazione")]
        public String DataCreazione { get; set; }

        [XmlElement(ElementName = "statorichiesta")]
        public StatoType StatoRichiesta { get; set; }

        [XmlElement(ElementName = "esito")]
        public Esito Esito { get; set; }

        [XmlIgnore]
        public String DataNotificaRiesame { get; set; }

        [XmlElement(ElementName = "esitoRiesame")]
        public Esito EsitoRiesame { get; set; }

        [XmlIgnore]
        public String DataNotificaRicorso { get; set; }

        [XmlElement(ElementName = "esitoTAR")]
        public Esito EsitoRicorso { get; set; }

        [XmlElement (ElementName = "presenzacontrointeressati")]
        public Boolean PresenzaControinteressati { get; set; }

        [XmlElement(ElementName = "numeroControinteressati")]
        public String NumeroControinteressati { get; set; }

        [XmlElement(ElementName = "oggetto")]
        public Oggetto Oggetto { get; set; }
    }

    public enum StatoType
    {
        [XmlEnum(Name = "In Corso")]
        InCorso,

        [XmlEnum(Name = "Sospesa")]
        Sospesa,

        [XmlEnum(Name = "Riesame")]
        Riesame,

        [XmlEnum(Name = "Ricorso TAR")]
        RicorsoTAR,

        [XmlEnum(Name = "Ricorso Consiglio di Stato")]
        RicorsoConsiglioDiStato,

        [XmlEnum(Name = "Chiusa")]
        Chiusa
    }

}
