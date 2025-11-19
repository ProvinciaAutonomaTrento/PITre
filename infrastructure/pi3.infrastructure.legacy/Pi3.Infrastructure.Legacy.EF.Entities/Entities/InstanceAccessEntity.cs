// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class InstanceAccessEntity
    {
        public long SYSTEM_ID { get; set; }

        public string DESCRIPTION { get; set; }

        public DateTime DTA_CREAZIONE { get; set; }

        public DateTime? DTA_CHIUSURA { get; set; }

        public long ID_PEOPLE_PROPRIETARIO { get; set; }

        public long ID_GRUPPO_PROPRIETARIO { get; set; }              
                
        public long? ID_RICHIEDENTE { get; set; }

        public DateTime? DTA_RICHIESTA { get; set; }

        public long? ID_DOCUMENTO_RICHIESTO { get; set; }

        public string? NOTE { get; set; }

        public string? CHA_STATO_DOWNLOAD_INOLTRO { get; set; }

        public long? ID_PROFILE_DOWNLOAD { get; set; }
    }
}
