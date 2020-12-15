using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetProfili
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetProfili"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetProfiliRequest")]
    public class GetProfiliRequest : Request
    {
        /// <summary>
        /// Indica quali tipologie documentali restituire
        /// </summary>
        /// <remarks>
        /// Filtro obbligatorio, può essere Documento o Fascicolo
        /// </remarks>
        public Dominio.TipiOggettoProfiloEnum TipoProfilo
        {
            get;
            set;
        }
    }
}