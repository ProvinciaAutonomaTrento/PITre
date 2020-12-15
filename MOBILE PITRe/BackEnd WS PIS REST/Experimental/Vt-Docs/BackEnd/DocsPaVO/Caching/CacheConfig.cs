using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Caching
{
    [Serializable()]
    public class CacheConfig
    {
        public string localita { get; set; }
        public string idAmministrazione { get; set; }

        public bool caching { get; set; }

        public double massima_dimensione_caching { set; get; }

        public double massima_dimensione_file { get; set; }

        public string doc_root_server { get; set; }

        public string ora_inizio_cache { get; set; }

        public string ora_fine_cache { get; set; }
        /// <summary>
        /// url del web service DocsPaWS.asmx del comando generale
        /// </summary>
        public string urlwscaching { get; set; }
        /// <summary>
        /// URL del modulo di caching CachingServices.asmx
        /// </summary>
        public string url_ws_caching_locale { get; set; }

        public string doc_root_server_locale { get; set; }
    }
}
