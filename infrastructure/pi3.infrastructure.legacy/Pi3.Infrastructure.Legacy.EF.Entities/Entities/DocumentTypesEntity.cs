// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DocumentTypesEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? TYPE_ID { get; set; }
        public string? DESCRIPTION { get; set; }
        public string? DISABLED { get; set; }
        public string? STORAGE_TYPE { get; set; }
        public long? RETENTION_DAYS { get; set; }
        public long? MAX_VERSIONS { get; set; }
        public long? MAX_SUBVERSIONS { get; set; }
        public string? FULL_TEXT { get; set; }
        public long? TARGET_DOCSRVR { get; set; }
        public long? RET_2 { get; set; }
        public string? RET_2_TYPE { get; set; }
        public string? KEEP_CRITERIA { get; set; }
        public long? VERSIONS_TO_KEEP { get; set; }
        public string? CHA_TIPO_CANALE { get; set; }
        public string? DELETED { get; set; }
        public string? LABEL { get; set; }
    }
}
