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
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/CreateFascicoloRequest")]
    public class CreateFascicoloRequest : Request
    {
        /// <summary>
        /// Codice del nodo di titolario in cui inserire il fascicolo
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
        /// Dati del fascicolo di conservazione da creare
        /// </summary>
        public Dominio.Fascicolo Fascicolo
        {
            get;
            set;
        }
    }
}