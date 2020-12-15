using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetSupporti
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetSupporti"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetSupportiResponse")]
    public class GetSupportiResponse : Response
    {
        /// <summary>
        /// Lista dei supporti dell'istanza di conservazione
        /// </summary>
        public Dominio.Supporto[] Supporti
        {
            get;
            set;
        }
    }
}