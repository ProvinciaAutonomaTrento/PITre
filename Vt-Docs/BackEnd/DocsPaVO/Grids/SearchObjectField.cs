using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml.Serialization;
using System.Drawing;
using System.ComponentModel;

namespace DocsPaVO.Grids
{
    [Serializable()]
    public class SearchObjectField
    {
        /// <summary>
        /// Identificativo univoco del field dell'oggetto
        /// </summary>
        public string SearchObjectFieldID { get; set; }

        /// <summary>
        /// Valore dell'oggetto
        /// </summary>
        public string SearchObjectFieldValue { get; set; }

        /// <summary>
        /// Id tipo atto
        /// </summary>
        public string SearchObjectFieldIDTipoAtto { get; set; }

        /// <summary>
        /// Nome del campo
        /// </summary>
        public string SearchObjectFieldName { get; set; }
    }
}
