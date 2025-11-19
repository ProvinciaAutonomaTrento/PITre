// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class NotificaEntity
    {
        public string? VAR_IDENTIFICATIVO { get; set; }
        public string? VAR_MSGID { get; set; }
        public string? VAR_TIPO_RICEVUTA { get; set; }
        public string? VAR_CONSEGNA { get; set; }
        public string? VAR_RICEZIONE { get; set; }
        public string? VAR_ERRORE_RICEVUTA { get; set; }
        public string? VAR_MITTENTE { get; set; }
        public string? VAR_DESTINATARIO { get; set; }
        public string? VAR_RISPOSTE { get; set; }
        public string? VAR_GESTIONE_EMITTENTE { get; set; }
        public string? VAR_OGGETTO { get; set; }
        public string? VAR_TIPO_DESTINATARIO { get; set; }
        public string? VAR_ZONA { get; set; }
        public long? VERSION_ID { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_TIPO_NOTIFICA { get; set; }
        public long? DOCNUMBER { get; set; }
        public DateTime? VAR_GIORNO_ORA { get; set; }
        public string? VAR_ERRORE_ESTESO { get; set; }
    }
}
