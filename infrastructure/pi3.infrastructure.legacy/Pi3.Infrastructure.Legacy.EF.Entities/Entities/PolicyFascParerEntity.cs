// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PolicyFascParerEntity
    {
        public DateTime? DTA_ESECUZIONE_POLICY { get; set; }
        public DateTime? DATA_APERTURA_FROM { get; set; }
        public DateTime? DATA_APERTURA_TO { get; set; }
        public DateTime? DATA_CHIUSURA_FROM { get; set; }
        public DateTime? DATA_CHIUSURA_TO { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_GROUP_RUOLO_RESP { get; set; }
        public long? CHA_ESECUZIONE_GIORNO { get; set; }
        public long? CHA_ESECUZIONE_MESE { get; set; }
        public long? ID_TITOLARIO { get; set; }
        public long? ID_CLASSIFICA { get; set; }
        public long? ID_TEMPLATE { get; set; }
        public long? ID_REGISTRO { get; set; }
        public string? CHA_ATTIVA { get; set; }
        public string? CHA_PERIODICITA { get; set; }
        public string? CHA_DATA_APERTURA_TIPO { get; set; }
        public string? CHA_DATA_CHIUSURA_TIPO { get; set; }
        public string? CHA_ATTIVA_NOTIFICA { get; set; }
        public string? CHA_STATO_VERSAMENTO { get; set; }
        public string? VAR_DESCRIZIONE { get; set; }
        public string? VAR_CODICE { get; set; }
    }
}
