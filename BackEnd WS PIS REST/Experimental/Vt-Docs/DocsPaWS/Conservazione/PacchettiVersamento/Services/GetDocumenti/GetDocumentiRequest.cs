using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetDocumenti
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "GetDocumenti"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetDocumentiRequest")]
    public class GetDocumentiRequest : Request
    {
        /// <summary>
        /// Identificativo dell'istanza di conservazione in cui sono contenuti i documenti da estrarre
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }
    }
}