// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class MvProspettiDocClassCompAllEntity
    {
        public long? ID_AMM { get; set; }
        public long? ID_REGISTR { get; set; }
        public long? ID_ANNO { get; set; }
        public string? SEDE { get; set; }
        public long? TITOLARIO { get; set; }
        public long? TOT_DOC_CLASS { get; set; }
        public string? COD_CLASS { get; set; }
        public string? DESC_CLASS { get; set; }
        public long? TOT_DOC_CLASS_VT { get; set; }
        public long? PERC_DOC_CLASS_VT { get; set; }
        public string? NUM_LIVELLO { get; set; }
    }
}
