// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FormattaFascEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_PESO { get; set; }
        public string? CHA_VISUALIZZA { get; set; }
        public string? VAR_STRINGA { get; set; }

    }
}
