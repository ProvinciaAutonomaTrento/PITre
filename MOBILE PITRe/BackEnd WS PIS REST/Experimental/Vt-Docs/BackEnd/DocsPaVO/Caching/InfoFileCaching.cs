using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Caching
{
    /// <summary>
    /// Mantiene i dettagli relativi ad un file in stato "cached"
    /// </summary>
    [Serializable()]
    public class InfoFileCaching
    {
        /// <summary>
        /// DocNumber del documento inserito in cache
        /// </summary>
        public int DocNumber { get; set; }

        /// <summary>
        /// Indica l'indirizzo di rete in cui è presente il file in stato "cached"
        /// </summary>
        public string Localita { get; set; }
        public string idAmministrazione { get; set; }

        /// <summary>
        /// Indica se il file è presente o meno nel server centrale
        /// </summary>
        public int Aggiornato { get; set; }

        /// <summary>
        /// Indica il path completo del file "cached"
        /// </summary>
        public string CacheFilePath { get; set; }

        /// <summary>
        /// Indica la dimensione in byte del file
        /// </summary>
       // public double FileSize { get; set; }

        /// <summary>
        /// Indica la versione del file
        /// </summary>
        public int Version_id { get; set; }


        public int locked {get;set;}
        
        public string comptype {get;set;}

        public int file_size{get; set;}

        public string alternate_path {get; set;} 

        public string var_impronta {get; set;}

        public string ext {get; set;}

        public string last_access { get; set; }
    }
}
