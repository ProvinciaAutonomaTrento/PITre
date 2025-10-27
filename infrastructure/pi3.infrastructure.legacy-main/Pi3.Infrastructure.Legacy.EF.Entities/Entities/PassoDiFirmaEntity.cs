// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PassoDiFirmaEntity
    {
        public string? TICK { get; set; }
        public string? CHA_AUTOMATICO { get; set; }
        public string? CHA_FACOLTATIVO { get; set; }
        public string? CHA_POS_SEGNATURA { get; set; }
        public DateTime? SCADENZA { get; set; }
        public long ID_PASSO { get; set; }
        public long ID_PROCESSO { get; set; }
        public long NUMERO_SEQUENZA { get; set; }
        public long TIPO_EVENTO { get; set; }
        public long? ID_RUOLO_COINVOLTO { get; set; }
        public long? ID_UTENTE_COINVOLTO { get; set; }
        public long? ELIMINATO { get; set; }
        public long? ID_TIPO_RUOLO_COINVOLTO { get; set; }
        public long? ID_AOO { get; set; }
        public long? ID_RF { get; set; }
        public long? ID_MAIL_REGISTRO { get; set; }
        public long? ID_TIPOLOGIA { get; set; }
        public long? ID_STATO_DIAGRAMMA { get; set; }
        public string? VAR_POS_SEGNATURA { get; set; }
        public string? NOTE { get; set; }
        public string? TIPO_FIRMA { get; set; }
    }
}
