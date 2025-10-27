// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ModelloMdAppoEntity
    {
        public string? CHA_TIPO_MITT_DEST { get; set; }
        public string? VAR_NOTE_SING { get; set; }
        public string? CHA_TIPO_TRASM { get; set; }
        public string? CHA_TIPO_URP { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_MODELLO { get; set; }
        public long ID_CORR_GLOBALI { get; set; }
        public long? ID_RAGIONE { get; set; }
        public long? SCADENZA { get; set; }
        public string? HIDE_DOC_VERSIONS { get; set; }
    }
}
