// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class InstanceAccessDocEntity
    {
        public long SYSTEM_ID { get; set; }

        public long ID_INST_ACC { get; set; }

        public long? DOCNUMBER { get; set; }

        public long? ID_PROJECT { get; set; }

        public long? ID_PARENT { get; set; }

        public string? TIPO_RICHIESTA { get; set; }

        public string? ENABLE { get; set; }
    }
}
