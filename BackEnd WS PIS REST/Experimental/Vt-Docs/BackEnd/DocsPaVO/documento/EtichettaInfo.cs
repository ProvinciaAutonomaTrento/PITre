using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    /// <summary>
    /// commento
    /// </summary>
    [Serializable()]
    public class EtichettaInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Codice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IdAmministrazione { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Etichetta { get; set; }
    }
}
