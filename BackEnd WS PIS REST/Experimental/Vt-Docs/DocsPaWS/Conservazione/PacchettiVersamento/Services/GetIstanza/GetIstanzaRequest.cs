using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetIstanzaRequest")]
    public class GetIstanzaRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'istanza di conservazione da reperire
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }
    }
}