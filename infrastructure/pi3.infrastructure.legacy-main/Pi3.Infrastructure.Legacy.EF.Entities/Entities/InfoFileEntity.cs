// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class InfoFileEntity
    {
        public string? CHA_CONFORME { get; set; }
        public string? CHA_ESTENSIONE_CONFORME { get; set; }
        public string? CHA_PRESENZA_MACRO { get; set; }
        public string? CHA_PRESENZA_FORMS { get; set; }
        public string? CHA_PRESENZA_JAVASCRIPT { get; set; }
        public string? CHA_NOTIFICA { get; set; }
        public DateTime? DTA_ACQUISIZIONE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_PROFILE { get; set; }
        public long? ID_DOCUMENTO_PRINCIPALE { get; set; }
        public long? VERSION_ID { get; set; }
        public string? VAR_ESTENSIONE { get; set; }
        public string? VAR_NOME_FILE { get; set; }
        public string? VAR_DESC_INFO_FILE { get; set; }

    }
}
