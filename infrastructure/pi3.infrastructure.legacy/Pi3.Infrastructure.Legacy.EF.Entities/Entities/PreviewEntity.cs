// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PreviewEntity
    {
        public long TOTAL_PAGES { get; set; }
        public long PAGE_TO { get; set; }
        public long PAGE_FROM { get; set; }
        public String DOC_NUMBER { get; set; }
        public String VERSION_ID { get; set; }
        public string? FILE_HASH { get; set; }
        public string? PATH { get; set; }
    }
}
