// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class TrasmUtenteEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_TRASM_SINGOLA { get; set; }

        public long? ID_PEOPLE { get; set; }

        public DateTime? DTA_VISTA { get; set; }

        public DateTime? DTA_ACCETTATA { get; set; }

        public DateTime? DTA_RIFIUTATA { get; set; }

        public DateTime? DTA_RISPOSTA { get; set; }

        public string? CHA_VISTA { get; set; }

        public string? CHA_ACCETTATA { get; set; }

        public string? CHA_RIFIUTATA { get; set; }

        public string? VAR_NOTE_ACC { get; set; }

        public string? VAR_NOTE_RIF { get; set; }

        public string? CHA_VALIDA { get; set; }

        public long? ID_TRASM_RISP_SING { get; set; }

        public string? CHA_IN_TODOLIST { get; set; }

        public DateTime? DTA_RIMOZIONE_TODOLIST { get; set; }

        public long? ID_PEOPLE_DELEGATO { get; set; }

        public string CHA_VISTA_DELEGATO { get; set; } = null!;

        public string CHA_ACCETTATA_DELEGATO { get; set; } = null!;

        public string CHA_RIFIUTATA_DELEGATO { get; set; } = null!;

        public string? CHA_RIMOZIONE_DELEGATO { get; set; }

        public TrasmSingolaEntity TRASM_SINGOLA { get; set; }
    }
}
