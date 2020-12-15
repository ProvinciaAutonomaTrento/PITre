using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.amministrazione;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Rappresenta le informazioni di una uo docspa da inserire nella rubrica comune
    /// </summary>
    [Serializable()]
    public class ElementoRubricaUO : ElementoRC
    {
        /// <summary>
        /// Rappresenta l'Uo docspa da inviare in rubrica comune
        /// </summary>
        public OrgUO UO;
    }
}
