// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class OggettiStoEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_PROFILE { get; set; }
        public long? ID_OGGETTO { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_RUOLO_IN_UO { get; set; }
        public string? VAR_MOTIVO { get; set; }
        public DateTime? DTA_MODIFICA { get; set; }
    }
}
