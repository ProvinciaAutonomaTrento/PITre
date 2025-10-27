// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DelegaEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_PEOPLE_DELEGANTE { get; set; }
        public long? ID_RUOLO_DELEGANTE { get; set; }
        public long? ID_PEOPLE_DELEGATO { get; set; }
        public long? ID_RUOLO_DELEGATO { get; set; }
        public DateTime? DATA_DECORRENZA { get; set; }
        public DateTime? DATA_SCADENZA { get; set; }
        public string? CHA_IN_ESERCIZIO { get; set; }
        public string? COD_PEOPLE_DELEGANTE { get; set; }
        public string? COD_RUOLO_DELEGANTE { get; set; }
        public string? COD_PEOPLE_DELEGATO { get; set; }
        public string? COD_RUOLO_DELEGATO { get; set; }
        public long? ID_UO_DELEGATO { get; set; }
    }
}
