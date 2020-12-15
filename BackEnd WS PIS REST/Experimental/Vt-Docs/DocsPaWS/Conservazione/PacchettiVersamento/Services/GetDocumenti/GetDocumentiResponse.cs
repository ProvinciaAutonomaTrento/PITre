using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetDocumenti
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetDocumenti"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetDocumentiResponse")]
    public class GetDocumentiResponse : Response
    {
        /// <summary>
        /// Lista dei documenti restituiti
        /// </summary>
        public Dominio.Documento[] Documenti
        {
            get;
            set;
        }
    }
}