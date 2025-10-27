// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ModelloDelegaEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ID_PEOPLE_DELEGANTE { get; set; }
        public long? ID_RUOLO_DELEGANTE { get; set; }
        public long ID_PEOPLE_DELEGATO { get; set; }
        public long ID_RUOLO_DELEGATO { get; set; }
        public long? INTERVALLO { get; set; }
        public DateTime? DTA_INIZIO { get; set; }
        public DateTime? DTA_FINE { get; set; }
        public string NOME { get; set; }
    }
}
