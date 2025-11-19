// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ConsVerificaEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_SUPPORTO { get; set; }

        public long? ID_ISTANZA { get; set; }

        public DateTime? DATA_VER { get; set; }

        public long? NUM_VER { get; set; }

        public string? VAR_NOTE { get; set; }

        public long? PERCENTUALE { get; set; }

        public string? ESITO { get; set; }

        public string? CHA_TIPO_VER { get; set; }
    }
}
