using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.AddDocumentoInIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "AddDocumentoInIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/AddDocumentoInIstanzaResponse")]
    public class AddDocumentoInIstanzaResponse : Response
    {
        /// <summary>
        /// System id dell'istanza in cui è stato inserito il documento
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }
    }
}