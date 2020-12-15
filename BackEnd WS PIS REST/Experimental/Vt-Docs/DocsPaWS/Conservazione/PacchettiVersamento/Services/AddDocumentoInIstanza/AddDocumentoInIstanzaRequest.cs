using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.AddDocumentoInIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "AddDocumentoInIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/AddDocumentoInIstanzaRequest")]
    public class AddDocumentoInIstanzaRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del documento da inserire nell'istanza di conservazione corrente
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        public string IdDocumento
        {
            get;
            set;
        }
    }
}