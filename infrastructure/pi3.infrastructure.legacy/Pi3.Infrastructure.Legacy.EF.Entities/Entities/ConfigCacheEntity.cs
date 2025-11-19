// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ConfigCacheEntity
    {
        public string? IDAMMINISTRAZIONE { get; set; }

        public long? CACHING { get; set; }

        public long? MASSIMA_DIMENSIONE_CACHING { get; set; }

        public long? MASSIMA_DIMENSIONE_FILE { get; set; }

        public string? DOC_ROOT_SERVER { get; set; }

        public string? ORA_INIZIO_CACHE { get; set; }

        public string? ORA_FINE_CACHE { get; set; }

        public string? URLWSCACHING { get; set; }

        public string? URL_WS_CACHING_LOCALE { get; set; }

        public string? DOC_ROOT_SERVER_LOCALE { get; set; }
    }
}
