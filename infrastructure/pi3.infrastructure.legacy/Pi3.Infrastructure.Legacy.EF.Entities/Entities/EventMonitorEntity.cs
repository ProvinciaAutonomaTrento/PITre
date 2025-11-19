// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class EventMonitorEntity
    {
        public long ID_DOCUMENTO { get; set; }
        public long ID_LOG { get; set; }
        public long ID_EVENTO { get; set; }
        public long ID_GROUP { get; set; }
        public long ID_PEOPLE_AZIONE { get; set; }
        public long? ID_DELEGANTE { get; set; }
        public long? ID_PEOPLE { get; set; }
        public DateTime? DATA_INSERIMENTO { get; set; }
    }
}
