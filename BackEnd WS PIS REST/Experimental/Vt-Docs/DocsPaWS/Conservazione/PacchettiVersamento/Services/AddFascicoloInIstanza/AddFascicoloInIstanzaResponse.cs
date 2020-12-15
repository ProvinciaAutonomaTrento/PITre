using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.AddFascicoloInIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "AddFascicoloInIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/AddFascicoloInIstanzaResponse")]
    public class AddFascicoloInIstanzaResponse : Response
    {
    }
}