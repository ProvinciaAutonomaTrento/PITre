// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AssTemplatesFascEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_OGGETTO { get; set; }

        public long? ID_TEMPLATE { get; set; }

        public string? ID_PROJECT { get; set; }

        public string? VALORE_OGGETTO_DB { get; set; }

        public int? ANNO { get; set; }

        public long? ID_AOO_RF { get; set; }

        public string? CODICE_DB { get; set; }

        public int? MANUAL_INSERT { get; set; }

        public int? VALORE_SC { get; set; }

        public DateTime? DTA_INS { get; set; }

        public string? ANNO_ACC { get; set; }
    }
}
