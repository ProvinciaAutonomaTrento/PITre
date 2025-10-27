// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ComponentEntity
    {
        public string? PATH { get; set; }
        public string? LOCKED { get; set; }
        public string? COMPTYPE { get; set; }
        public long? VERSION_ID { get; set; }
        public long? DOCNUMBER { get; set; }
        public long? FILE_SIZE { get; set; }
        public string? VAR_IMPRONTA { get; set; }
        public string? EXT { get; set; }
        public string? CHA_FIRMATO { get; set; }
        public string? VAR_NOMEORIGINALE { get; set; }
        public string? FILE_INFO { get; set; }
        public long? ID_PEOPLE_PUTFILE { get; set; }
        public long? ID_PEOPLE_DELEGATO_PUTFILE { get; set; }
        public DateTime? DTA_FILE_ACQUIRED { get; set; }
        public string? CHA_TIPO_FIRMA { get; set; }
    }
}
