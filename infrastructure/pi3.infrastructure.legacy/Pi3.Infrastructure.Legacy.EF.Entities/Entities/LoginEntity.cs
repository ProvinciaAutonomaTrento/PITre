// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class LoginEntity
    {
        public string? CHA_DELEGA { get; set; }
        public DateTime? DTA_CONNESSIONE { get; set; }
        public long? ID_AMM { get; set; }
        public string? USER_ID { get; set; }
        public string? USER_ID_DELEGATO { get; set; }
        public string? DST { get; set; }
        public string? SESSION_ID { get; set; }
        public string? IP_ADDRESS { get; set; }
    }
}
