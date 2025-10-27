// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Fatturazione;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Models
{
    public class SwitchServiceModel
    {
        public string ServiceName { get; set; }

        public bool UseDataPortalApi { get; set; }

        public string? Category { get; set; }
        public bool? ResponseAsRaw { get; set; }
    }
}
