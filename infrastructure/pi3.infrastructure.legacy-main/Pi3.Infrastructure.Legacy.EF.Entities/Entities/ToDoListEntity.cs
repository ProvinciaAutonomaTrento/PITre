// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ToDoListEntity
    {
        public long ID_TRASM_SINGOLA { get; set; }
        public long ID_TRASMISSIONE { get; set; }
        public long ID_TRASM_UTENTE { get; set; }
        public DateTime DTA_INVIO { get; set; }
        public long ID_PEOPLE_MITT { get; set; }
        public long ID_RUOLO_MITT { get; set; }
        public long ID_PEOPLE_DEST { get; set; }
        public long ID_RAGIONE_TRASM { get; set; }
        public string? VAR_NOTE_GEN { get; set; }
        public string? VAR_NOTE_SING { get; set; }
        public DateTime? DTA_SCADENZA { get; set; }
        public long? ID_PROFILE { get; set; }
        public long? ID_PROJECT { get; set; }
        public long? ID_RUOLO_DEST { get; set; }
        public long? ID_REGISTRO { get; set; }
        public string? CHA_TIPO_TRASM { get; set; }
        public DateTime DTA_VISTA { get; set; }
        public long? ID_PEOPLE_DELEGATO { get; set; }
    }
}
