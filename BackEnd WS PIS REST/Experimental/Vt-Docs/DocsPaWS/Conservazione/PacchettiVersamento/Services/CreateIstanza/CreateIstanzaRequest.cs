using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.CreateIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "CreateIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/CreateIstanzaRequest")]
    public class CreateIstanzaRequest : Request
    {
        /// <summary>
        /// Dati dell'istanza di conservazione da creare
        /// </summary>
        public Dominio.Istanza DatiIstanza
        {
            get;
            set;
        }
    }
}