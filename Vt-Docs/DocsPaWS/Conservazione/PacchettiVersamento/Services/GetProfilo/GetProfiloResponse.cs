using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetProfilo
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "GetProfilo"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetProfiloResponse")]
    public class GetProfiloResponse : Response
    {
        /// <summary>
        /// Dati del profilo dinamico
        /// </summary>
        public Dominio.DettaglioProfilo Profilo
        {
            get;
            set;
        }
    }
}