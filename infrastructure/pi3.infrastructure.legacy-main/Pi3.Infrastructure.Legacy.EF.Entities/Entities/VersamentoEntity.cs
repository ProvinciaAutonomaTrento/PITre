// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class VersamentoEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ID_PROFILE { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_RUOLO { get; set; }
        public string CHA_STATO { get; set; }
        public DateTime? DTA_INVIO { get; set; }
        public string? VAR_FILE_RISPOSTA { get; set; }
        public string? CHA_WARNING { get; set; }
        public long? ID_AMM { get; set; }
        public string? VAR_FILE_METADATI { get; set; }
        public long? NUM_TENTATIVI_INVIO { get; set; }
        public string? VAR_MESSAGGIO_ERRORE { get; set; }
        public string? VAR_CUSTOM_ENTE { get; set; }
        public string? VAR_CUSTOM_STRUTTURA { get; set; }
    }
}
