// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ModelloDestConNotOrEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ID_MODELLO_MITT_DEST { get; set; }
        public long ID_PEOPLE { get; set; }
        public long ID_MODELLO { get; set; }
    }
}
