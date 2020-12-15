using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetProfili
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetProfili"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetProfiliResponse")]
    public class GetProfiliResponse : Response
    {
        /// <summary>
        /// Lista dei profili restituiti
        /// </summary>
        public Dominio.Profilo[] Profili
        {
            get;
            set;
        }
    }
}