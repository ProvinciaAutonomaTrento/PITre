// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CheckinCheckoutEntity
    {
        public long SYSTEM_ID { get; set; }

        public long ID_DOCUMENT { get; set; }

        public long DOCUMENT_NUMBER { get; set; }

        public long ID_USER { get; set; }

        public long ID_ROLE { get; set; }

        public DateTime CHECK_OUT_DATE { get; set; }

        public string? DOCUMENT_LOCATION { get; set; }

        public string? MACHINE_NAME { get; set; }
    }
}
