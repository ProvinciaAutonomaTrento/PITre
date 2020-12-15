using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetIstanzaDaInviare
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetIstanzaDaInviare"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetIstanzaDaInviareResponse")]
    public class GetIstanzaDaInviareResponse : Response
    {
        /// <summary>
        /// Dati dell'istanza da inviare in Centro Servizi 
        /// </summary>
        public Dominio.Istanza Istanza
        {
            get;
            set;
        }
    }
}