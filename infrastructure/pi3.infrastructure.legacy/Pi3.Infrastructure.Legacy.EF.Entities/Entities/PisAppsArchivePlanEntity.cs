// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PisAppsArchivePlanEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_TIPO_FASC { get; set; }
        public long? ID_TIPO_DOC { get; set; }
        public long? ID_REGISTRO { get; set; }
        public long? ID_PIANO_CONSERVAZIONE { get; set; }
        public long? ID_PEOPLE { get; set; }
        public long? ID_INTEGRAZIONE { get; set; }
        public long? ID_GRUPPO { get; set; }
        public long? ID_AMM { get; set; }
        public DateTime? DTA_INS { get; set; }
        public string? DESC_INTEGRAZIONE { get; set; }
        public string? CODE_APPLICATION { get; set; }
        public string? COD_CLASSIFICA { get; set; }
    }
}
