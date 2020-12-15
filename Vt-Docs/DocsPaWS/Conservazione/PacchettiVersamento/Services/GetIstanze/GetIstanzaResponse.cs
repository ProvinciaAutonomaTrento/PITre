using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetIstanze
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetIstanzeResponse")]
    public class GetIstanzeResponse : Response
    {
        /// <summary>
        /// Istanze di conservazione restituite
        /// </summary>
        public Dominio.Istanza[] Istanze
        {
            get;
            set;
        }
    }
}