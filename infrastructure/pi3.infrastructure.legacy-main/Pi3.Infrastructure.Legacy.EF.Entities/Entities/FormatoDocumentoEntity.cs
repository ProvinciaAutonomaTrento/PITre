// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FormatoDocumentoEntity
    {
        public string? CHA_CONVERTIBLE { get; set; }
        public long SYSTEM_ID { get; set; }
        public long? ID_AMMINISTRAZIONE { get; set; }
        public long FILE_TYPE_USED { get; set; }
        public long MAX_FILE_SIZE { get; set; }
        public long MAX_FILE_SIZE_ALERT_MODE { get; set; }
        public long CONTAINS_FILE_MODEL { get; set; }
        public long DOCUMENT_TYPE { get; set; }
        public long? FILE_TYPE_SIGNATURE { get; set; }
        public long? FILE_TYPE_PRESERVATION { get; set; }
        public long? FILE_TYPE_VALIDATION { get; set; }
        public string FILE_EXTENSION { get; set; }
        public string DESCRIPTION { get; set; }

    }
}
