// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class CountToDoListEntity
    {
        public long ID_PEOPLE { get; set; }

        public DateTime TS_STAMPA { get; set; }

        public long? TOT_DOC { get; set; }

        public long? TOT_DOC_NO_LETTI { get; set; }

        public long? TOT_DOC_NO_ACCETTATI { get; set; }

        public long? TOT_FASC { get; set; }

        public long? TOT_FASC_NO_LETTI { get; set; }

        public long? TOT_FASC_NO_ACCETTATI { get; set; }

        public long? TOT_DOC_PREDISPOSTI { get; set; }
    }
}
