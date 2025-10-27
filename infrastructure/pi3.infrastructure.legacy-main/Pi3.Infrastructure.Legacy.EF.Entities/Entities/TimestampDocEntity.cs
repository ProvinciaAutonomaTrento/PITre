// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class TimestampDocEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? DOC_NUMBER { get; set; }
        public long? VERSION_ID { get; set; }
        public long? ID_PEOPLE { get; set; }
        public DateTime? DTA_CREAZIONE { get; set; }
        public DateTime? DTA_SCADENZA { get; set; }
        public string? NUM_SERIE { get; set; }
        public string? S_N_CERTIFICATO { get; set; }
        public string? ALG_HASH { get; set; }
        public string? SOGGETTO { get; set; }
        public string? PAESE { get; set; }
        public string? TSR_FILE { get; set; }
    }
}
