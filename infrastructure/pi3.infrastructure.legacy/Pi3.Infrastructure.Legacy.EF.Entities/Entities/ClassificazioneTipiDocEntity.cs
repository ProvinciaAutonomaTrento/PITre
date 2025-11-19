// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ClassificazioneTipiDocEntity
    {
        public long ID_AMM { get; set; }

        public string TIPO_DOC { get; set; } = null!;

        public string CHA_FASC_OBBLIGATORIA { get; set; } = null!;
    }
}
