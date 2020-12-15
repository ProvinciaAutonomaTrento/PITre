using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetIstanzaDaInviare
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "GetIstanzaDaInviare"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetIstanzaDaInviareRequest")]
    public class GetIstanzaDaInviareRequest : Request
    {
    }
}