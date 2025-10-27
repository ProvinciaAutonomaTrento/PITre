// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class IstanzaPassoFirmaEntity
    {
        public string? CHA_AUTOMATICO { get; set; }
        public string? CHA_POS_SEGNATURA { get; set; }
        public string? VAR_ERRORE { get; set; }
        public DateTime? ESEGUITO_IL { get; set; }
        public DateTime? SCADENZA { get; set; }
        public long ID_ISTANZA_PASSO { get; set; }
        public long ID_ISTANZA_PROCESSO { get; set; }
        public long ID_PASSO { get; set; }
        public long? ID_RUOLO_COINVOLTO { get; set; }
        public long? ID_UTENTE_COINVOLTO { get; set; }
        public long? ID_UTENTE_LOCKER { get; set; }
        public long TIPO_EVENTO { get; set; }
        public long NUMERO_SEQUENZA { get; set; }
        public long? ID_NOTIFICA_EFFETTUATA { get; set; }
        public long? ID_AOO { get; set; }
        public long? ID_RF { get; set; }
        public long? ID_MAIL_REGISTRO { get; set; }
        public long? ID_TIPOLOGIA { get; set; }
        public long? ID_STATO_DIAGRAMMA { get; set; }
        public string? NOTE { get; set; }
        public string? VAR_POS_SEGNATURA { get; set; }
        public string? MOTIVO_RESPINGIMENTO { get; set; }
        public string STATO_PASSO { get; set; }
        public string TIPO_FIRMA { get; set; }
        public string? DESC_UTENTE_LOCKER { get; set; }
    }
}
