// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class RagioneTrasmissioneEntity
    {
        public string? CHA_CEDE_DIRITTI { get; set; }
        public string? CHA_EREDITA { get; set; }
        public string? CHA_FASC_OBBLIGATORIA { get; set; }
        public string? CHA_MANTIENI_LETT { get; set; }
        public string? CHA_MANTIENI_SCRITT { get; set; }
        public string? CHA_PROC_RES { get; set; }
        public string CHA_RAG_SISTEMA { get; set; }
        public string? CHA_RISPOSTA { get; set; }
        public string? CHA_TIPO_DEST { get; set; }
        public string? CHA_TIPO_DIRITTI { get; set; }
        public string? CHA_TIPO_RAGIONE { get; set; }
        public string? CHA_TIPO_RISPOSTA { get; set; }
        public string? CHA_VIS { get; set; }
        public long? ID_AMM { get; set; }
        public long SYSTEM_ID { get; set; }
        public string? VAR_DESC_RAGIONE { get; set; }
        public string? VAR_NOTE { get; set; }
        public string? VAR_NOTIFICA_TRASM { get; set; }
        public string? VAR_TESTO_MSG_NOTIFICA_DOC { get; set; }
        public string? VAR_TESTO_MSG_NOTIFICA_FASC { get; set; }
    }
}
