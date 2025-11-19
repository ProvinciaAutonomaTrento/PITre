// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PolicyParerEntity
    {
        public string? CHA_BIG_FILES { get; set; }
        public string? CHA_ATTIVA_NOTIFICA { get; set; }
        public DateTime? DTA_ESECUZIONE_POLICY { get; set; }
        public DateTime? DATA_CREAZIONE_FROM { get; set; }
        public DateTime? DATA_CREAZIONE_TO { get; set; }
        public DateTime? DATA_PROTO_FROM { get; set; }
        public DateTime? DATA_PROTO_TO { get; set; }
        public DateTime? DATA_STAMPA_FROM { get; set; }
        public DateTime? DATA_STAMPA_TO { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_AMM { get; set; }
        public long? ID_GROUP_RUOLO_RESP { get; set; }
        public long? CHA_ESECUZIONE_GIORNO { get; set; }
        public long? CHA_ESECUZIONE_MESE { get; set; }
        public long? ID_TEMPLATE { get; set; }
        public long? ID_STATO { get; set; }
        public long? CHA_STATO { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? ID_RF { get; set; }
        public long? ID_UO_CREATRICE { get; set; }
        public long? ID_TITOLARIO { get; set; }
        public long? ID_FASCICOLO { get; set; }
        public long? ID_REPERTORIO_STAMPA { get; set; }
        public long? NUM_ANNO_STAMPA { get; set; }
        public long? NUM_GIORNI_DATA_CREAZIONE { get; set; }
        public long? NUM_GIORNI_DATA_PROTO { get; set; }
        public long? NUM_GIORNI_DATA_STAMPA { get; set; }
        public long? NUM_GIORNI_FIRMA { get; set; }
        public string? CHA_TIPO_POLICY { get; set; }
        public string? CHA_ATTIVA { get; set; }
        public string? CHA_PERIODICITA { get; set; }
        public string? CHA_TIPO_PROTO_A { get; set; }
        public string? CHA_TIPO_PROTO_P { get; set; }
        public string? CHA_TIPO_PROTO_I { get; set; }
        public string? CHA_TIPO_PROTO_G { get; set; }
        public string? CHA_UO_SOTTOPOSTE { get; set; }
        public string? CHA_DOC_DIGITALI { get; set; }
        public string? CHA_FIRMATO { get; set; }
        public string? CHA_MARCATO { get; set; }
        public string? CHA_SCADENZA_TIMESTAMP { get; set; }
        public string? CHA_DATA_CREAZIONE_TIPO { get; set; }
        public string? CHA_DATA_PROTO_TIPO { get; set; }
        public string? CHA_TIPO_REGISTRO_STAMPA { get; set; }
        public string? CHA_DATA_STAMPA_TIPO { get; set; }
        public string? CHA_STATO_VERSAMENTO { get; set; }
        public string? CHA_ESCLUDI_FATTURE { get; set; }
        public string? VAR_DESCRIZIONE { get; set; }
        public string? CHA_TIPO_CLASS { get; set; }
        public string? VAR_CODICE { get; set; }
        public string? VAR_ENTE { get; set; }
        public string? VAR_STRUTTURA { get; set; }
    }
}
