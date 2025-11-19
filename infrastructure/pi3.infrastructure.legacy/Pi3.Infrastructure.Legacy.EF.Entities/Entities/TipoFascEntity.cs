// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class TipoFascEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? VAR_DESC_FASC { get; set; }
        public long? ID_AMM { get; set; }
        public int? ABILITATO_SI_NO { get; set; }
        public string? IN_ESERCIZIO { get; set; }
        public string? PATH_MOD_1 { get; set; }
        public string? EXT_MOD_1 { get; set; }
        public string? PATH_MOD_2 { get; set; }
        public string? EXT_MOD_2 { get; set; }
        public int? GG_SCADENZA { get; set; }
        public int? GG_PRE_SCADENZA { get; set; }
        public string? CHA_PRIVATO { get; set; }
        public int? IPERFASCICOLO { get; set; }
        public int? NUM_MESI_CONSERVAZIONE { get; set; }
    }
}
