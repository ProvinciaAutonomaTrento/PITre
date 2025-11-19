// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class EsecuzionePolicyParerEntity
    {
        public long ID_POLICY { get; set; }
        public DateTime? DATA_ULTIMA_ESECUZIONE { get; set; }
        public DateTime? DATA_PROSSIMA_ESECUZIONE { get; set; }
        public long? NUM_ESECUZIONI { get; set; }

    }
}
