// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public  class AreaConservazioneEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_AMM { get; set; }

        public long? ID_PEOPLE { get; set; }

        public long? ID_RUOLO_IN_UO { get; set; }

        public string? CHA_STATO { get; set; }

        public long? VAR_TIPO_SUPPORTO { get; set; }

        public string? VAR_NOTE { get; set; }

        public string? VAR_DESCRIZIONE { get; set; }

        public DateTime? DATA_APERTURA { get; set; }

        public DateTime? DATA_INVIO { get; set; }

        public DateTime? DATA_CONSERVAZIONE { get; set; }

        public string? VAR_MARCA_TEMPORALE { get; set; }

        public string? VAR_FIRMA_RESPONSABILE { get; set; }

        public string? VAR_LOCAZIONE_FISICA { get; set; }

        public DateTime? DATA_PROX_VERIFICA { get; set; }

        public DateTime? DATA_ULTIMA_VERIFICA { get; set; }

        public DateTime? DATA_RIVERSAMENTO { get; set; }

        public string? VAR_TIPO_CONS { get; set; }

        public long? COPIE_SUPPORTI { get; set; }

        public string? VAR_NOTE_RIFIUTO { get; set; }

        public string? VAR_FORMATO_DOC { get; set; }

        public string? USER_ID { get; set; }

        public long? ID_GRUPPO { get; set; }

        public long? ID_PROFILE_TRASMISSIONE { get; set; }

        public int? ID_POLICY { get; set; }

        public string? CONSOLIDA { get; set; }

        public int? ID_POLICY_VALIDAZIONE { get; set; }

        public string? VAR_FILE_CHIUSURA_FIRMATO { get; set; }

        public string? VAR_FILE_CHIUSURA { get; set; }

        public string? IS_PREFERRED { get; set; }

        public long? VALIDATION_MASK { get; set; }

        public DateTime? DATA_RIFIUTO { get; set; }

        public long? ESITO_VERIFICA { get; set; }

        public long? ID_ISTANZA_RIGENERATA { get; set; }
    }
}
