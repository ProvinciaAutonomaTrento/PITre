// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CacheEntity
    {
        public long? DOCNUMBER { get; set; }

        public string? PATHCACHE { get; set; }

        public string? IDAMMINISTRAZIONE { get; set; }

        public long? AGGIORNATO { get; set; }

        public long? VERSION_ID { get; set; }

        public string? LOCKED { get; set; }

        public string? COMPTYPE { get; set; }

        public long? FILE_SIZE { get; set; }

        public string? ALTERNATE_PATH { get; set; }

        public string? VAR_IMPRONTA { get; set; }

        public string? EXT { get; set; }

        public string? LAST_ACCESS { get; set; }
    }
}
