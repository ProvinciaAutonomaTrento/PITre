using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// 
    /// </summary>
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/TipiSupportoEnum")]
    public enum TipiSupportoEnum
    {
        Storage,
        SupportoEsterno,
    }
}