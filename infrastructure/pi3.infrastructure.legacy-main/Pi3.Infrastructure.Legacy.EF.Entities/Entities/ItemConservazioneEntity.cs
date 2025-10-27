// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ItemConservazioneEntity
    {
        public string? CHA_TIPO_DOC { get; set; }
        public string? CHA_STATO { get; set; }
        public string? CHA_TIPO_OGGETTO { get; set; }
        public string? CHA_ESITO { get; set; }
        public string? POLICY_VALIDA { get; set; }
        public string? VALIDAZIONE_FIRMA { get; set; }
        public string? ESITO_FIRMA { get; set; }
        public string? VALIDAZIONE_MARCA { get; set; }
        public string? VALIDAZIONE_FORMATO { get; set; }
        public string? VAR_XML_METADATI { get; set; }
        public DateTime? DATA_INS { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_CONSERVAZIONE { get; set; }
        public long? ID_PROFILE { get; set; }
        public long? ID_PROJECT { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? SIZE_ITEM { get; set; }
        public long? DOCNUMBER { get; set; }
        public long? NUMERO_ALLEGATI { get; set; }
        public string? MASK_VALIDAZIONE_POLICY { get; set; }
        public string? VAR_OGGETTO { get; set; }
        public string? VAR_TIPO_FILE { get; set; }
        public string? COD_FASC { get; set; }
        public string? VAR_TIPO_ATTO { get; set; }

    }
}
