using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Services.CreateIstanza
{
    /// <summary>
    /// Oggetto contenente i dati di risposta restituiti dal servizio di "CreateIstanza"
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/CreateIstanzaResponse")]
    public class CreateIstanzaResponse : Response
    {
        /// <summary>
        /// Dati dell'istanza di conservazione creata
        /// </summary>
        /// <remarks>
        /// Il sistema restituirà valorizzati alcuni attributi quali:
        /// - Id (Contatore autoincrement)
        /// - ModoCreazione (Manuale)
        /// - Stato (DaInviare)
        /// - DataApertura
        /// </remarks>
        public Dominio.Istanza DatiIstanza
        {
            get;
            set;
        }
    }
}