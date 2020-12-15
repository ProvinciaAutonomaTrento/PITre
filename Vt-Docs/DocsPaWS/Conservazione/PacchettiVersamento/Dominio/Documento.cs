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
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/Documento")]
    public class Documento
    {
        /// <summary>
        /// Identificativo univoco del documento presente in un'istanza di conservazione
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
        /// Identificativo univoco del documento cui è allegato il presente documento
        /// </summary>
        /// <remarks>
        /// Se il dato è valorizzato, il documento è un allegato
        /// </remarks>
        public string IdDocumentoPrincipale
        {
            get;
            set;
        }

        /// <summary>
        /// Identificativo univoco del fascicolo in cui il documento è inserito
        /// </summary>
        public string IdFascicolo
        {
            get;
            set;
        }

        /// <summary>
        /// Codice del fascicolo in cui il documento è inserito
        /// </summary>
        /// <remarks>
        /// Il dato è valorizzato dal sistema nel caso in cui il documento è stato fascicolato
        /// </remarks>
        public string CodiceFascicolo
        {
            get;
            set;
        }

        /// <summary>
        /// Oggetto del documento
        /// </summary>
        public string Oggetto
        {
            get;
            set;
        }

        /// <summary>
        /// File associato al documento
        /// </summary>
        public File File
        {
            get;
            set;
        }

        /// <summary>
        /// Dati del profilo dinamico associato al documento
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