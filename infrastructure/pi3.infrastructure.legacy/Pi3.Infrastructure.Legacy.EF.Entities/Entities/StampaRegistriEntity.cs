// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class StampaRegistriEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_REGISTRO { get; set; }

        public long? NUM_PROTO_START { get; set; }

        public long? NUM_PROTO_END { get; set; }

        public long? NUM_ANNO { get; set; }

        public long? NUM_ORD_FILE { get; set; }

        public long? NUM_PAGINA_END { get; set; }

        public long? DOCNUMBER { get; set; }

        public DateTime? DTA_STAMPA { get; set; }
    }
}
