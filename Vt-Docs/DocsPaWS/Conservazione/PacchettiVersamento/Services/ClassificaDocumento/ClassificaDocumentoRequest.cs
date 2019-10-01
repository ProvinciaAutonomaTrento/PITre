using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.ClassificaDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "ClassificaDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/ClassificaDocumentoRequest")]
    public class ClassificaDocumentoRequest : Request
    {
        /// <summary>
        /// Codice del nodo di titolario in cui classificare il documento
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        public string CodiceNodoTitolario
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del documento da classificare
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