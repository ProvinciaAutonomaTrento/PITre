// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class SwitchServiceInteropEntity
    {
        public string VAR_INTEROP_URL { get; set; }

        public string VAR_INSTANCE { get; set; }

        public string CHA_USE_NEW_INTEROP { get; set; }
    }
}
