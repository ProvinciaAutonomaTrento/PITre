using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetDocumentoResponse")]
    public class GetDocumentoResponse : Response
    {
        /// <summary>
        /// Dati del documento restituito
        /// </summary>
        public Dominio.Documento Documento
        {
            get;
            set;
        }
    }
}