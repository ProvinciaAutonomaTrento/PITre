// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class FirmaElettronicaEntity
    {
        public DateTime DATA_APPOSIZIONE { get; set; }
        public long ID_FIRMA { get; set; }
        public long ID_DOCUMENTO { get; set; }
        public long? NUM_ALL { get; set; }
        public long NUMERO_VERSIONE { get; set; }
        public long VERSION_ID { get; set; }
        public string DOC_ALL { get; set; }
        public string? XML { get; set; }

    }
}
