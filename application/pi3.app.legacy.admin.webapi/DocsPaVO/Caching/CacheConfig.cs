// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.Caching
{
    [Serializable()]
    [DataContract]
    public class CacheConfig
    {
        [DataMember]
        public string localita { get; set; }
        [DataMember]
        public string idAmministrazione { get; set; }

        [DataMember]
        public bool caching { get; set; }

        [DataMember]
        public double massima_dimensione_caching { set; get; }

        [DataMember]
        public double massima_dimensione_file { get; set; }

        [DataMember]
        public string doc_root_server { get; set; }

        [DataMember]
        public string ora_inizio_cache { get; set; }

        [DataMember]
        public string ora_fine_cache { get; set; }
        /// <summary>
        /// url del web service DocsPaWS.asmx del comando generale
        /// </summary>
        [DataMember]
        public string urlwscaching { get; set; }
        /// <summary>
        /// URL del modulo di caching CachingServices.asmx
        /// </summary>
        [DataMember]
        public string url_ws_caching_locale { get; set; }

        [DataMember]
        public string doc_root_server_locale { get; set; }
    }
}
