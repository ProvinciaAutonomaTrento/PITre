// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AssGridsEntity
    {
        public long GRID_ID { get; set; }

        public long USER_ID { get; set; }

        public long ROLE_ID { get; set; }

        public long? ADMINISTRATION_ID { get; set; }

        public string? TYPE_GRID { get; set; }
    }
}
