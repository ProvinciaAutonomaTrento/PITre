// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CorrAbilitatiEntity
    {
        public long SYSTEM_ID { get; set; }

        public long ID_CORR_GLOBALE { get; set; }

        public string CHA_TIPO_URP { get; set; } = null!;

        public string? CHA_APPLICAZIONE { get; set; }

        public long? ID_PARENT { get; set; }
    }
}
