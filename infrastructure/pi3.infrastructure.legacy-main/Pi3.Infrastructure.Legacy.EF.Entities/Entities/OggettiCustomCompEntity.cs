// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class OggettiCustomCompEntity
    {
        public long SYSTEM_ID { get; set; }

        public long? ID_TEMPLATE { get; set; }

        public long? ID_OGG_CUSTOM { get; set; }

        public int? POSIZIONE { get; set; }

        public string? ENABLEDHISTORY { get; set; }

        public string? CAMPO_XML_ASSOC { get; set; }

        public string? OPZIONI_XML_ASSOC { get; set; }
    }
}
