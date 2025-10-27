// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ChiaveConfigurazioneEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_AMM { get; set; }
        public string VAR_CODICE { get; set; }
        public string? VAR_DESCRIZIONE { get; set; }
        public string VAR_VALORE { get; set; }
        public string? CHA_TIPO_CHIAVE { get; set; }
        public string CHA_VISIBILE { get; set; }
        public string CHA_MODIFICABILE { get; set; }
        public string CHA_GLOBALE { get; set; }
        public string? VAR_CODICE_OLD_WEBCONFIG { get; set; }
        public string? VERSIONE_CD { get; set; }
        public string? CHA_CONSERVAZIONE { get; set; }
        public DateTime? DTA_INSERIMENTO { get; set; }
    }
}
