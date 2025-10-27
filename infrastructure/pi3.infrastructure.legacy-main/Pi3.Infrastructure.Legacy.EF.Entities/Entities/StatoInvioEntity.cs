// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class StatoInvioEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_CORR_GLOBALE { get; set; }
        public long? ID_PROFILE { get; set; }
        public long? ID_DOC_ARRIVO_PAR { get; set; }
        public long? ID_CANALE { get; set; }
        public DateTime? DTA_SPEDIZIONE { get; set; }
        public string? VAR_INDIRIZZO { get; set; }
        public string? VAR_CAP { get; set; }
        public string? VAR_CITTA { get; set; }
        public string? CHA_INTEROP { get; set; }
        public string? VAR_PROVINCIA { get; set; }
        public long? ID_DOCUMENTTYPE { get; set; }
        public string? VAR_SERVER_SMTP { get; set; }
        public long? NUM_PORTA_SMTP { get; set; }
        public string? VAR_CODICE_AOO { get; set; }
        public string? VAR_CODICE_AMM { get; set; }
        public string? VAR_PROTO_DEST { get; set; }
        public DateTime? DTA_PROTO_DEST { get; set; }
        public string? VAR_MOTIVO_ANNULLA { get; set; }
        public string? CHA_ANNULLATO { get; set; }
        public string? VAR_PROVVEDIMENTO { get; set; }
        public string? STATUS_C_MASK { get; set; }
    }
}
