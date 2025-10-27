// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FattAttivaCodFornitoreEntity
    {
        public long ID_AMM { get; set; }
        public long ID_REGISTRO { get; set; }
        public string? COD_FORNITORE { get; set; }
        public string? COD_FASCICOLO { get; set; }
        public string? CODICE_AMM_IPA { get; set; }

    }
}
