// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class IndexerHandledRequestEntity
    {
        public long ID { get; set; }
        public long ID_AMM { get; set; }
        public DateTime REQUEST_DATE { get; set; }
        public string ID_ELEMENT { get; set; }
        public int ID_ELEMENT_TYPE { get; set; }
        public DateTime ELEMENT_CREATION_DATE { get; set; }
        public int ID_OPERATION_TYPE { get; set; }
        public DateTime? LAST_PROCESSING_DATE { get; set; }
        public double? LAST_PROCESSING_ELAPSED { get; set; }
        public string? WARNINGS { get; set; }
    }
}
