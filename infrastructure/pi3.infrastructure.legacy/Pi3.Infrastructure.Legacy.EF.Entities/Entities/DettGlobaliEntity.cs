// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DettGlobaliEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_CORR_GLOBALI { get; set; }
        public long? ID_QUALIFICA_CORR { get; set; }
        public string? VAR_COD_PI { get; set; }
        public string? VAR_INDIRIZZO { get; set; }
        public string? VAR_COD_FISC { get; set; }
        public string? VAR_LOCALITA { get; set; }
        public string? VAR_COD_FISCALE { get; set; }
        public string? VAR_FAX { get; set; }
        public string? VAR_PROVINCIA { get; set; }
        public string? VAR_NOTE { get; set; }
        public string? VAR_NAZIONE { get; set; }
        public string? VAR_CAP { get; set; }
        public string? VAR_TELEFONO { get; set; }
        public string? VAR_TELEFONO2 { get; set; }
        public string? VAR_COD_IPA { get; set; }
        public string? VAR_CITTA { get; set; }
        public string? VAR_LUOGO_NASCITA { get; set; }
        public string? DTA_NASCITA { get; set; }
        public string? VAR_TITOLO { get; set; }
    }
}
