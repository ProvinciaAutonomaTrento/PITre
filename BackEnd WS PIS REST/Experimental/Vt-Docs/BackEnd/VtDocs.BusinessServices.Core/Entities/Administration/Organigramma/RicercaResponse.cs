using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class RicercaResponse : Response
    {
        /// <summary>
        /// Lista di elementi trovati dalla ricerca
        /// </summary>
        [System.Xml.Serialization.XmlArray()]
        [System.Xml.Serialization.XmlArrayItem(typeof(DocsPaVO.amministrazione.OrgRisultatoRicerca))]
        public List<DocsPaVO.amministrazione.OrgRisultatoRicerca> List
        {
            get;
            set;
        }
    }
}
