using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.FascicolaDocumento
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta forniti al servizio di "FascicolaDocumento"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/FascicolaDocumentoRequest")]
    public class FascicolaDocumentoRequest : Request
    {
        /// <summary>
        /// Identificativo univoco del fascicolo procedimentale in cui fascicolare il documento
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        public string IdFascicoloProcedimentale
        {
            get;
            set;
        }

        /// <summary>
        /// Eventuale percorso del sottofascicolo in cui inserire il documento
        /// </summary>
        /// <remarks>
        /// I nomi dei sottofascicoli devono essere separati dal carattere /
        /// </remarks>
        public string PathSottofascicolo
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del documento da fascicolare
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