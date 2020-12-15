using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.DeleteDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "DeleteDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/DeleteDocumentoResponse")]
    public class DeleteDocumentoResponse : Response
    {
    }
}