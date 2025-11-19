// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class StampaRepertoriEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_REPERTORIO { get; set; }

        public long? NUM_REP_START { get; set; }

        public long? NUM_REP_END { get; set; }

        public long? NUM_ANNO { get; set; }

        public long? DOCNUMBER { get; set; }

        public DateTime? DTA_STAMPA { get; set; }

        public long? REGISTRYID { get; set; }
    }
}
