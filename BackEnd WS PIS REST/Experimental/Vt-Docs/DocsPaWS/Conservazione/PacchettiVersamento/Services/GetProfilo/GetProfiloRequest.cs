using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetProfilo
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetProfilo"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetProfiloRequest")]
    public class GetProfiloRequest : Request
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

        /// <summary>
        /// Identificativo univoco del profilo da reperire
        /// </summary>
        public string IdProfilo
        {
            get;
            set;
        }
    }
}