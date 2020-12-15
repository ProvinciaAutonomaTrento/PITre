using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.ClassificaDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "ClassificaDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/ClassificaDocumentoResponse")]
    public class ClassificaDocumentoResponse : Response
    {
    }
}