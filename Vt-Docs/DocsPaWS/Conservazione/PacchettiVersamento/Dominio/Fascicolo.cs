using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Dati di un documento inserito in conservazione
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/Fascicolo")]
    public class Fascicolo
    {
        /// <summary>
        /// Identificativo univoco del fascicolo presente in un'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Il dato è contatore autoincrement restituito dal sistema 
        /// </remarks>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Codice univoco del fascicolo presente in un'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Il dato è restituito dal sistema 
        /// </remarks>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione del fascicolo presente in un'istanza di conservazione
        /// </summary>
        /// <remarks>
        /// Dato obbligatorio
        /// </remarks>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// Dati del profilo dinamico associato al fascicolo
        /// </summary>
        /// <remarks>
        /// Attributo facoltativo
        /// </remarks>
        public DettaglioProfilo Profilo
        {
            get;
            set;
        }
    }
}