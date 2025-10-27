// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class AppEntity
    {
        public long? FILE_TYPES { get; set; }
        public long? SYSTEM_ID { get; set; }
        public long? TIMEOUT { get; set; }
        public long? FILING_SCHEME { get; set; }
        public long? LANGUAGE { get; set; }
        public long? VIEWER { get; set; }
        public long? PRINTING { get; set; }
        public string? COUNT_KEYS { get; set; }
        public string? VER_TOLERANT { get; set; }
        public string? VALID_ON_PROFILE { get; set; }
        public string? READ_ONLY { get; set; }
        public string? OPEN_LAUNCH { get; set; }
        public string? ON_DESKTOP { get; set; }
        public string? DOS_MONITORING { get; set; }
        public string? DISABLED { get; set; }
        public string? PDFCOMPAT { get; set; }
        public string? INTEGRATED { get; set; }
        public string? SUPER_APP { get; set; }
        public string? USE_UNCNAME { get; set; }
        public string? DIRMON_STUBCHECK { get; set; }
        public string? MIME_TYPE { get; set; }
        public string? DESCRIPTION { get; set; }
        public string? DEFAULT_EXTENSION { get; set; }
        public string? OUTPUT_EXTS { get; set; }
        public string? APPLICATION { get; set; }
    }
}
