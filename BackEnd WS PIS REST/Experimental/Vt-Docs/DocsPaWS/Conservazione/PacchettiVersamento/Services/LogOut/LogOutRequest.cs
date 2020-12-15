using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.LogOut
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "LogOut"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/LogOutRequest")]
    public class LogOutRequest : Request
    {
    }
}