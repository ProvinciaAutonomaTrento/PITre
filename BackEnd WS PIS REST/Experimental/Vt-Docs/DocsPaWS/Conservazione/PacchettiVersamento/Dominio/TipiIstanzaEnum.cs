using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Possibili tipologie di un'istanza di conservazione
    /// </summary>
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/TipiIstanzaEnum")]
    public enum TipiIstanzaEnum
    {
        Consolidata,
        NonConsolidata,
        Esibizione,
        Interna,
    }
}