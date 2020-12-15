using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Grid
{
    [Serializable()]
    public class FieldValue
    {
        /// <summary>
        /// Valore
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Colore
        /// </summary>
        public string ColorBG{ get; set; }
    }
}
