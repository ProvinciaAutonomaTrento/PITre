// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class ListeDistrEntity
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_LISTA_DPA_CORR { get; set; }
        public long? ID_DPA_CORR { get; set; }

    }
}
