using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.GetSupporti
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "GetSupporti"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/GetSupportiRequest")]
    public class GetSupportiRequest : Request
    {
        /// <summary>
        /// Identificativo univoco dell'istanza di conservazione per cui è necessario scaricare i supporti
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }
    }
}