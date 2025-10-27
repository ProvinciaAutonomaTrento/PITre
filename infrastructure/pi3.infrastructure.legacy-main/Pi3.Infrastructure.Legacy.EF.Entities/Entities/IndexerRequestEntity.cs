// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class IndexerRequestEntity
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
        public String? LAST_ERROR { get; set; }
        public int REMAINING_ATTEMPTS { get; set; }
        public string? WARNINGS { get; set; }
    }
}
