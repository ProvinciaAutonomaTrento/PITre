using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.InsertDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "InsertIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/InsertDocumentoResponse")]
    public class InsertDocumentoResponse : Response
    {
        /// <summary>
        /// Dati del documento inserito in un'istanza di conservazione
        /// </summary>
        public Dominio.Documento Documento
        {
            get;
            set;
        }
    }
}