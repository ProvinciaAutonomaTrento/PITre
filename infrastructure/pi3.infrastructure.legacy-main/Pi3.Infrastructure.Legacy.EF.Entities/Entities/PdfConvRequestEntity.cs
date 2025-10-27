// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class PdfConvRequestEntity
    {
        public long ID { get; set; }
        public long ID_AMM { get; set; }
        public long ID_PEOPLE { get; set; }
        public long ID_GROUP { get; set; }
        public DateTime REQUEST_DATE { get; set; }
        public long ID_PROFILE { get; set; }
        public DateTime PROFILE_CREATION_DATE { get; set; }
        public DateTime? LAST_PROCESSING_DATE { get; set; }
        public double? LAST_PROCESSING_ELAPSED { get; set; }
        public String? LAST_ERROR { get; set; }
        public int REMAINING_ATTEMPTS { get; set; }
    }
}
