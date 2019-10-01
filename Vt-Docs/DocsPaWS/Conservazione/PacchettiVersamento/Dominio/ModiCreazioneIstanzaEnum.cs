using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Modalità con cui possono essere create le istanze di conservazione
    /// </summary>
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/ModiCreazioneIstanzaEnum")]
    public enum ModiCreazioneIstanzaEnum
    {
        /// <summary>
        /// In tale modalità, l'istanza è stata creata manualmente dal richiedente
        /// </summary>
        Manuale,

        /// <summary>
        /// In tale modalità, l'istanza è stata creata automaticamente dalle policy di creazione definite in amministrazione
        /// </summary>
        Automatica,
    }
}