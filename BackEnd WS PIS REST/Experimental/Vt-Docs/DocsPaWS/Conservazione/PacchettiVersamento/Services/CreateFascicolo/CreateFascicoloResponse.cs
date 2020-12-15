using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.CreateFascicolo
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "CreateFascicoloRequest"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/CreateFascicoloResponse")]
    public class CreateFascicoloResponse : Response
    {
        /// <summary>
        /// Dati del fascicolo di conservazione creato
        /// </summary>
        public Dominio.Fascicolo Fascicolo
        {
            get;
            set;
        }
    }
}