// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DataArrivoStoEntity
    {
        public long SYSTEM_ID { get; set; }

        public long DOCNUMBER { get; set; }

        public DateTime? DTA_ARRIVO { get; set; }

        public long ID_GROUP { get; set; }

        public long ID_PEOPLE { get; set; }

        public DateTime? DTA_MODIFICA { get; set; }
    }
}
