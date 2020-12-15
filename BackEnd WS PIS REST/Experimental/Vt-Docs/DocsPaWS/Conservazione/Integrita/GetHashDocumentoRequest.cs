using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.Integrita
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/CreateFascicoloRequest")]
    public class GetHashDocumentoRequest
    {
        /// <summary>
        /// Id utente centro servizi
        /// </summary>
        public string IdPeople
        {
            get;
            set;
        }

        /// <summary>
        /// Id documento da verificare
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }
    }
}