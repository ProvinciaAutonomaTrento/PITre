using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.FascicolaDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "FascicolaDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/FascicolaDocumentoResponse")]
    public class FascicolaDocumentoResponse : Response
    {
    }
}