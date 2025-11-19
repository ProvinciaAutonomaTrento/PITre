// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ProjectComponentEntity
    {
        public string? DESCRIPTION { get; set; }
        public long? LIBRARY { get; set; }
        public string? TYPE { get; set; }
        public long? PROJECT_ID { get; set; }
        public long? LINK { get; set; }
        public long? COMP_ORDER { get; set; }
        public string? VAR_CODICE_COMP { get; set; }
        public string? PROT_TIT { get; set; }
        public DateTime? DTA_CLASS { get; set; }
        public string CHA_FASC_PRIMARIA { get; set; }
    }
}
