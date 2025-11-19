// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class MetadatiDocumentoEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_PROFILE { get; set; }
        public long? ID_VERSION { get; set; }
        public string? METADATI_XML { get; set; }
        public DateTime? DTA_INSERIMENTO { get; set; }
        public DateTime? DTA_AZIONE { get; set; }
        public string? VAR_COD_AZIONE { get; set; }
        public string? VAR_DESC_AZIONE { get; set; }
        public string? VAR_OGGETTO { get; set; }
    }
}
