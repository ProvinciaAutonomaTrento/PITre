using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.DeleteDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "DeleteDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/DeleteDocumentoRequest")]
    public class DeleteDocumentoRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'istanza contenente il documento da rimuovere
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del documento da rimuovere
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }
    }
}