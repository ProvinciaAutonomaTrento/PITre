// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ContatoriFascEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_OGG { get; set; }

        public long? ID_TIPOLOGIA { get; set; }

        public long? ID_AOO { get; set; }

        public long? ID_RF { get; set; }

        public long? VALORE { get; set; }

        public long? ABILITATO { get; set; }

        public long? ANNO { get; set; }

        public long? VALORE_SC { get; set; }
    }
}
