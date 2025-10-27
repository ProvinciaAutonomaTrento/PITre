// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class TrasmSingolaEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_RAGIONE { get; set; }

        public long? ID_TRASMISSIONE { get; set; }

        public string? CHA_TIPO_DEST { get; set; }

        public long? ID_CORR_GLOBALE { get; set; }

        public string? VAR_NOTE_SING { get; set; }

        public string? CHA_TIPO_TRASM { get; set; }

        public DateTime? DTA_SCADENZA { get; set; }

        public long? ID_TRASM_UTENTE { get; set; }

        public string? CHA_SET_EREDITA { get; set; }

        public string? HIDE_DOC_VERSIONS { get; set; }

        public virtual List<TrasmUtenteEntity> TRASM_UTENTE { get; set; }

        public TrasmissioneEntity TRASMISSIONE { get; set; } 
    }
}
