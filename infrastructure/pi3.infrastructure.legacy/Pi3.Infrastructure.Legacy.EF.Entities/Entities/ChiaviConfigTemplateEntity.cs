// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public partial class ChiaviConfigTemplateEntity
    {
        public long SYSTEM_ID { get; set; }

        public string VAR_CODICE { get; set; } = null!;

        public string? VAR_DESCRIZIONE { get; set; }

        public string VAR_VALORE { get; set; } = null!;

        public string? CHA_TIPO_CHIAVE { get; set; }

        public string? CHA_VISIBILE { get; set; }

        public string? CHA_MODIFICABILE { get; set; }

        public string? CHA_INFASATO { get; set; }
    }
}


