using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.InviaCentroServizi
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "InviaCentroServizi"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/InviaCentroServiziResponse")]
    public class InviaCentroServiziResponse : Response
    {
    }
}