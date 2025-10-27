// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class TrasmissioneEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_RUOLO_IN_UO { get; set; }

        public long? ID_PEOPLE { get; set; }

        public string? CHA_TIPO_OGGETTO { get; set; }

        public long? ID_PROFILE { get; set; }

        public long? ID_PROJECT { get; set; }

        public DateTime? DTA_INVIO { get; set; }

        public string? VAR_NOTE_GENERALI { get; set; }

        public string? CHA_CESSIONE { get; set; }

        public string? CHA_SALVATA_CON_CESSIONE { get; set; }

        public long? ID_PEOPLE_DELEGATO { get; set; }

        public virtual List<TrasmSingolaEntity> TRASM_SINGOLE { get; set; }

    }
}
