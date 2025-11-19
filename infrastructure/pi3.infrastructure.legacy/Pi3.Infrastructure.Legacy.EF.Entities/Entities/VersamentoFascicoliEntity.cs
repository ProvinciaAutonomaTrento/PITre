// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class VersamentoFascicoliEntity
    {
        public long SYSTEM_ID { get; set; }
        public long ID_PROJECT { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_RUOLO { get; set; }
        public string CHA_STATO { get; set; }
        public DateTime? DTA_INVIO { get; set; }
        public string? VAR_FILE_RISPOSTA { get; set; }
        public string? CHA_WARNING { get; set; }
        public long? ID_AMM { get; set; }
        public string? VAR_MESSAGGIO_ERRORE { get; set; }
        public long? NUM_TENTATIVI_INVIO { get; set; }
        public string? VAR_CUSTOM_ENTE { get; set; }
        public string? VAR_CUSTOM_STRUTTURA { get; set; }
    }
}
