using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.areaConservazione
{
    /// <summary>
    /// Riporta un elemento di conservazione non valido in base alle regole di business
    /// </summary>
    [Serializable()]
    public class InvalidItemConservazione
    {
        /// <summary>
        /// Elemento di conservazione non valido
        /// </summary>
        public ItemsConservazione Item { get; set; }

        /// <summary>
        /// Riporta il motivo per il quale l'elemento di conservazione non è valido
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
