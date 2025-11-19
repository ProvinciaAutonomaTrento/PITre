// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CanaliRegEntity
    {
        public long SYSTEM_ID { get; set; }

        public int? ID_CANALE { get; set; }

        public int? ID_REGISTRO { get; set; }

        public int? ID_DOCUMENTTYPE { get; set; }
    }
}
