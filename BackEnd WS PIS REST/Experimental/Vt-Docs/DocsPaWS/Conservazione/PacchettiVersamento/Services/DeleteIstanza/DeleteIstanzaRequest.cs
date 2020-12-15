using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.DeleteIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "DeleteIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/DeleteIstanzaRequest")]
    public class DeleteIstanzaRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'istanza di conservazione da rimuovere
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }
    }
}