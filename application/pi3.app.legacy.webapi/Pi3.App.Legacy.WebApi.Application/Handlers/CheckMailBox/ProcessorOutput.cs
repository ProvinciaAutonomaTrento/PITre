// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    internal class ProcessorOutput
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public long? DocNumber { get; set; }

        public int? ProcessedAttachments { get; set; }
    }

}
