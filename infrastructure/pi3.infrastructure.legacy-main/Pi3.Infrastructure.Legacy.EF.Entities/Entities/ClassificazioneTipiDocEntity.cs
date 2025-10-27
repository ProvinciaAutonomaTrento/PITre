// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
