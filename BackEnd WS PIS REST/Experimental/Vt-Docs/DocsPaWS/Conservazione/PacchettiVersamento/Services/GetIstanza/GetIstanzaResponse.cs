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
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetIstanzaResponse")]
    public class GetIstanzaResponse : Response
    {
        /// <summary>
        /// Dati dell'istanza di conservazione
        /// </summary>
        public Dominio.Istanza Istanza
        {
            get;
            set;
        }
    }
}