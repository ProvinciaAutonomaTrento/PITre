using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Rappresenta l'unità organizzativa
    /// </summary>
    [Serializable()]
    public class UO
    {
        /// <summary>
        /// Dati identificativi dell'unità organizzativa
        /// </summary>
        public DocsPaVO.amministrazione.OrgUO DatiUO
        {
            get;
            set;
        }

        /// <summary>
        /// Ruoli facenti parte dell'unità organizzativa
        /// </summary>
        [System.Xml.Serialization.XmlArray()]
        [System.Xml.Serialization.XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRuolo))]
        public List<DocsPaVO.amministrazione.OrgRuolo> Ruoli
        {
            get;
            set;
        }

        /// <summary>
        /// Lista delle unità organizzative sottoposte
        /// </summary>
        [System.Xml.Serialization.XmlArray()]
        [System.Xml.Serialization.XmlArrayItem(typeof(VtDocs.BusinessServices.Entities.Administration.Organigramma.UO))]
        public List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UO> UOSottoposte
        {
            get;
            set;
        }

        /// <summary>
        /// Lista degli utenti responsabili dell'unità organizzativa
        /// </summary>
        [System.Xml.Serialization.XmlArray()]
        [System.Xml.Serialization.XmlArrayItem(typeof(VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile))]
        public List<VtDocs.BusinessServices.Entities.Administration.Organigramma.UtenteResponsabile> Responsabili
        {
            get;
            set;
        }

        /// <summary>
        /// Percorso completo dell'UO
        /// </summary>
        public string PathIdUO
        {
            get;
            set;
        }
    }
}
