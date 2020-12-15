using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaWS.Conservazione.PacchettiVersamento.Dominio
{
    /// <summary>
    /// Indica le tipologie di oggetti documentali per cui è stato definito il profilo 
    /// </summary>
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/TipiOggettoProfiloEnum")]
    public enum TipiOggettoProfiloEnum
    {
        /// <summary>
        /// Il profilo è riferito al documento
        /// </summary>
        Documento,

        /// <summary>
        /// Il profilo è riferito al fascicolo
        /// </summary>
        Fascicolo,
    }

    /// <summary>
    /// Campo del profilo dinamico
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/CampoProfilo")]
    public class CampoProfilo
    {
        /// <summary>
        /// Nome del campo
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// Indica se il campo è obbligatorio o meno
        /// </summary>
        public bool Obbligatorio
        {
            get;
            set;
        }

        /// <summary>
        /// Valore associato al campo
        /// </summary>
        public string Valore
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/Profilo")]
    public class Profilo
    {
        /// <summary>
        /// Identificativo univoco del profilo
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del profilo
        /// </summary>
        public string Nome
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Dati di un profilo dinamico per i documenti o fascicoli
    /// </summary>
    [Serializable()]
    [System.Xml.Serialization.XmlType(Namespace = "http://www.valueteam.com/Conservazione/PacchettiVersamento/DettaglioProfilo")]
    public class DettaglioProfilo
    {
        /// <summary>
        /// Identificativo univoco del profilo
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del profilo
        /// </summary>
        public string Nome
        {
            get;
            set;
        }

        /// <summary>
        /// Campi del profilo
        /// </summary>
        public CampoProfilo[] Campi
        {
            get;
            set;
        }
    }
}