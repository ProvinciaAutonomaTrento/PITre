using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.InviaCentroServizi
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "InviaCentroServizi"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/InviaCentroServiziRequest")]
    public class InviaCentroServiziRequest : Request
    {
        /// <summary>
        /// Descrizione da inviare a Centro Servizi
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// Eventuali note aggiuntive per il Centro Servizi
        /// </summary>
        /// <remarks>
        /// Dato facoltativo
        /// </remarks>
        public string NoteDiInvio
        {
            get;
            set;
        }

        /// <summary>
        /// Tipologia dell'istanza di conservazione richiesta
        /// </summary>
        public Dominio.TipiIstanzaEnum Tipo
        {
            get;
            set;
        }
    }
}