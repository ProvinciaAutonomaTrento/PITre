using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "GetDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetDocumentoRequest")]
    public class GetDocumentoRequest : Request
    {
        /// <summary>
        /// Identificativo dell'istanza di conservazione in cui è contenuto il documento da estrarre
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del documento da reperire
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// Se true, saranno restituiti insieme ai metadati anche il contenuto del file
        /// </summary>
        public bool GetFile
        {
            get;
            set;
        }
    }
}