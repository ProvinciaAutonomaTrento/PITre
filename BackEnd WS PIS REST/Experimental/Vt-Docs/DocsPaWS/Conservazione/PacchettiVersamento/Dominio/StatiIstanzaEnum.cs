using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Possibili stati cui può venirsi a trovare un'istanza di conservazione
    /// </summary>
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/StatiIstanzaEnum")]
    public enum StatiIstanzaEnum
    {
        [Description(Value = "N")]
        DaInviare,

        [Description(Value = "I")]
        Inviata,

        [Description(Value = "R")]
        Rifiutata,

        [Description(Value = "L")]
        InLavorazione,

        [Description(Value = "F")]
        Firmata,

        [Description(Value = "V")]
        Conservata,

        [Description(Value = "C")]
        Chiusa,

        [Description(Value = "E")]
        Errore,
    }
}