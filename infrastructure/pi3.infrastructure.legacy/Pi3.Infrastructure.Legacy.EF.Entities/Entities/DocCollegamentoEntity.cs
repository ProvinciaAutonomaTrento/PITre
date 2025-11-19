// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class DocCollegamentoEntity
    {
        public long ID_DOCUMENTO { get; set; }
        public long ID_DOC_COLLEGATO { get; set; }
        public long ID_TIPO_COLLEGAMENTO { get; set; }
        public long ID_ROOT { get; set; }
    }
}
