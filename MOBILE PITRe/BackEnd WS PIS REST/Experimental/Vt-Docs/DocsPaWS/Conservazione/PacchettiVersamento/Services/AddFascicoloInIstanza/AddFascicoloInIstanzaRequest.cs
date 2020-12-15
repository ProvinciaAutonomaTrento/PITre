using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.AddFascicoloInIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "AddFascicoloInIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/AddFascicoloInIstanzaRequest")]
    public class AddFascicoloInIstanzaRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del fascicolo procedimentale da inserire nell'istanza di conservazione corrente
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        public string IdFascicoloProcedimentale
        {
            get;
            set;
        }
    }
}